using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    #region Singleton
    private static SoundManager instance = null;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }

    public static SoundManager Instance
    {
        get
        {
            if (null == instance) return null;
            return instance;
        }
    }
    #endregion

    [SerializeField] private GameObject audioPrefab;

    [SerializeField] private List<AudioClip> bgmClips = new List<AudioClip>();
    [SerializeField] private List<AudioClip> effClips = new List<AudioClip>();

    private float effVolume = 1.0f;

    public void CreateAudioSource(Vector3 pos, EffectClip clipIdx)
    {
        GameObject go = Instantiate(audioPrefab, pos, Quaternion.identity, transform);
        go.GetComponent<AudioSource>().clip = effClips[(int)clipIdx];
        go.GetComponent<AudioSource>().volume = effVolume;
        go.GetComponent<AudioSource>().Play();

        Destroy(go, effClips[(int)clipIdx].length);

    }

}
