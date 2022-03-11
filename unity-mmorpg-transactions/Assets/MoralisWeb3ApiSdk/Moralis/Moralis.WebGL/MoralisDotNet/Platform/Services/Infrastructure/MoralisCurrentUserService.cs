using System;
using System.Threading;
using Moralis.WebGL.Platform.Abstractions;
using Moralis.WebGL.Platform.Objects;
using Moralis.WebGL.Platform.Utilities;
using Cysharp.Threading.Tasks;

namespace Moralis.WebGL.Platform.Services.Infrastructure
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

        public async UniTask SetAsync(TUser user, CancellationToken cancellationToken)
        {
            UniTask saveTask = default;

            if (user is null)
            {
                IDataCache<string, object> loadResp = await StorageController.LoadAsync();
                await loadResp.RemoveAsync(nameof(CurrentUser));
            }
            else
            {
                string data = JsonSerializer.Serialize(user);

                IDataCache<string, object> loadResp = await StorageController.LoadAsync();
                await loadResp.AddAsync(nameof(CurrentUser), data);
            }

            CurrentUser = user;
        }

        public async UniTask<TUser> GetAsync(IServiceHub<TUser> serviceHub, CancellationToken cancellationToken = default) 
        {
            TUser cachedCurrent = null;

            lock (Mutex)
                cachedCurrent = CurrentUser;

            if (cachedCurrent == null)
            {
                object data = default;
                IDataCache<string, object> loadResp = await StorageController.LoadAsync();
                loadResp.TryGetValue(nameof(CurrentUser), out data);
                TUser user = default;

                if (data is string)
                {
                    user = (TUser)JsonSerializer.Deserialize<TUser>(data.ToString());
                    CurrentUser = user;
                    cachedCurrent = user;
                }
            }

            return cachedCurrent;
        }

        public async UniTask<bool> ExistsAsync(CancellationToken cancellationToken)
        {
            bool result = true;

            if (CurrentUser == null)
            {
                IDataCache<string, object> loadResp = await StorageController.LoadAsync();
                result = loadResp.ContainsKey(nameof(CurrentUser));
            }

            return result;
        }

        public bool IsCurrent(TUser user)
        {
            lock (Mutex)
                return CurrentUser == user;
        }

        public void ClearFromMemory() => CurrentUser = default;

        public async UniTask ClearFromDiskAsync()
        {
            lock (Mutex)
            {
                ClearFromMemory();
            }

            IDataCache<string, object> loadResp = await StorageController.LoadAsync();
            await loadResp.RemoveAsync(nameof(CurrentUser));
        }

        public async UniTask<string> GetCurrentSessionTokenAsync(IServiceHub<TUser> serviceHub, CancellationToken cancellationToken = default)
        {
            string result = null;
            TUser user = await GetAsync(serviceHub, cancellationToken);
            result = user?.sessionToken;

            return result;
        }

        public async UniTask LogOutAsync(IServiceHub<TUser> serviceHub, CancellationToken cancellationToken = default)
        {
            TUser user = await GetAsync(serviceHub, cancellationToken);
            await ClearFromDiskAsync();
        }

    }
}
