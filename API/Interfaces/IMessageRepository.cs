using System;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IMessageRepository
{

    void AddMessage(Messagee message);

    void DeleteMessage(Messagee message);

    Task<Messagee?>GetMessage(string messageId);

    Task<PaginatedResult<MessageDto>> GetMessagesForMember(MessageParams messageParams);

    Task<IReadOnlyList<MessageDto>> GetMessageThread(string currentMemberId, string recipientId);

    Task<bool> SaveAllAsync();

    void AddGroup(Group group);

    Task RemoveConnection(string connectionId);

    Task<Connection?> GetConnection(string connectionId);


    Task<Group?> GetMessageGroup(string groupName);

    Task<Group?> GetGroupForConnection(string connectionId);

}
