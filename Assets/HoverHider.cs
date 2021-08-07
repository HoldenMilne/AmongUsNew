using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class HoverHider : MonoBehaviour
{
    public float inSpeed = 2f;
    public float outSpeed = 1f;
    public Canvas toShowAndHide;

    private bool hasClickedOnce;

    private CanvasGroup cg;

    // Start is called before the first frame update
    void Start()
    {
        cg = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Toggle()
    {
        hasClickedOnce = true;
        toShowAndHide.enabled = !toShowAndHide.enabled;
    }

    public void OnHover()
    {
        if (hasClickedOnce)
        {
            StopAllCoroutines();
            StartCoroutine(FadeIn());
        }
    }
    
    public void OnHoverLost()
    {
        if (!toShowAndHide.enabled && hasClickedOnce)
        {
            StopAllCoroutines();
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeIn()
    {
        var a = cg.alpha;
        while (a<1f)
        {

            a += Time.deltaTime * inSpeed;
            cg.alpha = a;

            yield return null;


        }

        cg.alpha = 1;
    }
    
    private IEnumerator FadeOut()
    {
        var a = cg.alpha;
        while (a>0f)
        {

            a -= Time.deltaTime * outSpeed;
            cg.alpha = a;

            yield return null;


        }

        cg.alpha = 0;
    }
}
