using System;
using System.Collections.Generic;
using System.Threading;
using Moralis.WebGL.Platform.Abstractions;
using Moralis.WebGL.Platform.Objects;
using Cysharp.Threading.Tasks;

namespace Moralis.WebGL.Platform.Abstractions
{
    public interface IObjectService
    {
        UniTask<T> FetchAsync<T>(T item, string sessionToken, CancellationToken cancellationToken = default) where T : MoralisObject;

        UniTask<string> SaveAsync(MoralisObject item, IDictionary<string, IMoralisFieldOperation> operations, string sessionToken, CancellationToken cancellationToken = default);

        //IList<Task<T>> SaveAllAsync<T>(IList<IObjectState> states, IList<IDictionary<string, string sessionToken, IServiceHub<MoralisUser> serviceHub, CancellationToken cancellationToken = default) where T : MoralisObject;

        UniTask DeleteAsync(MoralisObject item, string sessionToken, CancellationToken cancellationToken = default);

        //IList<Task> DeleteAllAsync<T>(IList<T> items, string sessionToken, CancellationToken cancellationToken = default) where T : MoralisObject;
    }
}
