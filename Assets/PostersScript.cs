using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostersScript : MonoBehaviour
{
    public GameObject curtain;
    // Start is called before the first frame update
    Vector3 beginPos;
    IEnumerator Start()
    {
        beginPos = transform.position;
        while(true)
        {
            yield return new WaitUntil(() => curtain.activeSelf);
            transform.position = new Vector3(0.87f, transform.position.y, transform.position.z);
            yield return new WaitUntil(() => !curtain.activeSelf);
            transform.position = beginPos;
        }
    }
}
