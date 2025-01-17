using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TickController : MonoBehaviour
{
    public int FPS = 60;
    public int FramesBetweemTicks = 10000;
    public int FirstFrame = 0;
    public bool TickEveryFrame = false;
    public bool MakeScreenshot = true;
    public ScreenshotMaker ScreenshotMaker;
    
    private List<ITickable> _tickableObjects;
    private int _ticks = 1;
    private float _deltaTime = 0.0f;
    
    void Start()
    {
        _deltaTime = 1f / FPS;
        _tickableObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<ITickable>().Where(o => ((MonoBehaviour)o).enabled).ToList();
        
        foreach (var tickableObject in _tickableObjects)
        {
            tickableObject.Tick(_deltaTime * FirstFrame);
        }
    }

    void Update()
    {
        if (TickEveryFrame || Time.frameCount >= FramesBetweemTicks * _ticks)
        {
            bool debug = (_ticks == 1 || _ticks == 841) && !MakeScreenshot;
            if (debug)
            {
                ScreenshotMaker?.MakeScreenshot();
            }
            
            ++_ticks;
            if (MakeScreenshot)
            {
                ScreenshotMaker?.MakeScreenshot();
            }
            foreach (var tickableObject in _tickableObjects)
            {
                tickableObject.Tick(_deltaTime, debug);
            }
        }
    }
}
