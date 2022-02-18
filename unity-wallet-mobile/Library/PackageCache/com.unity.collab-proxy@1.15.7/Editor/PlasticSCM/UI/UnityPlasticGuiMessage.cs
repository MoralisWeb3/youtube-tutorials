using UnityEditor;

using Codice.Client.Common;
using PlasticGui;

namespace Unity.PlasticSCM.Editor.UI
{
    internal class UnityPlasticGuiMessage : GuiMessage.IGuiMessage
    {
        internal UnityPlasticGuiMessage(EditorWindow parentWindow)
        {
            mParentWindow = parentWindow;
        }

        void GuiMessage.IGuiMessage.ShowMessage(
            string title,
            string message,
            GuiMessage.GuiMessageType alertType)
        {
            EditorUtility.DisplayDialog(
                BuildDialogTitle(title, alertType),
                message,
                PlasticLocalization.GetString(PlasticLocalization.Name.CloseButton));
        }

        void GuiMessage.IGuiMessage.ShowError(string message)
        {
            EditorUtility.DisplayDialog(
                PlasticLocalization.GetString(PlasticLocalization.Name.ErrorPlasticSCM),
                message,
                PlasticLocalization.GetString(PlasticLocalization.Name.CloseButton));
        }

        GuiMessage.GuiMessageResponseButton GuiMessage.IGuiMessage.ShowQuestion(
            string title,
            string message,
            string firstActionButton,
            string secondActionButton,
            string thirdActionButton,
            bool isFirstButtonEnabled)
        {
            if (!isFirstButtonEnabled)
            {
                bool result = EditorUtility.DisplayDialog(
                    title,
                    message,
                    secondActionButton,
                    thirdActionButton);

                return (result) ?
                    GuiMessage.GuiMessageResponseButton.Second :
                    GuiMessage.GuiMessageResponseButton.Third;
            }

            int intResult = EditorUtility.DisplayDialogComplex(
                title,
                message,
                firstActionButton,
                secondActionButton,
                thirdActionButton);

            return GetResponse(intResult);
        }

        bool GuiMessage.IGuiMessage.ShowQuestion(
            string title,
            string message,
            string yesButton)
        {
            return EditorUtility.DisplayDialog(
                title,
                message,
                yesButton,
                PlasticLocalization.GetString(PlasticLocalization.Name.NoButton));
        }

        bool GuiMessage.IGuiMessage.ShowYesNoQuestion(string title, string message)
        {
            return EditorUtility.DisplayDialog(
                title,
                message,
                PlasticLocalization.GetString(PlasticLocalization.Name.YesButton),
                PlasticLocalization.GetString(PlasticLocalization.Name.NoButton));
        }

        GuiMessage.GuiMessageResponseButton GuiMessage.IGuiMessage.ShowYesNoCancelQuestion(
            string title, string message)
        {
            int intResult = EditorUtility.DisplayDialogComplex(
                title,
                message,
                PlasticLocalization.GetString(PlasticLocalization.Name.YesButton),
                PlasticLocalization.GetString(PlasticLocalization.Name.NoButton),
                PlasticLocalization.GetString(PlasticLocalization.Name.CancelButton));

            return GetResponse(intResult);
        }

        bool GuiMessage.IGuiMessage.ShowYesNoQuestionWithType(
            string title, string message, GuiMessage.GuiMessageType messageType)
        {
            return EditorUtility.DisplayDialog(
                BuildDialogTitle(title, messageType),
                message,
                PlasticLocalization.GetString(PlasticLocalization.Name.YesButton),
                PlasticLocalization.GetString(PlasticLocalization.Name.NoButton));
        }

        static GuiMessage.GuiMessageResponseButton GetResponse(int dialogResult)
        {
            switch (dialogResult)
            {
                case 0:
                    return GuiMessage.GuiMessageResponseButton.First;
                case 1:
                    return GuiMessage.GuiMessageResponseButton.Second;
                case 2:
                    return GuiMessage.GuiMessageResponseButton.Third;
                default:
                    return GuiMessage.GuiMessageResponseButton.Second;
            }
        }

        static string BuildDialogTitle(
            string title,
            GuiMessage.GuiMessageType alertType)
        {
            string alertTypeText = GetAlertTypeText(alertType);
            return string.Format("{0} - {1}", alertTypeText, title);
        }

        static string GetAlertTypeText(GuiMessage.GuiMessageType alertType)
        {
            string alertTypeText = string.Empty;

            switch (alertType)
            {
                case GuiMessage.GuiMessageType.Informational:
                    alertTypeText = "Information";
                    break;
                case GuiMessage.GuiMessageType.Warning:
                    alertTypeText = "Warning";
                    break;
                case GuiMessage.GuiMessageType.Critical:
                    alertTypeText = "Error";
                    break;
                case GuiMessage.GuiMessageType.Question:
                    alertTypeText = "Question";
                    break;
            }

            return alertTypeText;
        }

        EditorWindow mParentWindow;
    }
}
