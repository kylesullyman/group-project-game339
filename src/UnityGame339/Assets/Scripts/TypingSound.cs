using UnityEngine;

public class TypingSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] clickSounds;

    void Update()
    {
        foreach (char c in Input.inputString) // only actual typing
        {
            PlayRandomSound();
        }
    }

    void PlayRandomSound()
    {
        int index = Random.Range(0, clickSounds.Length);
        audioSource.PlayOneShot(clickSounds[index]);
    }
}