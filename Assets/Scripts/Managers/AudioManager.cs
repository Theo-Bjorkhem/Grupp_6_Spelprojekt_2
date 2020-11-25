﻿using UnityEngine;

[System.Serializable]
public class Sound
{
    public string myName;
    public AudioClip myClip;
    
    [Range(0f, 1f)]
    public float myVolume = 1f;
    [Range(0f, 2f)]
    public float myPitch = 1f;

    [Range(0f, 0.5f)]
    public float myRandomVolume = 0.1f;
    [Range(0f, 0.5f)]
    public float myRandomPitch = 0.1f;

    private AudioSource mySource;

    public void SetSource(AudioSource aSource)
    {
        mySource = aSource;
        mySource.clip = myClip;
        mySource.playOnAwake = false;
    }

    public void Play()
    {
        mySource.loop = false;
        mySource.volume = myVolume * (1 + Random.Range(-myRandomVolume / 2f, myRandomVolume / 2f));
        mySource.pitch = myPitch * (1 + Random.Range(-myRandomPitch / 2f, myRandomPitch / 2f));
        mySource.Play();
    }

    public void Loop()
    {
        mySource.loop = true;
        mySource.volume = myVolume * (1 + Random.Range(-myRandomVolume / 2f, myRandomVolume / 2f));
        mySource.pitch = myPitch * (1 + Random.Range(-myRandomPitch / 2f, myRandomPitch / 2f));
        mySource.Play();
    }
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager ourInstance;

    [SerializeField]
    Sound[] mySounds = { };

    private void Awake()
    {
        if (ourInstance != null)
        {
            Debug.LogError("More than one audioManager in the scene");
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            ourInstance = this;
        }
    }

    private void Start()
    {
        for (int i = 0; i < mySounds.Length; i++)
        {
            GameObject go = new GameObject("Sound" + i + mySounds[i].myName);
            go.transform.SetParent(this.transform);
            mySounds[i].SetSource(go.AddComponent<AudioSource>());
        }

        PlayLoop("Ambience");
    }

    public void PlaySound(string aName)
    {
        for (int i = 0; i < mySounds.Length; i++)
        {
            if (mySounds[i].myName == aName)
            {
                mySounds[i].Play();
                return;
            }
        }

        Debug.LogError("AudioManager: No sound with name: " + aName + " exists.");
    }

    public void PlayLoop(string aName)
    {
        for (int i = 0; i < mySounds.Length; i++)
        {
            if (mySounds[i].myName == aName)
            {
                mySounds[i].Loop();
                return;
            }
        }

        Debug.LogError("AudioManager: No sound with name: " + aName + " exists.");
    }
}
