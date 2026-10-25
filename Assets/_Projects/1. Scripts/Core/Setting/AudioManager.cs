using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("MIXER")]
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioMixerGroup bgmGroup;

    [Header("LIBRARY")]
    [SerializeField] private List<AudioClipData> sfxLibrary = new List<AudioClipData>();
    [SerializeField] private List<AudioClipData> bgmLibrary = new List<AudioClipData>();

    [Header("VOLUME")]
    [Range(0f, 1f)] [SerializeField] private float masterVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float bgmVolume = 1f;

    [Header("POOL")]
    [SerializeField] private int sfxPoolSize = 10;

    [Header("BGM")]
    [SerializeField] private string defaultBgmId;
    [SerializeField] private bool autoPlayDefaultBgm = false;

    [Header("BGM PLAYLIST")]
    [SerializeField] private List<string> bgmPlaylist = new List<string>();
    [SerializeField] private bool shufflePlaylist = true;

    private AudioSource bgmSource;
    private List<AudioSource> sfxPool = new List<AudioSource>();
    private Dictionary<string, AudioClipData> sfxLookup = new Dictionary<string, AudioClipData>();
    private Dictionary<string, AudioClipData> bgmLookup = new Dictionary<string, AudioClipData>();
    private string currentBgmId;
    private Coroutine playlistCoroutine;
    private bool playlistActive;

    void Awake(){
        if(Instance != null && Instance != this){ Destroy(gameObject); return; }
        Instance = this;

        foreach(var a in sfxLibrary){
            if(a == null || string.IsNullOrEmpty(a.id) || a.clip == null) continue;
            sfxLookup[a.id] = a;
        }
        foreach(var a in bgmLibrary){
            if(a == null || string.IsNullOrEmpty(a.id) || a.clip == null) continue;
            bgmLookup[a.id] = a;
        }

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = false;
        bgmSource.playOnAwake = false;
        bgmSource.outputAudioMixerGroup = bgmGroup;

        for(int i = 0; i < sfxPoolSize; i++){
            AudioSource src = gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            src.outputAudioMixerGroup = sfxGroup;
            sfxPool.Add(src);
        }

        ApplyVolumesToMixer();
    }

    void Start(){
        if(autoPlayDefaultBgm && !string.IsNullOrEmpty(defaultBgmId)) PlayBgm(defaultBgmId);
    }

    public void PlaySfx(string id){
        if(!sfxLookup.TryGetValue(id, out var data)) return;

        AudioSource source = GetFreeSfxSource();
        if(source == null) return;

        source.clip = data.clip;
        source.volume = data.volume;
        source.pitch = Random.Range(data.minPitch, data.maxPitch);
        source.Play();
    }

    public void PlayBgm(string id){
        if(!bgmLookup.TryGetValue(id, out var data)) return;

        currentBgmId = id;
        bgmSource.clip = data.clip;
        bgmSource.volume = data.volume;
        bgmSource.pitch = 1f;
        bgmSource.Play();
    }

    public void StopBgm(){
        currentBgmId = null;
        bgmSource.Stop();
    }


    //Ini buat logic playlist lagu tanpa duplicate, saya males cari BGM lain
    public void StartRandomPlaylist(){
        if(bgmPlaylist == null || bgmPlaylist.Count == 0){
            Debug.LogWarning("BGM playlist is empty");
            return;
        }

        StopRandomPlaylist();
        playlistActive = true;
        playlistCoroutine = StartCoroutine(PlaylistRoutine());
    }

    public void StopRandomPlaylist(){
        playlistActive = false;
        if(playlistCoroutine != null){
            StopCoroutine(playlistCoroutine);
            playlistCoroutine = null;
        }
    }

    IEnumerator PlaylistRoutine(){
        while(playlistActive){
            string id = PickNextBgmId();
            if(string.IsNullOrEmpty(id)) break;

            if(!bgmLookup.TryGetValue(id, out var data)){
                yield return null;
                continue;
            }

            currentBgmId = id;
            bgmSource.clip = data.clip;
            bgmSource.volume = data.volume;
            bgmSource.pitch = 1f;
            bgmSource.Play();

            float waitTime = data.clip.length;
            float elapsed = 0f;
            while(elapsed < waitTime && playlistActive){
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        playlistActive = false;
        playlistCoroutine = null;
    }

    string PickNextBgmId(){
        if(bgmPlaylist == null || bgmPlaylist.Count == 0) return null;

        List<string> valid = new List<string>();
        foreach(var id in bgmPlaylist){
            if(string.IsNullOrEmpty(id)) continue;
            if(!bgmLookup.ContainsKey(id)) continue;
            if(id == currentBgmId) continue;
            valid.Add(id);
        }

        if(valid.Count == 0){
            if(bgmPlaylist.Count > 0) return bgmPlaylist[0];
            return null;
        }

        if(shufflePlaylist) return valid[Random.Range(0, valid.Count)];

        int currentIndex = bgmPlaylist.IndexOf(currentBgmId);
        int nextIndex = (currentIndex + 1) % bgmPlaylist.Count;
        int guard = 0;
        while(bgmPlaylist[nextIndex] == currentBgmId && guard < bgmPlaylist.Count){
            nextIndex = (nextIndex + 1) % bgmPlaylist.Count;
            guard++;
        }
        return bgmPlaylist[nextIndex];
    }

    AudioSource GetFreeSfxSource(){
        foreach(var src in sfxPool){
            if(!src.isPlaying) return src;
        }
        return sfxPool.Count > 0 ? sfxPool[0] : null;
    }

    public void SetMasterVolume(float value){
        masterVolume = Mathf.Clamp01(value);
        if(mainMixer != null) mainMixer.SetFloat("MasterVolume", LinearToDb(masterVolume));
    }

    public void SetSfxVolume(float value){
        sfxVolume = Mathf.Clamp01(value);
        if(mainMixer != null) mainMixer.SetFloat("SFXVolume", LinearToDb(sfxVolume));
    }

    public void SetBgmVolume(float value){
        bgmVolume = Mathf.Clamp01(value);
        if(mainMixer != null) mainMixer.SetFloat("BGMVolume", LinearToDb(bgmVolume));
    }

    public float GetMasterVolume() => masterVolume;
    public float GetSfxVolume() => sfxVolume;
    public float GetBgmVolume() => bgmVolume;

    void ApplyVolumesToMixer(){
        if(mainMixer == null) return;
        mainMixer.SetFloat("MasterVolume", LinearToDb(masterVolume));
        mainMixer.SetFloat("SFXVolume", LinearToDb(sfxVolume));
        mainMixer.SetFloat("BGMVolume", LinearToDb(bgmVolume));
    }

    static float LinearToDb(float linear) => Mathf.Log10(Mathf.Max(linear, 0.0001f)) * 20f;
}