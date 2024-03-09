using InternalAssets.Scripts.Services;
using InternalAssets.Scripts.Services.NarrativeService.Impls;
using UnityEngine;
using UnityEngine.Playables;

namespace InternalAssets.Scripts.NarrativeClips.Act1
{
    public class GoWatchTVClip : PlayableNarrativeClip
    {
        [SerializeField] private PlayableDirector playableDirector;
        
        public override void OnStart()
        {
            //ServicesHolder.RaycastService._onHit += OnRaycast;
            ServicesHolder.ObjectiveService.PrintObjective(objectiveText, 0.05f);
        }

        protected override void EndClip()
        {
            
        }

        protected override void EndCallback()
        {
            
        }
    }
}