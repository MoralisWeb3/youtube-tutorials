using System.IO;

using UnityEditor;
using UnityEngine;

using Codice.LogWrapper;
using PlasticGui.Help;
using Unity.PlasticSCM.Editor.AssetUtils;

namespace Unity.PlasticSCM.Editor.UI
{
    internal class Images
    {
        internal enum Name
        {
            None,

            IconPlastic,
            IconCloseButton,
            IconPressedCloseButton,
            IconAdded,
            IconAddedLocal,
            IconAddedOverlay,
            IconPrivateOverlay,
            IconCheckedOutLocalOverlay,
            IconDeleted,
            IconDeletedLocalOverlay,
            IconDeletedRemote,
            IconDeletedRemoteOverlay,
            IconChanged,
            IconOutOfSync,
            IconOutOfSyncOverlay,
            IconMoved,
            IconMergeLink,
            Ignored,
            IgnoredOverlay,
            IconConflicted,
            IconMergeConflict,
            IconMergeConflictOverlay,
            IconConflictResolvedOverlay,
            IconLockedLocalOverlay,
            IconLockedRemoteOverlay,
            IconMerged,
            IconFsChanged,
            IconMergeCategory,
            XLink,
            Ok,
            NotOnDisk,
            IconRepository,
            IconPlasticView,
            IconPlasticViewNotify,
            IconPlasticNotifyIncoming,
            IconPlasticNotifyConflict,
            Loading,
            IconEmptyGravatar,
            Step1,
            Step2,
            Step3,
            StepOk,
            ButtonSsoSignInUnity,
            ButtonSsoSignInEmail,
            ButtonSsoSignInGoogle,
            IconBranch,
            IconUndo,
            Refresh
        }

        internal static Texture2D GetHelpImage(HelpImage image)
        {
            // We use the dark version for both the light/dark skins since it matches the grey background better
            string helpImageFileName = string.Format(
                "d_{0}.png",
                HelpImageName.FromHelpImage(image));

            string imageRelativePath = GetImageFileRelativePath(helpImageFileName);
            Texture2D result = TryLoadImage(imageRelativePath, imageRelativePath);

            if (result != null)
                return result;

            mLog.WarnFormat("Image not found: {0}", helpImageFileName);
            return GetEmptyImage();
        }

        internal static Texture2D GetImage(Name image)
        {
            string imageFileName = image.ToString().ToLower() + ".png";
            string imageFileName2x = image.ToString().ToLower() + "@2x.png";

            string darkImageFileName = string.Format("d_{0}", imageFileName);
            string darkImageFileName2x = string.Format("d_{0}", imageFileName2x);

            string imageFileRelativePath = GetImageFileRelativePath(imageFileName);
            string imageFileRelativePath2x = GetImageFileRelativePath(imageFileName2x);

            string darkImageFileRelativePath = GetImageFileRelativePath(darkImageFileName);
            string darkImageFileRelativePath2x = GetImageFileRelativePath(darkImageFileName2x);

            Texture2D result = null;

            if (EditorGUIUtility.isProSkin)
                result = TryLoadImage(darkImageFileRelativePath, darkImageFileRelativePath2x);

            if (result != null)
                return result;

            result = TryLoadImage(imageFileRelativePath, imageFileRelativePath2x);

            if (result != null)
                return result;

            mLog.WarnFormat("Image not found: {0}", imageFileName);
            return GetEmptyImage();
        }

        internal static Texture GetFileIcon(string path)
        {
            string relativePath = GetRelativePath.ToApplication(path);

            return GetFileIconFromRelativePath(relativePath);
        }

        internal static Texture GetFileIconFromCmPath(string path)
        {
            return GetFileIconFromRelativePath(
                path.Substring(1).Replace("/",
                Path.DirectorySeparatorChar.ToString()));
        }

        internal static Texture GetDropDownIcon()
        {
            if (mPopupIcon == null)
                mPopupIcon = EditorGUIUtility.IconContent("icon dropdown").image;

            return mPopupIcon;
        }

        internal static Texture GetDirectoryIcon()
        {
            if (mDirectoryIcon == null)
                mDirectoryIcon = EditorGUIUtility.IconContent("Folder Icon").image;

            return mDirectoryIcon;
        }

        internal static Texture GetPrivatedOverlayIcon()
        {
            if (mPrivatedOverlayIcon == null)
                mPrivatedOverlayIcon = GetImage(Name.IconPrivateOverlay);

            return mPrivatedOverlayIcon;
        }

        internal static Texture GetAddedOverlayIcon()
        {
            if (mAddedOverlayIcon == null)
                mAddedOverlayIcon = GetImage(Name.IconAddedOverlay);

            return mAddedOverlayIcon;
        }

        internal static Texture GetDeletedLocalOverlayIcon()
        {
            if (mDeletedLocalOverlayIcon == null)
                mDeletedLocalOverlayIcon = GetImage(Name.IconDeletedLocalOverlay);

            return mDeletedLocalOverlayIcon;
        }

