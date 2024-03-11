using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InternalAssets.Scripts.Services
{
    public class GibberishService : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource1;
        [SerializeField] private AudioSource audioSource2;
        [SerializeField] private List<AudioClip> samples;
        private Sequence _streamSequence;
        
        private void Start()
        {
            PlayStream();
        }
        
        public float PlaySound1()
        {
            audioSource1.clip = samples[Random.Range(0, samples.Count)];
            audioSource1.Play();
            return audioSource1.clip.length;
        }
        
        public float PlaySound2()
        {
            audioSource2.clip = samples[Random.Range(0, samples.Count)];
            audioSource2.Play();
            return audioSource2.clip.length;
        }

        private bool isFirst;

        public void PlayStream()
        {
            if (_streamSequence != null)
            {
                _streamSequence.Kill();
                _streamSequence = null;
            }

            float length = 0;

            _streamSequence = DOTween.Sequence()
                .AppendInterval((isFirst ? PlaySound1() : PlaySound2()) / 2f)
                .OnComplete(() =>
                {
                    isFirst = !isFirst;
                    PlayStream();
                });
        }

        public void StopStream()
        {
            if (_streamSequence != null)
            {
                _streamSequence.Kill();
                _streamSequence = null;
            }
        }
    }
}