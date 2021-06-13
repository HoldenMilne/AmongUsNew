using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFlipper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Close(Canvas canvas)
    {
        canvas.enabled = false;
    }
    
    public void Open(Canvas canvas)
    {
        canvas.enabled = true;
    }
}
