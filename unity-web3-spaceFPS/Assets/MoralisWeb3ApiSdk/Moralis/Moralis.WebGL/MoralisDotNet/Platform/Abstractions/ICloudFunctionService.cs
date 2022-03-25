using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Moralis.WebGL.Platform.Abstractions
{
    public interface ICloudFunctionService
    {
        UniTask<T> CallFunctionAsync<T>(string name, IDictionary<string, object> parameters, string sessionToken, CancellationToken cancellationToken = default);
    }
}
