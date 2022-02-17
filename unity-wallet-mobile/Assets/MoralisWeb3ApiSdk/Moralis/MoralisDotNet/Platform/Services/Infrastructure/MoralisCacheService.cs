using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moralis.Platform.Abstractions;
using Moralis.Platform.Objects;
using Moralis.Platform.Utilities;
using static Moralis.Platform.Resources;

namespace Moralis.Platform.Services.Infrastructure
{
    /// <summary>
    /// Implements `IStorageController` for PCL targets, based off of PCLStorage.
    /// </summary>
    public class MoralisCacheService<TUser> : IDiskFileCacheService where TUser : MoralisUser
    {
        class FileBackedCache : IDataCache<string, object>
        {
            public FileBackedCache(FileInfo file) => File = file;

            internal Task SaveAsync() => Lock(() => File.WriteContentAsync(JsonUtilities.Encode(Storage)));

            internal Task LoadAsync() => File.ReadAllTextAsync().ContinueWith(task =>
            {
                lock (Mutex)
                {
                    try
                    {
                        Storage = JsonUtilities.Parse(task.Result) as Dictionary<string, object>;
                    }
                    catch
                    {
                        Storage = new Dictionary<string, object> { };
                    }
                }
            });

            // TODO: Check if the call to ToDictionary is necessary here considering contents is IDictionary<string object>.

            internal void Update(IDictionary<string, object> contents) => Lock(() => Storage = contents.ToDictionary(element => element.Key, element => element.Value));

            public Task AddAsync(string key, object value)
            {
                lock (Mutex)
                {
                    Storage[key] = value;
                    return SaveAsync();
                }
            }

            public Task RemoveAsync(string key)
            {
                lock (Mutex)
                {
                    Storage.Remove(key);
                    return SaveAsync();
                }
            }

            public void Add(string key, object value) => throw new NotSupportedException(FileBackedCacheSynchronousMutationNotSupportedMessage);

            public bool Remove(string key) => throw new NotSupportedException(FileBackedCacheSynchronousMutationNotSupportedMessage);

            public void Add(KeyValuePair<string, object> item) => throw new NotSupportedException(FileBackedCacheSynchronousMutationNotSupportedMessage);

            public bool Remove(KeyValuePair<string, object> item) => throw new NotSupportedException(FileBackedCacheSynchronousMutationNotSupportedMessage);

            public bool ContainsKey(string key) => Lock(() => Storage.ContainsKey(key));

            public bool TryGetValue(string key, out object value)
            {
                lock (Mutex)
                {
                    return (Result: Storage.TryGetValue(key, out object found), value = found).Result;
                }
            }

            public void Clear() => Lock(() => Storage.Clear());

            public bool Contains(KeyValuePair<string, object> item) => Lock(() => Elements.Contains(item));

            public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => Lock(() => Elements.CopyTo(array, arrayIndex));

            public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => Storage.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => Storage.GetEnumerator();

            public FileInfo File { get; set; }

            public object Mutex { get; set; } = new object { };

            // ALTNAME: Operate

            TResult Lock<TResult>(Func<TResult> operation)
            {
                lock (Mutex)
                {
                    return operation.Invoke();
                }
            }

            void Lock(Action operation)
            {
                lock (Mutex)
                {
                    operation.Invoke();
                }
            }

            ICollection<KeyValuePair<string, object>> Elements => Storage as ICollection<KeyValuePair<string, object>>;

            Dictionary<string, object> Storage { get; set; } = new Dictionary<string, object> { };

            public ICollection<string> Keys => Storage.Keys;

            public ICollection<object> Values => Storage.Values;

            public int Count => Storage.Count;

            public bool IsReadOnly => Elements.IsReadOnly;

            public object this[string key]
            {
                get => Storage[key];
                set => throw new NotSupportedException(FileBackedCacheSynchronousMutationNotSupportedMessage);
            }
        }

        /// <summary>
        /// Set this for systems (Unity based Android, iOs, etc.) that may not 
        /// be able to access Environment.SpecialFolder.LocalApplicationData
        /// </summary>
        public static string BaseFilePath { get; set; }

        FileInfo File { get; set; }
        FileBackedCache Cache { get; set; }
        TaskQueue Queue { get; } = new TaskQueue { };

        /// <summary>
        /// Creates a Moralis storage controller and attempts to extract a previously created settings storage file from the persistent storage location.
        /// </summary>
        public MoralisCacheService() { }

        /// <summary>
        /// Creates a Moralis storage controller with the provided <paramref name="file"/> wrapper.
        /// </summary>
        /// <param name="file">The file wrapper that the storage controller instance should target</param>
        public MoralisCacheService(FileInfo file) => EnsureCacheExists(file);

        FileBackedCache EnsureCacheExists(FileInfo file = default) => Cache ??= new FileBackedCache(file ?? (File ??= PersistentCacheFile));

