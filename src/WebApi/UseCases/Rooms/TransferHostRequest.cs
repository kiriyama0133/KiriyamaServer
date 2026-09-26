using System.ComponentModel.DataAnnotations;

namespace KiriyamaServer.WebApi.UseCases.Rooms;

/// <summary>
/// 转让房主请求。发起者身份以节点 ID 为准（服务端会校验它确实是房主），
/// 不信任客户端自称的昵称。
/// </summary>
public sealed class TransferHostRequest
{
    /// <summary>发起转让的节点 ID（必须是当前房主）。</summary>
    [Required]
    public required string RequesterNodeId { get; init; }

    /// <summary>接手房主的节点 ID（必须已在房间内）。</summary>
    [Required]
    public required string TargetNodeId { get; init; }
}
