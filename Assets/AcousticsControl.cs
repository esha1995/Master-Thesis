using System;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using System.Collections.Generic;


public class AcousticsControl : MonoBehaviour
{   public GameObject resetMenu;
    public GameObject[] openWindows;
    public GameObject[] closedWindows;
    public aChild[] children;
    public GameObject acousticPanel;
    public GameObject MaterialChooserObj;
    public GameObject settings;
    private GameObject prevChooser;
    SelectEnterEventArgs curEnterEventArgs;
    public Button hearingLossOnOff;
    Vector3 curRayPos, curRayNorm;
    public float menuDistToCam = 1.5f;
    public float menuScale = 1.0f;
    public bool furOn = true;
    public enum SourceType {Teacher, Speaker, Playground, Children};
    public static AcousticsControl Instance;

    public List<GameObject> currentResetObjects = new List<GameObject>();
    public HearingLossSimulator hearingLossSimulator;

    private float settingsTimer = 5.0f;
    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
        
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
#if !UNITY_EDITOR
        settings.SetActive(false);
#endif
    }

    void Start()
    {
        hearingLossSimulator = new HearingLossSimulator();
        hearingLossSimulator.SetHearingLoss(HearingLossSimulator.HearingLoss.Off, true); // LEFT OFF
        hearingLossSimulator.SetHearingLoss(HearingLossSimulator.HearingLoss.Off, false); // RIGHT OFF
    }

    public void SetHearingLoss(HearingLossSimulator.HearingLoss hearingLoss, bool left)
    {
        hearingLossSimulator.SetHearingLoss(hearingLoss, left);
    }

    public void IncreaseSettingsTimer()
    {
        settingsTimer += 5.0f;
    }

    public void ResetAll()
    {
        currentResetObjects.Clear();

        foreach(MaterialChooser.surfaces surface in Enum.GetValues(typeof(MaterialChooser.surfaces)))
        {
            Transform parent = GameObject.FindGameObjectWithTag(Enum.GetName(typeof(MaterialChooser.surfaces), surface)).transform;
            GameObject firstChild = parent.GetChild(0).gameObject;
            if(firstChild.activeSelf) currentResetObjects.Add(firstChild);
            else firstChild.SetActive(true);

            for(int i = 1; i<parent.childCount; ++i)
            {
                GameObject child = parent.GetChild(i).gameObject;
                if(child.gameObject.activeSelf)
                {
                    currentResetObjects.Add(child);
                }
                child.SetActive(false);
            }
        }

     //    print("current reset amount: " + currentResetObjects.Count);
    }


    public void ChangeAcousticMaterial(MaterialChooser.MaterialFloor material) 
    {ChangeAcousticMaterial(MaterialChooser.surfaces.Floor, (Enum)material); }
    public void ChangeAcousticMaterial(MaterialChooser.MaterialRoof material) 
    {ChangeAcousticMaterial(MaterialChooser.surfaces.Roof, (Enum)material); }
    public void ChangeAcousticMaterial(MaterialChooser.MaterialWall material) 
    {ChangeAcousticMaterial(MaterialChooser.surfaces.WallBack, (Enum)material);
    ChangeAcousticMaterial(MaterialChooser.surfaces.WallFront, (Enum)material);
    ChangeAcousticMaterial(MaterialChooser.surfaces.WallL, (Enum)material);
    ChangeAcousticMaterial(MaterialChooser.surfaces.WallR, (Enum)material); }
    public void ChangeAcousticMaterial(MaterialChooser.MaterialWall materialB, MaterialChooser.MaterialWall materialF, MaterialChooser.MaterialWall materialL, MaterialChooser.MaterialWall materialR) 
    {ChangeAcousticMaterial(MaterialChooser.surfaces.WallBack, (Enum)materialB);
    ChangeAcousticMaterial(MaterialChooser.surfaces.WallFront, (Enum)materialF);
    ChangeAcousticMaterial(MaterialChooser.surfaces.WallL, (Enum)materialL);
    ChangeAcousticMaterial(MaterialChooser.surfaces.WallR, (Enum)materialR); }

    public void ChangeAcousticMaterialRandom()
    {
        MaterialChooser.MaterialWall wallBack = (MaterialChooser.MaterialWall)UnityEngine.Random.Range(0, (int)MaterialChooser.MaterialWall.Length);
        MaterialChooser.MaterialWall wallFront = (MaterialChooser.MaterialWall)UnityEngine.Random.Range(0, (int)MaterialChooser.MaterialWall.Length);
        MaterialChooser.MaterialWall wallLeft = (MaterialChooser.MaterialWall)UnityEngine.Random.Range(0, (int)MaterialChooser.MaterialWall.Length);
        MaterialChooser.MaterialWall wallRight = (MaterialChooser.MaterialWall)UnityEngine.Random.Range(0, (int)MaterialChooser.MaterialWall.Length);
        MaterialChooser.MaterialFloor floor = (MaterialChooser.MaterialFloor)UnityEngine.Random.Range(0, (int)MaterialChooser.MaterialFloor.Length);
        MaterialChooser.MaterialRoof roof = (MaterialChooser.MaterialRoof)UnityEngine.Random.Range(0, (int)MaterialChooser.MaterialRoof.Length);
        ChangeAcousticMaterial(MaterialChooser.surfaces.WallBack, (Enum)wallBack);
        ChangeAcousticMaterial(MaterialChooser.surfaces.WallFront, (Enum)wallFront);
        ChangeAcousticMaterial(MaterialChooser.surfaces.WallL, (Enum)wallLeft);
        ChangeAcousticMaterial(MaterialChooser.surfaces.WallR, (Enum)wallRight); 
        ChangeAcousticMaterial(MaterialChooser.surfaces.Roof, (Enum)roof);
        ChangeAcousticMaterial(MaterialChooser.surfaces.Floor, (Enum)floor); 
    } 



    private void ChangeAcousticMaterial(MaterialChooser.surfaces surface, Enum enumValue)
    {
        Type enumType = enumValue.GetType();
        string name = Enum.GetName(enumType, enumValue);
        Transform parent = GameObject.FindGameObjectWithTag(Enum.GetName(typeof(MaterialChooser.surfaces), surface)).transform;

        for(int i = 0; i < parent.childCount; ++i)
        {
            GameObject child = parent.GetChild(i).gameObject;

            if(child.tag == name)
            {
                child.SetActive(true);
            }
            else
            {
                child.SetActive(false);
            }

            if(enumType == typeof(MaterialChooser.MaterialWall))
            {
                MaterialChooser.MaterialWall matWall = (MaterialChooser.MaterialWall)enumValue;
                if(matWall == MaterialChooser.MaterialWall.Curtain)
                {
                    if(child.tag  == "SoundPanel")
                    {
                        Destroy(child);
                    }
                }
            }
        }

        switch(surface)
        {
            case MaterialChooser.surfaces.WallBack: 
                MaterialChooser.curMaterialWallBack = (MaterialChooser.MaterialWall)enumValue;
            break;
            case MaterialChooser.surfaces.WallFront: 
                MaterialChooser.curMaterialWallFront = (MaterialChooser.MaterialWall)enumValue;
            break;
            case MaterialChooser.surfaces.WallL: 
                MaterialChooser.curMaterialWallL = (MaterialChooser.MaterialWall)enumValue;
            break;
            case MaterialChooser.surfaces.WallR:
                MaterialChooser.curMaterialWallR = (MaterialChooser.MaterialWall)enumValue;
            break;
            case MaterialChooser.surfaces.Floor:
                MaterialChooser.curMaterialFloor = (MaterialChooser.MaterialFloor)enumValue;
            break;
            case MaterialChooser.surfaces.Roof:
                MaterialChooser.curMaterialRoof = (MaterialChooser.MaterialRoof)enumValue;
            break;
        }
    }

    public void KeepReset()
    {
        foreach(MaterialChooser.surfaces surface in Enum.GetValues(typeof(MaterialChooser.surfaces)))
        {
            Transform parent = GameObject.FindGameObjectWithTag(Enum.GetName(typeof(MaterialChooser.surfaces), surface)).transform;
            for(int i = 0; i<parent.childCount; ++i)
            {
                GameObject child = parent.GetChild(i).gameObject;
                if(child.tag == "SoundPanel")
                {
                    Destroy(child);
                }
            }

            switch(surface)
            {
                case MaterialChooser.surfaces.Floor:
                    MaterialChooser.curMaterialFloor = MaterialChooser.startMaterialFloor;
                    break;
                case MaterialChooser.surfaces.Roof:
                    MaterialChooser.curMaterialRoof = MaterialChooser.startMaterialRoof;
                    break;
                case MaterialChooser.surfaces.WallFront:
                    MaterialChooser.curMaterialWallFront = MaterialChooser.startMaterialWallFront;
                    break;
                case MaterialChooser.surfaces.WallBack:
                    MaterialChooser.curMaterialWallBack = MaterialChooser.startMaterialWallBack;
                    break;
                case MaterialChooser.surfaces.WallL:
                    MaterialChooser.curMaterialWallL = MaterialChooser.startMaterialWallL;
                    break;
                case MaterialChooser.surfaces.WallR:
                    MaterialChooser.curMaterialWallL = MaterialChooser.startMaterialWallR;
                    break;
            }
        }
    }

    public void StopReset()
    {
        foreach(MaterialChooser.surfaces surface in Enum.GetValues(typeof(MaterialChooser.surfaces)))
        {
            Transform parent = GameObject.FindGameObjectWithTag(Enum.GetName(typeof(MaterialChooser.surfaces), surface)).transform;
            for(int i = 0; i<parent.childCount; ++i)
            {

                GameObject child = parent.GetChild(i).gameObject;
                if(currentResetObjects.Contains(child))
                {
                    print("match" + child.tag);
                    child.SetActive(true);
                }
                else
                {
                    child.SetActive(false);
                }
            }
        }
    }

    public void GetEventArgs(SelectEnterEventArgs enterEventArgs)
    {
        curEnterEventArgs = enterEventArgs;
    }

    public void OpenResetMenu(bool open)
    {
        if(!open)
            return;
        
        DestroyPrevMenus();
        resetMenu.SetActive(true);
    }

    public void DestroyPrevMenus()
    {
        GameObject[] prevMenus = GameObject.FindGameObjectsWithTag("Menu");
        foreach(GameObject menu in prevMenus)
        {
            Destroy(menu);
        }

        GameObject reset = GameObject.FindGameObjectWithTag("Reset");
        if(reset != null)
        {
            reset.SetActive(false);
        }

        GameObject hearingLoss = GameObject.FindGameObjectWithTag("HearingLoss");
        if(hearingLoss != null)
        {
            hearingLoss.SetActive(false);
        }
    }

    public void OpenMenu(MaterialChooser.surfaces surface)
    {
        DestroyPrevMenus();

        if(curEnterEventArgs.interactor.gameObject.TryGetComponent(out XRRayInteractor rayInteractor))
        {
            if(rayInteractor.TryGetHitInfo(out Vector3 rayPos, out Vector3 rayNorm, out _, out _))
            {
                curRayPos = rayPos;
                curRayNorm = rayNorm;
                Vector3 camPos = Camera.main.transform.position;
                Vector3 dir = camPos - rayPos;
                Vector3 pos = camPos + dir.normalized * -menuDistToCam;
                GameObject chooser = Instantiate(MaterialChooserObj, pos, Quaternion.LookRotation(dir * -1));
                prevChooser = chooser;
                MaterialChooser mChooser = chooser.GetComponent<MaterialChooser>();
                MaterialChooser.curSurfaceType = surface;
                mChooser.AddButtonsSurface(); 
                if(surface == MaterialChooser.surfaces.Floor && !furOn)
                {
                    mChooser.AddButtonsFur(true);
                }
                chooser.transform.localScale *= menuScale;
            }
        }

        settings.SetActive(false);
    }

    public void OpenMenuAudio(SourceType sourceType)
    {
        Destroy(prevChooser);
    
        if(curEnterEventArgs.interactor.gameObject.TryGetComponent(out XRRayInteractor rayInteractor))
        {
            if(rayInteractor.TryGetHitInfo(out Vector3 rayPos, out Vector3 rayNorm, out _, out _))
            {
                Vector3 camPos = Camera.main.transform.position;
                Vector3 dir = camPos - rayPos;
                Vector3 pos = camPos + dir.normalized * -menuDistToCam;
                GameObject chooser = Instantiate(MaterialChooserObj, pos, Quaternion.LookRotation(dir * -1));
                prevChooser = chooser;
                MaterialChooser mChooser = chooser.GetComponent<MaterialChooser>();
                mChooser.AddButtonAudioSource(sourceType);
                chooser.transform.localScale *= menuScale;
            }
        }

        settings.SetActive(false);

    }

    public void OpenMenuChairs()
    {
        Destroy(prevChooser);

        if(curEnterEventArgs.interactor.gameObject.TryGetComponent(out XRRayInteractor rayInteractor))
        {
            if(rayInteractor.TryGetHitInfo(out Vector3 rayPos, out Vector3 rayNorm, out _, out _))
            {
                Vector3 camPos = Camera.main.transform.position;
                Vector3 dir = camPos - rayPos;
                Vector3 pos = camPos + dir.normalized * -menuDistToCam;
                GameObject chooser = Instantiate(MaterialChooserObj, pos, Quaternion.LookRotation(dir * -1));
                prevChooser = chooser;
                MaterialChooser mChooser = chooser.GetComponent<MaterialChooser>();
                mChooser.AddButtonsFur(); 
                chooser.transform.localScale *= menuScale;
            }
        }

        settings.SetActive(false);

    }

    public void OpenSettings(bool button)
    {
        if(!button)
            return;
        DestroyPrevMenus();
        StartCoroutine(OpenSettingsCou());
    }

    private IEnumerator OpenSettingsCou()
    {
        bool set = !settings.activeSelf;
        
        settings.SetActive(set);

        yield return new WaitUntil(() => settings.activeSelf == set);

        if(settings.activeSelf)
        {
            DateTime opened = DateTime.Now;
            settingsTimer = 5.0f;
            yield return new WaitUntil(() => (opened - DateTime.Now).TotalSeconds > settingsTimer);
            settings.SetActive(false);
        }
    }

    public GameObject AddSoundPanel()
    {
        GameObject panel = Instantiate(acousticPanel, curRayPos, Quaternion.LookRotation(curRayNorm * -1)); 
        panel.transform.localScale *= 1.0f;
        return panel;
    }


}
public class HearingLossSimulator
{
    public enum HearingLoss {Off, Mild, Moderate, Severe};
    public HearingLoss curHearingLossL, curHearingLossR;

    public HearingLossSimulator()
    {
    }

    public void SetHearingLoss(HearingLoss hearingLossType, bool left)
    {
        string paramName = "";
        if(left)
        {
            paramName = "LOSS L";
            curHearingLossL = hearingLossType;
        } 
        else
        {
            paramName = "LOSS R";
            curHearingLossR = hearingLossType;
        } 
        
        
        string labelName = "OFF";
        switch(hearingLossType)
        {
            case HearingLoss.Mild:
                labelName = "MILD";
                break;
            case HearingLoss.Moderate:
                labelName = "MODERATE";
                break;
            case HearingLoss.Severe:
                labelName = "SEVERE";
                break;
            case HearingLoss.Off:
                break;
        }

        // set the parameter
        RuntimeManager.StudioSystem.setParameterByNameWithLabel(paramName, labelName);
        AcousticsControl.Instance.IncreaseSettingsTimer(); 
    }
}

[Serializable]
public class aChild
{
    public Animator animator;
    public StudioEventEmitter emitter;

    public void PlayChild()
    {
        animator.SetBool("talking", true);
        emitter.Play();
    }

    public void StopChild()
    {
        animator.SetBool("talking", false);
        emitter.Stop();
    }

}