        /// <summary>
        /// Loads a settings dictionary from the file wrapped by <see cref="File"/>.
        /// </summary>
        /// <returns>A storage dictionary containing the deserialized content of the storage file targeted by the <see cref="CacheController"/> instance</returns>
        public Task<IDataCache<string, object>> LoadAsync()
        {
            // Check if storage dictionary is already created from the controllers file (create if not)
            EnsureCacheExists();

            // Load storage dictionary content async and return the resulting dictionary type
            return Queue.Enqueue(toAwait => toAwait.ContinueWith(_ => Cache.LoadAsync().OnSuccess(__ => Cache as IDataCache<string, object>)).Unwrap(), CancellationToken.None);
        }

        /// <summary>
        /// Saves the requested data.
        /// </summary>
        /// <param name="contents">The data to be saved.</param>
        /// <returns>A data cache containing the saved data.</returns>
        public Task<IDataCache<string, object>> SaveAsync(IDictionary<string, object> contents) => Queue.Enqueue(toAwait => toAwait.ContinueWith(_ =>
        {
            EnsureCacheExists().Update(contents);
            return Cache.SaveAsync().OnSuccess(__ => Cache as IDataCache<string, object>);
        }).Unwrap());

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void RefreshPaths() => Cache = new FileBackedCache(File = PersistentCacheFile);

        // TODO: Attach the following method to AppDomain.CurrentDomain.ProcessExit if that actually ever made sense for anything except randomly generated file names, otherwise attach the delegate when it is known the file name is a randomly generated string.

        /// <summary>
        /// Clears the data controlled by this class.
        /// </summary>
        public void Clear()
        {
            if (new FileInfo(FallbackRelativeCacheFilePath) is { Exists: true } file)
            {
                file.Delete();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string RelativeCacheFilePath { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string AbsoluteCacheFilePath
        {
            get => StoredAbsoluteCacheFilePath ?? Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), RelativeCacheFilePath ?? FallbackRelativeCacheFilePath));
            set => StoredAbsoluteCacheFilePath = value;
        }

        string StoredAbsoluteCacheFilePath { get; set; }

        /// <summary>
        /// Gets the calculated persistent storage file fallback path for this app execution.
        /// </summary>
        public string FallbackRelativeCacheFilePath => StoredFallbackRelativeCacheFilePath ??= IdentifierBasedRelativeCacheLocationGenerator.Fallback.GetRelativeCacheFilePath(new MutableServiceHub<TUser> { CacheService = this });

        string StoredFallbackRelativeCacheFilePath { get; set; }

        /// <summary>
        /// Gets or creates the file pointed to by <see cref="AbsoluteCacheFilePath"/> and returns it's wrapper as a <see cref="FileInfo"/> instance.
        /// </summary>
        public FileInfo PersistentCacheFile
        {
            get
            {
                Directory.CreateDirectory(AbsoluteCacheFilePath.Substring(0, AbsoluteCacheFilePath.LastIndexOf(Path.DirectorySeparatorChar)));

                FileInfo file = new FileInfo(AbsoluteCacheFilePath);
                if (!file.Exists)
                    using (file.Create())
                        ; // Hopefully the JIT doesn't no-op this. The behaviour of the "using" clause should dictate how the stream is closed, to make sure it happens properly.

                return file;
            }
        }

        /// <summary>
        /// Gets the file wrapper for the specified <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The relative path to the target file</param>
        /// <returns>An instance of <see cref="FileInfo"/> wrapping the the <paramref name="path"/> value</returns>
        public FileInfo GetRelativeFile(string path)
        {
            return DefineRelativeFilePath(path);
        }

        public static FileInfo DefineRelativeFilePath(string path)
        {
            string basePath = BaseFilePath ?? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            Directory.CreateDirectory((path = Path.GetFullPath(Path.Combine(basePath, path))).Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar)));
            return new FileInfo(path);
        }
        // MoveAsync

        /// <summary>
        /// Transfers a file from <paramref name="originFilePath"/> to <paramref name="targetFilePath"/>.
        /// </summary>
        /// <param name="originFilePath"></param>
        /// <param name="targetFilePath"></param>
        /// <returns>A task that completes once the file move operation form <paramref name="originFilePath"/> to <paramref name="targetFilePath"/> completes.</returns>
        public async Task TransferAsync(string originFilePath, string targetFilePath)
        {
            if (!String.IsNullOrWhiteSpace(originFilePath) && !String.IsNullOrWhiteSpace(targetFilePath) && new FileInfo(originFilePath) is { Exists: true } originFile && new FileInfo(targetFilePath) is { } targetFile)
            {
                using StreamWriter writer = new StreamWriter(targetFile.OpenWrite(), Encoding.Unicode);
                using StreamReader reader = new StreamReader(originFile.OpenRead(), Encoding.Unicode);

                await writer.WriteAsync(await reader.ReadToEndAsync());
            }
        }
    }

}
