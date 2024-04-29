using System.Collections;
using UnityEngine;
using FMODUnity;
using FMOD;
using UnityEngine.InputSystem;
using System;
using System.Runtime.InteropServices;
using FMOD.Studio;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public Button micUIButton;

    public EventReference micRef;
    public RecordMic recMic;
    EVENT_CALLBACK micCallback;
    public bool micOn = false;
    public Bus micChannelGroup;
    public KeyCode micOnButton;
    
    public IEnumerator PlayTheMic()
    {
        GameObject mainCam = Camera.main.gameObject;
        StudioEventEmitter emitter = mainCam.GetComponent<StudioEventEmitter>();
        EventInstance micInstance = emitter.EventInstance;
        micCallback = new EVENT_CALLBACK(MicEventCallback);
        SoundRequirements sr = new SoundRequirements(recMic.sound);
        GCHandle stringHandle = GCHandle.Alloc(sr);
        micInstance.setUserData(GCHandle.ToIntPtr(stringHandle));
        micInstance.setCallback(micCallback);
        print("callback set");
        print("start");

        while(!micInstance.isValid())
        {
            print("not valid yet");
            yield return new WaitForSeconds(1.0f);
        }

        micInstance.start();
        RuntimeManager.GetBus("bus:/SpatializedSounds/MicrophoneBus").setMute(true);
    }


    public void TurnOnMic(bool on)
    {
        print("isPressed");
        RuntimeManager.GetBus("bus:/SpatializedSounds/MicrophoneBus").setMute(!on);
    }

    void Start() {
        if(micOn)
            recMic.TurnOnMic();
    }


    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT MicEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);
        instance.getUserData(out IntPtr soundPtr);
        GCHandle soundHandle =  GCHandle.FromIntPtr(soundPtr);
        SoundRequirements sr = soundHandle.Target as SoundRequirements;

        switch (type)
        {
            case FMOD.Studio.EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND:
            {
                var parameter = (FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES));
                parameter.sound = sr.sound.handle;
                parameter.subsoundIndex = -1;
                Marshal.StructureToPtr(parameter, parameterPtr, false);
                break;
            }
            case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND:
            {
                var parameter = (FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.PROGRAMMER_SOUND_PROPERTIES));
                var sound = new FMOD.Sound(parameter.sound);
                sound.release();
                break;
            }
            case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROYED:
            {
                soundHandle.Free();
                break;
            }
        }
        return FMOD.RESULT.OK;
    }
}

class SoundRequirements
{
    public FMOD.Sound sound;
    public SoundRequirements(FMOD.Sound sound)
    {
        this.sound = sound;
    }
}

