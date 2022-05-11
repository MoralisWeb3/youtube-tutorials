using System.Collections.Generic;
using System.Threading;
#if UNITY_WEBGL && !UNITY_EDITOR
using AOT;
using System.Runtime.InteropServices;
#endif
using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace Moralis.Network.Client
{
    public class MainThreadUtil : MonoBehaviour
    {
        private long mainThreadId;
        public static MainThreadUtil Instance { get; private set; }
        public static SynchronizationContext synchronizationContext { get; private set; }

        private Queue<UnityAction> actions = new Queue<UnityAction>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Setup()
        {
            Instance = new GameObject("MainThreadUtil")
                .AddComponent<MainThreadUtil>();
            synchronizationContext = SynchronizationContext.Current;
        }

        public static void Run(IEnumerator waitForUpdate)
        {
            synchronizationContext.Post(_ => Instance.StartCoroutine(
                waitForUpdate), null);
        }

        public static void Run(UnityAction action)
        {
            if (Thread.CurrentThread.ManagedThreadId == Instance.mainThreadId)
                action(); //If we trying to run this in the main thread, then we can just run it now
            else
                Instance.actions.Enqueue(action);
        }

        void Update()
        {
            while (actions.Count > 0)
            {
                var action = actions.Dequeue();

                if (action != null)
                {
                    action();
                }
            }
        }

        void Awake()
        {
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(gameObject);
            mainThreadId = Thread.CurrentThread.ManagedThreadId;
        }
    }
}
