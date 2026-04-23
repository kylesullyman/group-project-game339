using Game339.Shared.Services;
using UnityEngine;

namespace Game.Runtime
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip hitClip;

        private IDamageService _damageSvc;

        private void Start()
        {
            _damageSvc = ServiceResolver.Resolve<IDamageService>();
            _damageSvc.OnDamageApplied += PlayHit;
        }

        private void OnDestroy()
        {
            _damageSvc.OnDamageApplied -= PlayHit;
        }

        private void PlayHit() => audioSource.PlayOneShot(hitClip);
    }
}
