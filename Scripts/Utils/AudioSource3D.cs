using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSource3D : MonoBehaviour
{
    public AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void Play()
    {
        Managers.Sound._usingAudioSourcesQueue.Enqueue(this);
        audioSource.Play();
        Invoke("ReturnQueue", audioSource.clip.length);
    }

    public void ReturnQueue()
    {
        CancelInvoke("ReturnQueue");
        Managers.Sound._usingAudioSourcesQueue.Dequeue();
        Managers.Sound._audioSources3DQueue.Enqueue(this);
    }

    private void OnDestroy()
    {
        if (Application.isPlaying == false && Managers.Sound == null)
            return;

        GameObject root = GameObject.Find("@Sound");
        if (root == null)
            return;

        string soundName3D = System.Enum.GetName(typeof(Define.Sound3D), 0);

        Debug.Log("hi");

        GameObject go = new GameObject { name = soundName3D };
        go.transform.parent = root.transform;

        AudioSource3D audioSource3D = go.AddComponent<AudioSource3D>();
        Managers.Sound._audioSources3DQueue.Enqueue(audioSource3D);
    }
}
