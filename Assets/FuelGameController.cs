using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelGameController : MonoBehaviour
{
    public RectTransform fill;
    public AltFiller altFill;

    

    public float duration = 5f;

    public Image redLight, greenLight;
    private Vector2 startEnd;

    public RectTransform min;
    public RectTransform max;

    // Start is called before the first frame update
    void Start()
    {
        startEnd = new Vector2(min.anchoredPosition.y,max.anchoredPosition.y);
        
    }

    private bool pressed;

    private float progress = 0f;
    // Update is called once per frame
    void Update()
    {
        if (pressed && progress <1f)
        {
            progress += Time.deltaTime / duration;
            var pos = fill.anchoredPosition;
            pos.y = (startEnd.y - startEnd.x) * progress + startEnd.x;
            fill.anchoredPosition = pos;
            if (altFill != null)
            {
                altFill.progress = progress;
                
            }
            if (progress >= 1f)
            {
                StartCoroutine(Success());
            }
        }
    }

    public void onClick()
    {
        if (progress < 1f)
        {
            TurnOnRedLight();
            pressed = true;
            if (altFill != null)
            {
                altFill.pressed = true;
                
            }
        }
    }

    public void onRelease()
    {
        if (progress < 1f)
        {
            TurnOffRedLight();
            pressed = false;
            
            if (altFill != null)
            {
                altFill.pressed = false;
                
            }
        }
    }
    
    private void TurnOffRedLight()
    {
        redLight.color = Color.white;
    }

    private void TurnOnRedLight()
    {
        redLight.color = Color.red;
    }

    private void TurnOnGreenLight()
    {
        greenLight.color = Color.green;
    }

    IEnumerator Success()
    {
        TurnOffRedLight();
        TurnOnGreenLight();
        yield return new WaitForSeconds(1f);
        GetComponent<GameController>().reportCanvas.enabled = true;
    }
}
