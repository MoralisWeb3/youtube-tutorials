using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moralis.Platform.Abstractions;
using Moralis.Platform.Services.Models;
using Moralis.Platform.Utilities;

namespace Moralis.Platform.Services.ClientServices
{
    /// <summary>
    /// This service enables an application to call Moralis Cloud Functions.
    /// </summary>
    public class MoralisCloudFunctionService : ICloudFunctionService
    {
        IMoralisCommandRunner CommandRunner { get; }

        //IServiceHub<MoralisUser> Services { get; }

        IServerConnectionData ServerConnectionData { get; }

        IJsonSerializer JsonSerializer { get; }

        public MoralisCloudFunctionService(IMoralisCommandRunner commandRunner, IServerConnectionData serverConnectionData, IJsonSerializer jsonSerializer) => (CommandRunner, ServerConnectionData, JsonSerializer) = (commandRunner, serverConnectionData, jsonSerializer);
 //      public MoralisCloudFunctionService(IMoralisCommandRunner commandRunner, IServiceHub<MoralisUser> services, IServerConnectionData serverConnectionData) => (CommandRunner, Services, ServerConnectionData) = (commandRunner, services, serverConnectionData);

        /// <summary>
        /// Calls Moralis cloud function specified by 'name'.
        /// </summary>
        /// <typeparam name="T">Expected result type</typeparam>
        /// <param name="name">Name of cloud function</param>
        /// <param name="parameters">Parameters that will be passed to the cloud function</param>
        /// <param name="sessionToken">current user's session token</param>
        /// <param name="cancellationToken">Threading cancellation token</param>
        /// <returns>T - result of cloud function call.</returns>
        public Task<T> CallFunctionAsync<T>(string name, IDictionary<string, object> parameters, string sessionToken, CancellationToken cancellationToken = default)
        {
            MoralisCommand command = new MoralisCommand($"server/functions/{Uri.EscapeDataString(name)}", method: "POST", sessionToken: sessionToken, data: parameters is { } && parameters.Count > 0 ? JsonSerializer.Serialize(parameters) : "{}");
           
            return CommandRunner.RunCommandAsync(command, cancellationToken: cancellationToken).OnSuccess(task =>
            {
                T resp = default;

                if ((int)task.Result.Item1 < 400)
                {
                    resp = (T)JsonSerializer.Deserialize<T>(task.Result.Item2);
                }

                return resp;
            });
        }
    }
}
