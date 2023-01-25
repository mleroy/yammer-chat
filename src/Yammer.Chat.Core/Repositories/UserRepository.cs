using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yammer.Chat.Core.Models;
using Yammer.Chat.Core.Parsers;
using Yammer.Chat.Core.Services;

namespace Yammer.Chat.Core.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetCurrentUser(DetailsLevel detailsLevel = DetailsLevel.Minimal);
        Task UpdateCurrentUser(string firstName, string lastName, string jobTitle, string summary, string workPhone, string mobilePhone);
        Task UpdateCurrentUserMugshot(Mugshot mugshot);

        void AddOrUpdateUser(User user);
        Task<User> GetUser(long id, DetailsLevel detailsLevel = DetailsLevel.Minimal);

        Task UpdatePresences(IEnumerable<long> userIds);

        Task<IEnumerable<User>> GetTopColleagues(string prefix, CancellationToken cancellationToken);

        void Clear();
    }

    public class UserRepository : IUserRepository
    {
        private readonly IUserService userService;
        private readonly IUserParser userParser;
        private readonly IIdentityStore identityStore;

        private Dictionary<long, User> users;

        public UserRepository(IUserService userService, IUserParser userParser, IIdentityStore identityStore)
        {
            this.userService = userService;
            this.userParser = userParser;
            this.identityStore = identityStore;

            this.users = new Dictionary<long, User>();
        }

        public async Task<User> GetCurrentUser(DetailsLevel detailsLevel = DetailsLevel.Minimal)
        {
            User user = null;

            if (!this.users.TryGetValue(this.identityStore.UserId, out user) || user.AvailableDetails < detailsLevel)
            {
                var userDto = await this.userService.GetCurrentUser();
                user = this.userParser.ToModel(userDto);
                user.AvailableDetails = DetailsLevel.Full;

                this.AddOrUpdateUser(user);
            }

            return user;
        }

        public async Task<User> GetUser(long id, DetailsLevel detailsLevel = DetailsLevel.Minimal)
        {
            User user = null;

            if (!this.users.TryGetValue(id, out user) || user.AvailableDetails < detailsLevel)
            {
                var userDto = await this.userService.GetUser(id);
                user = this.userParser.ToModel(userDto);
                user.AvailableDetails = DetailsLevel.Full;

                this.AddOrUpdateUser(user);
            }

            return user;
        }

        public async Task UpdateCurrentUser(string firstName, string lastName, string jobTitle, string summary, string workPhone, string mobilePhone)
        {
            var user = await this.GetCurrentUser();

            user.FirstName = firstName;
            user.LastName = lastName;
            user.JobTitle = jobTitle;
            user.Summary = summary;
            user.WorkPhone = workPhone;
            user.MobilePhone = mobilePhone;

            await this.userService.UpdateUser(user);
        }

        public async Task UpdateCurrentUserMugshot(Mugshot mugshot)
        {
            var user = await this.GetCurrentUser();

            await this.userService.UpdateUserMugshot(user.Id, mugshot.Id);

            user.UpdateMugshotTemplate(mugshot.Id);
        }

        public void AddOrUpdateUser(User newUser)
        {
            User existingUser = null;

            if (this.users.TryGetValue(newUser.Id, out existingUser))
            {
                existingUser.Merge(newUser);
            }
            else
            {
                this.users.Add(newUser.Id, newUser);
            }
        }

        public async Task UpdatePresences(IEnumerable<long> userIds)
        {
            var usersEnvelope = await this.userService.GetPresences(userIds);
            var models = this.userParser.ToModel(usersEnvelope.Users);

            foreach (var user in models)
            {
                this.AddOrUpdateUser(user);
            }
        }

        public async Task<IEnumerable<User>> GetTopColleagues(string prefix, CancellationToken cancellationToken)
        {
            var users = await this.userService.GetAutocompleteUsers(prefix, cancellationToken);
            var models = this.userParser.ToModel(users);

            return models;
        }

        public void Clear()
        {
            this.users.Clear();
        }
    }
}
