using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class StopAnimation : MonoBehaviour
{

    public Animator animator;
    public StudioEventEmitter[] emitters;
    public StudioEventEmitter playingEmitter;
    bool talking = false;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        emitters = GetComponentsInChildren<StudioEventEmitter>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!talking)
        {
            foreach(var emitter in emitters)
            {
                if(emitter.IsPlaying())
                {
                    playingEmitter = emitter;
                    animator.SetBool("talking", true);
                    talking = true;
                    break;
                }
            }
        }
        else
        {
            if(!playingEmitter.IsPlaying())
            {
                talking = false;
                animator.SetBool("talking", false);
            }
        }
    }
}
