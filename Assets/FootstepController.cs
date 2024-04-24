using System;
using System.Collections;
using FMODUnity;
using UnityEngine;

public class FootstepController : MonoBehaviour
{
    public MaterialChooser.MaterialFloor curFootstepType;

    public StudioEventEmitter[] footStepEmitters;
    public GameObject footStepSound;
    public float distBeforeStep = 0.5f;
    private Vector3 prevPos;
    private Transform camTrans;

    void Start()
    { 
        camTrans = Camera.main.transform;
        UpdateFootstep(MaterialChooser.MaterialFloor.Ceramic);
        prevPos = camTrans.position;
        StartCoroutine(CheckForMovement());
    }

    private void UpdateFootstep(MaterialChooser.MaterialFloor footstepType)
    {
        this.curFootstepType = footstepType;

        foreach(MaterialChooser.MaterialFloor step in Enum.GetValues(typeof(MaterialChooser.MaterialFloor)))
        {
            RuntimeManager.StudioSystem.setParameterByName(
            Enum.GetName(typeof(MaterialChooser.MaterialFloor), step) + "Footstep", // param name
            step == footstepType ? 1.0f : 0.0f); // value
        }
    }

    IEnumerator CheckForMovement()
    {
        while(true)
        {
            yield return new WaitUntil(() => Vector3.Distance(camTrans.position, prevPos) > UnityEngine.Random.Range(distBeforeStep * 0.8f, distBeforeStep * 1.2f));
            PlayFootstep();
            prevPos = camTrans.position;
        }
    }

    public void PlayFootstep()
    {
        Vector3 mainCamPos = Camera.main.transform.position;
        if(Physics.Raycast(mainCamPos, Vector3.down, out RaycastHit hit, 10.0f))
        {
            if(hit.collider.gameObject.tag != "Floor")
                return;

            Vector3 hitPos = hit.point;
            UpdateFootstep(MaterialChooser.curMaterialFloor);
            //GameObject obj = Instantiate(footStepSound, hitPos, Quaternion.LookRotation(Vector3.up));
            //StudioEventEmitter emitter = obj.GetComponent<StudioEventEmitter>();
            //emitter.Play();
            if(!footStepEmitters[0].IsPlaying())
            {   
                footStepEmitters[0].Play();
                return;
            }
            else if(!footStepEmitters[1].IsPlaying())
            {   
                footStepEmitters[1].Play();
                return;
            }
            else if(!footStepEmitters[2].IsPlaying())
            {   
                footStepEmitters[2].Play();
                return;
            }
            else
            {
                footStepEmitters[3].Play();
            }
                
        }
    }
}
