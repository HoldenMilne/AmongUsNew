using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class AltFiller : MonoBehaviour
{
    public float progress;
    public bool pressed;

    private RectTransform rect;

    private float initSize;
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        initSize = rect.sizeDelta.y;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (pressed && progress < 1f)
        {
            var s = rect.sizeDelta;
            s.y = initSize * (1f - progress);
            rect.sizeDelta = s;
        }
        else if(progress>=1f)
        {
            rect.sizeDelta = Vector2.zero;
        }
    }
}
