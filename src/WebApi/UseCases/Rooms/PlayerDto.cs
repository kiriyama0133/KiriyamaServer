using System.ComponentModel.DataAnnotations;

namespace KiriyamaServer.WebApi.UseCases.Rooms;

/// <summary>房间内一名玩家的展示 DTO（camelCase 序列化，对齐客户端协议）。</summary>
public sealed class PlayerDto
{
    [Required]
    public Guid PlayerId { get; }

    [Required]
    public string Nickname { get; }

    [Required]
    public string NodeId { get; }

    [Required]
    public string VirtualIp { get; }

    public PlayerDto(Guid playerId, string nickname, string nodeId, string virtualIp)
    {
        PlayerId = playerId;
        Nickname = nickname;
        NodeId = nodeId;
        VirtualIp = virtualIp;
    }
}
