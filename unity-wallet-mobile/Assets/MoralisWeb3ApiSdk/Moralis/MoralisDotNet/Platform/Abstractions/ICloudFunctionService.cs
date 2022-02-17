using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Moralis.Platform.Abstractions
{
    public interface ICloudFunctionService
    {
        Task<T> CallFunctionAsync<T>(string name, IDictionary<string, object> parameters, string sessionToken, CancellationToken cancellationToken = default);
    }
}