        internal static Texture GetDeletedRemoteOverlayIcon()
        {
            if (mDeletedRemoteOverlayIcon == null)
                mDeletedRemoteOverlayIcon = GetImage(Name.IconDeletedRemoteOverlay);

            return mDeletedRemoteOverlayIcon;
        }

        internal static Texture GetCheckedOutOverlayIcon()
        {
            if (mCheckedOutOverlayIcon == null)
                mCheckedOutOverlayIcon = GetImage(Name.IconCheckedOutLocalOverlay);

            return mCheckedOutOverlayIcon;
        }

        internal static Texture GetOutOfSyncOverlayIcon()
        {
            if (mOutOfSyncOverlayIcon == null)
                mOutOfSyncOverlayIcon = GetImage(Name.IconOutOfSyncOverlay);

            return mOutOfSyncOverlayIcon;
        }

        internal static Texture GetConflictedOverlayIcon()
        {
            if (mConflictedOverlayIcon == null)
                mConflictedOverlayIcon = GetImage(Name.IconMergeConflictOverlay);

            return mConflictedOverlayIcon;
        }

        internal static Texture GetConflictResolvedOverlayIcon()
        {
            if (mConflictResolvedOverlayIcon == null)
                mConflictResolvedOverlayIcon = GetImage(Name.IconConflictResolvedOverlay);

            return mConflictResolvedOverlayIcon;
        }

        internal static Texture GetLockedLocalOverlayIcon()
        {
            if (mLockedLocalOverlayIcon == null)
                mLockedLocalOverlayIcon = GetImage(Name.IconLockedLocalOverlay);

            return mLockedLocalOverlayIcon;
        }

        internal static Texture GetLockedRemoteOverlayIcon()
        {
            if (mLockedRemoteOverlayIcon == null)
                mLockedRemoteOverlayIcon = GetImage(Name.IconLockedRemoteOverlay);

            return mLockedRemoteOverlayIcon;
        }

        internal static Texture GetIgnoredOverlayIcon()
        {
            if (mIgnoredverlayIcon == null)
                mIgnoredverlayIcon = GetImage(Name.IgnoredOverlay);

            return mIgnoredverlayIcon;
        }

        internal static Texture GetWarnIcon()
        {
            if (mWarnIcon == null)
                mWarnIcon = EditorGUIUtility.IconContent("console.warnicon.sml").image;

            return mWarnIcon;
        }

        internal static Texture GetInfoIcon()
        {
            if (mInfoIcon == null)
                mInfoIcon = EditorGUIUtility.IconContent("console.infoicon.sml").image;

            return mInfoIcon;
        }

        internal static Texture GetErrorDialogIcon()
        {
            if (mErrorDialogIcon == null)
                mErrorDialogIcon = EditorGUIUtility.IconContent("console.erroricon").image;

            return mErrorDialogIcon;
        }

        internal static Texture GetWarnDialogIcon()
        {
            if (mWarnDialogIcon == null)
                mWarnDialogIcon = EditorGUIUtility.IconContent("console.warnicon").image;

            return mWarnDialogIcon;
        }

        internal static Texture GetInfoDialogIcon()
        {
            if (mInfoDialogIcon == null)
                mInfoDialogIcon = EditorGUIUtility.IconContent("console.infoicon").image;

            return mInfoDialogIcon;
        }

        internal static Texture GetRefreshIcon()
        {
            if (mRefreshIcon == null)
                mRefreshIcon= GetImage(Name.Refresh);

            return mRefreshIcon;
        }

        internal static Texture GetSettingsIcon()
        {
            if (mSettingsIcon == null)
                mSettingsIcon = EditorGUIUtility.IconContent("settings").image;

            return mSettingsIcon;
        }

        internal static Texture GetCloseIcon()
        {
            if (mCloseIcon == null)
                mCloseIcon = EditorGUIUtility.FindTexture("winbtn_win_close");

            return mCloseIcon;
        }

        internal static Texture GetClickedCloseIcon()
        {
            if (mClickedCloseIcon == null)
                mClickedCloseIcon = EditorGUIUtility.FindTexture("winbtn_win_close_a");

            return mClickedCloseIcon;
        }

        internal static Texture GetHoveredCloseIcon()
        {
            if (mHoveredCloseIcon == null)
                mHoveredCloseIcon = EditorGUIUtility.FindTexture("winbtn_win_close_h");

            return mHoveredCloseIcon;
        }

        internal static Texture GetFileIcon()
        {
            if (mFileIcon == null)
                mFileIcon = EditorGUIUtility.FindTexture("DefaultAsset Icon");

            if (mFileIcon == null)
                mFileIcon = AssetPreview.GetMiniTypeThumbnail(typeof(DefaultAsset));

            if (mFileIcon == null)
                mFileIcon = GetEmptyImage();

            return mFileIcon;
        }

