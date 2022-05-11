using UnityEditor;

using Codice.Client.Common.Encryption;
using PlasticGui;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor.Configuration
{
    internal class MissingEncryptionPasswordPromptHandler :
        ClientEncryptionServiceProvider.IEncryptioPasswordProvider
    {
        internal MissingEncryptionPasswordPromptHandler(EditorWindow parentWindow)
        {
            mParentWindow = parentWindow;
        }

        string ClientEncryptionServiceProvider.IEncryptioPasswordProvider
            .GetEncryptionEncryptedPassword(string server)
        {
            string result = null;

            GUIActionRunner.RunGUIAction(delegate
            {
                result = AskForEncryptionPassword(server);
            });

            return result;
        }

        string AskForEncryptionPassword(string server)
        {
            EncryptionConfigurationDialogData dialogData =
                EncryptionConfigurationDialog.RequestEncryptionPassword(server, mParentWindow);

            if (!dialogData.Result)
                return null;

            return dialogData.EncryptedPassword;
        }

        EditorWindow mParentWindow;
    }
}
