namespace Packages.Rider.Editor.ProjectGeneration
{
  internal interface IGUIDGenerator
  {
    string ProjectGuid(string projectName, string assemblyName);
    string SolutionGuid(string projectName, string extension);
  }
}