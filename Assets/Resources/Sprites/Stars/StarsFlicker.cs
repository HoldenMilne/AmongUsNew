using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class StarsFlicker : MonoBehaviour
{
    [Range(0f, 30f)] public float minRateSeconds;
    [Range(1f, 50f)] public float maxRateSeconds;

    private float B = 0f;

    private float _b = 0;
    private Image image;
    // Start is called before the first frame update
    void Start()
    {
        ResetB();
        _b = B;
        image = GetComponent<Image>();
    }

    private bool up = true;
    // Update is called once per frame
    void Update()
    {
        if (up)
        {
            _b = Mathf.Lerp(_b, 0.85f, Time.deltaTime*B);
            if (_b >= 0.80f)
            {
                up = false;
            }
        }
        else
        {
            _b = Mathf.Lerp(.25f, _b, 1-Time.deltaTime*B);
            if (_b <= .30f)
            {
                up = true;
                ResetB();
            }
        }
        var y = .3f * Mathf.Sin(_b*Time.time)+.5f;
        var c = image.color;
        c.a = _b;
        image.color = c;

    }

    private void ResetB()
    {
        B = 2 * Mathf.PI / Random.Range(minRateSeconds, maxRateSeconds);
    }
}
