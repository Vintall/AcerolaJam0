using UnityEngine;
using UnityEngine.UI;

namespace InternalAssets.Scripts.Services
{
    public class SettingsApplyerService : MonoBehaviour
    {
        [SerializeField] private Slider sensitivitySlider;

        public void Apply()
        {
            StaticSettingsService.sensitivity = sensitivitySlider.value;
        }
    }
}