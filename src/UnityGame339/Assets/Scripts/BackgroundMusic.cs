using UnityEngine;

namespace Game.Runtime
{
    public class BackgroundMusic : MonoBehaviour
    {
        private static BackgroundMusic _instance;

        [SerializeField] private AudioSource audioSource;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            if (audioSource != null)
            {
                audioSource.loop = true;

                if (!audioSource.isPlaying)
                    audioSource.Play();
            }
        }
    }
}