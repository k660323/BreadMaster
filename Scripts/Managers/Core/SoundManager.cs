using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager
{
    AudioMixer mixer;
    public AudioMixer Mixer
    {
        get
        {
            if (mixer == null)
            {
                mixer = Managers.Resource.Load<AudioMixer>("MasterAudioMixer.mixer");
            }

            return mixer;
        }
    }

    AudioSource[] _audioSources2D = new AudioSource[(int)Define.Sound2D.MaxCount];
    
    const int audio3DCount = 60;
    public Queue<AudioSource3D> _audioSources3DQueue = new Queue<AudioSource3D>();
    public Queue<AudioSource3D> _usingAudioSourcesQueue = new Queue<AudioSource3D>();
    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            Init2DSound(root);
            Init3DSound(root);
        }
    }

    void Init2DSound(GameObject root)
    {
        string[] soundNames2D = System.Enum.GetNames(typeof(Define.Sound2D));
        for (int i = 0; i < soundNames2D.Length - 1; i++)
        {
            GameObject go = new GameObject { name = soundNames2D[i] };
            _audioSources2D[i] = go.AddComponent<AudioSource>();
            _audioSources2D[i].volume = 0.5f;
            go.transform.parent = root.transform;
        }

        _audioSources2D[(int)Define.Sound2D.Effect2D].outputAudioMixerGroup = Mixer.FindMatchingGroups("Effect")[0];
        _audioSources2D[(int)Define.Sound2D.Bgm].outputAudioMixerGroup = Mixer.FindMatchingGroups("Background")[0];
        _audioSources2D[(int)Define.Sound2D.Bgm].loop = true;
    }

    void Init3DSound(GameObject root)
    {
        string soundName3D = System.Enum.GetName(typeof(Define.Sound3D), 0);
        for (int i = 0; i < audio3DCount; i++)
        {
            GameObject go = new GameObject { name = soundName3D };
            go.transform.parent = root.transform;

            AudioSource3D audioSource3D = go.AddComponent<AudioSource3D>();
            audioSource3D.audioSource.outputAudioMixerGroup = Mixer.FindMatchingGroups("Effect")[0];
            audioSource3D.audioSource.volume = 0.5f;

            _audioSources3DQueue.Enqueue(audioSource3D);
        }
    }

    public void Clear()
    {
        foreach (AudioSource audioSource in _audioSources2D)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }

        foreach (AudioSource3D audioSource3D in _usingAudioSourcesQueue)
        {
            audioSource3D.audioSource.clip = null;
            audioSource3D.audioSource.Stop();
            audioSource3D.ReturnQueue();
        }

        _audioClips.Clear();
    }

    public void Play2D(string path, Define.Sound2D type = Define.Sound2D.Effect2D, float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip2D(path, type);
        Play2D(audioClip, type, pitch);
    }

    public void Play2D(AudioClip audioClip, Define.Sound2D type = Define.Sound2D.Effect2D, float pitch = 1.0f)
    {
        if (audioClip == null)
            return;

        if (type == Define.Sound2D.Bgm)
        {
            AudioSource audioSource = _audioSources2D[(int)Define.Sound2D.Bgm];
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            AudioSource audioSource = _audioSources2D[(int)Define.Sound2D.Effect2D];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }
    }

    AudioClip GetOrAddAudioClip2D(string path, Define.Sound2D type = Define.Sound2D.Effect2D)
    {
        AudioClip audioClip = null;

        if (type == Define.Sound2D.Bgm)
        {
            audioClip = Managers.Resource.Load<AudioClip>(path);
        }
        else
        {
            if (_audioClips.TryGetValue(path, out audioClip) == false)
            {
                audioClip = Managers.Resource.Load<AudioClip>(path);
                _audioClips.Add(path, audioClip);
            }
        }

        if (audioClip == null)
            Debug.Log($"AudioClip Missing! {path}");

        return audioClip;
    }

    public void Play3D(string path, Transform targetPos, bool isAttach = false, Define.Sound3D type = Define.Sound3D.Effect3D, float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip3D(path, type);
        Play3D(audioClip, targetPos, isAttach, type, pitch);
    }

    public void Play3D(AudioClip audioClip, Transform targetPos, bool isAttach = false, Define.Sound3D type = Define.Sound3D.Effect3D, float pitch = 1.0f)
    {
        if (audioClip == null)
            return;

        if (_audioSources3DQueue.Count == 0)
            return;

        AudioSource3D audioSource3D = _audioSources3DQueue.Dequeue();
        audioSource3D.audioSource.clip = audioClip;
        audioSource3D.transform.position = targetPos.position;
        if (isAttach)
            audioSource3D.transform.parent = targetPos;
        audioSource3D.audioSource.pitch = pitch;

        audioSource3D.Play();
    }

    AudioClip GetOrAddAudioClip3D(string path, Define.Sound3D type = Define.Sound3D.Effect3D)
    {
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}";

        AudioClip audioClip = null;

        if (_audioClips.TryGetValue(path, out audioClip) == false)
        {
            audioClip = Managers.Resource.Load<AudioClip>(path);
            _audioClips.Add(path, audioClip);
        }

        if (audioClip == null)
            Debug.Log($"AudioClip Missing! {path}");

        return audioClip;
    }
}
