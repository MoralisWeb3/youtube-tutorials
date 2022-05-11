using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AngryBirdsWeb3
{
    public abstract class WorldObject : MonoBehaviour
    {
        public static event Action<Vector3, int> IsDestroyed;
        [SerializeField] private int scorePoints;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject explosionPrefab;

        protected void AutoDestroy()
        {
            IsDestroyed?.Invoke(transform.position, scorePoints);
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            
            Destroy(gameObject);
        }
    }   
}
