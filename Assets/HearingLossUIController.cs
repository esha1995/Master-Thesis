using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class HearingLossUIController : MonoBehaviour
{
    public Button mildL, mildR, modL, modR, sevL, sevR, earL, earR;
    HearingLossSimulator.HearingLoss curHearingLossL = HearingLossSimulator.HearingLoss.Off;
    HearingLossSimulator.HearingLoss curHearingLossR = HearingLossSimulator.HearingLoss.Off;
    HearingLossSimulator.HearingLoss prevL = HearingLossSimulator.HearingLoss.Mild;
    HearingLossSimulator.HearingLoss prevR = HearingLossSimulator.HearingLoss.Mild;
    public float distToCam = 1.5f;
    bool onL = false;
    bool onR = false;

    void Start()
    {
        earL.GetComponent<Image>().color = Color.red;
        earR.GetComponent<Image>().color = Color.red;
        mildL.GetComponent<Image>().color = Color.green;
        mildR.GetComponent<Image>().color = Color.green;
        modL.GetComponent<Image>().color = Color.red;
        modR.GetComponent<Image>().color = Color.red;
        sevL.GetComponent<Image>().color = Color.red;
        sevR.GetComponent<Image>().color = Color.red;

#if !UNITY_EDITOR
        this.gameObject.SetActive(false);  
#endif


        earL.onClick.AddListener(delegate
        {
            if(onL)
            {
                earL.GetComponent<Image>().color = Color.red;
                mildL.interactable = false;
                sevL.interactable = false;
                modL.interactable = false;
                prevL = curHearingLossL;
                curHearingLossL = HearingLossSimulator.HearingLoss.Off;
                AcousticsControl.Instance.SetHearingLoss(curHearingLossL, true);
                onL = false;
            }
            else
            {
                earL.GetComponent<Image>().color = Color.green;
                mildL.interactable = true;
                sevL.interactable = true;
                modL.interactable = true;
                curHearingLossL = prevL;
                AcousticsControl.Instance.SetHearingLoss(curHearingLossL, true);
                onL = true;
            }    
        });

        sevL.onClick.AddListener(delegate
        {
            sevL.GetComponent<Image>().color = Color.green;
            modL.GetComponent<Image>().color = Color.red;
            mildL.GetComponent<Image>().color = Color.red;
            curHearingLossL = HearingLossSimulator.HearingLoss.Severe;
            AcousticsControl.Instance.SetHearingLoss(curHearingLossL, true);
        });

        modL.onClick.AddListener(delegate
        {
            sevL.GetComponent<Image>().color = Color.red;
            modL.GetComponent<Image>().color = Color.green;
            mildL.GetComponent<Image>().color = Color.red;
            curHearingLossL = HearingLossSimulator.HearingLoss.Moderate;
            AcousticsControl.Instance.SetHearingLoss(curHearingLossL, true);
        });

        mildL.onClick.AddListener(delegate
        {
            sevL.GetComponent<Image>().color = Color.red;
            modL.GetComponent<Image>().color = Color.red;
            mildL.GetComponent<Image>().color = Color.green;
            curHearingLossL = HearingLossSimulator.HearingLoss.Mild;
            AcousticsControl.Instance.SetHearingLoss(curHearingLossL, true);
        });

        sevR.onClick.AddListener(delegate
        {
            sevR.GetComponent<Image>().color = Color.green;
            modR.GetComponent<Image>().color = Color.red;
            mildR.GetComponent<Image>().color = Color.red;
            curHearingLossR = HearingLossSimulator.HearingLoss.Severe;
            AcousticsControl.Instance.SetHearingLoss(curHearingLossR, false);
        });

        modR.onClick.AddListener(delegate
        {
            sevR.GetComponent<Image>().color = Color.red;
            modR.GetComponent<Image>().color = Color.green;
            mildR.GetComponent<Image>().color = Color.red;
            curHearingLossR = HearingLossSimulator.HearingLoss.Moderate;
            AcousticsControl.Instance.SetHearingLoss(curHearingLossR, false);
        });

        mildR.onClick.AddListener(delegate
        {
            sevR.GetComponent<Image>().color = Color.red;
            modR.GetComponent<Image>().color = Color.red;
            mildR.GetComponent<Image>().color = Color.green;
            curHearingLossR = HearingLossSimulator.HearingLoss.Mild;
            AcousticsControl.Instance.SetHearingLoss(curHearingLossR, false);
        });

        earR.onClick.AddListener(delegate
        {
            if(onR)
            {
                earR.GetComponent<Image>().color = Color.red;
                sevR.interactable = false;
                modR.interactable = false;
                mildR.interactable = false;
                prevR = curHearingLossR;
                curHearingLossR = HearingLossSimulator.HearingLoss.Off;
                AcousticsControl.Instance.SetHearingLoss(curHearingLossR, false);
                onR = false;
            }
            else
            {
                earR.GetComponent<Image>().color = Color.green;
                sevR.interactable = true;
                modR.interactable = true;
                mildR.interactable = true;
                curHearingLossR = prevR;
                AcousticsControl.Instance.SetHearingLoss(curHearingLossR, false);
                onR = true;
            }    
        });
    }

    void OnEnable()
    {
        Transform cameraTransform = Camera.main.transform;
        this.transform.position = cameraTransform.position + cameraTransform.forward * distToCam;
        this.transform.rotation = cameraTransform.rotation;
    }
}
