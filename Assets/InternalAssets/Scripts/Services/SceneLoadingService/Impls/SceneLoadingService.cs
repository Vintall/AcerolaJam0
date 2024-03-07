using UnityEngine.SceneManagement;
using Zenject;

namespace InternalAssets.Scripts.Services.SceneLoadingService.Impls
{
    public class SceneLoadingService : ISceneLoadingService, IInitializable
    {
        public void LoadScene(int number)
        {
            SceneManager.LoadScene(number);
        }

        public void Initialize()
        {
            LoadScene(1);
        }
    }
}