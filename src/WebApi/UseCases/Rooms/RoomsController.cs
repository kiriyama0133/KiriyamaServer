using KiriyamaServer.Application.Boundaries.Rooms;
using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Application.Repositories;
using KiriyamaServer.Application.Services;
using KiriyamaServer.Domain.Games;
using KiriyamaServer.Domain.Rooms;
using KiriyamaServer.Infrastructure.Realtime;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Channels;

namespace KiriyamaServer.WebApi.UseCases.Rooms;

/// <summary>
/// 房间大厅控制器。维护游戏房间与玩家连接，提供密码保护的房间加入能力。
/// 路由使用 /api/rooms（不带版本前缀），与客户端 IRelayServerClient 的协议对齐。
/// </summary>
[Route("api/rooms")]
[ApiController]
public sealed class RoomsController : ControllerBase
{
    /// <summary>SSE 心跳间隔：定期发一行注释保活，避免中间代理掐掉空闲长连接。</summary>
    private static readonly TimeSpan SSE_HEARTBEAT_INTERVAL = TimeSpan.FromSeconds(15);

    /// <summary>SSE 数据载荷的序列化选项（camelCase，省略 null 字段）。</summary>
    private static readonly JsonSerializerOptions SseJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly IUseCase<ListRoomsInput> _listRoomsUseCase;
    private readonly ListRoomsPresenter _listRoomsPresenter;
    private readonly IUseCase<CreateRoomInput> _createRoomUseCase;
    private readonly CreateRoomPresenter _createRoomPresenter;
    private readonly IUseCase<JoinRoomInput> _joinRoomUseCase;
    private readonly JoinRoomPresenter _joinRoomPresenter;
    private readonly IUseCase<LeaveRoomInput> _leaveRoomUseCase;
    private readonly LeaveRoomPresenter _leaveRoomPresenter;
    private readonly IUseCase<ListPlayersInput> _listPlayersUseCase;
    private readonly ListPlayersPresenter _listPlayersPresenter;
    private readonly IUseCase<TransferHostInput> _transferHostUseCase;
    private readonly TransferHostPresenter _transferHostPresenter;
    private readonly IRoomRepository _roomRepository;
    private readonly RoomEventHub _roomEvents;

    public RoomsController(
        IUseCase<ListRoomsInput> listRoomsUseCase,
        ListRoomsPresenter listRoomsPresenter,
        IUseCase<CreateRoomInput> createRoomUseCase,
        CreateRoomPresenter createRoomPresenter,
        IUseCase<JoinRoomInput> joinRoomUseCase,
        JoinRoomPresenter joinRoomPresenter,
        IUseCase<LeaveRoomInput> leaveRoomUseCase,
        LeaveRoomPresenter leaveRoomPresenter,
        IUseCase<ListPlayersInput> listPlayersUseCase,
        ListPlayersPresenter listPlayersPresenter,
        IUseCase<TransferHostInput> transferHostUseCase,
        TransferHostPresenter transferHostPresenter,
        IRoomRepository roomRepository,
        RoomEventHub roomEvents)
    {
        _listRoomsUseCase = listRoomsUseCase;
        _listRoomsPresenter = listRoomsPresenter;
        _createRoomUseCase = createRoomUseCase;
        _createRoomPresenter = createRoomPresenter;
        _joinRoomUseCase = joinRoomUseCase;
        _joinRoomPresenter = joinRoomPresenter;
        _leaveRoomUseCase = leaveRoomUseCase;
        _leaveRoomPresenter = leaveRoomPresenter;
        _listPlayersUseCase = listPlayersUseCase;
        _listPlayersPresenter = listPlayersPresenter;
        _transferHostUseCase = transferHostUseCase;
        _transferHostPresenter = transferHostPresenter;
        _roomRepository = roomRepository;
        _roomEvents = roomEvents;
    }

    /// <summary>列出房间；可通过 game 查询参数按游戏板块过滤。</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<RoomDto>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult?> ListAsync([FromQuery] string? game = null, CancellationToken cancellationToken = default)
    {
        var input = string.IsNullOrWhiteSpace(game)
            ? ListRoomsInput.Instance
            : ListRoomsInput.ForGame(GameKey.From(game));

        await _listRoomsUseCase.ExecuteAsync(input, cancellationToken);
        return _listRoomsPresenter.ViewModel;
    }

    /// <summary>创建一个房间。</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RoomDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult?> CreateAsync([FromBody][Required] CreateRoomRequest request, CancellationToken cancellationToken = default)
    {
        var input = new CreateRoomInput(request.Name, request.HostName, request.HostNodeId, GameKey.From(request.Game), request.Password, request.MaxPlayers);
        await _createRoomUseCase.ExecuteAsync(input, cancellationToken);
        return _createRoomPresenter.ViewModel;
    }

