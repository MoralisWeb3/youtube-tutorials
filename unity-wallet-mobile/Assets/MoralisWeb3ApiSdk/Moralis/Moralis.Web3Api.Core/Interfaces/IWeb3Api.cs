
namespace Moralis.Web3Api.Interfaces
{
    public interface IWeb3Api
    {
        /// <summary>
        /// AccountApi operations.
        /// </summary>
        IAccountApi Account { get; }

        /// <summary>
        /// DefiApi operations
        /// </summary>
        IDefiApi Defi { get; }

        /// <summary>
        /// NativeApi operations.
        /// </summary>
        INativeApi Native { get; }

        /// <summary>
        /// ResolveApi operations.
        /// </summary>
        IResolveApi Resolve { get; }

        /// <summary>
        /// StorageApi operations.
        /// </summary>
        IStorageApi Storage { get; }

        /// <summary>
        /// TokenApi operations.
        /// </summary>
        ITokenApi Token { get; }

        /// <summary>
        /// Indicates that the client has been initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Initialize the client using serverUrl. If serverUrl is null default is used. 
        /// ApiKey is passed via Configuration signleton.
        /// </summary>
        /// <param name="serverUrl"></param>
        void Initialize(string serverUrl = null);
    }
}
