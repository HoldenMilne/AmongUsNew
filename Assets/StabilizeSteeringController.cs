using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StabilizeSteeringController : MonoBehaviour
{
    public RectTransform bg;

    public RectTransform cursor;
    public Image l1,l2,l3,l4;
    // Start is called before the first frame update
    private GameController gc;
    private float dif;
    void Start()
    {
        Rect rect = bg.rect;

        float x, y;
        dif = rect.xMax - rect.xMin;
        var dify = rect.yMax - rect.yMin;
        do
        {
            x = Random.Range(rect.xMin+dif*.02f, rect.xMax-dif*.02f);
            y = Random.Range(rect.yMin+dify*.02f, rect.yMax-dify*.02f);
        } while ((x <= dif * 0.02 && x>= -dif * 0.02&& y <=dif * 0.02 && y>=-dif * 0.02) || (Vector2.Distance(Vector2.zero, new Vector2(x,y))>dif*.81/2f));

        this.cursor.anchoredPosition =  new Vector2(x,y);

        gc = FindObjectOfType<GameController>();
        if(gc==null)gc = GameController.CreateGC();
    }

    IEnumerator placement()
    {
        Rect rect = bg.rect;

        float x, y;
        var dify = rect.yMax - rect.yMin;
        for (int i = 0; i < 100; i++)
        {
            do
            {
                x = Random.Range(rect.xMin+dif*.02f, rect.xMax-dif*.02f);
                y = Random.Range(rect.yMin+dify*.02f, rect.yMax-dify*.02f);
            } while ((x <= dif * 0.02 && x>= -dif * 0.02&& y <=dif * 0.02 && y>=-dif * 0.02) || (Vector2.Distance(Vector2.zero, new Vector2(x,y))>dif*.81/2f));

            this.cursor.anchoredPosition =  new Vector2(x,y);
            yield return new WaitForSeconds(.5f);
        }
    }
    
    public bool pressed,done;

    // Update is called once per frame
    void Update()
    {
        if (pressed&&!done)
        {
            var touch = gc.GetTouch(true);
            this.cursor.position =  touch;

        }else if (!pressed && !done)
        {
            if (Vector2.Distance(cursor.anchoredPosition, Vector2.zero) <= dif * 0.02)
            {
                cursor.anchoredPosition = Vector2.zero;
                done = true;
                StartCoroutine(Success());
            }
        }
    }

    public void OnClick()
    {
        if(!done)
            pressed = true;
        
    }
    public void OnRelease()
    {
        if(!done)
            pressed = false;
        
    }

    private bool blinking = false;

    bool NotBlinking()
    {
        return !blinking;
    }

    public float blinkDuration = 1f;
    public int nBlinks = 6;

    public Color onColor = new Color(.9f,.7f,.3f);
    public Color offColor = Color.white;
    IEnumerator Blink()
    {
        bool on = false;
        int i = 0;
        var img = cursor.GetComponent<Image>();
        var img1 = l1.GetComponent<Image>();
        var img2 = l2.GetComponent<Image>();
        var img3 = l3.GetComponent<Image>();
        var img4 = l4.GetComponent<Image>();
        while (++i < nBlinks*2)
        {
            if (on)
            {
                on = false;
                img.color = offColor;
                img1.color = offColor;
                img2.color = offColor;
                img3.color = offColor;
                img4.color = offColor;
            }
            else
            {
                on = true;
                img.color = onColor;
                img1.color = onColor;
                img2.color = onColor;
                img3.color = onColor;
                img4.color = onColor;
            }
            yield return new WaitForSeconds(blinkDuration/(nBlinks*2f));
        }

        blinking = false;
    }
    IEnumerator Success()
    {
        StartCoroutine(Blink());
        blinking = true;
        yield return new WaitUntil(NotBlinking);
        yield return new WaitForSeconds(1f);
        gc.reportCanvas.enabled = true;
    }
}
