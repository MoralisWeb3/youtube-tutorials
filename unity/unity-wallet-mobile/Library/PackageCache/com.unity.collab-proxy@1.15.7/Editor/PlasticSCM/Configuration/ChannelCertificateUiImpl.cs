using Codice.Client.Common;
using Codice.CM.Common;
using PlasticGui;
using PlasticPipe.Certificates;
using Unity.PlasticSCM.Editor.UI;
using UnityEditor;

namespace Unity.PlasticSCM.Editor.Configuration
{
    internal class ChannelCertificateUiImpl : IChannelCertificateUI
    {
        internal ChannelCertificateUiImpl()
        {
        }

        CertOperationResult IChannelCertificateUI.AcceptNewServerCertificate(PlasticCertInfo serverCertificate)
        {
            return GetUserResponse(
                PlasticLocalization.GetString(
                    PlasticLocalization.Name.NewCertificateTitle),
                PlasticLocalization.GetString(
                    PlasticLocalization.Name.NewCertificateMessage),
                serverCertificate);
        }

        CertOperationResult IChannelCertificateUI.AcceptChangedServerCertificate(PlasticCertInfo serverCertificate)
        {
            return GetUserResponse(
                PlasticLocalization.GetString(
                    PlasticLocalization.Name.ExistingCertificateChangedTitle),
                PlasticLocalization.GetString(
                    PlasticLocalization.Name.ExistingCertificateChangedMessage),
                serverCertificate);
        }

        bool IChannelCertificateUI.AcceptInvalidHostname(string certHostname, string serverHostname)
        {
            bool result = false;

            GUIActionRunner.RunGUIAction(delegate {
                result = EditorUtility.DisplayDialog(
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.InvalidCertificateHostnameTitle),
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.InvalidCertificateHostnameMessage,
                        certHostname, serverHostname),
                    PlasticLocalization.GetString(PlasticLocalization.Name.YesButton),
                    PlasticLocalization.GetString(PlasticLocalization.Name.NoButton));
            });

            return result;
        }

        CertOperationResult GetUserResponse(
            string title, string message, PlasticCertInfo serverCertificate)
        {
            GuiMessage.GuiMessageResponseButton result =
                GuiMessage.GuiMessageResponseButton.Third;

            GUIActionRunner.RunGUIAction(delegate {
                result = GuiMessage.ShowQuestion(
                    title, GetCertificateMessageString(message, serverCertificate),
                    PlasticLocalization.GetString(PlasticLocalization.Name.YesButton),
                    PlasticLocalization.GetString(PlasticLocalization.Name.NoButton),
                    PlasticLocalization.GetString(PlasticLocalization.Name.CancelButton),
                    true);
            });

            switch (result)
            {
                case GuiMessage.GuiMessageResponseButton.First:
                    return CertOperationResult.AddToStore;
                case GuiMessage.GuiMessageResponseButton.Second:
                    return CertOperationResult.DoNotAddToStore;
                case GuiMessage.GuiMessageResponseButton.Third:
                    return CertOperationResult.Cancel;
                default:
                    return CertOperationResult.Cancel;
            }
        }

        string GetCertificateMessageString(string message, PlasticCertInfo serverCertificate)
        {
            return string.Format(message,
                CertificateUi.GetCnField(serverCertificate.Subject),
                CertificateUi.GetCnField(serverCertificate.Issuer),
                serverCertificate.Format,
                serverCertificate.ExpirationDateString,
                serverCertificate.KeyAlgorithm,
                serverCertificate.CertHashString);
        }
    }
}
