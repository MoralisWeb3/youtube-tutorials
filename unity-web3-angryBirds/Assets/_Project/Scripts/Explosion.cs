using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AngryBirdsWeb3
{
    [RequireComponent(typeof(Animator))]
    public class Explosion : MonoBehaviour
    {
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            var clipInfo = _animator.GetCurrentAnimatorClipInfo(0);

            StartCoroutine(AutoDestroy(clipInfo[0].clip.length));
        }

        private IEnumerator AutoDestroy(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            Destroy(gameObject);
        }
    }   
}
