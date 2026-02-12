using System;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class MessageRepository(AppDbContext context) : IMessageRepository
{
    public void AddGroup(Group group)
    {
       context.Groups.Add(group);
    }

    public void AddMessage(Messagee message)
    {
        context.Messagees.Add(message);
    }

    public void DeleteMessage(Messagee message)
    {
       context.Messagees.Remove(message);
    }

    public async Task<Connection?> GetConnection(string connectionId)
    {
        return await context.Connections.FindAsync(connectionId);
    }

    public async Task<Group?> GetGroupForConnection(string connectionId)
    {
        return await context.Groups
        .Include(x=>x.Connections)
        .Where(x=>x.Connections.Any(c=>c.ConnectionId == connectionId))
        .FirstOrDefaultAsync();
    }

    public async Task<Messagee?> GetMessage(string messageId)
    {
        return await context.Messagees.FindAsync(messageId);
    }

    public async Task<Group?> GetMessageGroup(string groupName)
    {
     return await context.Groups
     .Include(x=>x.Connections)
     .FirstOrDefaultAsync(x=>x.Name == groupName);
    }

    public async Task<PaginatedResult<MessageDto>> GetMessagesForMember(MessageParams messageParams)
    {
        var query = context.Messagees
        .OrderByDescending(x=>x.MessageSent)
        .AsQueryable();

        query = messageParams.Container switch
        {
        "Outbox" => query.Where(x=>x.SenderId == messageParams.MemberId && x.SenderDeleted==false),
        _ => query.Where(x=>x.RecipientId == messageParams.MemberId && x.RecipientDeleted == false)
        };

var messageQuery = query.Select(MessageExtensions.ToDtoProjection());

return await PaginationHelper.CreateAsync(messageQuery,messageParams.PageNumber,messageParams.PageSize);

    }

    public async Task<IReadOnlyList<MessageDto>> GetMessageThread(string currentMemberId, string recipientId)
    {
        
        await context.Messagees
        .Where(x=>x.RecipientId == currentMemberId && x.SenderId == recipientId && x.DataRead ==null)
        .ExecuteUpdateAsync(setters => setters
        .SetProperty(x=>x.DataRead, DateTime.UtcNow));


return await context.Messagees
.Where(x=> (x.RecipientId == currentMemberId &&x.RecipientDeleted==false  &&  x.SenderId == recipientId)
    || (x.SenderId == currentMemberId && x.SenderDeleted==false && x.RecipientId == recipientId))
    .OrderBy(x=>x.MessageSent)
    .Select(MessageExtensions.ToDtoProjection())
    .ToListAsync();



    }

    public async Task RemoveConnection(string connectionId)
    {
        await context.Connections
       .Where(x=>x.ConnectionId == connectionId)
       .ExecuteDeleteAsync();
    }


}
