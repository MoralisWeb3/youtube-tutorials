using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using DG.Tweening;
using Vector3 = UnityEngine.Vector3;

namespace Main
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlasmaSphere : MonoBehaviour
    {
        //Components
        private Rigidbody _rigidbody;
        
        //Variables
        private const float DefaultForceMultiplier = 500f;

        //Other
        private Transform _playerT;
    
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            //Find Player.
            _playerT = GameObject.FindGameObjectWithTag("Player").transform;
            
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, 1f).OnComplete(() =>
            {
                ReleaseTowards(_playerT);
            });
        }

        private void ReleaseTowards(Transform targetTransform)
        {
            //Find correct direction vector.
            Vector3 directorVector = (targetTransform.position - transform.position).normalized;
            
            _rigidbody.AddForce(directorVector * DefaultForceMultiplier);
            transform.SetParent(null);

            StartCoroutine(Alive());
        }

        private IEnumerator Alive()
        {
            yield return new WaitForSeconds(5f);
            
            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            //If we hit the player just destroy the sphere.
            if (other.gameObject.CompareTag("Player"))
            {
                Destroy(gameObject);   
            }
        }
    }   
}