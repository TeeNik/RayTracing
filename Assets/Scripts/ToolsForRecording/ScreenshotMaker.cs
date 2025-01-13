using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ScreenshotMaker : MonoBehaviour
{
    public bool AutoStart = false;
    public float TimeBetweenScreenshots = 0.5f;
    public string ScreenshotNamePrefix = "Screenshots/";
    
    public int _ScreenshotCount = 0;
    private bool _DirectoryCreated = false;
    private string _DirectoryName;

    private void Start()
    {
        if (AutoStart)
        {
            StartCoroutine(MakeScreenshotCoroutine());
        }
    }

    IEnumerator MakeScreenshotCoroutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(TimeBetweenScreenshots);
            MakeScreenshot();
        }
    }

    private void CreateDirectory()
    {
        _DirectoryCreated = true;
        _DirectoryName = DateTime.Now.ToString("H:mm:ss");
        _DirectoryName = _DirectoryName.Replace(":", "-");
        Directory.CreateDirectory(ScreenshotNamePrefix + _DirectoryName);
    }
    
    public void MakeScreenshot()
    {
        if (!_DirectoryCreated)
        {
            CreateDirectory();
        }
        string fileName = ScreenshotNamePrefix + _DirectoryName + $"/Screenshot_{_ScreenshotCount:0000}" + ".jpeg";
        ScreenCapture.CaptureScreenshot(fileName);
        print("Screenshot made: " + fileName);
        _ScreenshotCount++;
    }
    
    [MenuItem("Window/Make Screenshot")]
    public static void MakeScreenshotEditor()
    {
        string fileName = "Screenshots/" + $"/Screenshot_{DateTime.Now:H_mm_ss}" + ".jpeg";
        ScreenCapture.CaptureScreenshot(fileName);
        print("Screenshot made: " + fileName);
    }
}
