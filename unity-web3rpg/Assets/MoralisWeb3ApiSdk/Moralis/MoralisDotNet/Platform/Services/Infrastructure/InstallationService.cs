using System;
using System.Threading.Tasks;
using Moralis.Platform.Abstractions;
using Moralis.Platform.Utilities;

namespace Moralis.Platform.Services.Infrastructure
{
    public class InstallationService : IInstallationService
    {

        static string InstallationIdKey { get; } = "InstallationId";

        object Mutex { get; } = new object { };

        public Guid? InstallationId { get; private set; }

        ICacheService StorageController { get; }

        public InstallationService(ICacheService storageController) => StorageController = storageController;

        public Task SetAsync(Guid? installationId)
        {
            lock (Mutex)
            {

                Task saveTask = installationId is { } ? StorageController.LoadAsync().OnSuccess(storage => storage.Result.AddAsync(InstallationIdKey, installationId.ToString())).Unwrap() : StorageController.LoadAsync().OnSuccess(storage => storage.Result.RemoveAsync(InstallationIdKey)).Unwrap();

                InstallationId = installationId;
                return saveTask;
            }
        }

        public Task<Guid?> GetAsync()
        {
            lock (Mutex)
                if (InstallationId is { })
                    return Task.FromResult(InstallationId);

            return StorageController.LoadAsync().OnSuccess(storageTask =>
            {
                storageTask.Result.TryGetValue(InstallationIdKey, out object id);

                try
                {
                    lock (Mutex)
                        return Task.FromResult(InstallationId = new Guid(id as string));
                }
                catch (Exception)
                {
                    Guid newInstallationId = Guid.NewGuid();
                    return SetAsync(newInstallationId).OnSuccess<Guid?>(_ => newInstallationId);
                }
            })
            .Unwrap();
        }

        public Task ClearAsync() => SetAsync(null);
    }
}
