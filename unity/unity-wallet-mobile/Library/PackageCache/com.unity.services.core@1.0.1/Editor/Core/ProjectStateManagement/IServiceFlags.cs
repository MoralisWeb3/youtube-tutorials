using System.Collections.Generic;

namespace Unity.Services.Core.Editor
{
    interface IServiceFlags
    {
        List<string> GetFlagNames();
        
        bool DoesFlagExist(string flagName);

        bool IsFlagActive(string flagName);
    }
}
