using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Moralis.Platform.Abstractions;
using Moralis.Platform.Operations;
using Moralis.Platform.Utilities;

namespace Moralis.Platform.Objects
{
    /// <summary>
    /// Base class for any objects that can be processed via Moralis. 
    /// Note: JsonIgnore internals just incase someone changes JsonSerializerSettings 
    /// </summary>
    public class MoralisObject
    {
        public MoralisObject()
        {
            this.ClassName = this.GetType().Name;
            this.objectId = null; 
            this.createdAt = DateTime.Now;
            this.updatedAt = DateTime.Now;
            this.ACL = new MoralisAcl();
            this.IsDirty = false;
            this.sessionToken = string.Empty;
            this.ObjectService = null;
        }

        public MoralisObject(string className, 
            string objectId = null,
            string sessionToken = null,
            DateTime? createdAt = null,
            DateTime? updatedAt = null,
            MoralisAcl ACL = null)
        {
            this.ClassName = className;
            this.objectId = objectId;
            this.sessionToken = sessionToken;
            this.createdAt = createdAt;
            this.updatedAt = updatedAt;
            this.ACL = ACL;
            this.IsDirty = true;
        }

        public string objectId;
        public string sessionToken;
        public DateTime? createdAt;
        public DateTime? updatedAt;
        public MoralisAcl ACL; 
        public string ClassName { get; set; }
        internal bool IsDirty { get; set; }
        internal IObjectService ObjectService { get; set; }
        internal TaskQueue TaskQueue { get; } = new TaskQueue { };
        internal object Mutex { get; } = new object { };
        internal IDictionary<string, IMoralisFieldOperation> CurrentOperations
        {
            get
            {
                lock (Mutex)
                {
                    return OperationSetQueue.Count > 0 ? OperationSetQueue.Last.Value : new Dictionary<string, IMoralisFieldOperation>();
                }
            }
        }

        LinkedList<IDictionary<string, IMoralisFieldOperation>> OperationSetQueue { get; } = new LinkedList<IDictionary<string, IMoralisFieldOperation>>();

        /// <summary>
        /// Deletes this object on the server.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task DeleteAsync(CancellationToken cancellationToken = default) => TaskQueue.Enqueue(toAwait => DeleteAsync(toAwait, cancellationToken), cancellationToken);

        public virtual async Task<bool> SaveAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                // MoralisUser is a special case not all properties can be passed to save.
                if (this is MoralisUser) ((MoralisUser)this).SetSaving(true);

                IDictionary<string, IMoralisFieldOperation> operations = this.StartSave();
                string resp = await this.ObjectService.SaveAsync(this, operations, sessionToken, cancellationToken);

                Dictionary<string, object> obj = JsonUtilities.Parse(resp) as Dictionary<string, object>;

                if (obj.ContainsKey("objectId"))
                {
                    this.objectId = (string)obj["objectId"];
                }

                if (obj.ContainsKey("createdAt"))
                {
                    DateTime dt = DateTime.Now;
                    if (DateTime.TryParse(obj["createdAt"].ToString(), out dt))
                    {
                        this.createdAt = dt;
                    }
                }

                if (obj.ContainsKey("updatedAt"))
                {
                    DateTime dt = DateTime.Now;
                    if (DateTime.TryParse(obj["updatedAt"].ToString(), out dt))
                    {
                        this.updatedAt = dt;
                    }
                }

                // Set user saving false so that local persistence has full object.
                if (this is MoralisUser) ((MoralisUser)this).SetSaving(false);

                Console.WriteLine($"Save response: {resp}");

                OperationSetQueue.Clear();

                return true;
            }
            catch (Exception exp)
            {
                Console.WriteLine($"Save failed: {exp.Message}");
            }

