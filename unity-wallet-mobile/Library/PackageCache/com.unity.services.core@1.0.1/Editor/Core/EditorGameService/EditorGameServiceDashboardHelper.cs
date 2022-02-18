using System;
using UnityEngine;

namespace Unity.Services.Core.Editor
{
    /// <summary>
    /// Used by <see cref="IEditorGameService"/> to simplify operations related to the dashboard
    /// </summary>
    public static class EditorGameServiceDashboardHelper
    {
        /// <summary>
        /// Opens the dashboard of the <see cref="IEditorGameService"/>
        /// </summary>
        /// <param name="editorGameService">The <see cref="IEditorGameService"/> who's dashboard should open</param>
        public static void OpenDashboard(this IEditorGameService editorGameService)
        {
            if (!editorGameService.HasDashboard)
            {
                throw new InvalidOperationException($"The service '{editorGameService.Name}' is not configured to use a Dashboard. " +
                    $"Make sure the service returns 'true' in 'HasDashboard' implementation.");
            }

            var formattedUrl = editorGameService.GetFormattedDashboardUrl();
            if (Uri.IsWellFormedUriString(formattedUrl, UriKind.Absolute))
            {
                EditorGameServiceAnalyticsSender.SendProjectSettingsGoToDashboardEvent(editorGameService.Identifier.GetKey());
                Application.OpenURL(formattedUrl);
            }
            else
            {
                throw new UriFormatException($"Dashboard Url for service '{editorGameService.Name}' is not properly formatted." +
                    $" Attempted to use url '{formattedUrl}'. Make sure the service returns a proper url in 'GetFormattedDashboardUrl()' implementation.");
            }
        }
    }
}
