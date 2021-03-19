using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrimeShieldsController : MonoBehaviour
{

    public int counter = 7;

    public ShieldPanel[] shields;

    public bool done;

    public Image gauge;

    // Start is called before the first frame update
    void Start()
    {
        if (shields == null)
        {
            shields = FindObjectsOfType<ShieldPanel>();
        }
        var v = Random.Range(2, 5);
        for (int i = 0; i < v; i++)
        {
            var r = 0;
            do
            {
                r = Random.Range(0, 7);

            } while (!shields[r].on);

            shields[r].SetOff();
        }
        UpdateGauge();
    }

    // Update is called once per frame
    void Update()
    {
        if (counter == 7)
        {
            done = true;
            StartCoroutine(Success());
        }
    }

    IEnumerator Success()
    {
        yield return new WaitForSeconds(1f);
        FindObjectOfType<GameController>().reportCanvas.enabled = true;
    }

    public void UpdateGauge()
    {
        var col = gauge.color;
        col.g = counter / 7f;
        col.b = counter / 7f;
        gauge.color = col;
    }
}
