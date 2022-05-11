using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Main
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioSource))]
    public class Boss : MonoBehaviour
    {
        public static Action Awaken;
        public static Action<Vector3> Dead;
        
        [Header("Properties")]
        [SerializeField] private float moveSpeed;
        private bool _awaken;

        [Header("Attack Related")] 
        [SerializeField] private Transform aimTransform;
        [SerializeField] private GameObject plasmaSpherePrefab;
        private const float DefaultWaitTime = 1.5f;
        
        [Header("Life Related")]
        [SerializeField] private Slider lifeBar;
        [SerializeField] private GameObject deathExplosion;

        //Components
        private Animator _animator;
        private AudioSource _audioSource;
        
        //Variables
        private GameObject _player;
        private bool _playerIsNear;

        private void OnEnable()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            _animator = GetComponent<Animator>();
            _audioSource = GetComponent<AudioSource>();
            
            lifeBar.gameObject.SetActive(false);
        }
        
        void FixedUpdate()
        {
            //We look at the player but we don't care about his height.
            var playerPosition = new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z);
            transform.LookAt(playerPosition, Vector3.up);
            
            if (!_playerIsNear) return;
    
            //We just want to move on the X and Z axis.
            var myPosition = transform.position;
            var targetPos = new Vector3(playerPosition.x, myPosition.y, playerPosition.z);
            myPosition = Vector3.MoveTowards(myPosition, targetPos, moveSpeed * Time.deltaTime);
            transform.position = myPosition;
        }

        private IEnumerator Shoot()
        {
            var waitTime = UnityEngine.Random.Range(DefaultWaitTime, DefaultWaitTime * 2);
            
            while (!_playerIsNear)
            {
                yield return new WaitForSeconds(waitTime);
                
                //Make sure we don't shoot while starting to run.
                if (_playerIsNear) yield break;
                
                _audioSource.Play();
                Instantiate(plasmaSpherePrefab, aimTransform);
            }
        }

        public void HandleLifeValue()
        {
            if (Mathf.Approximately(lifeBar.value, 0))
            {
                var myLastPosition = transform.position;
                Instantiate(deathExplosion, myLastPosition, Quaternion.identity);
                
                Dead?.Invoke(myLastPosition);
                Destroy(gameObject);
            }
        }

        #region COLLISION_RELATED
        
        //Boss has 2 COLLIDERS. On is wider and it's just a trigger to control if the player comes near or not.
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                //We activate lifeBar for the first time;
                if (!lifeBar.gameObject.activeSelf)
                {
                    lifeBar.gameObject.SetActive(true);
                    
                    //We also take the opportunity to trigger Awaken action.
                    Awaken?.Invoke();
                    _awaken = true;
                }
                
                _animator.Play("Run");
                _playerIsNear = true;
                
                StopAllCoroutines();
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _animator.Play("IdleBattle");
                _playerIsNear = false;
                
                StartCoroutine(Shoot());
            }
        }
        
        //The other is the main collider to detect player's projectile collisions.
        private void OnCollisionEnter(Collision other)
        {
            if (!_awaken) return;
            
            if (other.gameObject.CompareTag("Projectile"))
            {
                var projectile = other.gameObject.GetComponent<ProjectileBehaviour>();
                if (projectile is null) return;
                
                //Boss gets more damage depending on how charged is the projectile.
                lifeBar.value -= 0.1f * projectile.GetExtraForceMultiplier();
            }
        }

        #endregion
    }
}
