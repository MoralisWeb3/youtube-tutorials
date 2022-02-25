using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Moralis.WebGL.Platform;
using Moralis.WebGL.Platform.Abstractions;
using Moralis.WebGL.Platform.Exceptions;
using Moralis.WebGL.Platform.Objects;
using Moralis.WebGL.Platform.Queries;
using Moralis.WebGL.Platform.Services;
using Moralis.WebGL.Platform.Services.ClientServices;
using Moralis.WebGL.Platform.Utilities;

namespace Moralis.WebGL.Platform
{
    public class MoralisCloud<TUser> where TUser : MoralisUser
    {
        public IServiceHub<TUser> ServiceHub;

        public MoralisCloud(IServiceHub<TUser> serviceHub) => (ServiceHub) = (serviceHub);
        public async UniTask<T> RunAsync<T>(string name, IDictionary<string, object> parameters)
        {
            MoralisUser user = await this.ServiceHub.GetCurrentUserAsync();

            T result = await this.ServiceHub.CloudFunctionService.CallFunctionAsync<T>(name, parameters, user is { } ? user.sessionToken : "");

            return result;
        }
    }
}
