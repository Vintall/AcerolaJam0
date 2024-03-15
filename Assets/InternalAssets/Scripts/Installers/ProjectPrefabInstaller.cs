using InternalAssets.Scripts.Services.PlayableDirector.Impls;
using UnityEngine;
using Zenject;

namespace InternalAssets.Scripts.Installers
{
    public class ProjectPrefabInstaller : MonoInstaller
    {
        [SerializeField] private PlayableDirectorService _playableDirectorService;
        public override void InstallBindings()
        {
            BindPrefab(_playableDirectorService, this.transform);
        }
        
        private void BindPrefab<TContent>(TContent prefab, Transform parent)
            where TContent : Object
        {
            Container.BindInterfacesAndSelfTo<TContent>()
                .FromComponentInNewPrefab(prefab)
#if UNITY_EDITOR
                .UnderTransform(parent)
#endif
                .AsSingle()
                .OnInstantiated((_, o) => ((MonoBehaviour) o).gameObject.SetActive(false));
        }
    }
}