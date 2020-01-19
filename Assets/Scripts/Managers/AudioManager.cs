using System;
using UnityEngine;

[Serializable]
public class Sound
{
    public AudioClip clip;

    public string name;

    [Range(0f, 1f)]
    public float spatialBlend = 1;
    [Range(0f, 1f)]
    public float volume = 1;
    [Range(.1f, 3f)]
    public float pitch = 1;
    public bool loop;
    public bool playOnAwake;

    [HideInInspector]
    public AudioSource source;

    public Sound(AudioSource source, AudioClip clip, float spatialBlend, float volume, float pitch, bool loop, bool playOnAwake)
    {
        this.source = source;
        this.source.clip = clip;

        this.source.spatialBlend = spatialBlend;
        this.source.volume = volume;
        this.source.pitch = pitch;
        this.source.loop = loop;
        this.source.playOnAwake = playOnAwake;
    }
}

public class AudioManager : Manager<AudioManager>
{
    public const string _leviathan_shake_1_ = "sfx_leviathan_shake_1";
    public const string _leviathan_shake_2_ = "sfx_leviathan_shake_2";

    public const string _leviathan_call_1_ = "sfx_leviathan_call_1";
    public const string _leviathan_call_2_ = "sfx_leviathan_call_2";

    public const string _leviathan_aim_ = "sfx_leviathan_aim";
    public const string _leviathan_roar_ = "sfx_leviathan_roar";
    public const string _leviathan_wreck_ = "sfx_leviathan_wreck";

    public const string _intro_ = "intro";
    public const string _main_theme_ = "main_theme";
    public const string _shoreside_bay_ = "shoreside_bay";

    public Sound[] sounds;
    public AudioClip[] sounds3d;

    private void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.spatialBlend = s.spatialBlend;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;
        }
        Play(_intro_);
    }

    private Sound FindSound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
        }
        return s;
    }

    private AudioClip FindSound3D(string name)
    {
        AudioClip clip = Array.Find(sounds3d, sound => sound.name == name);
        if (clip == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
        }
        return clip;
    }

    public AudioClip Get(string name)
    {
        return FindSound3D(name);
    }

    public void Stop()
    {
        foreach (Sound s in sounds)
            s.source.Stop();
    }

    public float Play(string name)
    {
        Sound s = FindSound(name);
        if (s != null)
        {
            s.source.Play();
            return s.clip.length;
        }
        return -1;
    }

    public float Play(string name, float time)
    {
        Sound s = FindSound(name);
        if (s != null)
        {
            s.source.time = time;
            s.source.Play();
            return s.clip.length;
        }
        return -1;
    }
}