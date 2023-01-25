using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Yammer.Chat.Core.API.Dtos;
using Yammer.Chat.Core.Models;
using Yammer.Chat.Core.Repositories;

namespace Yammer.Chat.Core.Parsers
{
    public interface IMessageParser
    {
        Message[] Parse(MessagesEnvelope messagesEnvelope);
        Message Parse(MessageDto messageDto, Dictionary<ReferenceKey, ReferenceDto> references, MetaDto meta);
    }

    public class MessageParser : IMessageParser
    {
        private static readonly Regex placeHolderGroupRegex = new Regex(@"(\[\[\w+:\d+\]\])");
        private static readonly Regex placeHolderPartsRegex = new Regex(@"\[\[(\w+)\:(\d+)\]\]");

        private readonly IAttachmentParser attachmentParser;
        private readonly IUserParser userParser;
        private readonly IUserRepository userRepository;

        public MessageParser(IAttachmentParser attachmentParser, IUserParser userParser, IUserRepository userRepository)
        {
            this.attachmentParser = attachmentParser;
            this.userParser = userParser;
            this.userRepository = userRepository;
        }

        public Message[] Parse(MessagesEnvelope messagesEnvelope)
        {
            if (messagesEnvelope == null)
            {
                return new Message[0];
            }

            var references = messagesEnvelope.References.ToDictionary(dto => new ReferenceKey(dto.Type, dto.Id), dto => dto);

            this.StoreUsers(references);

            return messagesEnvelope.Messages
                .Select(m => this.Parse(m, references, messagesEnvelope.Meta))
                .ToArray();
        }

        public Message Parse(MessageDto messageDto, Dictionary<ReferenceKey, ReferenceDto> references, MetaDto meta)
        {
            var message = new Message
            {
                Id = messageDto.Id,
                ThreadId = messageDto.ThreadId,
                Sender = ParseSender(messageDto, references),
                BodyParts = ParseBody(messageDto, references),
                Timestamp = ParseCreatedAt(messageDto.CreatedAt),
                Likers = ParseLikers(messageDto),
                Attachments = this.attachmentParser.ToModel(messageDto.Attachments),
                IsFromCurrentUser = messageDto.SenderId == meta.CurrentUserId,
                ClientType = messageDto.ClientType == "Web" ? ClientType.Web : ClientType.Mobile
            };
            
            message.IsLikedByCurrentUser = message.Likers.Any(x => x.Id == meta.CurrentUserId);

            return message;
        }

        private static DateTime ParseCreatedAt(DateTime createdAt)
        {
            // Yammer server lives a few seconds in the future
            if (createdAt > DateTime.Now)
                return DateTime.Now;

            return createdAt;
        }

        private static MessagePart[] ParseBody(MessageDto message, Dictionary<ReferenceKey, ReferenceDto> referenceDictionary)
        {
            if (message.Body == null || string.IsNullOrEmpty(message.Body.Plain))
                return null;

            var parsed = placeHolderGroupRegex.Split(message.Body.Plain);

            return parsed.Select(part =>
            {
                if (placeHolderPartsRegex.IsMatch(part))
                {
                    var parsedPlaceholder = placeHolderPartsRegex.Match(part).Groups;
                    var key = new ReferenceKey(parsedPlaceholder[1].Value, Convert.ToInt64(parsedPlaceholder[2].Value));

                    var value = part;

                    if (referenceDictionary.ContainsKey(key))
                    {
                        var reference = referenceDictionary[key];

                        if (reference is UserReferenceDto)
                        {
                            value = (reference as UserReferenceDto).FullName;
                        }
                        else if (reference is GroupReferenceDto)
                        {
                            value = (reference as GroupReferenceDto).FullName;
                        }
                    }

                    return new MessagePart { Text = value };
                }

                return new MessagePart { Text = part };
            }).ToArray();
        }

        private ObservableCollection<User> ParseLikers(MessageDto messageDto)
        {
            if (messageDto.LikedBy == null)
            {
                return new ObservableCollection<User>();
            }

            return new ObservableCollection<User>(messageDto.LikedBy.Users.Select(u => this.userRepository.GetUser(u.UserId).Result));
        }

        private User ParseSender(MessageDto messageDto, Dictionary<ReferenceKey, ReferenceDto> references)
        {
            return this.userRepository.GetUser(messageDto.SenderId).Result;
        }

        private void StoreUsers(Dictionary<ReferenceKey, ReferenceDto> references)
        {
            foreach (var userReferenceDto in references.Where(x => x.Value is UserReferenceDto).Select(x => x.Value).Cast<UserReferenceDto>())
            {
                var user = this.userParser.ToModel(userReferenceDto);
                this.userRepository.AddOrUpdateUser(user);
            }
        }
    }
}
