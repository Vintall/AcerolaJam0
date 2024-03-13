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
        [SerializeField] private float ratio;
        private Sequence _streamSequence;

        [SerializeField] private float defaultVolume;
        
        private void Start()
        {
            
        }

        public void SetVolume(float volume)
        {
            audioSource1.volume = volume;
            audioSource2.volume = volume;
        }

        public void SetDefaultVolume()
        {
            audioSource1.volume = defaultVolume;
            audioSource2.volume = defaultVolume;
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

        public void PlayStream(float pitch = 1)
        {
            if (_streamSequence != null)
            {
                _streamSequence.Kill();
                _streamSequence = null;
            }

            if (Mathf.Abs(audioSource1.pitch - pitch) > 0.0001f)
            {
                audioSource1.pitch = pitch;
                audioSource2.pitch = pitch;
            }

            float length = 0;

            _streamSequence = DOTween.Sequence()
                .AppendInterval((isFirst ? PlaySound1() : PlaySound2()) / ratio)
                .OnComplete(() =>
                {
                    isFirst = !isFirst;
                    PlayStream(pitch);
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