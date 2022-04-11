using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    public class AudioManager : MonoBehaviour
    {
        [Header("AudioSources")]
        [SerializeField] private AudioSource backgroundAudioSrc;
        [SerializeField] private AudioSource eventsAudioSrc;
        
        [Header("Background Clips")]
        [SerializeField] private AudioClip bossRoomClip;
        
        [Header("Events Clips")]
        [SerializeField] private AudioClip noMoreEnemiesClip;
        [SerializeField] private AudioClip bossAwakenClip;
        [SerializeField] private AudioClip victoryClip;

        private void OnEnable()
        {
            GameManager.AllEnemiesDead += OnAllEnemiesDead;
            Boss.Awaken += OnBossAwaken;
            Boss.Dead += OnBossDead;
        }

        private void OnDisable()
        {
            GameManager.AllEnemiesDead -= OnAllEnemiesDead;
            Boss.Awaken -= OnBossAwaken;
            Boss.Dead -= OnBossDead;
        }

        #region EVENT_HANDLERS

        private void OnAllEnemiesDead()
        {
            eventsAudioSrc.clip = noMoreEnemiesClip;
            eventsAudioSrc.Play();
            
            backgroundAudioSrc.Stop();
        }
        
        private void OnBossAwaken()
        {
            eventsAudioSrc.clip = bossAwakenClip;
            eventsAudioSrc.Play();

            backgroundAudioSrc.clip = bossRoomClip;
            backgroundAudioSrc.Play();
        }
        
        private void OnBossDead(Vector3 bossLastPos)
        {
            eventsAudioSrc.clip = victoryClip;
            eventsAudioSrc.Play();
            
            backgroundAudioSrc.Stop();
        }

        #endregion
        
    }
}

