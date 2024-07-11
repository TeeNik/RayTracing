using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private CinemachineFreeLook _cinemachineFreeLook;
    
    void Start()
    {
        _cinemachineFreeLook = GetComponent<CinemachineFreeLook>();
        _cinemachineFreeLook.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _cinemachineFreeLook.enabled = true;
        } 
        else if (Input.GetMouseButtonUp(0))
        {
            _cinemachineFreeLook.enabled = false;
        }
    }
}