            return false;

        }

        /// <summary>
        /// Atomically increments the given key by 1.
        /// </summary>
        /// <param name="key">The key to increment.</param>
        public void Increment(string key) => Increment(key, 1);

        /// <summary>
        /// Atomically increments the given key by the given number.
        /// </summary>
        /// <param name="key">The key to increment.</param>
        /// <param name="amount">The amount to increment by.</param>
        public void Increment(string key, byte amount)
        {
            lock (Mutex)
            {
                PerformOperation(key, new MoralisIncrementOperation(amount));
            }
        }
        public void Increment(string key, int amount)
        {
            lock (Mutex)
            {
                PerformOperation(key, new MoralisIncrementOperation(amount));
            }
        }
        public void Increment(string key, long amount)
        {
            lock (Mutex)
            {
                PerformOperation(key, new MoralisIncrementOperation(amount));
            }
        }
        /// <summary>
        /// Atomically increments the given key by the given number.
        /// </summary>
        /// <param name="key">The key to increment.</param>
        /// <param name="amount">The amount to increment by.</param>
        public void Increment(string key, double amount)
        {
            lock (Mutex)
            {
                PerformOperation(key, new MoralisIncrementOperation(amount));
            }
        }
        public void Increment(string key, decimal amount)
        {
            lock (Mutex)
            {
                PerformOperation(key, new MoralisIncrementOperation(amount));
            }
        }
        public void Increment(string key, float amount)
        {
            lock (Mutex)
            {
                PerformOperation(key, new MoralisIncrementOperation(amount));
            }
        }
        /// <summary>
        /// Pushes new operations onto the queue and returns the current set of operations.
        /// </summary>
        internal IDictionary<string, IMoralisFieldOperation> StartSave()
        {
            lock (Mutex)
            {
                IDictionary<string, IMoralisFieldOperation> currentOperations = CurrentOperations;
                OperationSetQueue.AddLast(new Dictionary<string, IMoralisFieldOperation>());
                return currentOperations;
            }
        }

        internal Task DeleteAsync(Task toAwait, CancellationToken cancellationToken)
        {
            if (String.IsNullOrWhiteSpace(objectId))
            {
                return Task.FromResult(0);
            }

            string sessionToken = GetCurrentSessionToken();

            return toAwait.OnSuccess(_ => this.ObjectService.DeleteAsync(this, sessionToken, cancellationToken)).Unwrap().OnSuccess(_ => IsDirty = true);
        }

        //internal virtual Task<T> FetchAsyncInternal<T>(Task toAwait, CancellationToken cancellationToken) where T : MoralisObject => toAwait.OnSuccess(_ => ObjectId == null ? throw new InvalidOperationException("Cannot refresh an object that hasn't been saved to the server.") : Services.ObjectService.FetchAsync(this, GetCurrentSessionToken(), Services, cancellationToken)).Unwrap().OnSuccess(task =>
        //{
        //    HandleFetchResult(task.Result);
        //    return this;
        //});


        internal string GetCurrentSessionToken() => this.sessionToken;

        /// <summary>
        /// Fetches this object with the data from the server.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        //internal Task<MoralisObject> FetchAsyncInternal(CancellationToken cancellationToken) => TaskQueue.Enqueue(toAwait => FetchAsyncInternal(toAwait, cancellationToken), cancellationToken);

        //internal Task<MoralisObject> FetchIfNeededAsyncInternal(Task toAwait, CancellationToken cancellationToken) => !IsDataAvailable ? FetchAsyncInternal(toAwait, cancellationToken) : Task.FromResult(this);

        /// <summary>
        /// If this MoralisObject has not been fetched (i.e. <see cref="IsDataAvailable"/> returns
        /// false), fetches this object with the data from the server.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        //internal Task<MoralisObject> FetchIfNeededAsyncInternal(CancellationToken cancellationToken) => TaskQueue.Enqueue(toAwait => FetchIfNeededAsyncInternal(toAwait, cancellationToken), cancellationToken);

        /// <summary>
        /// PerformOperation is like setting a value at an index, but instead of
        /// just taking a new value, it takes a MoralisFieldOperation that modifies the value.
        /// </summary>
        internal void PerformOperation(string key, IMoralisFieldOperation operation)
        {
            lock (Mutex)
            {
                
                PropertyInfo prop = this.GetType().GetProperty(key, BindingFlags.Public | BindingFlags.Instance);
                
                if (null != prop && prop.CanWrite)
                {
                    object oldValue = prop.GetValue(this);
                    object newValue = operation.Apply(oldValue, key);
                    prop.SetValue(this, newValue);
                }

                bool wasDirty = CurrentOperations.Count > 0;
                CurrentOperations.TryGetValue(key, out IMoralisFieldOperation oldOperation);
                IMoralisFieldOperation newOperation = operation.MergeWithPrevious(oldOperation);
                CurrentOperations[key] = newOperation;

            }
        }
        /*
        /// <summary>
        /// Deletes this object on the server.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task DeleteAsync(CancellationToken cancellationToken = default) => TaskQueue.Enqueue(toAwait => DeleteAsync(toAwait, cancellationToken), cancellationToken);

        /// <summary>
        /// Gets a value for the key of a particular type.
        /// <typeparam name="T">The type to convert the value to. Supported types are
        /// MoralisObject and its descendents, Moralis types such as MoralisRelation and MoralisGeopoint,
        /// primitive types,IList&lt;T&gt;, IDictionary&lt;string, T&gt;, and strings.</typeparam>
        /// <param name="key">The key of the element to get.</param>
        /// <exception cref="KeyNotFoundException">The property is
        /// retrieved and <paramref name="key"/> is not found.</exception>
        /// </summary>
        public T Get<T>(string key)
        {
            return Conversion.To<T>(this[key]);
        }

        /// <summary>
        /// Access or create a Relation value for a key.
        /// </summary>
        /// <typeparam name="T">The type of object to create a relation for.</typeparam>
        /// <param name="key">The key for the relation field.</param>
        /// <returns>A MoralisRelation for the key.</returns>
        public MoralisRelation<T> GetRelation<T>(string key) where T : MoralisObject
        {
            // All the sanity checking is done when add or remove is called.

            TryGetValue(key, out MoralisRelation<T> relation);
            return relation ?? new MoralisRelation<T>(this, key);
        }

        /// <summary>
        /// A helper function for checking whether two MoralisObjects point to
        /// the same object in the cloud.
        /// </summary>
        public bool HasSameId(MoralisObject other)
        {
            lock (Mutex)
            {
                return other is { } && Equals(ClassName, other.ClassName) && Equals(ObjectId, other.ObjectId);
            }
        }

        #region Atomic Increment

        /// <summary>
        /// Atomically increments the given key by 1.
        /// </summary>
        /// <param name="key">The key to increment.</param>
        public void Increment(string key) => Increment(key, 1);

        /// <summary>
        /// Atomically increments the given key by the given number.
        /// </summary>
        /// <param name="key">The key to increment.</param>
        /// <param name="amount">The amount to increment by.</param>
        public void Increment(string key, long amount)
        {
            lock (Mutex)
            {
                CheckKeyIsMutable(key);
                PerformOperation(key, new MoralisIncrementOperation(amount));
            }
        }

        /// <summary>
        /// Atomically increments the given key by the given number.
        /// </summary>
        /// <param name="key">The key to increment.</param>
        /// <param name="amount">The amount to increment by.</param>
        public void Increment(string key, double amount)
        {
            lock (Mutex)
            {
                CheckKeyIsMutable(key);
                PerformOperation(key, new MoralisIncrementOperation(amount));
            }
        }

        /// <summary>
        /// Indicates whether key is unsaved for this MoralisObject.
        /// </summary>
        /// <param name="key">The key to check for.</param>
        /// <returns><c>true</c> if the key has been altered and not saved yet, otherwise
        /// <c>false</c>.</returns>
        public bool IsKeyDirty(string key)
        {
            lock (Mutex)
            {
                return CurrentOperations.ContainsKey(key);
            }
        }

        /// <summary>
        /// Removes a key from the object's data if it exists.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        public virtual void Remove(string key)
        {
            lock (Mutex)
            {
                CheckKeyIsMutable(key);
                PerformOperation(key, MoralisDeleteOperation.Instance);
            }
        }

        /// <summary>
        /// Atomically removes all instances of the objects in <paramref name="values"/>
        /// from the list associated with the given key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="values">The objects to remove.</param>
        public void RemoveAllFromList<T>(string key, IEnumerable<T> values)
        {
            lock (Mutex)
            {
                CheckKeyIsMutable(key);
                PerformOperation(key, new MoralisRemoveOperation(values.Cast<object>()));
            }
        }

        /// <summary>
        /// Clears any changes to this object made since the last call to <see cref="SaveAsync()"/>.
        /// </summary>
        public void Revert()
        {
            lock (Mutex)
            {
                if (CurrentOperations.Count > 0)
                {
                    CurrentOperations.Clear();
                    RebuildEstimatedData();
                    OnPropertyChanged(nameof(IsDirty));
                }
            }
        }

        /// <summary>
        /// Saves this object to the server.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task SaveAsync(CancellationToken cancellationToken = default) => TaskQueue.Enqueue(toAwait => SaveAsync(toAwait, cancellationToken), cancellationToken);

        /// <summary>
        /// Populates result with the value for the key, if possible.
        /// </summary>
        /// <typeparam name="T">The desired type for the value.</typeparam>
        /// <param name="key">The key to retrieve a value for.</param>
        /// <param name="result">The value for the given key, converted to the
        /// requested type, or null if unsuccessful.</param>
        /// <returns>true if the lookup and conversion succeeded, otherwise
        /// false.</returns>
        public bool TryGetValue<T>(string key, out T result)
        {
            lock (Mutex)
            {
                if (ContainsKey(key))
                {
                    try
                    {
                        T temp = Conversion.To<T>(this[key]);
                        result = temp;
                        return true;
                    }
                    catch
                    {
                        result = default;
                        return false;
                    }
                }

                result = default;
                return false;
            }
        }

        #endregion

        #region Delete Object

        internal Task DeleteAsync(Task toAwait, CancellationToken cancellationToken)
        {
            if (ObjectId == null)
            {
                return Task.FromResult(0);
            }

            string sessionToken = Services.GetCurrentSessionToken();

            return toAwait.OnSuccess(_ => Services.ObjectController.DeleteAsync(State, sessionToken, cancellationToken)).Unwrap().OnSuccess(_ => IsDirty = true);
        }

        internal virtual Task<MoralisObject> FetchAsyncInternal(Task toAwait, CancellationToken cancellationToken) => toAwait.OnSuccess(_ => ObjectId == null ? throw new InvalidOperationException("Cannot refresh an object that hasn't been saved to the server.") : Services.ObjectController.FetchAsync(State, Services.GetCurrentSessionToken(), Services, cancellationToken)).Unwrap().OnSuccess(task =>
        {
            HandleFetchResult(task.Result);
            return this;
        });

        #endregion

        #region Fetch Object(s)

        /// <summary>
        /// Fetches this object with the data from the server.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        internal Task<MoralisObject> FetchAsyncInternal(CancellationToken cancellationToken) => TaskQueue.Enqueue(toAwait => FetchAsyncInternal(toAwait, cancellationToken), cancellationToken);

        internal Task<MoralisObject> FetchIfNeededAsyncInternal(Task toAwait, CancellationToken cancellationToken) => !IsDataAvailable ? FetchAsyncInternal(toAwait, cancellationToken) : Task.FromResult(this);

        /// <summary>
        /// If this MoralisObject has not been fetched (i.e. <see cref="IsDataAvailable"/> returns
        /// false), fetches this object with the data from the server.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        internal Task<MoralisObject> FetchIfNeededAsyncInternal(CancellationToken cancellationToken) => TaskQueue.Enqueue(toAwait => FetchIfNeededAsyncInternal(toAwait, cancellationToken), cancellationToken);

        internal void HandleFailedSave(IDictionary<string, IMoralisFieldOperation> operationsBeforeSave)
        {
            lock (Mutex)
            {
                LinkedListNode<IDictionary<string, IMoralisFieldOperation>> opNode = OperationSetQueue.Find(operationsBeforeSave);
                IDictionary<string, IMoralisFieldOperation> nextOperations = opNode.Next.Value;
                bool wasDirty = nextOperations.Count > 0;
                OperationSetQueue.Remove(opNode);

                // Merge the data from the failed save into the next save.

                foreach (KeyValuePair<string, IMoralisFieldOperation> pair in operationsBeforeSave)
                {
                    IMoralisFieldOperation operation1 = pair.Value;

                    nextOperations.TryGetValue(pair.Key, out IMoralisFieldOperation operation2);
                    nextOperations[pair.Key] = operation2 is { } ? operation2.MergeWithPrevious(operation1) : operation1;
                }

                if (!wasDirty && nextOperations == CurrentOperations && operationsBeforeSave.Count > 0)
                {
                    OnPropertyChanged(nameof(IsDirty));
                }
            }
        }

        public virtual void HandleFetchResult(IObjectState serverState)
        {
            lock (Mutex)
            {
                MergeFromServer(serverState);
            }
        }

        internal virtual void HandleSave(IObjectState serverState)
        {
            lock (Mutex)
            {
                IDictionary<string, IMoralisFieldOperation> operationsBeforeSave = OperationSetQueue.First.Value;
                OperationSetQueue.RemoveFirst();

                // Merge the data from the save and the data from the server into serverData.

                MutateState(mutableClone => mutableClone.Apply(operationsBeforeSave));
                MergeFromServer(serverState);
            }
        }

        internal void MergeFromObject(MoralisObject other)
        {
            // If they point to the same instance, we don't need to merge

            lock (Mutex)
            {
                if (this == other)
                {
                    return;
                }
            }

            // Clear out any changes on this object.

            if (OperationSetQueue.Count != 1)
            {
                throw new InvalidOperationException("Attempt to MergeFromObject during save.");
            }

            OperationSetQueue.Clear();

            foreach (IDictionary<string, IMoralisFieldOperation> operationSet in other.OperationSetQueue)
            {
                OperationSetQueue.AddLast(operationSet.ToDictionary(entry => entry.Key, entry => entry.Value));
            }

            lock (Mutex)
            {
                State = other.State;
            }

            RebuildEstimatedData();
        }

        internal virtual void MergeFromServer(IObjectState serverState)
        {
            // Make a new serverData with fetched values.

            Dictionary<string, object> newServerData = serverState.ToDictionary(t => t.Key, t => t.Value);

            lock (Mutex)
            {
                // Trigger handler based on serverState

                if (serverState.ObjectId != null)
                {
                    // If the objectId is being merged in, consider this object to be fetched.

                    Fetched = true;
                    OnPropertyChanged(nameof(IsDataAvailable));
                }

                if (serverState.UpdatedAt != null)
                {
                    OnPropertyChanged(nameof(UpdatedAt));
                }

                if (serverState.CreatedAt != null)
                {
                    OnPropertyChanged(nameof(CreatedAt));
                }

                // We cache the fetched object because subsequent Save operation might flush the fetched objects into Pointers.

                IDictionary<string, MoralisObject> fetchedObject = CollectFetchedObjects();

                foreach (KeyValuePair<string, object> pair in serverState)
                {
                    object value = pair.Value;

                    if (value is MoralisObject)
                    {
                        // Resolve fetched object.

                        MoralisObject entity = value as MoralisObject;

                        if (fetchedObject.ContainsKey(entity.ObjectId))
                        {
                            value = fetchedObject[entity.ObjectId];
                        }
                    }
                    newServerData[pair.Key] = value;
                }

                IsDirty = false;
                MutateState(mutableClone => mutableClone.Apply(serverState.MutatedClone(mutableClone => mutableClone.ServerData = newServerData)));
            }
        }

        internal void MutateState(Action<MutableObjectState> mutator)
        {
            lock (Mutex)
            {
                State = State.MutatedClone(mutator);

                // Refresh the estimated data.

                RebuildEstimatedData();
            }
        }

        /// <summary>
        /// Override to run validations on key/value pairs. Make sure to still
        /// call the base version.
        /// </summary>
        internal virtual void OnSettingValue(ref string key, ref object value)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }
        }

        /// <summary>
        /// PerformOperation is like setting a value at an index, but instead of
        /// just taking a new value, it takes a MoralisFieldOperation that modifies the value.
        /// </summary>
        internal void PerformOperation(string key, IMoralisFieldOperation operation)
        {
            lock (Mutex)
            {
                EstimatedData.TryGetValue(key, out object oldValue);
                object newValue = operation.Apply(oldValue, key);

                if (newValue != MoralisDeleteOperation.Token)
                {
                    EstimatedData[key] = newValue;
                }
                else
                {
                    EstimatedData.Remove(key);
                }

                bool wasDirty = CurrentOperations.Count > 0;
                CurrentOperations.TryGetValue(key, out IMoralisFieldOperation oldOperation);
                IMoralisFieldOperation newOperation = operation.MergeWithPrevious(oldOperation);
                CurrentOperations[key] = newOperation;

                if (!wasDirty)
                {
                    OnPropertyChanged(nameof(IsDirty));
                }

                OnFieldsChanged(new[] { key });
            }
        }

        /// <summary>
        /// Regenerates the estimatedData map from the serverData and operations.
        /// </summary>
        internal void RebuildEstimatedData()
        {
            lock (Mutex)
            {
                EstimatedData.Clear();

                foreach (KeyValuePair<string, object> item in State)
                {
                    EstimatedData.Add(item);
                }
                foreach (IDictionary<string, IMoralisFieldOperation> operations in OperationSetQueue)
                {
                    ApplyOperations(operations, EstimatedData);
                }

                // We've just applied a bunch of operations to estimatedData which
                // may have changed all of its keys. Notify of all keys and properties
                // mapped to keys being changed.

                OnFieldsChanged(default);
            }
        }

        public IDictionary<string, object> ServerDataToJSONObjectForSerialization() => PointerOrLocalIdEncoder.Instance.Encode(State.ToDictionary(pair => pair.Key, pair => pair.Value), Services) as IDictionary<string, object>;

        /// <summary>
        /// Perform Set internally which is not gated by mutability check.
        /// </summary>
        /// <param name="key">key for the object.</param>
        /// <param name="value">the value for the key.</param>
        public void Set(string key, object value)
        {
            lock (Mutex)
            {
                OnSettingValue(ref key, ref value);

                if (!MoralisDataEncoder.Validate(value))
                {
                    throw new ArgumentException("Invalid type for value: " + value.GetType().ToString());
                }

                PerformOperation(key, new MoralisSetOperation(value));
            }
        }

        /// <summary>
        /// Allows subclasses to set values for non-pointer construction.
        /// </summary>
        internal virtual void SetDefaultValues() { }

        public void SetIfDifferent<T>(string key, T value)
        {
            bool hasCurrent = TryGetValue(key, out T current);

            if (value == null)
            {
                if (hasCurrent)
                {
                    PerformOperation(key, MoralisDeleteOperation.Instance);
                }
                return;
            }

            if (!hasCurrent || !value.Equals(current))
            {
                Set(key, value);
            }
        }

        #endregion
        */
    }
}
