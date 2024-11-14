using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScreenshotMaker : MonoBehaviour
{
    public float TimeBetweenScreenshots = 0.5f;
    public string ScreenshotNamePrefix = "Screenshots/";
    
    private int _ScreenshotCount = 0;

    private void Start()
    {
        StartCoroutine(MakeScreenshot());
    }

    IEnumerator MakeScreenshot()
    {
        string dateStr = DateTime.Now.ToString("H:mm");
        dateStr = dateStr.Replace(":", "-");

        Directory.CreateDirectory(ScreenshotNamePrefix + dateStr);

        while(true)
        {
            yield return new WaitForSeconds(TimeBetweenScreenshots);

            string fileName = ScreenshotNamePrefix + dateStr + $"/Screenshot_{_ScreenshotCount:0000}" + ".jpeg";
            ScreenCapture.CaptureScreenshot(fileName);
            _ScreenshotCount++;
        }
    }
    
}
