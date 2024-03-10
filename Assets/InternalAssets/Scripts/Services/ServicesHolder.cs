using System;
using HolmanPlayerController;
using InternalAssets.Scripts.Services.InteractionService;
using InternalAssets.Scripts.Services.UIServices;
using UnityEngine;

namespace InternalAssets.Scripts.Services
{
    public class ServicesHolder : MonoBehaviour
    {
        private static ServicesHolder _instance;
        [SerializeField] private UIInteractionService uiInteractionService;
        [SerializeField] private UIDialogService uiDialogService;
        [SerializeField] private ObjectiveService objectiveService;
        [SerializeField] private NarrativeService.Impls.NarrativeService narrativeService;
        [SerializeField] private RaycastService raycastService;
        [SerializeField] private InputHandler playerInputService;
        [SerializeField] private CameraFree playerCameraService;
        [SerializeField] private SoundService soundService;
        [SerializeField] private CollisionService collisionService;
        
        public static UIInteractionService UIInteractionService => _instance.uiInteractionService;
        public static UIDialogService UIDialogService => _instance.uiDialogService;
        public static ObjectiveService ObjectiveService => _instance.objectiveService;
        public static NarrativeService.Impls.NarrativeService NarrativeService => _instance.narrativeService;
        public static RaycastService RaycastService => _instance.raycastService;
        public static InputHandler PlayerInputService => _instance.playerInputService;
        public static CameraFree PlayerCameraService => _instance.playerCameraService;
        public static SoundService SoundService => _instance.soundService;
        public static CollisionService CollisionService => _instance.collisionService;
        
        private void Awake()
        {
            _instance = this;
        }
    }
}
