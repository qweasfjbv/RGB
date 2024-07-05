using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

    [SerializeField] private float fadeDuration;

    private float effVolume = 1.0f;
    private AudioSource audioSource;

    public void CreateAudioSource(Vector3 pos, EffectClip clipIdx)
    {
        GameObject go = Instantiate(audioPrefab, pos, Quaternion.identity, transform);
        go.GetComponent<AudioSource>().clip = effClips[(int)clipIdx];
        go.GetComponent<AudioSource>().volume = effVolume;
        go.GetComponent<AudioSource>().Play();

        Destroy(go, effClips[(int)clipIdx].length);

    }

    public void SilentBGM()
    {
        audioSource.DOFade(0f, fadeDuration/2);
    }

    public void ChangeBGM(BGMClip clip)
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource.clip.Equals(bgmClips[(int)clip]) && audioSource.isPlaying) return;

        audioSource.volume = 0f;
        audioSource.DOFade(1f, fadeDuration);
        audioSource.clip = bgmClips[(int)clip];
        audioSource.Play();

    }

}
