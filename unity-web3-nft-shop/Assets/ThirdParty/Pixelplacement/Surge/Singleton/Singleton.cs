/// <summary>
/// SURGE FRAMEWORK
/// Author: Bob Berkebile
/// Email: bobb@pixelplacement.com
/// 
/// A generic singleton.
/// 
/// </summary>

using System;
using UnityEngine;
using System.Collections;

namespace Pixelplacement
{
    [RequireComponent(typeof(Initialization))]
    public abstract class Singleton<T> : MonoBehaviour
    {
        //Public Properties:
        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("Singleton not registered! Make sure the GameObject running your singleton is active in your scene and has an Initialization component attached.");
                    return default(T);
                }
                return _instance;
            }
        }

        //Private Variables:
        [SerializeField] bool _dontDestroyOnLoad = false;
        static T _instance;

        //Virtual Methods:
        /// <summary>
        /// Override this method to have code run when this singleton is initialized which is guaranteed to run before Awake and Start.
        /// </summary>
        protected virtual void OnRegistration()
        {
        }

        //Public Methods:
        /// <summary>
        /// Generic method that registers the singleton instance.
        /// </summary>
        public void RegisterSingleton(T instance)
        {
            _instance = instance;
        }

        //Private Methods:
        protected void Initialize(T instance)
        {
            if (_dontDestroyOnLoad && _instance != null)
            {
                //there is already an instance:
                Destroy(gameObject);
                return;
            }

            if (_dontDestroyOnLoad)
            {
                //don't destroy on load only works on root objects so let's force this transform to be a root object:
                transform.parent = null;
                DontDestroyOnLoad(gameObject);
            }

            _instance = instance;
            OnRegistration();
        }
    }
}