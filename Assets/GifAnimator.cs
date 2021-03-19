using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GifAnimator : MonoBehaviour
{
    public bool running;
    public float timeDelay = 0.1f;

    public Sprite[] sprites;
    public Sprite[] alt_sprites;

    private bool coRouteOn = false;

    private Image img;
    public Image alt_img;

    public bool meshWithAlt = true;
    private bool MeshWithAlt = true;
    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>();
        MeshWithAlt = meshWithAlt;
    }

    // Update is called once per frame
    void Update()
    {
        if (running && !coRouteOn)
        {
            
            StartCoroutine(Run());
        }
    }

    private int sprIndex = 0;
    private int altSprIndex = 0;
    IEnumerator Run()
    {
        coRouteOn = true;
        var eb = GetComponent<EvilBlinker>();
           if(eb!=null)    eb.Run(GetComponent<Image>());
        while (running)
        {
            img.sprite = sprites[sprIndex];
            
            if (alt_img != null)
            {
                if (!alt_img.enabled) alt_img.enabled = true;
                if (MeshWithAlt)
                {
                    alt_img.sprite = alt_sprites[sprIndex];
                }
                else
                {
                    alt_img.sprite = alt_sprites[altSprIndex];
                    altSprIndex += 1;
                    altSprIndex = altSprIndex % alt_sprites.Length;
                }

            }
            sprIndex += 1;
            sprIndex = sprIndex % sprites.Length;

            yield return new WaitForSeconds(timeDelay);
        }

        coRouteOn = false;
    }
}
