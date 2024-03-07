using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InternalAssets.Scripts.Services.PlayableDirector;
using UnityEngine;
using Zenject;

namespace InternalAssets.Scripts
{
    public class NarrativeService : MonoBehaviour, INarrativeService
    {
        [SerializeField] private NarrativeClipsDatabase narrativeClipsDatabase;
        private readonly IPlayableDirectorService _playableDirectorService;
        private AbstractNarrativeClip currentNarrativeClip;
        private int firstId = 1;

        private void Start()
        {
            StartFirstAnimation();
        }

        public void StartFirstAnimation()
        {
            if (!narrativeClipsDatabase.NarraticeSlipsDictionary.ContainsKey(firstId))
                Debug.LogError("NoSuchKeyInDictionary");

            currentNarrativeClip =
                Instantiate(narrativeClipsDatabase.NarraticeSlipsDictionary[firstId], transform, true);

            currentNarrativeClip.OnEndCallback += OnEndCallBack;
            currentNarrativeClip.OnStart();
        }

        private void OnEndCallBack()
        {
            currentNarrativeClip.OnEndCallback -= OnEndCallBack;
            
            if(currentNarrativeClip.NextClipId == -1)
                Debug.LogError("EndGame");
            
            if (!narrativeClipsDatabase.NarraticeSlipsDictionary.ContainsKey(firstId))
                Debug.LogError("NoSuchKeyInDictionary");
            
            currentNarrativeClip = narrativeClipsDatabase.NarraticeSlipsDictionary[firstId];
            
            currentNarrativeClip.OnEndCallback += OnEndCallBack;
            currentNarrativeClip.OnStart();
        }

        
    }
}
