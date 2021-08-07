using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StationController : MonoBehaviour
{
    // Start is called before the first frame update
    private GameController gc;
    public Text text;
    public GameObject ePanel;
    public Canvas ePanelCanvas;
    private bool running = false;
    void Start()
    {
        gc = FindObjectOfType<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
     * TODO:
     * RequestLocationName will no longer return anything in the future, or will always return true.
     * Maybe make this an error/catch?
     */
    public void SendStationCode()
    {
        if (gc != null)
        {
            if (!gc.RequestLocationName(text.text.ToUpper()))
            {
                StartCoroutine(ErrCoroutine());
            }
            
        }
    }

    IEnumerator ErrCoroutine()
    {
        if (running) yield break;
        running = true;
        ePanelCanvas.enabled  = true;
        yield return new WaitForSeconds(1);

        Text t = ePanel.GetComponentInChildren<Text>();
        Image i = ePanel.GetComponentInChildren<Image>();
        Color ic = i.color;
        Color tc = t.color;
        
        float a1 = ic.a;
        float a2 = tc.a;
        float a = 0;
        while (a1 > 0 || a2 >0)
        {
            a += Time.deltaTime/4f;
            a = Mathf.Min(1, a);
            a1 -= Time.deltaTime*a;
            a2 -= Time.deltaTime*a;
            a1 = Mathf.Max(a1, 0);
            a2 = Mathf.Max(a2, 0);
            var c1 = i.color;
            var c2 = t.color;

            c1.a = a1;
            c2.a = a2;
            i.color = c1;
            t.color = c2;
            yield return null;
        }

        yield return new WaitForSeconds(.25f);
        ePanelCanvas.enabled  = false;
        t.color = tc;
        i.color = ic;
        running = false;
    }
    
}
