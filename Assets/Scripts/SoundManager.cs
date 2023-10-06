using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Create a static reference to the SoundManager instance.
    public static SoundManager Instance { get; private set; }

    public AudioSource[] destroySound;

    private void Awake()
    {
        // Ensure there is only one instance of SoundManager.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep the SoundManager GameObject alive across scenes.
        }
        else
        {
            Destroy(gameObject); // Destroy any additional instances that are created.
        }
    }

    public void PlayRandomDestroySound()
    {
        int clipToPlay = Random.Range(0, destroySound.Length);
        destroySound[clipToPlay].Play();
    }
}
