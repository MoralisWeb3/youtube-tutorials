using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using PlasticGui;
using Unity.PlasticSCM.Editor;
using System.Linq;
using Codice.CM.Common;
using UnityEngine.UIElements;
using Unity.PlasticSCM.Editor.UI.UIElements;
using UnityEditor.UIElements;

internal class TurnOffPlasticWindow : EditorWindow
{
    internal static void ShowWindow()
    {
        TurnOffPlasticWindow window = GetWindow<TurnOffPlasticWindow>();
        window.titleContent = new GUIContent(PlasticLocalization.GetString(
            PlasticLocalization.Name.TurnOffPlasticSCM));
        window.minSize = new Vector2(500, 350);
        window.maxSize = new Vector2(500, 350);
        window.Show();
    }

    void OnEnable()
    {
        mProgress = 0;
        EditorApplication.update += UpdateProgress;
        InitializeLayoutAndStyles();
        BuildComponents();
    }

    void OnDestroy()
    {
        Dispose();
    }

    void Dispose()
    {
        mTurnOffButton.clicked -= CreateButton_Clicked;
        EditorApplication.update -= UpdateProgress;
    }

    void InitializeLayoutAndStyles()
    {
        mRoot = rootVisualElement;
        mRoot.LoadLayout(typeof(TurnOffPlasticWindow).Name);
        mRoot.LoadStyle(typeof(TurnOffPlasticWindow).Name);
    }

    void BuildComponents()
    {
        mRoot = rootVisualElement;
        mTurnOffButton = mRoot.Query<Button>("turnoff-button").First();
        mTurnOffButton.text = PlasticLocalization.GetString(
            PlasticLocalization.Name.TurnOffPlasticSCM);
        mTurnOffButton.clicked += CreateButton_Clicked;

        mTurnOffLabel = mRoot.Query<Label>("turnoff-label").First();
        mTurnOffLabel.text = PlasticLocalization.GetString(
            PlasticLocalization.Name.TurnOffPlasticSCMDescrition);

        mTurnedOffLabel = mRoot.Query<Label>("progress-label").First();
        mTurnedOffLabel.text = string.Empty;

        mRoot.Add(mProgressBar = new ProgressBar());
        mProgressBar.style.marginLeft = 120f;
        mProgressBar.style.marginBottom = 2f;
        mProgressBar.title = string.Empty;
        mProgressBar.style.width = 300f;
        mProgressBar.style.display = DisplayStyle.None;
    }

    void UpdateProgress()
    {
        if (mProgress == 0) return;

        mProgressBar.style.display = DisplayStyle.Flex;

        if (mProgress == 1)
        {
            mProgressBar.value = 20f;
            mProgressBar.title = mProgressBar.value.ToString() + "%";
            mTurnedOffLabel.text = PlasticLocalization.GetString(
                PlasticLocalization.Name.TurnOffPlasticSCMClosingWindow);
            mPlasticWindow = Resources.FindObjectsOfTypeAll<PlasticWindow>().First();
            mPlasticWindow.Close();
            mProgress = 2;
            return;
        }
        if (mProgress == 2)
        {
            mProgressBar.value = 50f;
            mProgressBar.title = mProgressBar.value.ToString() +"%";
            mTurnedOffLabel.text = PlasticLocalization.GetString(
                PlasticLocalization.Name.TurnOffPlasticSCMCleaning);
            SetupCloudProjectId.SetCloudProjectId("");
            mProgress = 3;
            return;
        }
        if (mProgress == 3)
        {
            mProgressBar.value = 75f;
            mProgressBar.title = mProgressBar.value.ToString() +"%";
            mTurnedOffLabel.text = PlasticLocalization.GetString(
                PlasticLocalization.Name.TurnOffPlasticSCMDeleting);
            WorkspaceInfo workspaceInfo =
                       FindWorkspace.InfoForApplicationPath(
                       Application.dataPath,
                       PlasticWindow.PlasticApi);
            Plastic.API.RemoveWorkspace(workspaceInfo);
            mProgress = 4;
            return;
        }
        if (mProgress == 4)
        {
            mProgressBar.value = 100f;
            mProgressBar.title = mProgressBar.value.ToString() +"%";
            mTurnedOffLabel.text = PlasticLocalization.GetString(
                PlasticLocalization.Name.Done);
            mProgress = 9999;
            return;
        }
    }

    void CreateButton_Clicked()
    {
        if (mProgress != 0) return;
        if (EditorUtility.DisplayDialog(PlasticLocalization.GetString(
            PlasticLocalization.Name.TurningOffPlasticSCM),
            PlasticLocalization.GetString(PlasticLocalization.Name.TurnOffPlasticSCMAreYouSure),
            PlasticLocalization.GetString(PlasticLocalization.Name.YesButton),
            PlasticLocalization.GetString(PlasticLocalization.Name.NoButton)))
        {
            mProgress = 1;
        }
    }

    VisualElement mRoot;
    Button mTurnOffButton;
    Label mTurnOffLabel;
    Label mTurnedOffLabel;
    ProgressBar mProgressBar;
    int mProgress;
    PlasticWindow mPlasticWindow;
}