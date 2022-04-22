using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AngryBirdsWeb3
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(AudioSource))]
    public class Crate : WorldObject
    {
        [Header("FX Sprite")]
        [SerializeField] private Sprite damagedSprite;

        private SpriteRenderer _sr;
        private AudioSource _as;

        private int _maxHits;

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            _as = GetComponent<AudioSource>();
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("Bird"))
            {
                _sr.sprite = damagedSprite;
                _as.Play();
                _maxHits++;

                if (_maxHits == 2)
                {
                    AutoDestroy();
                }
            }
        }
    }   
}
