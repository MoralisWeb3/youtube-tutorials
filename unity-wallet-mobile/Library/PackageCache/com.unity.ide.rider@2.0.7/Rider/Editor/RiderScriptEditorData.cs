using System;
using Packages.Rider.Editor.Util;
using UnityEditor;
using UnityEngine;

namespace Packages.Rider.Editor
{
  internal class RiderScriptEditorData : ScriptableSingleton<RiderScriptEditorData>
  {
    [SerializeField] internal bool hasChanges = true; // sln/csproj files were changed 
    [SerializeField] internal bool shouldLoadEditorPlugin;
    [SerializeField] internal bool initializedOnce;
    [SerializeField] internal SerializableVersion editorBuildNumber;
    [SerializeField] internal RiderPathLocator.ProductInfo productInfo;

    public void Init()
    {
      if (editorBuildNumber == null)
      {
        Invalidate(RiderScriptEditor.CurrentEditor);
      }
    }

    public void Invalidate(string editorInstallationPath)
    {
      var riderBuildNumber = RiderPathLocator.GetBuildNumber(editorInstallationPath);
      editorBuildNumber = riderBuildNumber.ToSerializableVersion();
      productInfo = RiderPathLocator.GetBuildVersion(editorInstallationPath);
      if (riderBuildNumber == null)
        shouldLoadEditorPlugin = false;

      shouldLoadEditorPlugin = riderBuildNumber >= new Version("191.7141.156");
    }
  }
}