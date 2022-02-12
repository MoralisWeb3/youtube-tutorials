using System;
using System.Threading;
using System.Threading.Tasks;
using Moralis.Platform.Abstractions;
using Moralis.Platform.Objects;
using Moralis.Platform.Utilities;

namespace Moralis.Platform.Services.Infrastructure
{
    public class MoralisCurrentUserService<TUser> : ICurrentUserService<TUser> where TUser : MoralisUser
    {
        TUser currentUser;

        object Mutex { get; } = new object { };

        TaskQueue TaskQueue { get; } = new TaskQueue { };

        ICacheService StorageController { get; }

        IJsonSerializer JsonSerializer { get; }

        public MoralisCurrentUserService(ICacheService storageController, IJsonSerializer jsonSerializer) => (StorageController, JsonSerializer) = (storageController, jsonSerializer);

       
        public TUser CurrentUser
        {
            get
            {
                lock (Mutex)
                    return currentUser;
            }
            set
            {
                lock (Mutex)
                    currentUser = value;
            }
        }

        public Task SetAsync(TUser user, CancellationToken cancellationToken) => TaskQueue.Enqueue(toAwait => toAwait.ContinueWith(_ =>
        {
            Task saveTask = default;

            if (user is null)
                saveTask = StorageController.LoadAsync().OnSuccess(task => task.Result.RemoveAsync(nameof(CurrentUser))).Unwrap();
            else
            {
                string data = JsonSerializer.Serialize(user);

                saveTask = StorageController.LoadAsync().OnSuccess(task => task.Result.AddAsync(nameof(CurrentUser), data)).Unwrap();
            }

            CurrentUser = user;
            return saveTask;
        }).Unwrap(), cancellationToken);

        public Task<TUser> GetAsync(IServiceHub<TUser> serviceHub, CancellationToken cancellationToken = default) 
        {
            TUser cachedCurrent;

            lock (Mutex)
                cachedCurrent = CurrentUser;

            return cachedCurrent is { } ? Task.FromResult(cachedCurrent) : TaskQueue.Enqueue(toAwait => toAwait.ContinueWith(_ => StorageController.LoadAsync().OnSuccess(task =>
            {
                task.Result.TryGetValue(nameof(CurrentUser), out object data);
                TUser user = default;

                if (data is string { } serialization)
                    user = (TUser)JsonSerializer.Deserialize<TUser>(serialization);

                return CurrentUser = user;
            })).Unwrap(), cancellationToken);
        }

        public Task<bool> ExistsAsync(CancellationToken cancellationToken) => CurrentUser is { } ? Task.FromResult(true) : TaskQueue.Enqueue(toAwait => toAwait.ContinueWith(_ => StorageController.LoadAsync().OnSuccess(t => t.Result.ContainsKey(nameof(CurrentUser)))).Unwrap(), cancellationToken);

        public bool IsCurrent(TUser user)
        {
            lock (Mutex)
                return CurrentUser == user;
        }

        public void ClearFromMemory() => CurrentUser = default;

        public void ClearFromDisk()
        {
            lock (Mutex)
            {
                ClearFromMemory();

                TaskQueue.Enqueue(toAwait => toAwait.ContinueWith(_ => StorageController.LoadAsync().OnSuccess(t => t.Result.RemoveAsync(nameof(CurrentUser)))).Unwrap().Unwrap(), CancellationToken.None);
            }
        }

        public Task<string> GetCurrentSessionTokenAsync(IServiceHub<TUser> serviceHub, CancellationToken cancellationToken = default) => GetAsync(serviceHub, cancellationToken).OnSuccess(task => task.Result?.sessionToken);

        public Task LogOutAsync(IServiceHub<TUser> serviceHub, CancellationToken cancellationToken = default) => TaskQueue.Enqueue(toAwait => toAwait.ContinueWith(_ => GetAsync(serviceHub, cancellationToken)).Unwrap().OnSuccess(task => ClearFromDisk()), cancellationToken);

    }
}
