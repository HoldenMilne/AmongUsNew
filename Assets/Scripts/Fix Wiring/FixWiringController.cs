using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = Unity.Mathematics.Random;

public class FixWiringController : MonoBehaviour
{
    public GameObject[] startPoints = new GameObject[4];
    public GameObject[] existingWiresLeft = new GameObject[4];
    public GameObject[] existingWiresRight = new GameObject[4];
    public GameObject[] endPoints = new GameObject[4];

    private int[] targets = new int[4];

    private Vector2[] initials = new Vector2[4];
    private Vector2[] initialsParent = new Vector2[4];
    // Start is called before the first frame update
    private bool testing = false;

    private GameController gc;
    void Start()
    {
        gc = FindObjectOfType<GameController>();
        if(gc==null)gc = GameController.CreateGC();
        SetInits();
        SetTargets();
    }

    private void SetInits()
    {
        var i = 0;
        foreach (var g in startPoints)
        {
            initials[i] = g.transform.parent.position;
            i++;
        }
    }

    private void SetTargets()
    {
        int[] taken = new int[4];
        taken[0] = -1;
        taken[1] = -1;
        taken[2] = -1;
        taken[3] = -1;
        for (int i = 0; i < 3; i++)
        {
            var b = true;
            var col = startPoints[i].GetComponent<Image>().color;
            existingWiresLeft[i].GetComponent<Image>().color = col;
            var x = 0;
            while (b)
            {
                var r = new System.Random().Next(0, 4);
                if (taken[r] == -1)
                {
                    targets[i] = r;
                    taken[r] = 1;
                    endPoints[r].GetComponent<Image>().color = col;
                    existingWiresRight[r].GetComponent<Image>().color = col;
                    b = false;
                }
            }
        }

        // 4 gets the left over
        for (int i = 0; i < 4; i++)
        {
            if (taken[i] == -1)
            {
                var col = startPoints[3].GetComponent<Image>().color;
                existingWiresLeft[3].GetComponent<Image>().color = col;
                existingWiresRight[i].GetComponent<Image>().color = col;
                targets[3] = i;
                endPoints[i].GetComponent<Image>().color = col;
                break;
            }
            
        }
    }

    private bool pressing = false;

    private int current = -1;
    Vector2 touch;
    // Update is called once per frame
    void Update()
    {
        if (pressing && current!=-1)
        {
            var lr = startPoints[current].GetComponent<LineRenderer>();
            lr.positionCount = 3;

            touch = gc.GetTouch();

            var rect = startPoints[current].transform.parent;
            rect.position = touch;
            var t = existingWiresLeft[current].GetComponent<RectTransform>();
            var p1 = initials[current] - (Vector2) (rect.position - startPoints[current].transform.position);
            var p0 = t.position;
            p0.z = 0;
            
            var col = startPoints[current].GetComponent<Image>().color;
            lr.SetPosition(0,p0);
            lr.SetPosition(1,p1);
            lr.SetPosition(2,startPoints[current].transform.position);
            lr.startColor = col;
            lr.endColor = col;
            lr.enabled = true;
        }
    }

    private Vector2 GetTouch()
    {
        if (!testing)
            return GetTouchPosition();
        return GetMousePosition();
    }

    public void onPress(int i)
    {
        if(!successful[i]){
            pressing = true;
            current = i;
        }
    }
    
    private bool[] successful = new bool[]
    {
        false,false,false,false
    };

    private int successCounter = 0;
    public void onRelease(int i)
    {
        if (current == -1) return;
        pressing = false;
        if (CorrectLocation())
        {
            successful[current] = true;
            successCounter += 1;
            if (successCounter == 4)
            {
                StartCoroutine(Success());
            }
        }
        else
        {
            var rect = startPoints[current].transform.parent;
            rect.position = initials[current];
            startPoints[current].GetComponent<LineRenderer>().enabled = false;
        }
        current = -1;
    }

    private bool CorrectLocation()
    {
        if (touch != nul)
        {
            if (Vector2.Distance(endPoints[targets[current]].transform.parent.position, (Vector2) startPoints[current].transform.parent.position) < GetDistance())
            {
                
                return true;
            }
        }

        return false;
    }

    public CanvasScaler scaler;
    private double GetDistance()
    {
        var scale = scaler.scaleFactor;

        var y = endPoints[targets[current]].transform.parent.GetComponent<RectTransform>().sizeDelta.x * scale;
        y = (Camera.main.ScreenToViewportPoint(Vector2.up * y+endPoints[targets[current]].transform.parent.GetComponent<RectTransform>().anchoredPosition)-Camera.main.ScreenToViewportPoint(endPoints[targets[current]].transform.parent.GetComponent<RectTransform>().anchoredPosition)).magnitude;
        y *= 2f;
        Debug.Log(y);
        return y;
    }

    Vector2 nul = new Vector2(-1,-1);
    private Vector2 GetTouchPosition()
    {
        
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            var p = touch.position;
            p.x -= Screen.width/2f;
            p.y -= Screen.height/2f;
            Debug.Log(p);
            return p;

        }

        return nul;
    }
    
    private Vector2 GetMousePosition()
    {
        
        if (Input.GetMouseButton(0))
        {
            Vector2 p = Input.mousePosition;
            //p.x -= Screen.width/2f;
            //p.y -= Screen.height/2f;
           // p.x -= Screen.width/2f;
           // p.y -= Screen.height/2f;
            p = Camera.main.ScreenToWorldPoint(p);
            return p;

        }

        return nul;
    }
    
    

    IEnumerator Success()
    {
        yield return new WaitForSeconds(1f);

        Debug.Log("MADE IT");
        var gc = FindObjectOfType<GameController>();

        gc.reportCanvas.enabled = true;
    }
}
