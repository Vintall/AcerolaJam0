using System;
using System.Collections;
using System.Collections.Generic;
using InternalAssets.Scripts;
using InternalAssets.Scripts.Services.NarrativeService.NarrativeClips.Act1;
using UnityEngine;

public class NarrativeClipsDatabase : MonoBehaviour
{
    [SerializeField] private TestLeftNarrativeClip testLeftNarrativeClip;
    [SerializeField] private TestRightNarrativeClip testRightNarrativeClip;
    
    private List<AbstractNarrativeClip> _narrativeClips;
    private Dictionary<int, AbstractNarrativeClip> _narrativeClipsDictionary;

    public List<AbstractNarrativeClip> NarrativeClips => _narrativeClips;
    public Dictionary<int, AbstractNarrativeClip> NarraticeSlipsDictionary => _narrativeClipsDictionary;
    
    public void Awake()
    {
        _narrativeClips = new List<AbstractNarrativeClip>()
        {
            testLeftNarrativeClip,
            testRightNarrativeClip
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

public abstract class AbstractNarrativeClip : MonoBehaviour, INarrativeClip
{
    [SerializeField] protected int _nextClipId;
    [SerializeField] protected int _clipId;
    protected Action _onEndCallback;

    public abstract void OnStart();

    public int NextClipId => _nextClipId;

    public int ClipId => _clipId;

    public event Action OnEndCallback
    {
        add => _onEndCallback += value;
        remove => _onEndCallback -= value;
    }
}