using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldPanel : MonoBehaviour
{
    public bool on;

    public Color onColor = Color.white;
    public Color offColor = Color.red;

    private Image img;

    public PrimeShieldsController shieldsController;
    void Awake()
    {
        img = GetComponent<Image>();
        //onColor = img.color;
        //if (offColor == null)
            //offColor = Color.black;
    }
    
    public void SetOn()
    {
        if (shieldsController.done) return;
        on = true;
        img.color = onColor;
        shieldsController.counter += 1;
    }
    
    public void SetOff()
    {
        if (shieldsController.done) return;
        on = false;
        img.color = offColor;
        shieldsController.counter -= 1;
        
    }
    
    public void Toggle()
    {
        if (shieldsController.done) return;
        Debug.Log("TOGGLE");
        if (on)
        {
            SetOff();
        }
        else
        {
            SetOn();
        }

        shieldsController.UpdateGauge();
    }
}
