using System.Collections.Generic;
using System.Threading;
using Moralis.WebGL.Platform.Abstractions;
using Moralis.WebGL.Platform.Services.Models;
using Moralis.WebGL.Platform.Objects;
using Moralis.WebGL.Platform.Utilities;
using Cysharp.Threading.Tasks;
using System.Net;
using System;
using UnityEngine;

namespace Moralis.WebGL.Platform.Services.ClientServices
{
    public class MoralisUserService<TUser> : IUserService<TUser> where TUser : MoralisUser
    {
        IMoralisCommandRunner CommandRunner { get; }

        IJsonSerializer JsonSerializer { get; }

        IObjectService ObjectService { get; }

        public bool RevocableSessionEnabled { get; set; }

        public object RevocableSessionEnabledMutex { get; } = new object { };

        public MoralisUserService(IMoralisCommandRunner commandRunner, IObjectService objectService, IJsonSerializer jsonSerializer) => (CommandRunner, ObjectService, JsonSerializer) = (commandRunner, objectService, jsonSerializer);

        public async UniTask<TUser> SignUpAsync(IObjectState state, IDictionary<string, IMoralisFieldOperation> operations, CancellationToken cancellationToken = default)
        {
            Tuple<HttpStatusCode, string> cmdResp = await CommandRunner.RunCommandAsync(new MoralisCommand("server/classes/_User", method: "POST", data: JsonSerializer.Serialize(operations)), cancellationToken: cancellationToken);

            TUser resp = default;

            if ((int)cmdResp.Item1 < 300)
            {
                resp = (TUser)JsonSerializer.Deserialize<TUser>(cmdResp.Item2);
                resp.ObjectService = this.ObjectService;
            }
            else
            {
                Debug.Log($"SignUpAsync failed: {cmdResp.Item2}");
            }

            return resp;
        }

        public async UniTask<TUser> LogInAsync(string username, string password, IServiceHub<TUser> serviceHub, CancellationToken cancellationToken = default)
        {
            TUser result = default;
            Tuple<HttpStatusCode, string> cmdResp = await CommandRunner.RunCommandAsync(new MoralisCommand($"server/login?{MoralisService<TUser>.BuildQueryString(new Dictionary<string, object> { [nameof(username)] = username, [nameof(password)] = password })}", method: "GET", data: null), cancellationToken: cancellationToken);
            if ((int)cmdResp.Item1 < 300)
            {
                result = JsonSerializer.Deserialize<TUser>(cmdResp.Item2.ToString());

                result.ObjectService = this.ObjectService;
            }
            else
            {
                Debug.Log($"LogInAsync failed: {cmdResp.Item2}");
            }

            return result;
        }

        public async UniTask<TUser> LogInAsync(string authType, IDictionary<string, object> data, IServiceHub<TUser> serviceHub, CancellationToken cancellationToken = default)
        {
            TUser user = default;

            Dictionary<string, object> authData = new Dictionary<string, object>
            {
                [authType] = data
            };

            Tuple<HttpStatusCode, string> cmdResp = await CommandRunner.RunCommandAsync(new MoralisCommand("server/users", method: "POST", data: JsonSerializer.Serialize(new Dictionary<string, object> { [nameof(authData)] = authData })), cancellationToken: cancellationToken);
            
            if ((int)cmdResp.Item1 < 300)
            {
                user = JsonSerializer.Deserialize<TUser>(cmdResp.Item2.ToString());

                user.ObjectService = this.ObjectService;
            }
            else
            {
                Debug.Log($"LogInAsync failed: {cmdResp.Item2}");
            }

            return user;
        }

        public async UniTask<TUser> GetUserAsync(string sessionToken, IServiceHub<TUser> serviceHub, CancellationToken cancellationToken = default)
        {
            TUser user = default;
            Tuple<HttpStatusCode, string> cmdResp = await CommandRunner.RunCommandAsync(new MoralisCommand("server/users/me", method: "GET", sessionToken: sessionToken, data: default), cancellationToken: cancellationToken);
            if ((int)cmdResp.Item1 < 300)
            {
                user = JsonSerializer.Deserialize<TUser>(cmdResp.Item2.ToString());

                user.ObjectService = this.ObjectService;
            }
            else
            {
                Debug.Log($"GetUserAsync failed: {cmdResp.Item2}");
            }

            return user;
        }

        public async UniTask RequestPasswordResetAsync(string email, CancellationToken cancellationToken = default)
        {
            Tuple<HttpStatusCode, string> cmdResp = await CommandRunner.RunCommandAsync(new MoralisCommand("server/requestPasswordReset", method: "POST", data: JsonSerializer.Serialize(new Dictionary<string, object> { [nameof(email)] = email })), cancellationToken: cancellationToken);

            if((int)cmdResp.Item1 >= 400)
            {
                Debug.Log($"RequestPasswordResetAsync failed: {cmdResp.Item2}");
            }
        }
    }
}
