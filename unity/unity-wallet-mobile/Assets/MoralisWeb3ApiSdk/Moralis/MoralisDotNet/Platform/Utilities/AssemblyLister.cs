using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Moralis.Platform.Utilities
{
    /// <summary>
    /// A class that lets you list all loaded assemblies in a PCL-compliant way.
    /// </summary>
    public static class Lister
    {
        /// <summary>
        /// Get all of the assemblies used by this application.
        /// </summary>
        public static IEnumerable<Assembly> AllAssemblies
        {
            get
            {
                // For each of the loaded assemblies, deeply walk all of their references.
                HashSet<string> seen = new HashSet<string>();
                return AppDomain.CurrentDomain.GetAssemblies().SelectMany(asm => asm.DeepWalkReferences(seen));
            }
        }

        private static IEnumerable<Assembly> DeepWalkReferences(this Assembly assembly, HashSet<string> seen = null)
        {
            seen ??= new HashSet<string>();

            if (!seen.Add(assembly.FullName))
                return Enumerable.Empty<Assembly>();

            List<Assembly> assemblies = new List<Assembly> { assembly };

            foreach (AssemblyName reference in assembly.GetReferencedAssemblies())
            {
                if (seen.Contains(reference.FullName))
                    continue;

                Assembly referencedAsm = Assembly.Load(reference);
                assemblies.AddRange(referencedAsm.DeepWalkReferences(seen));
            }

            return assemblies;
        }
    }
}
