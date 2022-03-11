using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moralis.Platform.Abstractions;
using Moralis.Platform.Objects;

namespace Moralis.Platform.Abstractions
{
    public interface IObjectService
    {
        Task<T> FetchAsync<T>(T item, string sessionToken, CancellationToken cancellationToken = default) where T : MoralisObject;

        Task<string> SaveAsync(MoralisObject item, IDictionary<string, IMoralisFieldOperation> operations, string sessionToken, CancellationToken cancellationToken = default);

        //IList<Task<T>> SaveAllAsync<T>(IList<IObjectState> states, IList<IDictionary<string, string sessionToken, IServiceHub<MoralisUser> serviceHub, CancellationToken cancellationToken = default) where T : MoralisObject;

        Task DeleteAsync(MoralisObject item, string sessionToken, CancellationToken cancellationToken = default);

        //IList<Task> DeleteAllAsync<T>(IList<T> items, string sessionToken, CancellationToken cancellationToken = default) where T : MoralisObject;
    }
}
