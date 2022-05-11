/// <summary>
/// SURGE FRAMEWORK
/// Author: Bob Berkebile
/// Email: bobb@pixelplacement.com
/// 
/// Simple system for turning anything into a button.
/// 
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using Pixelplacement.TweenSystem;
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR;
#else
using UnityEngine.VR;
#endif

namespace Pixelplacement
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    [ExecuteInEditMode]
    public sealed class ColliderButton : MonoBehaviour
    {
        //Public Events:
        public ColliderButtonEvent OnSelected;
        public ColliderButtonEvent OnDeselected;
        public ColliderButtonEvent OnClick;
        public ColliderButtonEvent OnPressed;
        public ColliderButtonEvent OnReleased;
        public static event Action<ColliderButton> OnSelectedGlobal;
        public static event Action<ColliderButton> OnDeselectedGlobal;
        public static event Action<ColliderButton> OnClickGlobal;
        public static event Action<ColliderButton> OnPressedGlobal;
        public static event Action<ColliderButton> OnReleasedGlobal;

        //Public Enums:
        public enum EaseType { EaseOut, EaseOutBack };

        //Public Properties:
        public bool IsSelected
        {
            get;
            private set;
        }

        //Public Variables:
        public KeyCode[] keyInput;
        public bool _unityEventsFolded;
        public bool _scaleResponseFolded;
        public bool _colorResponseFolded;
        public bool applyColor;
        public bool applyScale;
        public LayerMask collisionLayerMask = -1;
        public Renderer colorRendererTarget;
        public Image colorImageTarget;
        public Color selectedColor = Color.gray;
        public Color pressedColor = Color.green;
        public Color disabledColor = new Color(.5f, .5f, .5f, .5f);
        public float colorDuration = .1f;
        public Transform scaleTarget;
        public Vector3 normalScale;
        public Vector3 selectedScale;
        public Vector3 pressedScale;
        public float scaleDuration = .1f;
        public EaseType scaleEaseType;
        public bool resizeGUIBoxCollider = true;
        public bool centerGUIBoxCollider = true;
        public Vector2 guiBoxColliderPadding;
        public bool interactable = true;

        //Private Variables:
        bool _clicking;
        int _selectedCount;
        bool _colliderSelected;
        bool _pressed;
        bool _released;
        bool _vrRunning;
        RectTransform _rectTransform;
        EventTrigger _eventTrigger;
        EventTrigger.Entry _pressedEventTrigger;
        EventTrigger.Entry _releasedEventTrigger;
        EventTrigger.Entry _enterEventTrigger;
        EventTrigger.Entry _exitEventTrigger;
        int _colliderCount;
        BoxCollider _boxCollider;
        TweenBase _colorTweenImage = null;
        TweenBase _colorTweenMaterial;
        TweenBase _scaleTween;
        Color _normalColorRenderer;
        Color _normalColorImage;
        bool _interactableStatus = true;

        //Init:
        private void Reset()
        {
            //var sets:
            applyColor = true;
            keyInput = new KeyCode[] { KeyCode.Mouse0 };

            //hook up image to help users:
            Image image = GetComponent<Image>();
            if (image != null)
            {
                colorImageTarget = image;
            }

            //hook up renderer to help users:
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null && renderer.sharedMaterial.HasProperty("_Color"))
            {
                colorRendererTarget = renderer;
            }
        }

        private void Awake()
        {
            if (Application.isPlaying)
            {
                //color setups:
                if (colorRendererTarget != null)
                {
                    if (colorRendererTarget.material.HasProperty("_Color"))
                    {
                        _normalColorRenderer = colorRendererTarget.material.color;
                    }
                }
                if (colorImageTarget != null)
                {
                    _normalColorImage = colorImageTarget.color;
                }
            }

            //scale setup:
            scaleTarget = transform;
            normalScale = transform.localScale;

            //set initial size on gui collider:
            _rectTransform = GetComponent<RectTransform>();
            _boxCollider = GetComponent<BoxCollider>();
            if (_rectTransform != null && _boxCollider != null) ResizeGUIBoxCollider(_boxCollider);

            //set up rigidbody:
            GetComponent<Rigidbody>().isKinematic = true;

            //refs:
            _rectTransform = GetComponent<RectTransform>();
            _boxCollider = GetComponent<BoxCollider>();

            if (!Application.isPlaying) return;

            //rect and event triggers:
            _rectTransform = GetComponent<RectTransform>();
            if (_rectTransform != null)
            {
                _eventTrigger = gameObject.AddComponent<EventTrigger>();
                _pressedEventTrigger = new EventTrigger.Entry();
                _pressedEventTrigger.eventID = EventTriggerType.PointerDown;
                _releasedEventTrigger = new EventTrigger.Entry();
                _releasedEventTrigger.eventID = EventTriggerType.PointerUp;
                _enterEventTrigger = new EventTrigger.Entry();
                _enterEventTrigger.eventID = EventTriggerType.PointerEnter;
                _exitEventTrigger = new EventTrigger.Entry();
                _exitEventTrigger.eventID = EventTriggerType.PointerExit;
            }

            //events:
            if (_rectTransform != null)
            {
                //event registrations:
                _pressedEventTrigger.callback.AddListener((data) => { OnPointerDownDelegate((PointerEventData)data); });
                _eventTrigger.triggers.Add(_pressedEventTrigger);
                _releasedEventTrigger.callback.AddListener((data) => { OnPointerUpDelegate((PointerEventData)data); });
                _eventTrigger.triggers.Add(_releasedEventTrigger);
                _enterEventTrigger.callback.AddListener((data) => { OnPointerEnterDelegate((PointerEventData)data); });
                _eventTrigger.triggers.Add(_enterEventTrigger);
                _exitEventTrigger.callback.AddListener((data) => { OnPointerExitDelegate((PointerEventData)data); });
                _eventTrigger.triggers.Add(_exitEventTrigger);
            }
        }

        //Flow:
        private void OnEnable()
        {
            if (!Application.isPlaying) return;

            ColorReset();
        }

        private void OnDisable()
        {
            if (!Application.isPlaying) return;

            //resets:
            _pressed = false;
            _released = false;
            _clicking = false;
            _colliderSelected = false;
            _selectedCount = 0;
            _colliderCount = 0;

            ColorReset();
            ScaleReset();
        }

        //Loops:
        private void Update()
        {
            //disabled?
            if (_interactableStatus != interactable)
            {
                if (interactable)
                {
                    ColorNormal();
                }
                else
                {
                    ColorDisabled();
                }

                //handle a Unity GUI button in case it is also attached:
                Button button = GetComponent<Button>();
                if (button != null)
                {
                    button.interactable = interactable;
                }

                _interactableStatus = interactable;
            }

            //update gui colliders:
            if (resizeGUIBoxCollider && _rectTransform != null && _boxCollider != null)
            {
                //fit a box collider:
                ResizeGUIBoxCollider(_boxCollider);
            }

            //for in editor updating of the gui collider:
            if (!Application.isPlaying) return;

            //VR status:
#if UNITY_2017_2_OR_NEWER
            _vrRunning = (XRSettings.isDeviceActive);
#else
            _vrRunning = (VRSettings.isDeviceActive);
#endif

            if (!interactable) return;

            //collider collision started:
            if (!_colliderSelected && _colliderCount > 0)
            {
                _colliderSelected = true;
                Selected();
            }

            //collider collision ended:
            if (_colliderSelected && _colliderCount == 0)
            {
                _colliderSelected = false;
                Deselected();
            }

            //process input:
            if (keyInput != null && _selectedCount > 0)
            {
                foreach (var item in keyInput)
                {
                    if (Input.GetKeyDown(item))
                    {
                        if (_selectedCount == 0) return;
                        Pressed();
                    }

                    if (Input.GetKeyUp(item))
                    {
                        Released();
                    }
                }
            }
        }

        //Event Handlers:
        private void OnTriggerStay(Collider other)
        {
            if (_colliderCount == 0)
            {
                _colliderCount++;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            _colliderCount++;
        }

        private void OnTriggerExit(Collider other)
        {
            _colliderCount--;
        }

        private void OnPointerDownDelegate(PointerEventData data)
        {
            if (Array.IndexOf(keyInput, KeyCode.Mouse0) == -1) return;
            Pressed();
        }

        private void OnPointerUpDelegate(PointerEventData data)
        {
            if (Array.IndexOf(keyInput, KeyCode.Mouse0) == -1) return;
            Released();
        }

        private void OnPointerEnterDelegate(PointerEventData data)
        {
            Selected();
        }

        private void OnPointerExitDelegate(PointerEventData data)
        {
            Deselected();
        }

        private void OnMouseDown()
        {
            if (_vrRunning) return;
            if (Array.IndexOf(keyInput, KeyCode.Mouse0) == -1) return;
            Pressed();
        }

        private void OnMouseUp()
        {
            if (_vrRunning) return;
            if (Array.IndexOf(keyInput, KeyCode.Mouse0) == -1) return;
            Released();
            if (Application.isMobilePlatform)
            {
                Deselected();
            }
        }

        private void OnMouseEnter()
        {
            if (Application.isMobilePlatform) return;
            if (_vrRunning) return;
            Selected();
        }

        private void OnMouseExit()
        {
            if (_vrRunning) return;
            Deselected();
        }

        public void Deselected()
        {
            if (!interactable) return;

            _selectedCount--;
            if (_selectedCount < 0) _selectedCount = 0;
            if (_selectedCount > 0) return;
            _clicking = false;
            ColorNormal();
            ScaleNormal();
            if (!Application.isMobilePlatform)
            {
                if (OnDeselected != null) OnDeselected.Invoke(this);
                if (OnDeselectedGlobal != null) OnDeselectedGlobal.Invoke(this);
                IsSelected = false;
            }
        }

        public void Selected()
        {
            if (!interactable) return;

            _selectedCount++;
            if (_selectedCount != 1) return;

            _pressed = false;
            _released = false;

            _clicking = false;
            ColorSelected();
            ScaleSelected();
            if (OnSelected != null) OnSelected.Invoke(this);
            if (OnSelectedGlobal != null) OnSelectedGlobal(this);
            IsSelected = true;
        }

        public void Pressed()
        {
            if (!interactable) return;

            //handheld devices normally have touch screens which means selection is not a separate phase:
            if (SystemInfo.deviceType != DeviceType.Handheld)
            {
                if (_selectedCount <= 0) return;

            }

            if (_pressed) return;
            _pressed = true;
            _released = false;

            _clicking = true;
            ColorPressed();
            ScalePressed();
            if (OnPressed != null) OnPressed.Invoke(this);
            if (OnPressedGlobal != null) OnPressedGlobal(this);
        }

        public void Released()
        {
            if (!interactable) return;

            if (_released) return;
            _pressed = false;
            _released = true;

            if (_selectedCount != 0)
            {
                ColorSelected();
                ScaleSelected();
            }

            if (_clicking)
            {
                if (OnClick != null) OnClick.Invoke(this);
                if (OnClickGlobal != null) OnClickGlobal(this);
            }
            _clicking = false;
            if (OnReleased != null) OnReleased.Invoke(this);
            if (OnReleasedGlobal != null) OnReleasedGlobal(this);
        }

        //Private Methods:
        private void ResizeGUIBoxCollider(BoxCollider boxCollider)
        {
            if (!resizeGUIBoxCollider) return;
            boxCollider.size = new Vector3(_rectTransform.rect.width + guiBoxColliderPadding.x, _rectTransform.rect.height + guiBoxColliderPadding.y, _boxCollider.size.z);

            if (centerGUIBoxCollider)
            {
                float centerX = (Mathf.Abs(_rectTransform.pivot.x - 1) - .5f) * boxCollider.size.x;
                float centerY = (Mathf.Abs(_rectTransform.pivot.y - 1) - .5f) * boxCollider.size.y;
                boxCollider.center = new Vector3(centerX, centerY, boxCollider.center.z);
            }
        }

        private void ColorReset()
        {
            //stop running tweens:
            if (_colorTweenImage != null)
            {
                _colorTweenImage.Stop();
            }

            if (_colorTweenMaterial != null)
            {
                _colorTweenMaterial.Stop();
            }

            if (!applyColor) return;

            //reset material color:
            if (colorRendererTarget != null && colorRendererTarget.material.HasProperty("_Color"))
            {
                colorRendererTarget.material.color = _normalColorRenderer;
            }

            //reset image color:
            if (colorImageTarget != null)
            {
                colorImageTarget.color = _normalColorImage;
            }
        }

        private void ColorNormal()
        {
            if (!applyColor) return;

            //tween material color:
            if (colorRendererTarget != null && colorRendererTarget.material.HasProperty("_Color"))
            {
                _colorTweenMaterial = Tween.Color(colorRendererTarget, _normalColorRenderer, colorDuration, 0, null, Tween.LoopType.None, null, null, false);
            }

            //tween image color:
            if (colorImageTarget != null)
            {
                Tween.Color(colorImageTarget, _normalColorImage, colorDuration, 0, null, Tween.LoopType.None, null, null, false);
            }
        }

        private void ColorSelected()
        {
            if (!applyColor) return;

            //tween material color:
            if (colorRendererTarget != null && colorRendererTarget.material.HasProperty("_Color"))
            {
                _colorTweenMaterial = Tween.Color(colorRendererTarget, selectedColor, colorDuration, 0, null, Tween.LoopType.None, null, null, false);
            }

            //tween image color:
            if (colorImageTarget != null)
            {
                Tween.Color(colorImageTarget, selectedColor, colorDuration, 0, null, Tween.LoopType.None, null, null, false);
            }
        }

        private void ColorPressed()
        {
            if (!applyColor) return;

            //tween material color:
            if (colorRendererTarget != null && colorRendererTarget.material.HasProperty("_Color"))
            {
                _colorTweenMaterial = Tween.Color(colorRendererTarget, pressedColor, colorDuration, 0, null, Tween.LoopType.None, null, null, false);
            }

            //tween image color:
            if (colorImageTarget != null)
            {
                Tween.Color(colorImageTarget, pressedColor, colorDuration, 0, null, Tween.LoopType.None, null, null, false);
            }
        }

        private void ColorDisabled()
        {
            if (!applyColor) return;

            //tween material color:
            if (colorRendererTarget != null && colorRendererTarget.material.HasProperty("_Color"))
            {
                _colorTweenMaterial = Tween.Color(colorRendererTarget, disabledColor, colorDuration, 0, null, Tween.LoopType.None, null, null, false);
            }

            //tween image color:
            if (colorImageTarget != null)
            {
                Tween.Color(colorImageTarget, disabledColor, colorDuration, 0, null, Tween.LoopType.None, null, null, false);
            }
        }

        private void ScaleReset()
        {
            if (_scaleTween != null) _scaleTween.Stop();
            scaleTarget.localScale = normalScale;
        }

        private void ScaleNormal()
        {
            if (!applyScale) return;
            AnimationCurve curve = null;
            switch (scaleEaseType)
            {
                case EaseType.EaseOut:
                    curve = Tween.EaseOutStrong;
                    break;

                case EaseType.EaseOutBack:
                    curve = Tween.EaseOutBack;
                    break;
            }
            _scaleTween = Tween.LocalScale(scaleTarget, normalScale, scaleDuration, 0, curve, Tween.LoopType.None, null, null, false);
        }

        private void ScaleSelected()
        {
            if (!applyScale) return;
            AnimationCurve curve = null;
            switch (scaleEaseType)
            {
                case EaseType.EaseOut:
                    curve = Tween.EaseOutStrong;
                    break;

                case EaseType.EaseOutBack:
                    curve = Tween.EaseOutBack;
                    break;
            }
            _scaleTween = Tween.LocalScale(scaleTarget, Vector3.Scale(normalScale, selectedScale), scaleDuration, 0, curve, Tween.LoopType.None, null, null, false);
        }

        private void ScalePressed()
        {
            if (!applyScale) return;
            AnimationCurve curve = null;
            switch (scaleEaseType)
            {
                case EaseType.EaseOut:
                    curve = Tween.EaseOutStrong;
                    break;

                case EaseType.EaseOutBack:
                    curve = Tween.EaseOutBack;
                    break;
            }
            _scaleTween = Tween.LocalScale(scaleTarget, Vector3.Scale(normalScale, pressedScale), scaleDuration, 0, curve, Tween.LoopType.None, null, null, false);
        }
    }
}