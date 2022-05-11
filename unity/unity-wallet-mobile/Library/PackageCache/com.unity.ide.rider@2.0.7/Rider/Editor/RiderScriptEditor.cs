using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Packages.Rider.Editor.ProjectGeneration;
using Packages.Rider.Editor.Util;
using Unity.CodeEditor;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Packages.Rider.Editor
{
  [InitializeOnLoad]
  internal class RiderScriptEditor : IExternalCodeEditor
  {
    IDiscovery m_Discoverability;
    IGenerator m_ProjectGeneration;
    RiderInitializer m_Initiliazer = new RiderInitializer();

    static RiderScriptEditor()
    {
      try
      {
        // todo: make ProjectGeneration lazy
        var projectGeneration = new ProjectGeneration.ProjectGeneration();
        var editor = new RiderScriptEditor(new Discovery(), projectGeneration);
        CodeEditor.Register(editor);
        var path = GetEditorRealPath(CurrentEditor);

        if (IsRiderInstallation(path))
        {
          RiderPathLocator.RiderInfo[] installations = null;

          if (!RiderScriptEditorData.instance.initializedOnce)
          {
            installations = RiderPathLocator.GetAllRiderPaths().OrderBy(a => a.BuildNumber).ToArray();
            // is likely outdated
            if (installations.Any() && installations.All(a => GetEditorRealPath(a.Path) != path))
            {
              if (RiderPathLocator.GetIsToolbox(path)) // is toolbox - update
              {
                var toolboxInstallations = installations.Where(a => a.IsToolbox).ToArray();
                if (toolboxInstallations.Any())
                {
                  var newEditor = toolboxInstallations.Last().Path;
                  CodeEditor.SetExternalScriptEditor(newEditor);
                  path = newEditor;
                }
                else
                {
                  var newEditor = installations.Last().Path;
                  CodeEditor.SetExternalScriptEditor(newEditor);
                  path = newEditor;
                }
              }
              else // is non toolbox - notify
              {
                var newEditorName = installations.Last().Presentation;
                Debug.LogWarning($"Consider updating External Editor in Unity to Rider {newEditorName}.");
              }
            }

            ShowWarningOnUnexpectedScriptEditor(path);
            RiderScriptEditorData.instance.initializedOnce = true;
          }

          if (!FileSystemUtil.EditorPathExists(path)) // previously used rider was removed
          {
            if (installations == null)
              installations = RiderPathLocator.GetAllRiderPaths().OrderBy(a => a.BuildNumber).ToArray();
            if (installations.Any())
            {
              var newEditor = installations.Last().Path;
              CodeEditor.SetExternalScriptEditor(newEditor);
              path = newEditor;
            }
          }

          RiderScriptEditorData.instance.Init();

          editor.CreateSolutionIfDoesntExist();
          if (RiderScriptEditorData.instance.shouldLoadEditorPlugin)
          {
            editor.m_Initiliazer.Initialize(path);
          }

          RiderFileSystemWatcher.InitWatcher(
            Directory.GetCurrentDirectory(), "*.*", (sender, args) =>
            {
              var extension = Path.GetExtension(args.Name);
              if (extension == ".sln" || extension == ".csproj")
                RiderScriptEditorData.instance.hasChanges = true;
            });

          RiderFileSystemWatcher.InitWatcher(
            Path.Combine(Directory.GetCurrentDirectory(), "Library"),
            "EditorOnlyScriptingUserSettings.json",
            (sender, args) => { RiderScriptEditorData.instance.hasChanges = true; });

          RiderFileSystemWatcher.InitWatcher(
            Path.Combine(Directory.GetCurrentDirectory(), "Packages"),
            "manifest.json", (sender, args) => { RiderScriptEditorData.instance.hasChanges = true; });

          // can't switch to non-deprecated api, because UnityEditor.Build.BuildPipelineInterfaces.processors is internal
#pragma warning disable 618
          EditorUserBuildSettings.activeBuildTargetChanged += () =>
#pragma warning restore 618
          {
            RiderScriptEditorData.instance.hasChanges = true;
          };
        }
      }
      catch (Exception e)
      {
        Debug.LogException(e);
      }
    }

    private static void ShowWarningOnUnexpectedScriptEditor(string path)
    {
      // Show warning, when Unity was started from Rider, but external editor is different https://github.com/JetBrains/resharper-unity/issues/1127
      try
      {
        var args = Environment.GetCommandLineArgs();
        var commandlineParser = new CommandLineParser(args);
        if (commandlineParser.Options.ContainsKey("-riderPath"))
        {
          var originRiderPath = commandlineParser.Options["-riderPath"];
          var originRealPath = GetEditorRealPath(originRiderPath);
          var originVersion = RiderPathLocator.GetBuildNumber(originRealPath);
          var version = RiderPathLocator.GetBuildNumber(path);
          if (originVersion != null && originVersion != version)
          {
            Debug.LogWarning("Unity was started by a version of Rider that is not the current default external editor. Advanced integration features cannot be enabled.");
            Debug.Log($"Unity was started by Rider {originVersion}, but external editor is set to: {path}");
          }
        }
      }
      catch (Exception e)
      {
        Debug.LogException(e);
      }
    }

    internal static string GetEditorRealPath(string path)
    {
      if (string.IsNullOrEmpty(path))
      {
        return path;
      }

      if (!FileSystemUtil.EditorPathExists(path))
        return path;

      if (SystemInfo.operatingSystemFamily != OperatingSystemFamily.Windows)
      {
        var realPath = FileSystemUtil.GetFinalPathName(path);

        // case of snap installation
        if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Linux)
        {
          if (new FileInfo(path).Name.ToLowerInvariant() == "rider" &&
              new FileInfo(realPath).Name.ToLowerInvariant() == "snap")
          {
            var snapInstallPath = "/snap/rider/current/bin/rider.sh";
            if (new FileInfo(snapInstallPath).Exists)
              return snapInstallPath;
          }
        }

        // in case of symlink
        return realPath;
      }

      return path;
    }

    public RiderScriptEditor(IDiscovery discovery, IGenerator projectGeneration)
    {
      m_Discoverability = discovery;
      m_ProjectGeneration = projectGeneration;
    }

    private static string[] defaultExtensions
    {
      get
      {
        var customExtensions = new[] {"json", "asmdef", "log", "xaml"};
        return EditorSettings.projectGenerationBuiltinExtensions.Concat(EditorSettings.projectGenerationUserExtensions)
          .Concat(customExtensions).Distinct().ToArray();
      }
    }

    private static string[] HandledExtensions
    {
      get
      {
        return HandledExtensionsString.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries).Select(s => s.TrimStart('.', '*'))
          .ToArray();
      } 
    }

    private static string HandledExtensionsString
    {
      get { return EditorPrefs.GetString("Rider_UserExtensions", string.Join(";", defaultExtensions));}
      set { EditorPrefs.SetString("Rider_UserExtensions", value); }
    }

    private static bool SupportsExtension(string path)
    {
      var extension = Path.GetExtension(path);
      if (string.IsNullOrEmpty(extension))
        return false;
      // cs is a default extension, which should always be handled
      return extension == ".cs" || HandledExtensions.Contains(extension.TrimStart('.'));
    }

    public void OnGUI()
    {
      if (RiderScriptEditorData.instance.shouldLoadEditorPlugin)
      {
        HandledExtensionsString = EditorGUILayout.TextField(new GUIContent("Extensions handled: "), HandledExtensionsString);
      }

      EditorGUILayout.LabelField("Generate .csproj files for:");
      EditorGUI.indentLevel++;
      SettingsButton(ProjectGenerationFlag.Embedded, "Embedded packages", "");
      SettingsButton(ProjectGenerationFlag.Local, "Local packages", "");
      SettingsButton(ProjectGenerationFlag.Registry, "Registry packages", "");
      SettingsButton(ProjectGenerationFlag.Git, "Git packages", "");
      SettingsButton(ProjectGenerationFlag.BuiltIn, "Built-in packages", "");
#if UNITY_2019_3_OR_NEWER
      SettingsButton(ProjectGenerationFlag.LocalTarBall, "Local tarball", "");
#endif
      SettingsButton(ProjectGenerationFlag.Unknown, "Packages from unknown sources", "");
      SettingsButton(ProjectGenerationFlag.PlayerAssemblies, "Player projects", "For each player project generate an additional csproj with the name 'project-player.csproj'");
      RegenerateProjectFiles();
      EditorGUI.indentLevel--;
    }

    void RegenerateProjectFiles()
    {
      var rect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(new GUILayoutOption[] {}));
      rect.width = 252;
      if (GUI.Button(rect, "Regenerate project files"))
      {
        m_ProjectGeneration.Sync();
      }
    }

    void SettingsButton(ProjectGenerationFlag preference, string guiMessage, string toolTip)
    {
      var prevValue = m_ProjectGeneration.AssemblyNameProvider.ProjectGenerationFlag.HasFlag(preference);
      var newValue = EditorGUILayout.Toggle(new GUIContent(guiMessage, toolTip), prevValue);
      if (newValue != prevValue)
      {
        m_ProjectGeneration.AssemblyNameProvider.ToggleProjectGeneration(preference);
      }
    }

    public void SyncIfNeeded(string[] addedFiles, string[] deletedFiles, string[] movedFiles, string[] movedFromFiles,
      string[] importedFiles)
    {
      m_ProjectGeneration.SyncIfNeeded(addedFiles.Union(deletedFiles).Union(movedFiles).Union(movedFromFiles),
        importedFiles);
    }

    public void SyncAll()
    {
      AssetDatabase.Refresh();
      m_ProjectGeneration.SyncIfNeeded(new string[] { }, new string[] { });
    }

    public void Initialize(string editorInstallationPath) // is called each time ExternalEditor is changed
    {
      RiderScriptEditorData.instance.Invalidate(editorInstallationPath);
      m_ProjectGeneration.Sync(); // regenerate csproj and sln for new editor
    }

    public bool OpenProject(string path, int line, int column)
    {
      if (path != "" && !SupportsExtension(path)) // Assets - Open C# Project passes empty path here
      {
        return false;
      }
      
      if (path == "" && SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX)
      {
        // there is a bug in DllImplementation - use package implementation here instead https://github.cds.internal.unity3d.com/unity/com.unity.ide.rider/issues/21
        return OpenOSXApp(path, line, column);
      }

      if (!IsUnityScript(path))
      {
        m_ProjectGeneration.SyncIfNeeded(affectedFiles: new string[] { }, new string[] { });
        var fastOpenResult = EditorPluginInterop.OpenFileDllImplementation(path, line, column);
        if (fastOpenResult)
          return true;
      }

      if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX)
      {
        return OpenOSXApp(path, line, column);
      }

      var solution = GetSolutionFile(path); // TODO: If solution file doesn't exist resync.
      solution = solution == "" ? "" : $"\"{solution}\"";
      var process = new Process
      {
        StartInfo = new ProcessStartInfo
        {
          FileName = CodeEditor.CurrentEditorInstallation,
          Arguments = $"{solution} -l {line} \"{path}\"",
          UseShellExecute = true,
        }
      };

      process.Start();

      return true;
    }

    private bool OpenOSXApp(string path, int line, int column)
    {
      var solution = GetSolutionFile(path);
      solution = solution == "" ? "" : $"\"{solution}\"";
      var pathArguments = path == "" ? "" : $"-l {line} \"{path}\"";
      var process = new Process
      {
        StartInfo = new ProcessStartInfo
        {
          FileName = "open",
          Arguments = $"-n -j \"{CodeEditor.CurrentEditorInstallation}\" --args {solution} {pathArguments}",
          CreateNoWindow = true,
          UseShellExecute = true,
        }
      };

      process.Start();

      return true;
    }

    private string GetSolutionFile(string path)
    {
      if (IsUnityScript(path))
      {
        return Path.Combine(GetBaseUnityDeveloperFolder(), "Projects/CSharp/Unity.CSharpProjects.gen.sln");
      }

      var solutionFile = m_ProjectGeneration.SolutionFile();
      if (File.Exists(solutionFile))
      {
        return solutionFile;
      }

      return "";
    }

    static bool IsUnityScript(string path)
    {
      if (UnityEditor.Unsupported.IsDeveloperBuild())
      {
        var baseFolder = GetBaseUnityDeveloperFolder().Replace("\\", "/");
        var lowerPath = path.ToLowerInvariant().Replace("\\", "/");

        if (lowerPath.Contains((baseFolder + "/Runtime").ToLowerInvariant())
          || lowerPath.Contains((baseFolder + "/Editor").ToLowerInvariant()))
        {
          return true;
        }
      }

      return false;
    }

    static string GetBaseUnityDeveloperFolder()
    {
      return Directory.GetParent(EditorApplication.applicationPath).Parent.Parent.FullName;
    }

    public bool TryGetInstallationForPath(string editorPath, out CodeEditor.Installation installation)
    {
      if (FileSystemUtil.EditorPathExists(editorPath) && IsRiderInstallation(editorPath))
      {
        var info = new RiderPathLocator.RiderInfo(editorPath, false);
        installation = new CodeEditor.Installation
        {
          Name = info.Presentation,
          Path = info.Path
        };
        return true;
      }

      installation = default;
      return false;
    }

    public static bool IsRiderInstallation(string path)
    {
      if (IsAssetImportWorkerProcess())
        return false;
      
      if (string.IsNullOrEmpty(path))
      {
        return false;
      }

      var fileInfo = new FileInfo(path);
      var filename = fileInfo.Name.ToLowerInvariant();
      return filename.StartsWith("rider", StringComparison.Ordinal);
    }

    private static bool IsAssetImportWorkerProcess()
    {
#if UNITY_2020_2_OR_NEWER
      return UnityEditor.AssetDatabase.IsAssetImportWorkerProcess();
#elif UNITY_2019_3_OR_NEWER
      return UnityEditor.Experimental.AssetDatabaseExperimental.IsAssetImportWorkerProcess();
#else
      return false;
#endif
    }

    public static string CurrentEditor // works fast, doesn't validate if executable really exists
      => EditorPrefs.GetString("kScriptsDefaultApp");

    public CodeEditor.Installation[] Installations => m_Discoverability.PathCallback();

    private void CreateSolutionIfDoesntExist()
    {
      if (!m_ProjectGeneration.HasSolutionBeenGenerated())
      {
        m_ProjectGeneration.Sync();
      }
    }
  }
}