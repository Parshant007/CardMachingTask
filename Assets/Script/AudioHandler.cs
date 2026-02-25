using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    public static AudioHandler Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null & Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip[] audioClips;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(int id, float volume = 1f)
    {
        audioSource.PlayOneShot(audioClips[id], volume);
    }
}
