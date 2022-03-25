using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class ProjectileBehaviour : MonoBehaviour
    {
        [SerializeField] private GameObject explosionPrefab;
    
        private Rigidbody _rigidbody;
        private Collider _collider;

        private float _defaultForceMultiplier = 200f;
        private float _extraForceMultiplier;
    
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();

            _collider.enabled = false;
        }

        public void Release(Transform directionTransform, float extraForceMultiplier)
        {
            _extraForceMultiplier = extraForceMultiplier;
            _rigidbody.AddForce(directionTransform.forward * _defaultForceMultiplier * _extraForceMultiplier);
            _rigidbody.useGravity = true;
            transform.SetParent(null);
            _collider.enabled = true;
        }

        private void OnCollisionEnter(Collision other)
        {
            //We don't want to explode when hitting an Enemy boundary (we want to give the enemy a chance to dodge).
            if (other.gameObject.CompareTag("EnemyBounds")) return;

            //If it hits the root of the enemy or any other thing, explode!
            var explosionPosition = transform.position;
            Instantiate(explosionPrefab, explosionPosition, Quaternion.identity);
            
            Destroy(gameObject);
        }

        public float GetExtraForceMultiplier()
        {
            return _extraForceMultiplier;
        }
    }   
}
