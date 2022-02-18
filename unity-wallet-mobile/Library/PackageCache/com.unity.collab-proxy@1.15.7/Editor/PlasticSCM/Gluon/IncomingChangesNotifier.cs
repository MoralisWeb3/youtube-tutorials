using PlasticGui.Gluon.WorkspaceWindow;
using Unity.PlasticSCM.Editor.UI;

namespace Unity.PlasticSCM.Editor.Gluon
{
    internal class IncomingChangesNotifier :
        IIncomingChangesNotifier,
        CheckIncomingChanges.IUpdateIncomingChanges
    {
        bool IIncomingChangesNotifier.HasNotification
        {
            get { return mHasNotification; }
        }

        IncomingChangesNotification IIncomingChangesNotifier.Notification
        {
            get { return mNotification; }
        }

        internal IncomingChangesNotifier(
            PlasticWindow plasticWindow)
        {
            mPlasticWindow = plasticWindow;
        }

        void CheckIncomingChanges.IUpdateIncomingChanges.Hide()
        {
            mPlasticWindow.SetNotificationStatus(
                PlasticNotification.Status.None,
                string.Empty,
                string.Empty);
                
            mNotification.Clear();

            mHasNotification = false;

            mPlasticWindow.Repaint();
        }

        void CheckIncomingChanges.IUpdateIncomingChanges.Show(
            string infoText,
            string actionText,
            string tooltipText,
            CheckIncomingChanges.Severity severity)
        {
            PlasticNotification.Status status = PlasticNotification.Status.None;
            if (severity == CheckIncomingChanges.Severity.Info)
                status = PlasticNotification.Status.IncomingChanges;
            else if (severity == CheckIncomingChanges.Severity.Warning)
                status = PlasticNotification.Status.Conflicts;

            mPlasticWindow.SetNotificationStatus(
                status,
                infoText,
                actionText);
            
            UpdateData(
                mNotification,
                infoText,
                actionText,
                tooltipText,
                status);

            mHasNotification = true;

            mPlasticWindow.Repaint();
        }

        static void UpdateData(
            IncomingChangesNotification data,
            string infoText,
            string actionText,
            string tooltipText,
            PlasticNotification.Status status)
        {
            data.InfoText = infoText;
            data.ActionText = actionText;
            data.TooltipText = tooltipText;
            data.HasUpdateAction = false;
            data.Status = status;
        }

        bool mHasNotification;
        IncomingChangesNotification mNotification = new IncomingChangesNotification();

        PlasticWindow mPlasticWindow;
    }
}