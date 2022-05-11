using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AngryBirdsWeb3
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Bird : MonoBehaviour
    {
        // Public Action Events
        public static event Action<bool> IsPressed;
        public static event Action IsDead;
        
        // Private Own Components
        private Rigidbody2D _rb;
        private AudioSource _as;
        private SpriteRenderer _sr;
        private Camera _camera;
        
        // Private Control Variables
        private bool _flying;
        private const float AutoDestroyTime = 3f;
        

        #region UNITY_LIFECYCLE

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.freezeRotation = true;

            _as = GetComponent<AudioSource>();
            _sr = GetComponent<SpriteRenderer>();
            
            _camera = Camera.main;
        }

        private void Update()
        {
            // If we haven't collided with anything and we get out of the screen, we're also going to destroy ourselves.
            if (_flying)
            {
                var xPos = _camera.WorldToScreenPoint(transform.position).x;

                // If Bird is out of Screen borders.
                if (xPos > Screen.width || xPos < 0)
                {
                    DestroyMyself();
                }
            }   
        }

        #endregion


        #region PUBLIC_METHODS

        public void Init(Texture2D tex)
        {
            _sr.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }

        #endregion

        
        #region PRIVATE_METHODS

        private IEnumerator AutoDestroy()
        {
            yield return new WaitForSeconds(AutoDestroyTime);
            
            DestroyMyself();
        }

        private void DestroyMyself()
        {
            IsDead?.Invoke();
            Destroy(gameObject);
        }

        #endregion
        
        
        #region COLLISION_EVENTS

        private void OnCollisionEnter2D(Collision2D col)
        {
            //We just want to call AutoDestroy after one collision, collisions after the first one doesn't count.
            if (!_flying) return;
            
            if (col.gameObject.CompareTag("Floor") || col.gameObject.CompareTag("WorldObject"))
            {
                _flying = false;
                StartCoroutine(AutoDestroy());
            }
        }

        #endregion

        
        #region MOUSE_EVENTS

        private void OnMouseDown()
        {
            _rb.isKinematic = true;
            
            IsPressed?.Invoke(true);
            Debug.Log("MouseDown");
        }

        private void OnMouseUp()
        {
            _rb.isKinematic = false;
            _rb.freezeRotation = false;
            _flying = true;
            _as.Play();

            IsPressed?.Invoke(false);
            Debug.Log("MouseUp");
        }

        #endregion
    }   
}
