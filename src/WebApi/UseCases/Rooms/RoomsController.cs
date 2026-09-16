using KiriyamaServer.Application.Boundaries.Rooms;
using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Domain.Games;
using KiriyamaServer.Domain.Rooms;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace KiriyamaServer.WebApi.UseCases.Rooms;

/// <summary>
/// 房间大厅控制器。维护游戏房间与玩家连接，提供密码保护的房间加入能力。
/// 路由使用 /api/rooms（不带版本前缀），与客户端 IRelayServerClient 的协议对齐。
/// </summary>
[Route("api/rooms")]
[ApiController]
public sealed class RoomsController : ControllerBase
{
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
        ListPlayersPresenter listPlayersPresenter)
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
        var input = new CreateRoomInput(request.Name, request.HostName, GameKey.From(request.Game), request.Password, request.MaxPlayers);
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

    /// <summary>离开一个房间（服务端清除该节点的房间 Tag）。</summary>
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
}
