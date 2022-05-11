using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AngryBirdsWeb3
{
    [RequireComponent(typeof(LineRenderer))]
    public class Slingshot : MonoBehaviour
    {
        [Header("My Elements")]
        [SerializeField] private Transform hookT;

        [Header("Control Variables")]
        [SerializeField] private float maxDragDistance;

        // Private Own Components
        private Rigidbody2D _hookRb;
        private SpringJoint2D _hookJoint;
        private LineRenderer _lineRenderer;
        
        // Private Control Variables
        private bool _projectilePressed;
        private float _releaseDelay;


        #region UNITY_LIFECYCLE

        private void Awake()
        {
            _hookRb = hookT.GetComponent<Rigidbody2D>();
            _hookJoint = hookT.GetComponent<SpringJoint2D>();
            _lineRenderer = GetComponent<LineRenderer>();
            
            //We don't want to enable lineRenderer until we have a projectile ready.
            _lineRenderer.enabled = false;
            
            // Calculate when we have to release our projectile.
            _releaseDelay = 1 / (_hookJoint.frequency * 4);
        }

        private void OnEnable()
        {
            Bird.IsPressed += BirdOnIsPressed;
        }
        
        private void OnDisable()
        {
            Bird.IsPressed -= BirdOnIsPressed;
        }

        private void Update()
        {
            if (_hookJoint.connectedBody is null) return;
            
            SetLineRendererPositions();
            
            if (_projectilePressed)
            {
                DragProjectile();
            }
        }

        #endregion


        #region PUBLIC_METHODS

        public void AddProjectile(GameObject projectile, Texture2D nftTexture)
        {
            var newProjectile = Instantiate(projectile, hookT);

            // If we have selected the NFT we will have a texture. If we clicked "SKIP", it will be null.
            if (nftTexture != null)
            {
                newProjectile.GetComponent<Bird>().Init(nftTexture);
            }

            if (!newProjectile.GetComponent<Rigidbody2D>())
            {
                Debug.Log("Projectile GameObject doesn't have a RigidBody2D component.");
                return;
            }
            
            _hookJoint.connectedBody = newProjectile.GetComponent<Rigidbody2D>();
            _hookJoint.connectedBody.GetComponent<Collider2D>().isTrigger = true;  //We don't want the bird to collide with anything until we have released.
            _lineRenderer.enabled = true;
        }
        
        private IEnumerator Release()
        {
            yield return new WaitForSeconds(_releaseDelay);
            
            _hookJoint.connectedBody.GetComponent<Collider2D>().isTrigger = false; //Now yes, we want to collide with Environment and World Objects.
            _hookJoint.connectedBody = null;
            _lineRenderer.enabled = false;
        }

        #endregion


        #region PRIVATE_METHODS

        private void DragProjectile()
        {
            if (Camera.main is null)
            {
                Debug.Log("We need a Camera on the scene!");
                return;
            }
            
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var distanceToHook = Vector2.Distance(mousePosition, _hookRb.position);

            if (distanceToHook > maxDragDistance)
            {
                Vector2 directionToMouse = (mousePosition - _hookRb.position).normalized;
                _hookJoint.connectedBody.position = _hookRb.position + directionToMouse * maxDragDistance;
            }
            else
            {
                _hookJoint.connectedBody.position = mousePosition;   
            }
        }
        
        private void SetLineRendererPositions()
        {
            var positions = new Vector3[2];

            positions[0] = _hookRb.position;
            positions[1] = _hookJoint.connectedBody.position;
            
            _lineRenderer.SetPositions(positions);
        }

        #endregion
        

        #region EVENT_HANDLERS

        private void BirdOnIsPressed(bool isPressed)
        {
            _projectilePressed = isPressed;

            // When we stop pressing/dragging the projectile, we release!
            if (!isPressed)
            {
                StartCoroutine(Release());   
            }
        }

        #endregion
    }   
}
