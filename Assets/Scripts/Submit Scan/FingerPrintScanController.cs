using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FingerPrintScanController : MonoBehaviour
{
    private float progress = 0f;

    public RectTransform circle;
    public RectTransform cover;

    public float duration = 5f;
    private bool pressed = false;

    private Vector2 startEnd = new Vector2();

    public Color scan_color;
    public Color err_color;
    
    public Text scanning;
    
    // Start is called before the first frame update
    void Start()
    {
        startEnd.x = cover.anchoredPosition.y;
        startEnd.y = cover.anchoredPosition.y + circle.sizeDelta.y;
        //circle.gameObject.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
    }

    void Update()
    {
        if (pressed && progress <1f)
        {
            progress += Time.deltaTime / duration;
            var pos = cover.anchoredPosition;
            pos.y = (startEnd.y - startEnd.x) * progress + startEnd.x;
            cover.anchoredPosition = pos;
            if (progress >= 1f)
            {
                StartCoroutine(Success());
            }
        }
    }
    
    IEnumerator Success()
    {
        
        scanning.text = "[ SCAN COMPLETE ]";
        scanning.color = scan_color;
        
        yield return new WaitForSeconds(1f);

        var gc = FindObjectOfType<GameController>();

        gc.reportCanvas.enabled = true;
    }

    
    public void onPress ()
    {
        pressed = true;
        /*
        var t = Camera.main.WorldToScreenPoint(GetComponent<GameController>().GetTouch(true));
        var rect = gameObject.GetComponent<RectTransform>();
        var t2 = Camera.main.WorldToScreenPoint(rect.position);
        var rad = rect.sizeDelta.x/2f;*/
        
        // Set Scanning text and start blinker
        if (progress < 1f)
        {
            scanning.color = scan_color;
            scanning.text = "[ SCANNING... ]";
            StartCoroutine(Blinking());
        }
    }

    IEnumerator Blinking()
    {
        var a = scanning.color.a;
        var up = false;
        while (progress < 1f && pressed)
        {

            if (a <= .25f)
            {
                up = true;
            }else if (a >= 1f)
            {
                up = false;
            }
            if (up)
            {
                a += Time.deltaTime;
            }
            else
            {
                a -= Time.deltaTime;
            }
            if (a <= .25f)
            {
                up = true;
                a = .25f;
            }else if (a >= 1f)
            {
                up = false;
                a = 1f;
            }

            var col = scanning.color;
                col.a = a;
            scanning.color = col;
            yield return null;
        }
    }

    public void onRelease ()
    {
        pressed = false;
        if (progress < 1f)
        {
            scanning.text = "[ SCAN INCOMPLETE ]";
            scanning.color = err_color;
        }
    }
}
