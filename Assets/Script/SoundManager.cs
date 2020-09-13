using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public enum AudioType
    {
        BUTTONCLICK = 0,
        SLIDE,
        SLIDE2,
        GAMESTART,
        GAMEOVER,
        REMOVE,
        Max
    }

    public enum BGMType
    {
        LOBBY = 0,
        INGAME,
        Max
    }

    public AudioClip[] m_AudioClip = new AudioClip[(int)AudioType.Max];
    public AudioClip[] m_AudioClip_BGM = new AudioClip[(int)BGMType.Max];

    AudioSource[] m_AudioSource = new AudioSource[(int)AudioType.Max];
    AudioSource m_AudioSource_BGM;

    public static SoundManager instance = null;

    void Awake()
    {
        instance = this;

        Init();
    }

    public void Init()
    {
        for (int i = 0; i < (int)AudioType.Max; i++)
        {
            m_AudioSource[i] = gameObject.AddComponent<AudioSource>();
            m_AudioSource[i].clip = m_AudioClip[i];
            m_AudioSource[i].loop = false;
            m_AudioSource[i].volume = 1.0f;
        }

        m_AudioSource_BGM = gameObject.AddComponent<AudioSource>();
        m_AudioSource_BGM.clip = m_AudioClip_BGM[0];
        m_AudioSource_BGM.loop = true;
        m_AudioSource_BGM.volume = 1.0f;

        //SetBGMSound(DataManager.instance.GetBGMSound());
        //SetEffectSound(DataManager.instance.GetEffectSound());
    }

    public void PlayAudio(AudioType type)
    {
        if(type == AudioType.SLIDE)
        {
            int randNum = Random.Range(0, 2);
            if(randNum == 1)
            {
                type = AudioType.SLIDE2;
            }
        }

        m_AudioSource[(int)type].Play();
    }

    public void Play_BGM(BGMType type)
    {
        m_AudioSource_BGM.clip = m_AudioClip_BGM[(int)type];
        
        m_AudioSource_BGM.time = 0;
        m_AudioSource_BGM.Play();
    }

    public void Pause_BGM()
    {
        m_AudioSource_BGM.Pause();
    }

    public void Unpause_BGM()
    {
        m_AudioSource_BGM.UnPause();
    }

    public void Stop_BGM()
    {
        m_AudioSource_BGM.Stop();
    }
    
    public void SetBGMSound(float vol)
    {
        m_AudioSource_BGM.volume = vol;
    }

    public void SetEffectSound(float vol)
    {
        for (int i = 0; i < (int)AudioType.Max; i++)
        {
            m_AudioSource[i].volume = vol;
        }
    }
}
