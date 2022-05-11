using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moralis.Platform
{
    public class ResourceWrapper
    {

        /// <summary>
        ///   Looks up a localized string similar to Mutation of a file-backed cache is an asynchronous operation as the tracked file needs to be modified..
        /// </summary>
        public static string FileBackedCacheSynchronousMutationNotSupportedMessage
        {
            get
            {
                return Resources.ResourceManager.GetString("FileBackedCacheSynchronousMutationNotSupportedMessage", Resources.Culture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Cannot perform file operations with in-memory cache controller..
        /// </summary>
        public static string TransientCacheControllerDiskFileOperationNotSupportedMessage
        {
            get
            {
                return Resources.ResourceManager.GetString("TransientCacheControllerDiskFileOperationNotSupportedMessage", Resources.Culture);
            }
        }
    }
}
