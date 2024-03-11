using System;
using System.Collections.Generic;
using HolmanPlayerController;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InternalAssets.Scripts.Services
{
    public class PlayerStepsService : MonoBehaviour
    {
        [SerializeField] private InputHandler inputHandler;

        [SerializeField] private AudioSource left;
        [SerializeField] private AudioSource right;

        [SerializeField] private float walkRefresh;
        [SerializeField] private float runRefresh;

        [SerializeField] private Transform playedFeet;

        [SerializeField] private List<AudioClip> grass;
        [SerializeField] private List<AudioClip> wood;
        [SerializeField] private List<AudioClip> stone;

        private float lastSound;

        private void Start()
        {
            lastSound = Time.time;
        }

        private bool isRight;
        public void Update()
        {
            if (!(Input.GetKey(KeyCode.A) ||
                  Input.GetKey(KeyCode.W) ||
                  Input.GetKey(KeyCode.S) ||
                  Input.GetKey(KeyCode.D)))
                return;
            
            if (inputHandler.walking && 
                Time.time > lastSound + walkRefresh)
            {
                Play();
            }

            if (inputHandler.running &&
                Time.time > lastSound + runRefresh)
            {
                Play();
            }
        }

        private void Play()
        {
            Ray ray = new Ray(playedFeet.position + Vector3.up * 0.3f, Vector3.down);

            RaycastHit[] hits = Physics.RaycastAll(ray, 1f);

            RaycastHit hit;

            if(hits.Length == 0)
                return;
            
            if (hits[0].transform.CompareTag("Player"))
            {
                hit = hits[1];
            }
            else
            {
                hit = hits[0];
            }

            AudioClip playableClip = null;

            Debug.Log($"Hit name: {hit.transform.name} Tag{hit.transform.tag}");

            if (hit.transform.CompareTag("Land"))
                playableClip = grass[Random.Range(0, grass.Count)];
            else if (hit.transform.CompareTag("FloorWood"))
                playableClip = wood[Random.Range(0, wood.Count)];
            else
                playableClip = stone[Random.Range(0, stone.Count)];

            if (isRight)
            {
                right.clip = playableClip;
                right.Play();
            }
            else
            {
                left.clip = playableClip;
                left.Play();
            }

            lastSound = Time.time;
            isRight = !isRight;
        }
    }
}