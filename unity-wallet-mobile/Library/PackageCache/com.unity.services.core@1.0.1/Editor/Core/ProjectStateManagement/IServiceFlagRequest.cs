using UnityEngine;

namespace Unity.Services.Core.Editor
{
    interface IServiceFlagRequest
    {
        IAsyncOperation<IServiceFlags> FetchServiceFlags();
    }
}
