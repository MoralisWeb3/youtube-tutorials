using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moralis.Platform.Abstractions;
using Moralis.Platform.Services.Models;
using Moralis.Platform.Objects;
using Moralis.Platform.Utilities;

namespace Moralis.Platform.Services.ClientServices
{
    public class MoralisUserService<TUser> : IUserService<TUser> where TUser : MoralisUser
    {
        IMoralisCommandRunner CommandRunner { get; }

        IJsonSerializer JsonSerializer { get; }

        public bool RevocableSessionEnabled { get; set; }

        public object RevocableSessionEnabledMutex { get; } = new object { };

        public MoralisUserService(IMoralisCommandRunner commandRunner, IJsonSerializer jsonSerializer) => (CommandRunner, JsonSerializer) = (commandRunner, jsonSerializer);

        public Task<TUser> SignUpAsync(IObjectState state, IDictionary<string, IMoralisFieldOperation> operations, CancellationToken cancellationToken = default) => CommandRunner.RunCommandAsync(new MoralisCommand("server/classes/_User", method: "POST", data: JsonSerializer.Serialize(operations)), cancellationToken: cancellationToken).OnSuccess(task => //JsonConvert.DeserializeObject(task.Result.Item2).MutatedClone(mutableClone => mutableClone.IsNew = true));
        {
            TUser resp = default;

            if ((int)task.Result.Item1 < 300)
            {
                resp = (TUser)JsonSerializer.Deserialize<TUser>(task.Result.Item2);
            }

            return resp;
        });

        public Task<TUser> LogInAsync(string username, string password, IServiceHub<TUser> serviceHub, CancellationToken cancellationToken = default) => CommandRunner.RunCommandAsync(new MoralisCommand($"server/login?{MoralisService<TUser>.BuildQueryString(new Dictionary<string, object> { [nameof(username)] = username, [nameof(password)] = password })}", method: "GET", data: null), cancellationToken: cancellationToken).OnSuccess(task => (int)task.Result.Item1 < 300 ? JsonSerializer.Deserialize<TUser>(task.Result.Item2.ToString()) : default);

        public Task<TUser> LogInAsync(string authType, IDictionary<string, object> data, IServiceHub<TUser> serviceHub, CancellationToken cancellationToken = default)
        {
            Dictionary<string, object> authData = new Dictionary<string, object>
            {
                [authType] = data
            };

            return CommandRunner.RunCommandAsync(new MoralisCommand("server/users", method: "POST", data: JsonSerializer.Serialize(new Dictionary<string, object> { [nameof(authData)] = authData })), cancellationToken: cancellationToken).OnSuccess(task => 
            {
                TUser user = default;
                if ((int)task.Result.Item1 < 300)
                {
                    user = JsonSerializer.Deserialize<TUser>(task.Result.Item2.ToString());
                    user.SessionToken = user.SessionToken;
                }

                return user;
            });
        }

        public Task<TUser> GetUserAsync(string sessionToken, IServiceHub<TUser> serviceHub, CancellationToken cancellationToken = default) => CommandRunner.RunCommandAsync(new MoralisCommand("server/users/me", method: "GET", sessionToken: sessionToken, data: default), cancellationToken: cancellationToken).OnSuccess(task => (int)task.Result.Item1 < 300 ? JsonSerializer.Deserialize<TUser>(task.Result.Item2.ToString()) : default);

        public Task RequestPasswordResetAsync(string email, CancellationToken cancellationToken = default) => CommandRunner.RunCommandAsync(new MoralisCommand("server/requestPasswordReset", method: "POST", data: JsonSerializer.Serialize(new Dictionary<string, object> { [nameof(email)] = email })), cancellationToken: cancellationToken);
    }
}
