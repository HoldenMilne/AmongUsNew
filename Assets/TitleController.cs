using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleController : MonoBehaviour
{

    public float speed;

    public float min;

    public float max;

    private float alpha = 0f;

    private Image img;

    private Color col;
    bool fade_out = true; // start at full alpha
    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>();
        alpha = img.color.a;
        col = img.color;
        if (min >= max)
        {
            if (min <= 0)
            {
                max = 1f;
            }
            else
            {
                min = 0f;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (fade_out)
        {
            alpha -= Time.deltaTime * speed;
        }
        else
        {
            alpha += Time.deltaTime * speed;
        }

        if (alpha <= min)
        {
            fade_out = false;
            alpha = min;
        }else if (alpha >= max)
        {
            fade_out = true;
            alpha = max;
        }

        col.a = alpha;
        img.color = col;
    }
}
