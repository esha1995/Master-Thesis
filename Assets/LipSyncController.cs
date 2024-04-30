using System.Collections;
using FMODUnity;
using UnityEngine;
using UnityEngine.Audio;

public class LipSyncController : MonoBehaviour
{

    public AudioClip syncClip;
    public SkinnedMeshRenderer morphTarget;
    public StudioEventEmitter teacherSpeaking;
    public GameObject LipSyncInterface;
    public AudioMixerGroup mixGroupNoAudio;
    private AudioSource lipSyncSource;
    public static LipSyncController Instance;

    private void Awake() 
    { 
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

    public AudioSource GetLipSyncSource() 
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return lipSyncSource; 
#endif
        return null;
    } 
    public void SetLipSyncClip(AudioClip clip) 
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        lipSyncSource.clip = clip; 
#endif
    }

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Instantiate(LipSyncInterface);
        lipSyncSource = gameObject.AddComponent<AudioSource>(); 
        lipSyncSource.outputAudioMixerGroup = mixGroupNoAudio;
        lipSyncSource.clip = syncClip;
        OVRLipSyncContext context = gameObject.AddComponent<OVRLipSyncContext>();
        OVRLipSyncContextMorphTarget morphTarget = gameObject.AddComponent<OVRLipSyncContextMorphTarget>();
        morphTarget.skinnedMeshRenderer = this.morphTarget;
        context.audioSource = lipSyncSource;
        context.audioLoopback = true;
        context.enabled = true;
#endif
    }

    IEnumerator StartLipSyncCou()
    {
        yield return null;
#if UNITY_ANDROID && !UNITY_EDITOR
        yield return new WaitUntil(() => teacherSpeaking.IsPlaying());
        lipSyncSource.Play();
#endif
    }

    public void StartLipSync()
    {
        StartCoroutine(StartLipSyncCou());
    }

    public void StopLipSync()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        lipSyncSource.Stop();
#endif
    }
}
