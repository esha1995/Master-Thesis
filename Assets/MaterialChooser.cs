using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MaterialChooser : MonoBehaviour
{
    public static bool curtainPressed = false;
    public static bool windowPressed = false;
    public static bool sourcePressed = false;
    public Transform buttonParent;
    public GameObject buttonPrefab;
    public static surfaces curSurfaceType = surfaces.Floor;
    public enum surfaces {Floor, Roof, WallBack, WallFront, WallR, WallL};
    public enum MaterialFloor {Wood, Ceramic, Carpet, Length};
    public enum MaterialRoof {Wood, Ceramic, Fibreglass, Length};
    public enum MaterialWall {Plaster, Curtain, Length};
    public static MaterialFloor curMaterialFloor = MaterialFloor.Ceramic;
    public static MaterialRoof curMaterialRoof;
    public static MaterialWall curMaterialWallL, curMaterialWallR, curMaterialWallBack, curMaterialWallFront;

    public static MaterialFloor startMaterialFloor = MaterialFloor.Ceramic;
    public static MaterialRoof startMaterialRoof = MaterialRoof.Ceramic;
    public static MaterialWall startMaterialWallL = MaterialWall.Plaster, startMaterialWallR = MaterialWall.Plaster, startMaterialWallBack = MaterialWall.Plaster, startMaterialWallFront;

    public TextMeshProUGUI menuText;
    void Start()
    {   
        if(!curtainPressed) return;
        StartCoroutine(DestroyYourself());
    }

    public void AddButtonsSurface()
    {   
        menuText.text = "Choose material";
        switch(curSurfaceType)
        {
            case surfaces.WallBack: case surfaces.WallFront: case surfaces.WallL: case surfaces.WallR:
                foreach(MaterialWall material in Enum.GetValues(typeof(MaterialWall)))
                    AddButtonsSurface(material);
                    switch(curSurfaceType)
                    {
                        case surfaces.WallBack: 
                            if(curMaterialWallBack != MaterialWall.Curtain)
                            {
                                AddButtonSoundPanel();
                            }
                        break;
                        case surfaces.WallFront: 
                            if(curMaterialWallFront != MaterialWall.Curtain)
                            {
                                AddButtonSoundPanel();
                            }
                        break;
                        case surfaces.WallL: 
                            if(curMaterialWallL != MaterialWall.Curtain)
                            {
                                AddButtonSoundPanel();
                            }
                        break;
                        case surfaces.WallR:
                            if(curMaterialWallR != MaterialWall.Curtain)
                            {
                                AddButtonSoundPanel();
                            }
                        break;
                    }
                break;
            case surfaces.Floor:
                foreach(MaterialFloor material in Enum.GetValues(typeof(MaterialFloor)))
                    AddButtonsSurface(material);
                break;
            case surfaces.Roof:
                foreach(MaterialRoof material in Enum.GetValues(typeof(MaterialRoof)))
                    AddButtonsSurface(material);
                break;
        }

    }

    public void AddButtonsFur(bool addFur = false)
    {
        // menuText.text = "Furniture";
        GameObject buttonGo = Instantiate(buttonPrefab, buttonParent);
        Button button = buttonGo.GetComponent<Button>();
        button.GetComponentInChildren<TextMeshProUGUI>().text = addFur ? "Add furniture" : "Remove furniture";
        button.onClick.AddListener(delegate{
            GameObject chairParent = GameObject.FindGameObjectWithTag("Furniture");
            for(int i = 0; i<chairParent.transform.childCount; ++i)
            {
                GameObject child = chairParent.transform.GetChild(i).gameObject;
                child.SetActive(addFur);
            }
            AcousticsControl.Instance.furOn = addFur;
            Destroy(this.gameObject);
        });
    }

    public void AddButtonAudioSource(AcousticsControl.SourceType sourceType)
    {   
        string sourceName = Enum.GetName(typeof(AcousticsControl.SourceType), sourceType);
        menuText.text = sourceName;
        GameObject buttonGo = Instantiate(buttonPrefab, buttonParent);
        Button button = buttonGo.GetComponent<Button>();
        GameObject[] soundSources = GameObject.FindGameObjectsWithTag(sourceName);
        GameObject soundSource = soundSources[0];
        List<StudioEventEmitter> emitters = new List<StudioEventEmitter>();
        foreach(var source in soundSources) emitters.AddRange(source.GetComponentsInChildren<StudioEventEmitter>());
        bool stopPlaying = emitters[0].IsPlaying();
        button.GetComponentInChildren<TextMeshProUGUI>().text = stopPlaying ? "Stop source" : "Start source";
        button.onClick.AddListener(delegate{
            if(!sourcePressed)
            {
                sourcePressed = true;
                TuturialScript.Instance.Outro();
            }

            if(sourceType == AcousticsControl.SourceType.Teacher)
            {
                Animator animator = soundSource.GetComponent<Animator>();
                animator.SetBool("talking", !stopPlaying);
#if UNITY_ANDROID && !UNITY_EDITOR
                LipSyncController.Instance.GetLipSyncSource().clip = LipSyncController.Instance.syncClip;
                if(stopPlaying) LipSyncController.Instance.StopLipSync();
                else LipSyncController.Instance.StartLipSync();
#endif
            }

            if(sourceType == AcousticsControl.SourceType.Children)
            {
                foreach(var child in soundSources)
                {
                    Animator animator = child.GetComponentInChildren<Animator>();
                    animator.SetBool("talking", !stopPlaying);
                }
            }

            if(sourceType == AcousticsControl.SourceType.Playground)
            {
                for(int i = 0; i < AcousticsControl.Instance.openWindows.Length; ++i)
                {
                    AcousticsControl.Instance.openWindows[i].SetActive(!stopPlaying);
                    AcousticsControl.Instance.closedWindows[i].SetActive(stopPlaying);
                }
            }
            

            if(stopPlaying)
            {
                    foreach(var emitter in emitters) emitter.Stop();
            } 
            else
            {
                if(sourceType == AcousticsControl.SourceType.Teacher) emitters[0].Play();
                else{foreach(var emitter in emitters) emitter.Play();}
            } 
            Destroy(this.gameObject);
        });
    }

    void AddButtonSoundPanel()
    {
        GameObject ButtonAddObj = Instantiate(buttonPrefab, buttonParent);
        Button buttonAdd = ButtonAddObj.GetComponent<Button>();
        buttonAdd.GetComponentInChildren<TextMeshProUGUI>().text = "Add sound panel";

        GameObject ButtonRemoveObj = Instantiate(buttonPrefab, buttonParent);
        Button buttonRemove = ButtonRemoveObj.GetComponent<Button>();
        buttonRemove.GetComponentInChildren<TextMeshProUGUI>().text = "Remove all panels";

        buttonAdd.onClick.AddListener(delegate {
            GameObject panel = AcousticsControl.Instance.AddSoundPanel();
            if(panel != null)
            {
                Transform parent = GameObject.FindGameObjectWithTag(Enum.GetName(typeof(surfaces), curSurfaceType)).transform;
                panel.transform.parent = parent;
            }

            Destroy(this.gameObject);
        });


        buttonRemove.onClick.AddListener(delegate {
            Transform parent = GameObject.FindGameObjectWithTag(Enum.GetName(typeof(surfaces), curSurfaceType)).transform;
            for(int i = 0; i < parent.childCount; ++i)
            {
                if(parent.GetChild(i).tag == "SoundPanel")
                {
                    Destroy(parent.GetChild(i).gameObject);
                }
            }
            Destroy(this.gameObject);
        });
    }

    void AddButtonsSurface(MaterialWall material){AddButtonsSurface((Enum)material); }
    void AddButtonsSurface(MaterialFloor material){AddButtonsSurface((Enum)material); }
    void AddButtonsSurface(MaterialRoof material){AddButtonsSurface((Enum)material); }
    void AddButtonsSurface(Enum enumValue)
    {   
        Type enumType = enumValue.GetType();
        string name = Enum.GetName(enumType, enumValue);
        string buttonName = "";
        if(name == "Ceramic") buttonName = "Tiles (Ceramic)";
        if(name == "Fibreglass") buttonName = "Panels (Fibre glass)";
        if(name == "Carpet") buttonName = "Carpet (Wool)";
        if(name == "Plaster") buttonName = "Wall (Plaster/cement)";
        if(name == "Wood") buttonName = "Wood";
        if(name == "Curtain") buttonName = "Curtain (Wool)";

        if(name == "Length")
            return;

        GameObject buttonGo = Instantiate(buttonPrefab, buttonParent);
        Button button = buttonGo.GetComponent<Button>();
        button.GetComponentInChildren<TextMeshProUGUI>().text = buttonName;
        button.onClick.AddListener(delegate
        {
            if(!curtainPressed)
            {
                TuturialScript.Instance.Window();
                curtainPressed = true;
            }
            curtainPressed = true;
            Transform parent = GameObject.FindGameObjectWithTag(Enum.GetName(typeof(surfaces), curSurfaceType)).transform;
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

                if(enumType == typeof(MaterialWall))
                {
                    MaterialWall matWall = (MaterialWall)enumValue;
                    if(matWall == MaterialWall.Curtain)
                    {
                        if(child.tag  == "SoundPanel")
                        {
                            Destroy(child);
                        }
                    }
                }
            }

            switch(curSurfaceType)
            {
                case surfaces.WallBack: 
                    curMaterialWallBack = (MaterialWall)enumValue;
                break;
                case surfaces.WallFront: 
                    curMaterialWallFront = (MaterialWall)enumValue;
                break;
                case surfaces.WallL: 
                    curMaterialWallL = (MaterialWall)enumValue;
                break;
                case surfaces.WallR:
                    curMaterialWallR = (MaterialWall)enumValue;
                break;
                case surfaces.Floor:
                    curMaterialFloor = (MaterialFloor)enumValue;
                break;
                case surfaces.Roof:
                    curMaterialRoof = (MaterialRoof)enumValue;
                break;
            }

            Destroy(this.gameObject);

        });


    }
        IEnumerator DestroyYourself()
        {
            yield return new WaitForSeconds(15.0f);
            Destroy(this.gameObject);
        }

}
