using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

namespace AngryBirdsWeb3
{
    public class ScoreObject : MonoBehaviour
    {
        [SerializeField] private TextMeshPro scoreLabel;
        private const float DestroyTime = 1f;

        private void Awake()
        {
            transform.localScale = Vector3.zero;
        }

        private void Start()
        {
            transform.DOScale(Vector3.one, DestroyTime).SetEase(Ease.OutQuart);
            scoreLabel.DOFade(0, DestroyTime).SetEase(Ease.InQuart);
            
            StartCoroutine(AutoDestroy(DestroyTime));
        }

        public void Init(int scoreValue)
        {
            scoreLabel.text = scoreValue.ToString();
        }
        
        private IEnumerator AutoDestroy(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            Destroy(gameObject);
        }
    }   
}
