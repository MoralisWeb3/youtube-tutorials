using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moralis.Platform.Abstractions;
using Moralis.Platform.Services.Models;
using Moralis.Platform.Objects;
using Moralis.Platform.Queries;
using Moralis.Platform.Services.ClientServices;
using Moralis.Platform.Utilities;

namespace Moralis.Platform.Services.Infrastructure
{
    internal class MoralisQueryService : IQueryService
    {

        IMoralisCommandRunner CommandRunner { get; }

        IJsonSerializer JsonSerializer { get; }
        string SessionToken { get; }

        public IObjectService ObjectService { get; }

        public MoralisQueryService(IMoralisCommandRunner commandRunner, string sessionToken, IJsonSerializer jsonSerializer, IObjectService objectService) => (CommandRunner, SessionToken, JsonSerializer, ObjectService) = (commandRunner, sessionToken, jsonSerializer, objectService);

        public Task<IEnumerable<T>> FindAsync<T>(MoralisQuery<T> query, string sessionToken, CancellationToken cancellationToken = default) where T : MoralisObject => FindAsync(query.ClassName, query.BuildParameters(), sessionToken, cancellationToken).OnSuccess(t => JsonSerializer.Deserialize<List<T>>(t.Result) as IEnumerable<T>);

        public Task<IEnumerable<T>> AggregateAsync<T>(MoralisQuery<T> query, string sessionToken, CancellationToken cancellationToken = default) where T : MoralisObject => AggregateAsync(query.ClassName, query.BuildParameters(), sessionToken, cancellationToken).OnSuccess(t => JsonSerializer.Deserialize<List<T>>(t.Result) as IEnumerable<T>);

        public Task<int> CountAsync<T>(MoralisQuery<T> query, string sessionToken, CancellationToken cancellationToken = default) where T : MoralisObject
        {
            IDictionary<string, object> parameters = query.BuildParameters();
            parameters["limit"] = 0;
            parameters["count"] = 1;

            return FindAsync(query.ClassName, parameters, sessionToken, cancellationToken).OnSuccess(task => { CountQueryResult result = JsonSerializer.Deserialize<CountQueryResult>(task.Result); Console.WriteLine($"Raw count data: {task.Result}"); return result.count; });
        }

        public Task<IEnumerable<T>> DistinctAsync<T>(MoralisQuery<T> query, string sessionToken, CancellationToken cancellationToken = default) where T : MoralisObject
        {
            IDictionary<string, object> parameters = query.BuildParameters();
            parameters["distinct"] = ""; // key
            parameters["where"] = ""; // where ?
            parameters["hint"] = ""; // hint

            return AggregateAsync(query.ClassName, parameters, sessionToken, cancellationToken).OnSuccess(t => JsonSerializer.Deserialize<List<T>>(t.Result) as IEnumerable<T>);
        }

        public Task<T> FirstAsync<T>(MoralisQuery<T> query, string sessionToken, CancellationToken cancellationToken = default) where T : MoralisObject
        {
            IDictionary<string, object> parameters = query.BuildParameters();
            parameters["limit"] = 1;

            return FindAsync(query.ClassName, parameters, sessionToken, cancellationToken).OnSuccess(task =>
            {
                
                IList<T> l = JsonSerializer.Deserialize<List<T>>(task.Result);

                return l.FirstOrDefault();
            });
        }

        Task<string> FindAsync(string className, IDictionary<string, object> parameters, string sessionToken, CancellationToken cancellationToken = default) => CommandRunner.RunCommandAsync(new MoralisCommand($"server/classes/{Uri.EscapeDataString(className)}?{MoralisService<MoralisUser>.BuildQueryString(parameters)}", method: "GET", sessionToken: sessionToken, data: null), cancellationToken: cancellationToken).OnSuccess(t => t.Result.Item2);

        Task<string> AggregateAsync(string className, IDictionary<string, object> parameters, string sessionToken, CancellationToken cancellationToken = default) => CommandRunner.RunCommandAsync(new MoralisCommand($"server/aggregate/{Uri.EscapeDataString(className)}?{MoralisService<MoralisUser>.BuildQueryString(parameters)}", method: "GET", sessionToken: sessionToken, data: null), cancellationToken: cancellationToken).OnSuccess(t => t.Result.Item2);

    }
}
