using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class VirtualButtonScript : MonoBehaviour
{
    public GameObject sphereGo, cubeGo;
    public GameObject LiveStream,LiveStream2;
    private bool curActive;
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
        //sphereGo.SetActive(false);
        //cubeGo.SetActive(true);

        curActive = false;
        Debug.Log("Got it");
    }
    
    
    public void OnButtonPressed(VirtualButtonBehaviour vb)
    {
        //sphereGo.SetActive(true);
        //cubeGo.SetActive(false);
        Debug.Log("Button Pressed");
        if(curActive == true){
            LiveStream.SetActive(false);
            LiveStream2.SetActive(false);
            curActive = false;
        }
        if(curActive == false){
            LiveStream.SetActive(true);
            LiveStream2.SetActive(true);
            curActive = true;
        }
    }

    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        //cubeGo.SetActive(true);
        //sphereGo.SetActive(false);
    }
}