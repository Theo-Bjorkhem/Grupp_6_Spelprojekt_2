using UnityEngine;

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
        StartAudio();
    }

    public void Loop()
    {
        mySource.loop = true;
        StartAudio();
    }

    private void StartAudio()
    {
        if (mySource.isPlaying)
        {
            return;
        }

        mySource.volume = myVolume * (1 + Random.Range(-myRandomVolume / 2f, myRandomVolume / 2f));
        mySource.pitch = myPitch * (1 + Random.Range(-myRandomPitch / 2f, myRandomPitch / 2f));
        mySource.Play();        
    }

    public void Stop()
    {
        if (!mySource.isPlaying)
        {
            return;
        }

        mySource.Stop();
    }
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager ourInstance;

    [SerializeField]
    private bool myMute = false;

    [SerializeField]
    Sound[] mySounds = { };

    private void Awake()
    {
        if (ourInstance != null)
        {
            Destroy(gameObject);
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

        if (!myMute)
        {
            PlayLoop("MusicMainMenu");
        }
    }

    public void PlaySound(string aName)
    {
        if (myMute)
        {
            return;
        }

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
        if (myMute)
        {
            return;
        }

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

    public void ToggleMute()
    {
        myMute = !myMute;
        MuteUpdate();
    }

    public void SetMute(bool aSet)
    {
        myMute = aSet;
        MuteUpdate();
    }

    private void MuteUpdate()
    {
        //Audio gets unmuted
        if (!myMute)
        {
            PlayLoop("Ambience");
            PlayLoop("MusicMainMenu");
            return;
        }

        //Audio gets muted
        for (int i = 0; i < mySounds.Length; i++)
        {
            mySounds[i].Stop();
        }
    }

    public bool IsMuted()
    {
        return myMute;
    }

    public void Stop(string aName)
    {
        for (int i = 0; i < mySounds.Length; i++)
        {
            if (mySounds[i].myName == aName)
            {
                mySounds[i].Stop();
                return;
            }
        }

        Debug.LogError("AudioManager: No sound with name: " + aName + " exists.");
    }
}