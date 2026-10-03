using Core.Enums;
using UnityEngine;

namespace Core.Scripts
{
    public class CameraSwitchTrigger : MonoBehaviour
    {
        [SerializeField] private bool _storage = false;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
                UnityEventManager.TriggerEvent(nameof(EnumUnityEventName.SwitchCameraPosition), _storage);
        }
    }
}