using KiriyamaServer.Application.Interfaces;

namespace KiriyamaServer.Application.Boundaries.Rooms;

/// <summary>转让房主用例的输出（转让后的新房主信息）。</summary>
public sealed class TransferHostOutput : IOutputType
{
    public string RoomId { get; }

    /// <summary>新房主昵称。</summary>
    public string HostName { get; }

    /// <summary>新房主节点 ID。</summary>
    public string HostNodeId { get; }

    public TransferHostOutput(string roomId, string hostName, string hostNodeId)
    {
        RoomId = roomId;
        HostName = hostName;
        HostNodeId = hostNodeId;
    }
}

/// <summary>转让房主用例的输出端口。</summary>
public interface ITransferHostOutputPort : IErrorHandler
{
    void Standard(TransferHostOutput output);

    /// <summary>房间不存在或已销毁。</summary>
    void NotFound();

    /// <summary>发起者不是房主（或目标是房主自己）——无权转让。</summary>
    void Forbidden(string message);
}
