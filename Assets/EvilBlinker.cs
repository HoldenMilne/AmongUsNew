using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvilBlinker : MonoBehaviour
{
    private Image img;

    private bool running = false;

    private bool allow_blinky = false;

    public EvilBlinker lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Run(Image getComponent)
    {
        img = getComponent;
        running = true;
        StartCoroutine(Blinker());
        if (lineRenderer == null)
        {
            StartCoroutine(RandomLines());
        }
        else
        {
            lineRenderer.running = true;
            lineRenderer.Run(null);
            lineRenderer.enabled = true;
        }
    }

    IEnumerator RandomLines()
    {
        
        LineRenderer lr = GetComponent<LineRenderer>();
        var pos = new Vector3[lr.positionCount];
        for (var i = 0; i < lr.positionCount; i++)
        {
            pos[i] = lr.GetPosition(i);
        }
        RectTransform rect = GetComponent<RectTransform>();
        lr.enabled = true;
        while(running)
        {
            lr.SetPosition(Random.Range(0,pos.Length),GetPosition());
            lr.SetPosition(Random.Range(0,pos.Length),GetPosition());
            lr.SetPosition(Random.Range(0,pos.Length),GetPosition());
            lr.SetPosition(Random.Range(0,pos.Length),GetPosition());
            lr.SetPosition(Random.Range(0,pos.Length),GetPosition());
            lr.SetPosition(Random.Range(0,pos.Length),GetPosition());
            yield return new WaitForSeconds(Random.Range(.025f,.05f));
        }

        lr.enabled = false;
    }

    private Vector3 GetPosition()
    {
        var w  = Screen.width;
        var h  = Screen.height;
        var p1 = Camera.main.ScreenToWorldPoint(new Vector3(w/4f,h/8f));
        var p2 = Camera.main.ScreenToWorldPoint(new Vector3(w/4f*3f,h/2f,0));
        return new Vector3(Random.Range(p1.x,p2.x),Random.Range(p1.y,p2.y),10);
    }

    IEnumerator Blinker()
    {
        var col = img.color;
        
        var A = col.a;
        var a = col.a;
        while (allow_blinky&&running)
        {
            var r = Random.Range(0, 2);
            if (r == 0 && a>0)
            {
                a = 0;
            }
            else
            {
                a = A;
            }

            col.a = a;
            img.color = col;
            
            yield return new WaitForSeconds(Random.Range(.025f,.05f));
        }
    }
}
