using System.Threading;
using System.Threading.Tasks;
using Moralis.Platform.Abstractions;
using Moralis.Platform.Objects;
using Moralis.Platform.Services.Models;
using Moralis.Platform.Utilities;

namespace Moralis.Platform.Services.ClientServices
{
    public class MoralisSessionService<TUser> : ISessionService<TUser> where TUser : MoralisUser
    {
        IMoralisCommandRunner CommandRunner { get; }

        IJsonSerializer JsonSerializer { get; }

        public MoralisSessionService(IMoralisCommandRunner commandRunner, IJsonSerializer jsonSerializer) => (CommandRunner, JsonSerializer) = (commandRunner, jsonSerializer);

        public Task<MoralisSession> GetSessionAsync(string sessionToken, IServiceHub<TUser> serviceHub, CancellationToken cancellationToken = default) => CommandRunner.RunCommandAsync(new MoralisCommand("sessions/me", method: "GET", sessionToken: sessionToken, data: null), cancellationToken: cancellationToken).OnSuccess(task =>
        {
            MoralisSession resp = default;
            if ((int)task.Result.Item1 < 300)
            {
                resp = JsonSerializer.Deserialize<MoralisSession>(task.Result.Item2);
            }

            return resp;
        });

        public Task RevokeAsync(string sessionToken, CancellationToken cancellationToken = default) => CommandRunner.RunCommandAsync(new MoralisCommand("logout", method: "POST", sessionToken: sessionToken, data: "{}"), cancellationToken: cancellationToken);

        public Task<MoralisSession> UpgradeToRevocableSessionAsync(string sessionToken, IServiceHub<TUser> serviceHub, CancellationToken cancellationToken = default) => CommandRunner.RunCommandAsync(new MoralisCommand("upgradeToRevocableSession", method: "POST", sessionToken: sessionToken, data: "{}"), cancellationToken: cancellationToken).OnSuccess(task =>
        {
            MoralisSession resp = default;
            if ((int)task.Result.Item1 < 300)
            {
                resp = JsonSerializer.Deserialize<MoralisSession>(task.Result.Item2);
            }

            return resp;
        });

        public bool IsRevocableSessionToken(string sessionToken) => sessionToken.Contains("r:");
    }
}
