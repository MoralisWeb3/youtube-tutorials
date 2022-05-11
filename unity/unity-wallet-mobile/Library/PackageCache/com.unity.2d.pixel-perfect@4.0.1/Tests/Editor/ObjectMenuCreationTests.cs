using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEditor;

namespace UnityEngine.U2D
{
    internal class ObjectMenuCreationTests
    {
        [Test]
        public void ExecuteMenuCommandCreatesGameObjectWithPixelPerfectCamera()
        {
            var transformCount = Object.FindObjectsOfType<Transform>();
            EditorApplication.ExecuteMenuItem("GameObject/2D Object/Pixel Perfect Camera");
            LogAssert.NoUnexpectedReceived();
            Assert.True(Object.FindObjectsOfType<Transform>().Length > transformCount.Length);
        }
    }
}
