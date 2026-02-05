using System;
using System.Linq.Expressions;
using API.DTOs;
using API.Entities;
using Microsoft.Build.Tasks;

namespace API.Extensions;

//instead of automapper    messagee => messageDto
public static class MessageExtensions
{
    public static MessageDto ToTdo(this Messagee message)
    {
        return new MessageDto
        {
            Id = message.Id,
            SenderId = message.SenderId,
            SenderDisplayName =message.Sender.DisplayName,
            SenderImageUrl = message.Sender.ImageUrl,
            RecipientId= message.RecipientId,
            RecipientDisplayName = message.Recipient.DisplayName,
            RecipientImageUrl = message.Recipient.ImageUrl,
            Content  = message.Content,
            DataRead = message.DataRead,
            MessageSent = message.MessageSent

        };



    }

    public static Expression<Func<Messagee, MessageDto>> ToDtoProjection()
    {
        
                return message => new MessageDto
{
    
             Id = message.Id,
            SenderId = message.SenderId,
            SenderDisplayName =message.Sender.DisplayName,
            SenderImageUrl = message.Sender.ImageUrl,
            RecipientId= message.RecipientId,
            RecipientDisplayName = message.Recipient.DisplayName,
            RecipientImageUrl = message.Recipient.ImageUrl,
            Content  = message.Content,
            DataRead = message.DataRead,
            MessageSent = message.MessageSent




};

    }
}
