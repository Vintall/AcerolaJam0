using System.Collections.Generic;
using InternalAssets.Scripts.NarrativeClips.Act1;
using InternalAssets.Scripts.Services.NarrativeService.Impls;
using UnityEngine;

public class NarrativeClipsDatabase : MonoBehaviour
{
    [SerializeField] private List<AbstractNarrativeClip> narrativeClips;
    private Dictionary<int, AbstractNarrativeClip> _narrativeClipsDictionary;

    public List<AbstractNarrativeClip> NarrativeClips => narrativeClips;
    public Dictionary<int, AbstractNarrativeClip> NarrativeClipsDictionary => _narrativeClipsDictionary;
    
    public void Awake()
    {
        _narrativeClipsDictionary = new Dictionary<int, AbstractNarrativeClip>();
        
        foreach (var narrativeClip in narrativeClips)
        {
            if(_narrativeClipsDictionary.ContainsKey(narrativeClip.ClipId))
                continue;

            _narrativeClipsDictionary.Add(narrativeClip.ClipId, narrativeClip);
        }
    }
}

