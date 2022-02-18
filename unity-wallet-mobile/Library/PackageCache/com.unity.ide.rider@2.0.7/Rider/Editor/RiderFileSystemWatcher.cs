using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Packages.Rider.Editor
{
  internal static class RiderFileSystemWatcher
  {
    public static void InitWatcher(string watchDirectory,
      string filter, FileSystemEventHandler onChanged)
    {
      Task.Run(() =>
      {
        var watcher = new FileSystemWatcher();
        watcher.Path = watchDirectory;
        watcher.NotifyFilter = NotifyFilters.LastWrite; //Watch for changes in LastWrite times
        watcher.Filter = filter;
        watcher.Changed += onChanged;
        watcher.Deleted += onChanged;
        
        watcher.EnableRaisingEvents = true;// Begin watching.
        return watcher;
      }).ContinueWith(task =>
      {
        try
        {
          var watcher = task.Result;
          AppDomain.CurrentDomain.DomainUnload += (EventHandler) ((_, __) =>
          {
            watcher.Dispose();
          });
        }
        catch (Exception ex)
        {
          Debug.LogError(ex);
        }
      }, TaskScheduler.FromCurrentSynchronizationContext());
    }
  }
}