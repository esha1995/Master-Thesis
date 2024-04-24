using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class StopAnimation : MonoBehaviour
{

    public Animator animator;
    public StudioEventEmitter eventEmitter;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        eventEmitter = GetComponentInChildren<StudioEventEmitter>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!eventEmitter.IsPlaying() && animator.GetBool("talking"))
        {
            animator.SetBool("talking", false);
        }
    }
}
