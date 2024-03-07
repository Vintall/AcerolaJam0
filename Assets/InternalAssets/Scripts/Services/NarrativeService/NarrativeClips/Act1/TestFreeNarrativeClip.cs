using InternalAssets.Scripts.Services.NarrativeService.Impls;
using UnityEngine;

namespace InternalAssets.Scripts.NarrativeClips.Act1
{
    [CreateAssetMenu(menuName = "NarrativeClips/Act1/Playable/TestFreeNarrativeClip", fileName = "TestFreeNarrativeClip")]
    public class TestFreeNarrativeClip : PlayableNarrativeClip, INarrativeClip
    {
        [SerializeField] private float testField;
        public override void OnStart()
        {
            
        }

        public override void OnEnd()
        {
            
        }
    }
}