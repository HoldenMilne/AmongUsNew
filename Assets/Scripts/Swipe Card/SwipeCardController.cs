using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwipeCardController : MonoBehaviour
{
    [Range(.01f,5f)] public float initMoveSpeed=2f;
    [Range(.5f,5f)] public float timeDelayOnComplete=1.5f;
    private int state = 0;

    private bool pressed = false;

    private bool moving = false;

    private RectTransform rect;

    public RectTransform swipeStart;
    public RectTransform swipeEnd;
    public Vector2 timeNeededForSwipe=new Vector2(1,1.5f);

    public Text text;
    private float onTouchXDistance ;
    private GameController gc;
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        gc = FindObjectOfType<GameController>();
        if (gc == null)
        {
            gc = GameController.CreateGC();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (state == 2 && pressed)
        {
            var touch = gc.GetTouch();
            MoveToTouchPosition(touch);
            time += Time.deltaTime;
        }
    }

    private float time;
    public void onPress()
    {
        switch (state)
        {
            case 0:
                state = 1;   
                StartCoroutine(MoveToSwipePosition(initMoveSpeed));
                break;
            case 1:
                StopAllCoroutines();
                rect.anchoredPosition = swipeStart.anchoredPosition;
                state = 2;
                //onTouchXDistance = gc.GetTouch().x - rect.anchoredPosition.x;
                break;
            case 2:
      
                pressed = true;
                time = 0;
                break;
        }
    }
    
    public void onRelease()
    {
        if (state == 2)
        {
            pressed = false;
            StartCoroutine(MoveToSwipePosition(initMoveSpeed*2f));
        }
    }
    
    

    IEnumerator MoveToSwipePosition(float speed)
    {
        while (Vector2.Distance(rect.anchoredPosition, swipeStart.anchoredPosition) > rect.sizeDelta.x/15f)
        {
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, swipeStart.anchoredPosition, Time.deltaTime * speed);
            
            yield return null;
            
        }

        rect.anchoredPosition = swipeStart.anchoredPosition;     
        Debug.Log("STATE"+state);
        state = 2;

    }
    
    
    void MoveToTouchPosition(Vector2 touch)
    {
        var t = touch;
        
        Vector2 line = (swipeEnd.transform.position - swipeStart.transform.position).normalized;
        
        var dot = Vector3.Dot(line,touch-(Vector2)swipeStart.transform.position);
        
        // get the new point by taking the magnitude (dot) in the line direction.
        var newPoint = line * dot + (Vector2)swipeStart.transform.position;
        newPoint.x -= onTouchXDistance;
        if (true||Vector2.Distance(touch, newPoint) < rect.sizeDelta.x/2f)
        {
            rect.transform.position = newPoint;

            if (rect.transform.position.x > swipeEnd.transform.position.x && time>= timeNeededForSwipe.x  && time<=timeNeededForSwipe.y)
            {
                state = 3;
                rect.transform.position = swipeEnd.transform.position;
                StartCoroutine(Success());
                StartCoroutine(Blink());
                SetText("ACCEPTED. THANK YOU.");
            }else if (rect.transform.position.x > swipeEnd.transform.position.x)
            {
                onRelease();
                SetText(time < timeNeededForSwipe.x);
            }
        }
    }

    private void SetText(bool b)
    {
        string text = "TOO " + (b ? "FAST" : "SLOW") + ". TRY AGAIN.";
        SetText(text);
    }
    private void SetText(string s)
    {
        text.text = s;
    }

    IEnumerator Success()
    {
        yield return new WaitForSeconds(timeDelayOnComplete+.5f);
        gc.reportCanvas.enabled = true;
    }
    
    IEnumerator Blink()
    {
        var col = text.color;
        var initA = col.a;
        var a = col.a;
        var nblinks = 6;
        var on = true;
        var counter = 0;
        while (true)
        {
            if (on)
            {
                on = false;
                a = .25f;
            }
            else
            {
                on = true;
                a = initA;
                
            }

            col.a = a;
            text.color = col;
            yield return new WaitForSecondsRealtime(timeDelayOnComplete / (nblinks*2));
            counter += 1;
            if (counter == nblinks) break;

        }
        Debug.Log("COMPLETE");
        col.a = initA;
        text.color = col;
        yield return null;
    }
}
