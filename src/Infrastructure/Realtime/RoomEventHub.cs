using KiriyamaServer.Application.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace KiriyamaServer.Infrastructure.Realtime;

/// <summary>
/// 房间实时事件的进程内广播中心：用例发布、SSE 端点订阅。
///
/// 单例注册 —— 订阅者集合要跨请求共享（发布事件的是一次 HTTP 请求，
/// 接收事件的是另一条长连接）。
/// </summary>
public sealed class RoomEventHub(ILogger<RoomEventHub> logger) : IRoomEventPublisher
{
    /// <summary>单个订阅者的待发队列容量；写满丢最旧的（实时事件只关心最新的那条）。</summary>
    private const int CHANNEL_CAPACITY = 64;

    private readonly ILogger<RoomEventHub> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private readonly ConcurrentDictionary<string, ConcurrentDictionary<Guid, Channel<RoomEvent>>> _rooms =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// 订阅某个房间的事件流。返回的句柄释放时自动退订；
    /// <paramref name="reader"/> 用于逐个读出推送来的事件。
    /// </summary>
    public IDisposable Subscribe(string roomId, out ChannelReader<RoomEvent> reader)
    {
        Channel<RoomEvent> channel = Channel.CreateBounded<RoomEvent>(new BoundedChannelOptions(CHANNEL_CAPACITY)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleReader = true,
            SingleWriter = false
        });

        Guid subscriptionId = Guid.NewGuid();

        ConcurrentDictionary<Guid, Channel<RoomEvent>> subscribers =
            _rooms.GetOrAdd(roomId, _ => new ConcurrentDictionary<Guid, Channel<RoomEvent>>());

        subscribers[subscriptionId] = channel;
        reader = channel.Reader;

        _logger.LogDebug("已订阅房间 {RoomId} 的事件（当前 {Count} 个订阅者）", roomId, subscribers.Count);

        return new SubscriptionHandle(() =>
        {
            if (subscribers.TryRemove(subscriptionId, out Channel<RoomEvent>? removed))
            {
                removed.Writer.TryComplete();
            }

            // 房间没有订阅者了就回收字典项，避免长期运行累积空的房间条目。
            if (subscribers.IsEmpty)
            {
                _rooms.TryRemove(roomId, out _);
            }

            _logger.LogDebug("已退订房间 {RoomId} 的事件", roomId);
        });
    }

    /// <inheritdoc />
    public void Publish(string roomId, RoomEvent roomEvent)
    {
        if (!_rooms.TryGetValue(roomId, out ConcurrentDictionary<Guid, Channel<RoomEvent>>? subscribers)
            || subscribers.IsEmpty)
        {
            return;
        }

        int delivered = 0;

        foreach (Channel<RoomEvent> channel in subscribers.Values)
        {
            if (channel.Writer.TryWrite(roomEvent))
            {
                delivered++;
            }
        }

        _logger.LogInformation(
            "已向房间 {RoomId} 广播事件 {Type}（{Delivered} 个订阅者收到）",
            roomId, roomEvent.Type, delivered);
    }

    /// <inheritdoc />
    public void PublishRoomClosed(string roomId, string reason)
        => Publish(roomId, new RoomEvent(RoomEventTypes.RoomClosed, Reason: reason));

    /// <inheritdoc />
    public void PublishHostChanged(string roomId, string hostName, string hostNodeId)
        => Publish(roomId, new RoomEvent(RoomEventTypes.HostChanged, HostName: hostName, HostNodeId: hostNodeId));

    /// <summary>退订句柄：释放时把订阅从广播中心摘掉（重复释放无副作用）。</summary>
    private sealed class SubscriptionHandle(Action unsubscribe) : IDisposable
    {
        private Action? _unsubscribe = unsubscribe;

        public void Dispose() => Interlocked.Exchange(ref _unsubscribe, null)?.Invoke();
    }
}
