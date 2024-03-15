using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace InternalAssets.Scripts.Services.SceneLoadingService.Impls
{
    public class SceneLoadingService : MonoBehaviour
    {
        public void LoadScene(int number)
        {
            SceneManager.LoadScene(number);
        }
    }
}