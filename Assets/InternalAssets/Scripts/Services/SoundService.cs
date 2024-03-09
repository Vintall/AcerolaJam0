using System.Collections.Generic;
using UnityEngine;

namespace InternalAssets.Scripts.Services
{
    public class SoundService : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private List<AudioSource> typingSources;
        
        public void PlaySound(AudioClip clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }

        public void PlayTypingSound()
        {
            typingSources[Random.Range(0, typingSources.Count)].Play();
        }
    }
}