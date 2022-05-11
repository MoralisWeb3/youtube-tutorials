using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moralis.Platform.Abstractions;
using Moralis.Platform.Exceptions;
using Moralis.Platform.Objects;
using Moralis.Platform.Queries;
using Moralis.Platform.Services.Models;
using Moralis.Platform.Utilities;

namespace Moralis.Platform.Services.ClientServices
{
    public class MoralisObjectService : IObjectService
    {
        //IServiceHub<MoralisUser> Services { get; }
        IMoralisCommandRunner CommandRunner { get; }

        IServerConnectionData ServerConnectionData { get; }

        IJsonSerializer JsonSerializer { get; }

        public MoralisObjectService(IMoralisCommandRunner commandRunner, IServerConnectionData serverConnectionData, IJsonSerializer jsonSerializer)
        {
            CommandRunner = commandRunner;
            //Services = serviceHub;
            ServerConnectionData = serverConnectionData;
            JsonSerializer = jsonSerializer;

            
        }

        public Task<T> FetchAsync<T>(T item, string sessionToken, CancellationToken cancellationToken = default) where T : MoralisObject
        {
            MoralisCommand command = new MoralisCommand($"server/classes/{Uri.EscapeDataString(item.ClassName)}/{Uri.EscapeDataString(item.objectId)}", method: "GET", sessionToken: sessionToken, data: default);
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

        public Task<string> SaveAsync(MoralisObject item, IDictionary<string, IMoralisFieldOperation> operations, string sessionToken, CancellationToken cancellationToken = default)
        {
            MoralisCommand command = new MoralisCommand(item.objectId == null ? $"server/classes/{Uri.EscapeDataString(item.ClassName)}" : $"server/classes/{Uri.EscapeDataString(item.ClassName)}/{item.objectId}", method: item.objectId is null ? "POST" : "PUT", sessionToken: sessionToken, data: operations is { } && operations.Count > 0 ? JsonSerializer.Serialize(operations, JsonSerializer.DefaultOptions).JsonInsertParseDate() : JsonSerializer.Serialize(item, JsonSerializer.DefaultOptions).JsonInsertParseDate());
            return CommandRunner.RunCommandAsync(command, cancellationToken: cancellationToken).OnSuccess(task => 
            {
                string resp = default;

                if ((int)task.Result.Item1 < 400)
                {
                    resp = task.Result.Item2;
                }

                return resp;
            });
        }

        //public IList<Task<T>> SaveAllAsync<T>(IList<T> items, IList<IDictionary<string, IMoralisFieldOperation>> operationsList, string sessionToken, CancellationToken cancellationToken = default) where T : MoralisObject => ExecuteBatchRequests<T>(items.Zip(operationsList, (item, operations) => new MoralisCommand(item.ObjectId is null ? $"server/classes/{Uri.EscapeDataString(item.ClassName)}" : $"server/classes/{Uri.EscapeDataString(item.ClassName)}/{Uri.EscapeDataString(item.ObjectId)}", method: item.ObjectId is null ? "POST" : "PUT", data: operations is { } && operations.Count > 0 ? JsonConvert.SerializeObject(operations, jsonSettings).JsonInsertParseDate() : JsonConvert.SerializeObject(item, jsonSettings).JsonInsertParseDate())).ToList(), sessionToken, cancellationToken).Select(task => task.Result).ToList();

        public Task DeleteAsync(MoralisObject item, string sessionToken, CancellationToken cancellationToken = default) => CommandRunner.RunCommandAsync(new MoralisCommand($"server/classes/{item.ClassName}/{item.objectId}", method: "DELETE", sessionToken: sessionToken, data: null), cancellationToken: cancellationToken);
               
        //public IList<Task> DeleteAllAsync<T>(IList<T> items, string sessionToken, CancellationToken cancellationToken = default) where T : MoralisObject => ExecuteBatchRequests<T>(items.Where(item => item.ObjectId is { }).Select(item => new MoralisCommand($"server/classes/{Uri.EscapeDataString(item.ClassName)}/{Uri.EscapeDataString(item.ObjectId)}", method: "DELETE", data: default)).ToList(), sessionToken, cancellationToken).Cast<Task>().ToList();

        int MaximumBatchSize { get; } = 50;


        //internal IList<Task<T>> ExecuteBatchRequests<T>(IList<MoralisCommand> requests, string sessionToken, CancellationToken cancellationToken = default) where T : MoralisObject
        //{
        //    List<Task<T>> tasks = new List<Task<T>>();
        //    int batchSize = requests.Count;

        //    IEnumerable<MoralisCommand> remaining = requests;

        //    while (batchSize > MaximumBatchSize)
        //    {
        //        List<MoralisCommand> process = remaining.Take(MaximumBatchSize).ToList();

        //        remaining = remaining.Skip(MaximumBatchSize);
        //        tasks.AddRange(ExecuteBatchRequest<T>(process, sessionToken, cancellationToken));
        //        batchSize = remaining.Count();
        //    }

        //    tasks.AddRange(ExecuteBatchRequest(remaining.ToList(), sessionToken, cancellationToken));
        //    return tasks;
        //}

        IList<Task<IDictionary<string, object>>> ExecuteBatchRequest<T>(IList<MoralisCommand> requests, string sessionToken, CancellationToken cancellationToken = default)
        {
            int batchSize = requests.Count;

            List<Task<IDictionary<string, object>>> tasks = new List<Task<IDictionary<string, object>>> { };
            List<TaskCompletionSource<IDictionary<string, object>>> completionSources = new List<TaskCompletionSource<IDictionary<string, object>>> { };

            for (int i = 0; i < batchSize; ++i)
            {
                TaskCompletionSource<IDictionary<string, object>> tcs = new TaskCompletionSource<IDictionary<string, object>>();

                completionSources.Add(tcs);
                tasks.Add(tcs.Task);
            }

            List<object> encodedRequests = requests.Select(request =>
            {
                Dictionary<string, object> results = new Dictionary<string, object>
                {
                    ["method"] = request.Method,
                    ["path"] = request is { Path: { }, Resource: { } } ? request.Target.AbsolutePath : new Uri(new Uri(ServerConnectionData.ServerURI), request.Path).AbsolutePath,
                };

                if (request.DataObject != null)
                    results["body"] = request.DataObject;

                return results;
            }).Cast<object>().ToList();

            MoralisCommand command = new MoralisCommand("batch", method: "POST", sessionToken: sessionToken, data: JsonSerializer.Serialize(new Dictionary<string, object> { [nameof(requests)] = encodedRequests }, JsonSerializer.DefaultOptions));

            CommandRunner.RunCommandAsync(command, cancellationToken: cancellationToken).ContinueWith(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    foreach (TaskCompletionSource<IDictionary<string, object>> tcs in completionSources)
                        if (task.IsFaulted)
                            tcs.TrySetException(task.Exception);
                        else if (task.IsCanceled)
                            tcs.TrySetCanceled();

                    return;
                }

                IList<object> resultsArray = Conversion.As<IList<object>>(task.Result.Item2);
                int resultLength = resultsArray.Count;

                if (resultLength != batchSize)
                {
                    foreach (TaskCompletionSource<IDictionary<string, object>> completionSource in completionSources)
                        completionSource.TrySetException(new InvalidOperationException($"Batch command result count expected: {batchSize} but was: {resultLength}."));

                    return;
                }

                for (int i = 0; i < batchSize; ++i)
                {
                    Dictionary<string, object> result = resultsArray[i] as Dictionary<string, object>;
                    TaskCompletionSource<IDictionary<string, object>> target = completionSources[i];

                    if (result.ContainsKey("success"))
                        target.TrySetResult(result["success"] as IDictionary<string, object>);
                    else if (result.ContainsKey("error"))
                    {
                        IDictionary<string, object> error = result["error"] as IDictionary<string, object>;
                        target.TrySetException(new MoralisFailureException((MoralisFailureException.ErrorCode)(long)error["code"], error[nameof(error)] as string));
                    }
                    else
                        target.TrySetException(new InvalidOperationException("Invalid batch command response."));
                }
            });

            return tasks;
        }
    }
}
