using UnityEditor;

using Codice.Client.Common;
using Codice.Client.Common.Connection;
using PlasticGui;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.Configuration.CloudEdition.Welcome;

namespace Unity.PlasticSCM.Editor.Configuration
{
    internal class CredentialsUiImpl : AskCredentialsToUser.IGui
    {
        internal CredentialsUiImpl(EditorWindow parentWindow)
        {
            mParentWindow = parentWindow;
        }

        AskCredentialsToUser.DialogData AskCredentialsToUser.IGui.AskUserForCredentials(string servername)
        {
            AskCredentialsToUser.DialogData result = null;

            GUIActionRunner.RunGUIAction(delegate
            {
                result = CredentialsDialog.RequestCredentials(
                        servername, mParentWindow);
            });

            return result;
        }

        void AskCredentialsToUser.IGui.ShowSaveProfileErrorMessage(string message)
        {
            GUIActionRunner.RunGUIAction(delegate
            {
                GuiMessage.ShowError(string.Format(
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.CredentialsErrorSavingProfile),
                    message));
            });
        }

        AskCredentialsToUser.DialogData AskCredentialsToUser.IGui.AskUserForSSOCredentials(string cloudServer)
        {
            AskCredentialsToUser.DialogData result = null;

            // Check SSO auto login here
            GUIActionRunner.RunGUIAction(delegate
            {
                result = RunCredentialsRequest(cloudServer);
            });

            return result;
        }

        private AskCredentialsToUser.DialogData RunCredentialsRequest(string cloudServer)
        {
            AutoLogin autoLogin = new AutoLogin();
            var response = autoLogin.Run();

            if (response != ResponseType.None)
            {
                return autoLogin.BuildCredentialsDialogData(response);
            }
            else
            {
                return SSOCredentialsDialog.RequestCredentials(cloudServer, mParentWindow);
            }
        }

        EditorWindow mParentWindow;
    }
}
