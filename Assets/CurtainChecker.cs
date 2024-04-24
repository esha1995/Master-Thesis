using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CurtainChecker : MonoBehaviour
{
    public GameObject curtain;
    public XRSimpleInteractable interactable;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        while(true)
        {           
            yield return new WaitUntil(() => curtain.activeSelf);
            interactable.enabled = false;
            yield return new WaitUntil(() => !curtain.activeSelf);
            interactable.enabled = true;
        }
    }
}
