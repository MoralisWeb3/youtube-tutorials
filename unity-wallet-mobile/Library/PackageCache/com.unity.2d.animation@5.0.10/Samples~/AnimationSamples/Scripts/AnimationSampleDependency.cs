using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.U2D.Animation.Sample.Dependency
{
    [ExecuteInEditMode]
    internal class AnimationSampleDependency : MonoBehaviour
    {
        public UnityEngine.UI.Text textField = null;
        public GameObject gameCanvas = null;

        void Update()
        {
#if PSDIMPORTER_ENABLED
            if(textField != null)
                textField.enabled = false;
            if(gameCanvas != null)
                gameCanvas.SetActive(true);
#else
            if(textField != null)
                textField.enabled = true;
            if(gameCanvas != null)
                gameCanvas.SetActive(false);
#endif
        }
    }
}