    /// <summary>加入一个房间（密码校验，成功后服务端给节点打房间 Tag）。</summary>
    [HttpPost("{roomId}/join")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(JoinRoomResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult?> JoinAsync([FromRoute][Required] string roomId, [FromBody][Required] JoinRoomRequest request, CancellationToken cancellationToken = default)
    {
        var input = new JoinRoomInput(RoomId.From(roomId), request.Nickname, request.NodeId, request.VirtualIp, request.Password);
        await _joinRoomUseCase.ExecuteAsync(input, cancellationToken);
        return _joinRoomPresenter.ViewModel;
    }

    /// <summary>离开一个房间（服务端清除该节点的房间 Tag；房主离开则解散整个房间）。</summary>
    [HttpPost("{roomId}/leave")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult?> LeaveAsync([FromRoute][Required] string roomId, [FromBody][Required] LeaveRoomRequest request, CancellationToken cancellationToken = default)
    {
        var input = new LeaveRoomInput(RoomId.From(roomId), request.NodeId);
        await _leaveRoomUseCase.ExecuteAsync(input, cancellationToken);
        return _leaveRoomPresenter.ViewModel;
    }

    /// <summary>列出房间内的玩家（客户端房间页面展示成员与做延迟探测）。</summary>
    [HttpGet("{roomId}/players")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult?> ListPlayersAsync([FromRoute][Required] string roomId, CancellationToken cancellationToken = default)
    {
        var input = new ListPlayersInput(RoomId.From(roomId));
        await _listPlayersUseCase.ExecuteAsync(input, cancellationToken);
        return _listPlayersPresenter.ViewModel;
    }

    /// <summary>转让房主（仅当前房主可发起，目标必须在房内）。</summary>
    [HttpPost("{roomId}/transfer-host")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult?> TransferHostAsync(
        [FromRoute][Required] string roomId,
        [FromBody][Required] TransferHostRequest request,
        CancellationToken cancellationToken = default)
    {
        var input = new TransferHostInput(RoomId.From(roomId), request.RequesterNodeId, request.TargetNodeId);
        await _transferHostUseCase.ExecuteAsync(input, cancellationToken);
        return _transferHostPresenter.ViewModel;
    }

    /// <summary>
    /// 订阅房间事件流（SSE）。房主退出导致房间解散、房主转让等状态变化会实时推给客户端，
    /// 客户端据此立即反应（例如被请出房间），无需等待下一次轮询。
    ///
    /// 事件格式：<c>event: &lt;类型&gt;</c> + <c>data: &lt;JSON&gt;</c>，类型见 RoomEventTypes。
    /// </summary>
    [HttpGet("{roomId}/events")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task EventsAsync([FromRoute][Required] string roomId, CancellationToken cancellationToken)
    {
        // 长连接开始前先确认房间存在，房间不存在就直接 404，不让客户端干等。
        var room = await _roomRepository.FindAsync(RoomId.From(roomId), cancellationToken);

        if (room is null)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            await Response.WriteAsJsonAsync(
                new ProblemDetails
                {
                    Title = "Room not found",
                    Detail = "The room does not exist or has been closed.",
                    Status = StatusCodes.Status404NotFound
                },
                cancellationToken).ConfigureAwait(false);
            return;
        }

        Response.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";

        // 反向代理（nginx 等）默认会缓冲响应，导致事件被攒着不发。
        // 注意不要设 Connection: keep-alive —— HTTP/2 里这个头是非法的，会让请求直接失败。
        Response.Headers["X-Accel-Buffering"] = "no";

        using IDisposable subscription = _roomEvents.Subscribe(roomId, out ChannelReader<RoomEvent> reader);

        // 立刻发一行，让客户端确认连接已建立。
        await Response.WriteAsync(": connected\n\n", cancellationToken).ConfigureAwait(false);
        await Response.Body.FlushAsync(cancellationToken).ConfigureAwait(false);

        while (!cancellationToken.IsCancellationRequested)
        {
            RoomEvent? roomEvent = null;
            bool timedOut = false;

            using (var waitCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                waitCts.CancelAfter(SSE_HEARTBEAT_INTERVAL);

                try
                {
                    if (await reader.WaitToReadAsync(waitCts.Token).ConfigureAwait(false))
                    {
                        reader.TryRead(out roomEvent);
                    }
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    // 只是心跳超时，不是客户端断开。
                    timedOut = true;
                }
            }

            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            if (roomEvent is not null)
            {
                string payload = JsonSerializer.Serialize(roomEvent, SseJsonOptions);
                await Response.WriteAsync($"event: {roomEvent.Type}\ndata: {payload}\n\n", cancellationToken).ConfigureAwait(false);
            }
            else if (timedOut)
            {
                await Response.WriteAsync(": ping\n\n", cancellationToken).ConfigureAwait(false);
            }

            await Response.Body.FlushAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
