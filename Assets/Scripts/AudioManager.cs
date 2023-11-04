using System;
using UnityEngine;
using UnityEngine.Audio;

public enum Audio
{
    JUMP, JUMP_IMPACT, WALK, KEY_PICKED, DOUBLEJUMP_PICKED, ORB_ACTIVE,
    M_AMBIENCE, M_VICTORY
}


public class AudioManager : MonoBehaviour
{

    [SerializeField]
    public AudioClip[] clips;
    private AudioSource[] sources;

    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    public static AudioManager instance;
    void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        GenerateAudioSources();
    }

    private void GenerateAudioSources()
    {
        string[] names = Enum.GetNames(typeof(Audio));
        sources = new AudioSource[names.Length];

        for (int i=0; i < names.Length; i++)
        {
            sources[i] = gameObject.AddComponent<AudioSource>();
            sources[i].clip = GetClipByName(names[i]);
            sources[i].outputAudioMixerGroup = sfxMixerGroup;
        }
    }

    private AudioClip GetClipByName(string name)
    {
        foreach (AudioClip clip in clips)
            if (clip.name.ToUpper().Equals(name))
                return clip;
        throw new ArgumentException($"Clip of name {name} not found!");
    }

    public void MarkAsMusic(Audio audio)
    {
        sources[(int)audio].outputAudioMixerGroup = musicMixerGroup;
    }
    public void SetLooping(Audio audio, bool loop)
    {
        sources[(int)audio].loop = loop;
    }
    public void SetVolume(Audio audio, float volume)
    {
        sources[(int)audio].volume = volume;
    }

    public void Play(Audio audio)
    {
        sources[(int)audio].Play();
    }
    public void Stop(Audio audio)
    {
        sources[(int)audio].Stop();
    }
    public float Length(Audio audio)
    {
        return sources[(int)audio].clip.length;
    }
}
