using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

using Moralis.Platform.Objects;
using Moralis.Platform.Queries;
using MoralisWeb3ApiSdk;

namespace Main
{
    //Data class that will be saved to Moralis DB.
    [Serializable]
    public class EnemyData: MoralisObject
    {
        public List<float> initPosition;
        public float size = 1f; //Default
        public float speed = 5f; //Default

        public EnemyData() : base("EnemyData") {}
    }
    
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Animator))]
    public class Enemy : MonoBehaviour
    {
        public static Action<string> Dead;
        
        [SerializeField] private Collider rootCollider;

        private bool _initialized;

        private GameObject _player;
        private Animator _animator;
        private Collider _collider;

        private bool _isDodging = false;
        private const float DodgeDefaultSpeed = 0.5f;
        private Tween _dodgeTween;

        //Fields that will be filled from our respective EnemyData object in the DB when this GameObject is Initialized.
        private string _id;
        private float _size;
        private float _moveSpeed;

        #region UNITY_LIFECYCLE

        private void OnEnable()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            _animator = GetComponent<Animator>();
            _collider = GetComponent<Collider>();
            
            //We don't want to collide with out root
            Physics.IgnoreCollision(_collider, rootCollider);
        }

        void FixedUpdate()
        {
            if (!_initialized) return;
            
            //We are always looking at the Player.
            var playerPosition = _player.transform.position;
            transform.LookAt(playerPosition, Vector3.up);

            if (_isDodging) return;
            
            //If we are not dodging, move towards the player and look at him.
            transform.position = Vector3.MoveTowards(transform.position, playerPosition, _moveSpeed * Time.deltaTime);
        }

        private void OnDestroy()
        {
            //Security check for Tween
            _dodgeTween?.Kill();
            Dead?.Invoke(_id);
        }

        #endregion

        #region PUBLIC_METHODS

        public void Initialize(string objectId, float defaultSize, float defaultSpeed)
        {
            _id = objectId;
            _size = defaultSize;
            _moveSpeed = defaultSpeed;

            transform.localScale *= _size;
            
            _initialized = true;
        }

        #endregion

        #region COLLISION/TRIGGER_METHODS

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Projectile"))
            {
                //If it's already dodging, we don't want to dodge again.
                if (_isDodging) return;

                //If it's not, we could randomly decide to dodge. 50% chance.
                var random = new System.Random();
                var dodgeBool = random.Next(2) == 1;

                //It always dodges now, but we could use "dodgeBool" to randomize it.
                if (true)
                {
                    float defaultValue = 2f;
                    //What we do randomize, is the X axis value we will take when the projectile tries to hits us (Left or Right?).
                    var xRandom = new System.Random();
                    var xBool = xRandom.Next(2) == 1;
                    float newX;
                    
                    if (xBool)
                    {
                        newX = transform.position.x + defaultValue;    
                    }
                    else
                    {
                        newX = transform.position.x - defaultValue;
                    }

                    //And then, we also randomize how UP we will go.
                    float randomY = UnityEngine.Random.Range(0f, defaultValue);
                    float newY = transform.position.y + randomY;
                    
                    //We finally create the new Vector3 and we call "Dodge()".
                    Vector3 dodgePos = new Vector3(newX, newY, transform.position.z);
                    Dodge(dodgePos, 0.1f, true);
                }
            }

            //If our boundaries collide with another EnemyRoot, let's dodge to the opposite direction.
            if (other.gameObject.CompareTag("EnemyBounds")) 
            {
                //Only if we're not dodging already.
                if (_isDodging) return;

                var myPosition = transform.position;
                Vector3 directionToOther = (myPosition - other.transform.position).normalized;
                Vector3 dodgePos = myPosition + (directionToOther * 2f);

                Dodge(dodgePos, DodgeDefaultSpeed, false);
            }
            
            //If we hit the player
            if (other.gameObject.CompareTag("Player"))
            {
                //With this animation, the root of the Enemy will advance and try to hit the Player.
                //Go to EnemyRoot to see what happens there.
                _animator.Play("Attack03");
            }
        }

        private void OnTriggerStay(Collider other)
        {
            //If we collide with floor we want dodge in the UP direction.
            if (other.gameObject.CompareTag("Floor")) 
            {
                //Only if we're not dodging already.
                if (_isDodging) return;

                var myPosition = transform.position;
                Vector3 dodgePos = new Vector3(myPosition.x, myPosition.y + 2f, myPosition.z);

                Dodge(dodgePos, DodgeDefaultSpeed, false);
            }
        }

        #endregion

        #region PRIVATE_METHODS

        private void Dodge(Vector3 targetPos, float dodgeSpeed, bool fromProjectile)
        {
            _isDodging = true;

            //If we're dodging from the projectile we play the animation
            if (fromProjectile)
            {
                _animator.Play("Victory");
            }

            _dodgeTween = transform.DOMove(targetPos, dodgeSpeed).OnComplete(() => { _isDodging = false; });
        }

        #endregion
    }
}