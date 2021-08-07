using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class UnlockManifoldsController : MonoBehaviour
{
    public GameObject buttonsParent;

    public Image[] buttons;
    private int[] indices;

    public Color pressedColor;

    
    public Sprite[] orderedSprites;
    public Color defaultColor;

    private bool[] taken;
    // Start is called before the first frame update
    void Start()
    {
        List<Image> images = new List<Image>();
        foreach (var b in buttonsParent.GetComponentsInChildren<Image>())
        {
            break;
            images.Add(b);
        }

        //buttons = new Image[images.Count];
        indices= new int[buttons.Length];
        taken = new bool[buttons.Length];
        for (int i = 0; i < buttons.Length; i++)
        {
            int r;
            do
            {
                r = Random.Range(0, buttons.Length);
            } while (taken[r]);

            taken[r] = true;// sprites that have been used
            indices[i] = r+1;
            buttons[i].sprite = GetSprite(r);
        }
    }

    private Sprite GetSprite(int i)
    {
        return orderedSprites[i];
    }

    private int current = 0;

    private bool done = false;
    public void OnPress(int next)
    {
        if (done) return;
        var index = indices[next];
        if (index == current + 1)
        {
            current = index;
            buttons[next].color = pressedColor;
            if (index >= 10)
            {
                done = true;
                StartCoroutine(Success());
                StartCoroutine(BeepThoseBoys());
            }
        }
        else
        {
            Reset();
        }
    }
    
    // maybe I will.  Maybe I won't

    IEnumerator BeepThoseBoys()
    {
        yield break;
    }

    void Reset()
    {
        current = 0;
        foreach (var b in buttons)
        {
            b.color = defaultColor;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Success()
    {
        yield return new WaitForSeconds(1f);

        Debug.Log("Success");
        FindObjectOfType<GameController>().Success();

    }
}
