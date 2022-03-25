using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;

namespace Main
{
    [RequireComponent(typeof(PlayerInput))]
    public class WeaponController : MonoBehaviour
    {
        [Header("Projectile")]
        [SerializeField] private Transform aimTransform;
        [SerializeField] private Transform projectileInitTransform;
        [SerializeField] private GameObject projectilePrefab;

        [Header("Animator")]
        [SerializeField] private Animator gunAnimator;
        
        [Header("AudioClips")]
        [SerializeField] private AudioClip chargingSound;
        [SerializeField] private AudioClip releaseSound;

        [Header("HUD")]
        [SerializeField] private Slider overheatSlider;
        
        //Components
        private PlayerInput _playerInput;
        private ProjectileBehaviour _currentProjectile;
        private AudioSource _audioSource;
        
        //Projectile charging
        private Vector3 _maxScale;
        private bool _charging;
        private const float ChargeSpeed = 0.05f;
        private const float ForceMultiplier = 0.5f;
        private float _releaseExtraForce = 1f;
        
        //Overheat
        private const float DefaultOverheat = 0.25f;
        private const float RefreshingTime = 3f;
        private Tween _refreshHeatTween;

        #region UNITY_LIFECYCLE

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _playerInput = GetComponent<PlayerInput>();
            
            //We don't listen to any input until the user has logged in.
            _playerInput.DeactivateInput();
        }

        private void OnEnable()
        {
            MoralisWeb3Manager.LoggedInSuccessfully += OnUserLoggedIn;
            PlayerController.Dead += OnPlayerDead;
            Boss.Dead += OnBossDead;
        }

        private void OnDisable()
        {
            MoralisWeb3Manager.LoggedInSuccessfully -= OnUserLoggedIn;
            PlayerController.Dead -= OnPlayerDead;
            Boss.Dead -= OnBossDead;
        }

        private void Start()
        {
            //We want to deactivate the collision between the released projectile and the walls. We do that through Layers.
            Physics.IgnoreLayerCollision(6, 7); //Layer "Projectile" and Layer "Walls".
        }
        
        private void Update()
        {
            if (_charging)
            {
                _currentProjectile.transform.localScale += Vector3.one * Time.deltaTime * ChargeSpeed;
                _releaseExtraForce += Time.deltaTime * ForceMultiplier;

                if (_currentProjectile.transform.localScale.x >= _maxScale.x)
                {
                    if (_audioSource.isPlaying) _audioSource.Stop();
                    
                    _charging = false;
                    Debug.Log("Fireball charged");
                }
            }
        }
        
        #endregion

        #region NEW_INPUT_SYSTEM

        public void HandleFireAction(InputAction.CallbackContext context)
        {
            // We don't want to shoot when this weapon is disabled.
            if (!enabled) return;
            
            if (context.performed)
            {
                var projectile = Instantiate(projectilePrefab, projectileInitTransform);
                _currentProjectile = projectile.GetComponent<ProjectileBehaviour>();

                _maxScale = _currentProjectile.transform.localScale;
                _currentProjectile.transform.localScale /= 2f;
                _releaseExtraForce = 1f;

                if (_audioSource.isPlaying) _audioSource.Stop();
                _audioSource.clip = chargingSound;
                _audioSource.Play();
                
                _charging = true;
            }

            if (context.canceled)
            {
                _currentProjectile.Release(aimTransform, _releaseExtraForce);
                
                if (_audioSource.isPlaying) _audioSource.Stop();
                _audioSource.clip = releaseSound;
                _audioSource.Play();
                gunAnimator.Play("GunAnimation");
                
                _charging = false;
                Debug.Log("Fireball released");
                
                //Overheat increments (less slider value).
                overheatSlider.value -= DefaultOverheat * _releaseExtraForce;
                
                //Very important coroutine execution in order to prevent another charge before properly refreshing.
                StartCoroutine(WaitToRefresh(DefaultOverheat * _releaseExtraForce));
            }
        }
        
        #endregion

        #region EVENT_HANDLERS

        private void OnUserLoggedIn()
        {
            _playerInput.ActivateInput();
        }
        
        private void OnPlayerDead()
        {
            //We no longer need this weapon.
            enabled = false;
        }
        
        private void OnBossDead(Vector3 bossLastPos)
        {
            //We no longer need this weapon.
            enabled = false;
        }

        #endregion
        
        #region PRIVATE_METHODS

        private IEnumerator WaitToRefresh(float waitTime)
        {
            _playerInput.DeactivateInput();
            if (_refreshHeatTween != null) _refreshHeatTween.Kill();

            //Nice implementation.
            if (overheatSlider.value < 0.5f) waitTime *= 2f;
            if (overheatSlider.value < 0.25f) waitTime *= 2f;
            
            yield return new WaitForSeconds(waitTime);
            
            _playerInput.ActivateInput();
            //Start the tween to refresh overheat.
            _refreshHeatTween = overheatSlider.DOValue(1, RefreshingTime);
        }
        
        #endregion
    }
}
