using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour
{
    private static SFXManager instance;
    private AudioSource audioSource;

    public static SFXManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("Mais de uma instância de AudioSource");
        }

        instance = this;
        audioSource = GetComponent<AudioSource>();
    }
    
    public void PlayClip(AudioClip clipToPlay)
    {
        audioSource.clip = clipToPlay;
        audioSource.Play();
    }
}
