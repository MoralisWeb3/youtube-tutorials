using System.Collections.Generic;
using System.Threading;
using Moralis.WebGL.Platform.Queries;
using Moralis.WebGL.Platform.Objects;
using Cysharp.Threading.Tasks;

namespace Moralis.WebGL.Platform.Abstractions
{
    public interface IQueryService
    {
        IObjectService ObjectService { get; }
        //Task<IEnumerable<T>> FindAsync<T>(MoralisQuery<T> query, MoralisUser user, CancellationToken cancellationToken = default) where T : MoralisObject;

        //Task<IEnumerable<T>> AggregateAsync<T>(MoralisQuery<T> query, MoralisUser user, CancellationToken cancellationToken = default) where T : MoralisObject;

        //Task<int> CountAsync<T>(MoralisQuery<T> query, MoralisUser user, CancellationToken cancellationToken = default) where T : MoralisObject;

        //Task<T> FirstAsync<T>(MoralisQuery<T> query, MoralisUser user, CancellationToken cancellationToken = default) where T : MoralisObject;

        //Task<IEnumerable<T>> DistinctAsync<T>(MoralisQuery<T> query, MoralisUser user, CancellationToken cancellationToken = default) where T : MoralisObject;
        UniTask<IEnumerable<T>> FindAsync<T>(MoralisQuery<T> query, string sessionToken, CancellationToken cancellationToken = default) where T : MoralisObject;

        UniTask<IEnumerable<T>> AggregateAsync<T>(MoralisQuery<T> query, string sessionToken, CancellationToken cancellationToken = default) where T : MoralisObject;

        UniTask<int> CountAsync<T>(MoralisQuery<T> query, string sessionToken, CancellationToken cancellationToken = default) where T : MoralisObject;

        UniTask<T> FirstAsync<T>(MoralisQuery<T> query, string sessionToken, CancellationToken cancellationToken = default) where T : MoralisObject;

        UniTask<IEnumerable<T>> DistinctAsync<T>(MoralisQuery<T> query, string sessionToken, CancellationToken cancellationToken = default) where T : MoralisObject;


    }
}