        internal static Texture2D GetLinkUnderlineImage()
        {
            if (mLinkUnderlineImage == null)
            {
                mLinkUnderlineImage = new Texture2D(1, 1);
                mLinkUnderlineImage.SetPixel(0, 0, UnityStyles.Colors.Link);
                mLinkUnderlineImage.Apply();
            }

            return mLinkUnderlineImage;
        }

        static Texture2D GetEmptyImage()
        {
            if (mEmptyImage == null)
            {
                mEmptyImage = new Texture2D(1, 1);
                mEmptyImage.SetPixel(0, 0, Color.clear);
                mEmptyImage.Apply();
            }

            return mEmptyImage;
        }

        static Texture GetFileIconFromRelativePath(string relativePath)
        {
            Texture result = AssetDatabase.GetCachedIcon(relativePath);

            if (result != null)
                return result;

            result = GetFileIconFromKnownExtension(relativePath);

            if (result != null)
                return result;

            return GetFileIcon();
        }

        static Texture GetFileIconFromKnownExtension(string relativePath)
        {
            if (relativePath.EndsWith(UnityConstants.TREEVIEW_META_LABEL))
            {
                relativePath = relativePath.Substring(0,
                    relativePath.Length- UnityConstants.TREEVIEW_META_LABEL.Length);
            }

            string extension = Path.GetExtension(relativePath).ToLower();

            if (extension.Equals(".cs"))
                return EditorGUIUtility.IconContent("cs Script Icon").image;

            if (extension.Equals(".png") || extension.Equals(".jpg")
             || extension.Equals(".jpeg") || extension.Equals(".gif")
             || extension.Equals(".tga") || extension.Equals(".bmp")
             || extension.Equals(".tif") || extension.Equals(".tiff"))
                return EditorGUIUtility.IconContent("d_Texture Icon").image;

            if (extension.Equals(".mat"))
                return AssetPreview.GetMiniTypeThumbnail(typeof(UnityEngine.Material));

            if (extension.Equals(".fbx") || extension.Equals(".ma")
             || extension.Equals(".mb") || extension.Equals(".blend")
             || extension.Equals(".max") )
                return AssetPreview.GetMiniTypeThumbnail(typeof(UnityEngine.GameObject));

            if (extension.Equals(".wav") || extension.Equals(".mp3"))
                return AssetPreview.GetMiniTypeThumbnail(typeof(UnityEngine.AudioClip));

            if (extension.Equals(".anim"))
                return AssetPreview.GetMiniTypeThumbnail(typeof(UnityEngine.Animation));

            if (extension.Equals(".animator"))
                return AssetPreview.GetMiniTypeThumbnail(typeof(UnityEngine.Animator));

            if (extension.Equals(".shader"))
                return EditorGUIUtility.IconContent("d_Shader Icon").image;

            if (extension.Equals(".asset") && relativePath.StartsWith("ProjectSettings\\"))
                return EditorGUIUtility.IconContent("EditorSettings Icon").image;

            return null;
        }

        static string GetImageFileRelativePath(string imageFileName)
        {
            return Path.Combine(
                AssetsPath.GetImagesFolderRelativePath(),
                imageFileName);
        }

        static Texture2D TryLoadImage(string imageFileRelativePath, string image2xFilePath)
        {
            if (EditorGUIUtility.pixelsPerPoint > 1f && File.Exists(image2xFilePath))
                return LoadTextureFromFile(image2xFilePath);

            if (File.Exists(Path.GetFullPath(imageFileRelativePath)))
                return LoadTextureFromFile(imageFileRelativePath);

            return null;
        }

        static Texture2D LoadTextureFromFile(string path)
        {
            byte[] fileData = File.ReadAllBytes(path);
            Texture2D result = new Texture2D(1, 1);
            result.LoadImage(fileData); //auto-resizes the texture dimensions
            return result;
        }

        static Texture mFileIcon;
        static Texture mDirectoryIcon;

        static Texture mPrivatedOverlayIcon;
        static Texture mAddedOverlayIcon;
        static Texture mDeletedLocalOverlayIcon;
        static Texture mDeletedRemoteOverlayIcon;
        static Texture mCheckedOutOverlayIcon;
        static Texture mOutOfSyncOverlayIcon;
        static Texture mConflictedOverlayIcon;
        static Texture mConflictResolvedOverlayIcon;
        static Texture mLockedLocalOverlayIcon;
        static Texture mLockedRemoteOverlayIcon;
        static Texture mIgnoredverlayIcon;

        static Texture mWarnIcon;
        static Texture mInfoIcon;

        static Texture mErrorDialogIcon;
        static Texture mWarnDialogIcon;
        static Texture mInfoDialogIcon;

        static Texture mRefreshIcon;
        static Texture mSettingsIcon;

        static Texture mCloseIcon;
        static Texture mClickedCloseIcon;
        static Texture mHoveredCloseIcon;

        static Texture2D mLinkUnderlineImage;

        static Texture2D mEmptyImage;

        static Texture mPopupIcon;

        static readonly ILog mLog = LogManager.GetLogger("Images");
    }
}