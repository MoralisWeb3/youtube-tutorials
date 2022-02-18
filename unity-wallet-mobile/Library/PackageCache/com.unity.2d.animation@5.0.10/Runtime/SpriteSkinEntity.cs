using System;
using UnityEngine;
using UnityEngine.Scripting;

#if ENABLE_ENTITIES
using Unity.Entities;
#endif

namespace UnityEngine.U2D.Animation
{

    [AddComponentMenu("")]
    [Preserve]
    internal class SpriteSkinEntity
#if ENABLE_ENTITIES
        : GameObjectEntity
#else
        : MonoBehaviour
#endif
    {

#if ENABLE_ENTITIES
        bool enableEntitiesCached = true;
#if UNITY_EDITOR
        static bool assemblyReload = false;
#endif

        SpriteSkin m_SpriteSkin;
        SpriteSkin spriteSkin
        {
            get
            {
                if (m_SpriteSkin == null)
                    m_SpriteSkin = GetComponent<SpriteSkin>();
                return m_SpriteSkin;
            }
        }

        bool entitiesEnabled
        {
            get
            {
                if (m_SpriteSkin == null)
                    return false;
                return m_SpriteSkin.entitiesEnabled;
            }
        }

        protected override void OnEnable()
        {
            if (entitiesEnabled)
            { 
                base.OnEnable();
                SetupEntity();
                SetupSpriteSkin();

    #if UNITY_EDITOR
                UnityEditor.AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
                UnityEditor.AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
    #endif
            }
        }

#if UNITY_EDITOR
        public void OnBeforeAssemblyReload()
        {
            assemblyReload = true;
        }

        public void OnAfterAssemblyReload()
        {
            assemblyReload = false;
        }
#endif

        protected override void OnDisable()
        {
            if (entitiesEnabled)
            { 
                DeactivateSkinning();
#if UNITY_EDITOR
            if (!assemblyReload)
#endif
                base.OnDisable();
            }
            if (spriteSkin.isValid)
                spriteSkin.entitiesEnabled = false;
        }

        private void SetupEntity()
        {
            if (EntityManager == null)
                return;

            EntityManager.AddBuffer<Vertex>(Entity);
            EntityManager.AddBuffer<BoneTransform>(Entity);
            EntityManager.AddComponent(Entity, typeof(WorldToLocal));
            EntityManager.AddSharedComponentData(Entity, new SpriteComponent() { Value = null });
        }

        private void SetupSpriteSkin()
        {
            if (spriteSkin != null)
            {
                spriteSkin.ForceSkinning = true;

                if (spriteSkin.bounds.extents != Vector3.zero) //Maybe log a warning?
                    InternalEngineBridge.SetLocalAABB(spriteSkin.spriteRenderer, spriteSkin.bounds);
            }
        }

        private void DeactivateSkinning()
        {
            if (spriteSkin != null)
                spriteSkin.DeactivateSkinning();
        }

        private void Update()
        {
            if (entitiesEnabled != enableEntitiesCached)
            {
                if (entitiesEnabled)
                    OnEnable();
                else
             
                    OnDisable();
                enableEntitiesCached = entitiesEnabled;
            }
        }

#endif
    } 
}