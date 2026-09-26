using KiriyamaServer.Application.Exceptions;
using KiriyamaServer.Application.Interfaces;
using KiriyamaServer.Domain.Rooms;

namespace KiriyamaServer.Application.Boundaries.Rooms;

/// <summary>转让房主用例的输入。</summary>
public sealed class TransferHostInput : IInputType
{
    public RoomId RoomId { get; }

    /// <summary>发起转让的节点（必须是当前房主）。</summary>
    public string RequesterNodeId { get; }

    /// <summary>接手房主的节点（必须已在房间内）。</summary>
    public string TargetNodeId { get; }

    public TransferHostInput(RoomId roomId, string requesterNodeId, string targetNodeId)
    {
        RoomId = roomId;

        RequesterNodeId = string.IsNullOrWhiteSpace(requesterNodeId)
            ? throw new InputValidationException("Requester node id cannot be empty.")
            : requesterNodeId;

        TargetNodeId = string.IsNullOrWhiteSpace(targetNodeId)
            ? throw new InputValidationException("Target node id cannot be empty.")
            : targetNodeId;
    }
}
