using System.Linq;
using UnityEditor;
using UnityEngine;

namespace StarterAssets
{
    public partial class StarterAssetsDeployMenu : ScriptableObject
    {
        // prefab paths
        private const string ThirdPersonPrefabPath = "/ThirdPersonController/Prefabs/";
        private const string PlayerArmaturePrefabName = "PlayerArmature";

#if STARTER_ASSETS_PACKAGES_CHECKED
        /// <summary>
        /// Check the Armature, main camera, cinemachine virtual camera, camera target and references
        /// </summary>
        [MenuItem(MenuRoot + "/Reset Third Person Controller Armature", false)]
        static void ResetThirdPersonControllerArmature()
        {
            var thirdPersonControllers = FindObjectsOfType<ThirdPersonController>();
            var player = thirdPersonControllers.FirstOrDefault(controller => controller.GetComponent<Animator>() && controller.CompareTag(PlayerTag));
            GameObject playerGameObject;

            // player
            if (player == null)
                HandleInstantiatingPrefab(StarterAssetsPath + ThirdPersonPrefabPath,
                    PlayerArmaturePrefabName, out playerGameObject);
            else
                playerGameObject = player.gameObject;

            // cameras
            CheckCameras(ThirdPersonPrefabPath, playerGameObject.transform);
        }

        [MenuItem(MenuRoot + "/Reset Third Person Controller Capsule", false)]
        static void ResetThirdPersonControllerCapsule()
        {
            var thirdPersonControllers = FindObjectsOfType<ThirdPersonController>();
            var player = thirdPersonControllers.FirstOrDefault(controller => !controller.GetComponent<Animator>() && controller.CompareTag(PlayerTag));
            GameObject playerGameObject;

            // player
            if (player == null)
                HandleInstantiatingPrefab(StarterAssetsPath + ThirdPersonPrefabPath,
                    PlayerCapsulePrefabName, out playerGameObject);
            else
                playerGameObject = player.gameObject;

            // cameras
            CheckCameras(ThirdPersonPrefabPath, playerGameObject.transform);
        }
#endif
    }
}