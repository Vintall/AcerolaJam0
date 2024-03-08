using System.Collections.Generic;
using InternalAssets.Scripts.NarrativeClips.Act1;
using InternalAssets.Scripts.Services.NarrativeService.Impls;
using UnityEngine;

public class NarrativeClipsDatabase : MonoBehaviour
{
    [SerializeField] private WakeUpClip _wakeUpClip;
    [SerializeField] private FindFoodClip _findFoodClip;
    
    private List<AbstractNarrativeClip> _narrativeClips;
    private Dictionary<int, AbstractNarrativeClip> _narrativeClipsDictionary;

    public List<AbstractNarrativeClip> NarrativeClips => _narrativeClips;
    public Dictionary<int, AbstractNarrativeClip> NarrativeClipsDictionary => _narrativeClipsDictionary;
    
    public void Awake()
    {
        _narrativeClips = new List<AbstractNarrativeClip>()
        {
            _wakeUpClip,
            _findFoodClip
        };
        _narrativeClipsDictionary = new Dictionary<int, AbstractNarrativeClip>();
        
        foreach (var narrativeClip in _narrativeClips)
        {
            if(_narrativeClipsDictionary.ContainsKey(narrativeClip.ClipId))
                continue;

            _narrativeClipsDictionary.Add(narrativeClip.ClipId, narrativeClip);
        }
    }
}

