using System.Runtime.CompilerServices;

// Test assemblies
#if UNITY_INCLUDE_TESTS
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("Unity.Services.Core.EditorTests")]
#endif
