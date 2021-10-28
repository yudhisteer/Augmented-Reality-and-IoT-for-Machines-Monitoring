using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class VirtualButtonScript : MonoBehaviour
{
    public GameObject sphereGo, cubeGo;
    VirtualButtonBehaviour[] vbs;
    // Start is called before the first frame update
    void Start()
    {
        vbs = GetComponentsInChildren<VirtualButtonBehaviour>();
        for (int i = 0; i < vbs.Length; ++i)
        {
            vbs[i].RegisterOnButtonPressed(OnButtonPressed);
            vbs[i].RegisterOnButtonReleased(OnButtonReleased);
        }
        sphereGo.SetActive(false);
        cubeGo.SetActive(true);
    }

    public void OnButtonPressed(VirtualButtonBehaviour vb)
    {
        sphereGo.SetActive(true);
        cubeGo.SetActive(false);
    }

    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        cubeGo.SetActive(true);
        sphereGo.SetActive(false);
    }
}