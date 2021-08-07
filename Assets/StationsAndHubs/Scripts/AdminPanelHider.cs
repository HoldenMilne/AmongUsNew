using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdminPanelHider : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    // Start is called before the first frame update
    void Start()
    {
        // grab the correct object maybe?
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowHideCover(bool hide)
    {
        StopAllCoroutines();
        if (hide)
        {
            StartCoroutine(HideCover());
        }
        else
        {
            StartCoroutine(ShowCover());
        }
    }

    private IEnumerator HideCover()
    {
        var a = canvasGroup.alpha;
        while (a > 0f)
        {
            canvasGroup.alpha = a;
            a -= Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }
    private IEnumerator ShowCover()
    {
        var a = canvasGroup.alpha;
        while (a < 1f)
        {
            canvasGroup.alpha = a;
            a += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }
}
