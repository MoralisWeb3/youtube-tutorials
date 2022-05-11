using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.U2D.Animation
{
    internal class VertexBuffer
    {
        /// <summary>
        /// Number of buffers currently allocated.
        /// </summary>        
        public int bufferCount => m_Buffers.Length;
        
        private readonly int m_Id;
        private bool m_IsActive = true;
        private int m_DeactivateFrame = -1;
        
        private NativeByteArray[] m_Buffers;
        private int m_ActiveIndex = 0;

        public VertexBuffer(int id, int size, bool needDoubleBuffering)
        {
            m_Id = id;
            
            var noOfBuffers = needDoubleBuffering ? 2 : 1;
            m_Buffers = new NativeByteArray[noOfBuffers];
            for (var i = 0; i < noOfBuffers; i++)
                m_Buffers[i] = new NativeByteArray(new NativeArray<byte>(size, Allocator.Persistent, NativeArrayOptions.UninitializedMemory));
        }

        public override int GetHashCode() => m_Id;
        private static int GetCurrentFrame() => Time.frameCount;

        public NativeByteArray GetBuffer(int size)
        {
            if (!m_IsActive)
            {
                Debug.LogError($"Cannot request deactivated buffer. ID: {m_Id}");
                return null;
            }

            m_ActiveIndex = (m_ActiveIndex + 1) % m_Buffers.Length;
            if (m_Buffers[m_ActiveIndex].Length != size)
                ResizeBuffer(m_ActiveIndex, size);
                
            return m_Buffers[m_ActiveIndex];
        }

        private void ResizeBuffer(int bufferId, int newSize)
        {
            m_Buffers[bufferId].Dispose();
            m_Buffers[bufferId] = new NativeByteArray(new NativeArray<byte>(newSize, Allocator.Persistent, NativeArrayOptions.UninitializedMemory));
        }

        public void Deactivate()
        {
            if (!m_IsActive)
                    return;
            
            m_IsActive = false;
            m_DeactivateFrame = GetCurrentFrame();
        }

        public void Dispose()
        {
            for (var i = 0; i < m_Buffers.Length; i++)
            {
                if (m_Buffers[i].IsCreated)
                    m_Buffers[i].Dispose();
            }
        }
        
        public bool IsSafeToDispose() => !m_IsActive && GetCurrentFrame() > m_DeactivateFrame;
    }

    internal class BufferManager : ScriptableObject
    {
        private static BufferManager s_Instance;

        private Dictionary<int, VertexBuffer> m_Buffers = new Dictionary<int, VertexBuffer>();
        private Queue<VertexBuffer> m_BuffersToDispose = new Queue<VertexBuffer>();

        /// <summary>
        /// Number of buffers currently allocated.
        /// </summary>
        public int bufferCount 
        {
            get
            {
                var count = 0;
                foreach (var buffer in m_Buffers.Values)
                    count += buffer.bufferCount;
                return count;
            }
        }

        /// <summary>
        /// Creates two buffers instead of one if enabled. 
        /// </summary>
        public bool needDoubleBuffering
        {
            get;
            set;
        }

        public static BufferManager instance
        {
            get
            {
                if (s_Instance == null)
                {
                    var bufferMGRs = Resources.FindObjectsOfTypeAll<BufferManager>();
                    if (bufferMGRs.Length > 0)
                        s_Instance = bufferMGRs[0];
                    else
                        s_Instance = ScriptableObject.CreateInstance<BufferManager>();
                    s_Instance.hideFlags = HideFlags.HideAndDontSave;
                }
                return s_Instance;
            }
        }
        
        private void OnEnable()
        {
            if (s_Instance == null)
                s_Instance = this;
            
            needDoubleBuffering = SystemInfo.renderingThreadingMode != RenderingThreadingMode.Direct;
#if UNITY_EDITOR
            EditorApplication.update += Update;
#else
            Application.onBeforeRender += Update;
#endif
        }        

        private void OnDisable()
        {
            if (s_Instance == this)
                s_Instance = null;

            ForceClearBuffers();
            
#if UNITY_EDITOR            
            EditorApplication.update -= Update;
#else
            Application.onBeforeRender -= Update;
#endif            
        }

        private void ForceClearBuffers()
        {
            foreach (var vertexBuffer in m_Buffers.Values)
                vertexBuffer.Dispose();
            foreach (var vertexBuffer in m_BuffersToDispose)
                vertexBuffer.Dispose();

            m_Buffers.Clear();
            m_BuffersToDispose.Clear();
        }

        public NativeByteArray GetBuffer(int id, int bufferSize)
        {
            Profiler.BeginSample("BufferManager.GetBuffer");
            var foundBuffer = m_Buffers.TryGetValue(id, out var buffer);
            if (!foundBuffer)
                buffer = CreateBuffer(id, bufferSize);

            Profiler.EndSample();
            return buffer?.GetBuffer(bufferSize);
        }
        
        private VertexBuffer CreateBuffer(int id, int bufferSize)
        {
            if (bufferSize < 1)
            {
                Debug.LogError("Cannot create a buffer smaller than 1 byte.");
                return null;
            }

            var buffer = new VertexBuffer(id, bufferSize, needDoubleBuffering);
            m_Buffers.Add(id, buffer);

            return buffer;
        }        

        public void ReturnBuffer(int id)
        {
            Profiler.BeginSample("BufferManager.ReturnBuffer");
            if (m_Buffers.TryGetValue(id, out var buffer))
            {
                buffer.Deactivate();
                m_BuffersToDispose.Enqueue(buffer);
                m_Buffers.Remove(id);
            }

            Profiler.EndSample();
        }

        private void Update()
        {
            Profiler.BeginSample("BufferManager.Update");

            while (m_BuffersToDispose.Count > 0 && m_BuffersToDispose.Peek().IsSafeToDispose())
            {
                var buffer = m_BuffersToDispose.Dequeue();
                buffer.Dispose();
            }

            Profiler.EndSample();
        }
    }
}
