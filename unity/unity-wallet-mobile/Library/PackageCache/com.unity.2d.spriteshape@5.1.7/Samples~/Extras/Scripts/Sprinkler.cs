using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace SpriteShapeExtras
{
    public class Sprinkler : MonoBehaviour
    {
    
        public GameObject m_Prefab;
        public float m_RandomFactor = 10.0f;
        public bool m_UseNormals = false;
    
        float Angle(Vector3 a, Vector3 b)
        {
            float dot = Vector3.Dot(a, b);
            float det = (a.x * b.y) - (b.x * a.y);
            return Mathf.Atan2(det, dot) * Mathf.Rad2Deg;
        }
    
        // Use this for initialization. Plant the Prefabs on Startup
        void Start ()
        {
            SpriteShapeController ssc = GetComponent<SpriteShapeController>();
            Spline spl = ssc.spline;
    
            for (int i = 1; i < spl.GetPointCount() - 1; ++i)
            {
                if (Random.Range(0, 100) > (100 - m_RandomFactor) )
                {
                    var go = GameObject.Instantiate(m_Prefab);
                    go.transform.position = spl.GetPosition(i);
    
                    if (m_UseNormals)
                    {
                        Vector3 lt = Vector3.Normalize(spl.GetPosition(i - 1) - spl.GetPosition(i));
                        Vector3 rt = Vector3.Normalize(spl.GetPosition(i + 1) - spl.GetPosition(i));
                        float a = Angle(Vector3.up, lt);
                        float b = Angle(lt, rt);
                        float c = a + (b * 0.5f);
                        if (b > 0)
                            c = (180 + c);
                        go.transform.rotation = Quaternion.Euler(0, 0, c);
                    }
                }
            }
        }
        
        // Update is called once per frame
        void Update ()
        {
    
        }
    }
}