using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TuturialScript : MonoBehaviour
{
    public GameObject panelInfo;
    public TextMeshProUGUI panelText;
    public GameObject posters;
    private bool teacherTalking = false;
    public StudioEventEmitter teacherEmitter, curtainEmitter, windowEmitter, startSourceEmitte, outEmitter;
    public Animator teacherAnimator;
    public AudioClip welcomeLib, curtainLib, windowLib, startSourceLib, outroLib;
    public GameObject fade;
    public XRSimpleInteractable wallInteractable, windowInteractable, teacherInteractable;
    private bool wallPressed = false;

    private bool sourcePressed = false;
    private bool curtainPressed = false;

    public static TuturialScript Instance;

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

    private IEnumerator BeginEnum()
    {
        fade.SetActive(false);
        LipSyncController.Instance.SetLipSyncClip(welcomeLib);
        teacherEmitter.Play(); 
        LipSyncController.Instance.StartLipSync();
        yield return new WaitUntil(() => teacherEmitter.IsPlaying());
        yield return new WaitUntil(() => !teacherEmitter.IsPlaying());
        wallInteractable.enabled = true;
        panelInfo.SetActive(true);
    }

    public void Begin()
    {
        StartCoroutine(BeginEnum());
    }

    public void Curtain()
    {
        if(wallPressed) return;
        panelText.text = "Press on the button that says curtain";
        LipSyncController.Instance.SetLipSyncClip(curtainLib);
        curtainEmitter.Play();
        LipSyncController.Instance.StartLipSync();
        wallPressed = true;
    }

    public void Window()
    {
        panelText.text = "Look to the left, and use the ray interactor to press on the window";
        wallInteractable.enabled = false;
        windowInteractable.enabled = true;
        LipSyncController.Instance.SetLipSyncClip(windowLib);
        windowEmitter.Play();
        LipSyncController.Instance.StartLipSync();
    }

    public void StartSource()
    {
        if(sourcePressed) return;
        panelText.text = "Press on the start source button";
        sourcePressed = true;
        LipSyncController.Instance.SetLipSyncClip(startSourceLib);
        startSourceEmitte.Play();
        LipSyncController.Instance.StartLipSync();
    }

    public void Outro()
    {
        panelText.text = "Posters on the right contain information about additional options."; 
        StartCoroutine(StopInfoSign());
        LipSyncController.Instance.SetLipSyncClip(outroLib);
        outEmitter.Play();
        LipSyncController.Instance.StartLipSync();
        AcousticsControl.Instance.EnableInteractables(true);
        teacherInteractable.enabled = false;
        posters.SetActive(true);
    }

    public IEnumerator StopInfoSign()
    {
        yield return new WaitUntil(() => outEmitter.IsPlaying());
        yield return new WaitUntil(() => !outEmitter.IsPlaying());
        panelInfo.SetActive(false);
        teacherInteractable.enabled = true;
    }

}
