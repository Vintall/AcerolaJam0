using InternalAssets.Scripts.Services.PlayableDirector;
using UnityEngine;

namespace InternalAssets.Scripts.Services.NarrativeService.Impls
{
    public class NarrativeService : MonoBehaviour, INarrativeService
    {
        [SerializeField] private NarrativeClipsDatabase narrativeClipsDatabase;
        [SerializeField] private int firstClipId = 1;
        private readonly IPlayableDirectorService _playableDirectorService;
        private AbstractNarrativeClip _currentNarrativeClip;

        private void Start()
        {
            StartFirstAnimation();
        }
        
        public void StartFirstAnimation()
        {
            if (!narrativeClipsDatabase.NarrativeClipsDictionary.ContainsKey(firstClipId))
                Debug.LogError("NoSuchKeyInDictionary");

            _currentNarrativeClip = narrativeClipsDatabase.NarrativeClipsDictionary[firstClipId];
            
            _currentNarrativeClip.OnEndCallback += OnEndCallBack;
            _currentNarrativeClip.OnStart();
        }

        private void OnEndCallBack()
        {
            _currentNarrativeClip.OnEndCallback -= OnEndCallBack;
            
            if(_currentNarrativeClip.NextClipId == -1)
                Debug.LogError("EndGame");
            
            if (!narrativeClipsDatabase.NarrativeClipsDictionary.ContainsKey(_currentNarrativeClip.NextClipId))
                Debug.LogError("NoSuchKeyInDictionary");
            
            _currentNarrativeClip = narrativeClipsDatabase.NarrativeClipsDictionary[_currentNarrativeClip.NextClipId];
            
            _currentNarrativeClip.OnEndCallback += OnEndCallBack;
            _currentNarrativeClip.OnStart();
        }
    }
}
