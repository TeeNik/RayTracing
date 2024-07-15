using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotMaker : MonoBehaviour
{
    public float TimeBetweenScreenshots = 0.5f;
    public string ScreenshotNamePrefix = "Temp/Screenshots/Screenshot_";
    
    private int _ScreenshotCount = 0;

    private void Start()
    {
        StartCoroutine(MakeScreenshot());
    }

    IEnumerator MakeScreenshot()
    {
        while(true)
        {
            yield return new WaitForSeconds(TimeBetweenScreenshots);

            string fileName = ScreenshotNamePrefix + $"{_ScreenshotCount:0000}" + ".jpeg";
            ScreenCapture.CaptureScreenshot(fileName);
            _ScreenshotCount++;
        }
    }
    
}
