using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ResetAcousticsUI : MonoBehaviour
{
    public float distToCam = 1.0f;
    public Button keepReset, goBack;

    void OnEnable()
    {
        AcousticsControl.Instance.ResetAll();
        Transform cameraTransform = Camera.main.transform;
        this.transform.position = cameraTransform.position + cameraTransform.forward * distToCam;
        this.transform.rotation = cameraTransform.rotation;
    }

    void Start()
    {
        keepReset.onClick.AddListener(() => 
        {
            AcousticsControl.Instance.KeepReset();
            this.gameObject.SetActive(false);
        });

        goBack.onClick.AddListener(()=>  
        {
            AcousticsControl.Instance.StopReset();
            this.gameObject.SetActive(false);
        });

        StartCoroutine(KeepResetAutomatic());
    }

    IEnumerator KeepResetAutomatic()
    {
        yield return new WaitForSecondsRealtime(5.0f);
        AcousticsControl.Instance.KeepReset();
        this.gameObject.SetActive(false);
    }
}
