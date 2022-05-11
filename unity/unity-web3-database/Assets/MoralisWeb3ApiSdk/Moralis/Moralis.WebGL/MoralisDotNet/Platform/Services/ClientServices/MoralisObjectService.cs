using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Moralis.WebGL.Platform.Abstractions;
using Moralis.WebGL.Platform.Exceptions;
using Moralis.WebGL.Platform.Objects;
using Moralis.WebGL.Platform.Services.Models;
using Moralis.WebGL.Platform.Utilities;
using Cysharp.Threading.Tasks;
using System.Net;
using UnityEngine;

namespace Moralis.WebGL.Platform.Services.ClientServices
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

        public async UniTask<T> FetchAsync<T>(T item, string sessionToken, CancellationToken cancellationToken = default) where T : MoralisObject
        {
            MoralisCommand command = new MoralisCommand($"server/classes/{Uri.EscapeDataString(item.ClassName)}/{Uri.EscapeDataString(item.objectId)}", method: "GET", sessionToken: sessionToken, data: default);
            Tuple<HttpStatusCode, string> cmdResp = await CommandRunner.RunCommandAsync(command, cancellationToken: cancellationToken);
         
            T resp = default;

            if ((int)cmdResp.Item1 < 400)
            {
                resp = (T)JsonSerializer.Deserialize<T>(cmdResp.Item2);
                    
            }

            return resp;
        }

        public async UniTask<string> SaveAsync(MoralisObject item, IDictionary<string, IMoralisFieldOperation> operations, string sessionToken, CancellationToken cancellationToken = default)
        {
            MoralisCommand command = new MoralisCommand(item.objectId == null ? $"server/classes/{Uri.EscapeDataString(item.ClassName)}" : $"server/classes/{Uri.EscapeDataString(item.ClassName)}/{item.objectId}", method: item.objectId is null ? "POST" : "PUT", sessionToken: sessionToken, data: operations is { } && operations.Count > 0 ? JsonSerializer.Serialize(operations, JsonSerializer.DefaultOptions).JsonInsertParseDate() : JsonSerializer.Serialize(item, JsonSerializer.DefaultOptions).JsonInsertParseDate());
            Tuple<HttpStatusCode, string> cmdResp = await CommandRunner.RunCommandAsync(command, cancellationToken: cancellationToken);
            
            string resp = default;

            if ((int)cmdResp.Item1 < 400)
            {
                resp = cmdResp.Item2;
            }
            else
            {
                Debug.Log($"SaveAsync failed: {cmdResp.Item2}");
            }

            return resp;
        }

        //public IList<Task<T>> SaveAllAsync<T>(IList<T> items, IList<IDictionary<string, IMoralisFieldOperation>> operationsList, string sessionToken, CancellationToken cancellationToken = default) where T : MoralisObject => ExecuteBatchRequests<T>(items.Zip(operationsList, (item, operations) => new MoralisCommand(item.ObjectId is null ? $"server/classes/{Uri.EscapeDataString(item.ClassName)}" : $"server/classes/{Uri.EscapeDataString(item.ClassName)}/{Uri.EscapeDataString(item.ObjectId)}", method: item.ObjectId is null ? "POST" : "PUT", data: operations is { } && operations.Count > 0 ? JsonConvert.SerializeObject(operations, jsonSettings).JsonInsertParseDate() : JsonConvert.SerializeObject(item, jsonSettings).JsonInsertParseDate())).ToList(), sessionToken, cancellationToken).Select(task => task.Result).ToList();

        public async UniTask DeleteAsync(MoralisObject item, string sessionToken, CancellationToken cancellationToken = default)
        {
            Tuple<HttpStatusCode, string> cmdResp = await CommandRunner.RunCommandAsync(new MoralisCommand($"server/classes/{item.ClassName}/{item.objectId}", method: "DELETE", sessionToken: sessionToken, data: null), cancellationToken: cancellationToken);
            
            if ((int)cmdResp.Item1 >= 400)
            {
                Debug.Log($"SaveAsync failed: {cmdResp.Item2}");
            }
        }
               
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

        async UniTask<IList<UniTask<IDictionary<string, object>>>> ExecuteBatchRequestAsync<T>(IList<MoralisCommand> requests, string sessionToken, CancellationToken cancellationToken = default)
        {
            int batchSize = requests.Count;

            List<UniTask<IDictionary<string, object>>> tasks = new List<UniTask<IDictionary<string, object>>> { };
            List<UniTaskCompletionSource<IDictionary<string, object>>> completionSources = new List<UniTaskCompletionSource<IDictionary<string, object>>> { };

            for (int i = 0; i < batchSize; ++i)
            {
                UniTaskCompletionSource<IDictionary<string, object>> tcs = new UniTaskCompletionSource<IDictionary<string, object>>();

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

            Tuple<HttpStatusCode, string> cmdResp = await CommandRunner.RunCommandAsync(command, cancellationToken: cancellationToken);

            if ((int)cmdResp.Item1 < 400)
            {
                foreach (UniTaskCompletionSource<IDictionary<string, object>> tcs in completionSources)
                {
                    if (UniTaskStatus.Faulted.Equals(tcs.Task.Status))
                        tcs.TrySetException(new Exception("Update failed."));
                    else if (UniTaskStatus.Canceled.Equals(tcs.Task.Status))
                        tcs.TrySetCanceled();
                }
                

                IList<object> resultsArray = Conversion.As<IList<object>>(cmdResp.Item2);
                int resultLength = resultsArray.Count;

                if (resultLength != batchSize)
                {
                    foreach (UniTaskCompletionSource<IDictionary<string, object>> completionSource in completionSources)
                        completionSource.TrySetException(new InvalidOperationException($"Batch command result count expected: {batchSize} but was: {resultLength}."));
                }
                else
                {
                    for (int i = 0; i < batchSize; ++i)
                    {
                        Dictionary<string, object> result = resultsArray[i] as Dictionary<string, object>;
                        UniTaskCompletionSource<IDictionary<string, object>> target = completionSources[i];

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
                }
            }

            return tasks;
        }
    }
}
