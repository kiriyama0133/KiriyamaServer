namespace KiriyamaServer.Application.Services;

/// <summary>房间实时事件的类型名（与客户端 SSE 解析端约定的字符串一致）。</summary>
public static class RoomEventTypes
{
    /// <summary>房间已被销毁：房内其余客户端应立即退出房间回到大厅。</summary>
    public const string RoomClosed = "room_closed";

    /// <summary>房主已变更：客户端据此刷新房主标识与转让按钮的可见性。</summary>
    public const string HostChanged = "host_changed";
}

/// <summary>
/// 一条推送给房间订阅者的实时事件。
/// 未用到的字段为 null，序列化时会省略（客户端只读自己关心的字段）。
/// </summary>
/// <param name="Type">事件类型，取值见 <see cref="RoomEventTypes"/>。</param>
/// <param name="Reason">发生原因（房间销毁时给用户看的说明）。</param>
/// <param name="HostName">新房主昵称（<see cref="RoomEventTypes.HostChanged"/> 时有效）。</param>
/// <param name="HostNodeId">新房主节点 ID（<see cref="RoomEventTypes.HostChanged"/> 时有效）。</param>
public sealed record RoomEvent(
    string Type,
    string? Reason = null,
    string? HostName = null,
    string? HostNodeId = null);

/// <summary>
/// 房间实时事件广播（SSE 推送用）。
///
/// 房间销毁、房主变更这类「别人触发的状态变化」通过它主动推给订阅了该房间的客户端，
/// 客户端无需等待下一次轮询即可反应（例如房主退出后立即被请出房间）。
/// </summary>
public interface IRoomEventPublisher
{
    /// <summary>向某个房间的所有订阅者广播一条事件；无人订阅时静默丢弃。</summary>
    void Publish(string roomId, RoomEvent roomEvent);

    /// <summary>广播「房间已销毁」。</summary>
    void PublishRoomClosed(string roomId, string reason);

    /// <summary>广播「房主已变更」。</summary>
    void PublishHostChanged(string roomId, string hostName, string hostNodeId);
}
