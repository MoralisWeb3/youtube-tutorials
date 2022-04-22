using System;
using System.Collections.Generic;
using System.Linq;

namespace Moralis.WebGL.Platform.Utilities
{
    /// <summary>
    /// A wrapper over a dictionary from value generator to value. Uses the fact that lambda expressions in a specific location are cached, so the cost of instantiating a generator delegate is only incurred once at the call site of <see cref="GetValue{TData}(Func{TData})"/> and subsequent calls look up the result of the first generation from the dictionary based on the hash of the generator delegate. This is effectively a lazy initialization mechanism that allows the member type to remain unchanged.
    /// </summary>
    internal class LateInitializer
    {
        Lazy<Dictionary<Func<object>, object>> Storage { get; set; } = new Lazy<Dictionary<Func<object>, object>> { };

        public TData GetValue<TData>(Func<TData> generator)
        {
            lock (generator)
            {
                if (Storage.IsValueCreated && Storage.Value.Keys.OfType<Func<TData>>().FirstOrDefault() is { } key && Storage.Value.TryGetValue(key as Func<object>, out object data))
                {
                    return (TData) data;
                }
                else
                {
                    TData result = generator.Invoke();

                    Storage.Value.Add(generator as Func<object>, result);
                    return result;
                }
            }
        }

        public bool ClearValue<TData>()
        {
            lock (Storage)
            {
                if (Storage.IsValueCreated && Storage.Value.Keys.OfType<Func<TData>>().FirstOrDefault() is { } key)
                {
                    lock (key)
                    {
                        Storage.Value.Remove(key as Func<object>);
                        return true;
                    }
                }
            }

            return false;
        }

        public bool SetValue<TData>(TData value, bool initialize = true)
        {
            lock (Storage)
            {
                if (Storage.IsValueCreated && Storage.Value.Keys.OfType<Func<TData>>().FirstOrDefault() is { } key)
                {
                    lock (key)
                    {
                        Storage.Value[key as Func<object>] = value;
                        return true;
                    }
                }
                else if (initialize)
                {
                    Storage.Value[new Func<TData>(() => value) as Func<object>] = value;
                    return true;
                }
            }

            return false;
        }

        public bool Reset()
        {
            lock (Storage)
            {
                if (Storage.IsValueCreated)
                {
                    Storage.Value.Clear();
                    return true;
                }
            }

            return false;
        }

        public bool Used => Storage.IsValueCreated;
    }
}
