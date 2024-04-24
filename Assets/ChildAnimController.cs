using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildAnimController : MonoBehaviour
{
    public Vector2 offset, speed;
    public bool mirror = false;
    public int talkingType;

    Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        animator.SetFloat("Offset", Random.Range(offset.x, offset.y));
        animator.SetFloat("Speed", Random.Range(speed.x, speed.y));
        animator.SetInteger("TalkingType", talkingType);
        animator.SetBool("Mirror", mirror);
    }
}
