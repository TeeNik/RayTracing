using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool DisableMouseControl = false;
    
    private CinemachineFreeLook _cinemachineFreeLook;
    
    void Start()
    {
        _cinemachineFreeLook = GetComponent<CinemachineFreeLook>();
        _cinemachineFreeLook.enabled = DisableMouseControl;

        if (DisableMouseControl)
        {
            _cinemachineFreeLook.m_XAxis.m_MaxSpeed = 0;
            _cinemachineFreeLook.m_YAxis.m_MaxSpeed = 0;
        }
    }

    void Update()
    {
        if (DisableMouseControl)
        {
            return;
        }

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
