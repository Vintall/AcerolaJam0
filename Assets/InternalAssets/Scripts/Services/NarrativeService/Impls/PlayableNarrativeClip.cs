using System.Collections.Generic;
using InternalAssets.Scripts.Services.InteractionService;
using UnityEngine;

namespace InternalAssets.Scripts.Services.NarrativeService.Impls
{
    public abstract class PlayableNarrativeClip : AbstractNarrativeClip
    {
        [SerializeField] protected List<InteractableObject> interactableObjects;
        [SerializeField] protected string objectiveText;
        protected abstract void EndCallback();
    }
}