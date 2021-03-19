
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartReactorController : MonoBehaviour
{
    
    /*    0    1    2
     *    3    4    5
     *    6    7    8
    */

    public Image[] images = new Image[9];
    public Image[] buttons = new Image[9];
    public Image[] ss_lamps = new Image[5];
    public Image[] player_lamps = new Image[5];
    private int[] order = new int[5];

    private int level = 1;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < order.Length; i++)
        {
            order[i] = Random.Range(0, 9);
        }

        StartCoroutine(Saying());
    }


    public Color showColor;
    public Color errColor;
    private bool simonSaying = false;

    IEnumerator Saying()
    {
        simonSaying = true;

        SetLevelLamps();
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < level; i++)
        {
            Show(order[i]);
            yield return new WaitForSecondsRealtime(.5f);
            Hide(order[i]);
            yield return new WaitForSecondsRealtime(.2f);
        }

        simonSaying = false;
        AlertToStart();
        
    }

    private void SetLevelLamps()
    {
        for (int l = 0; l < level; l++)
        {
            ss_lamps[l].enabled = true;
            
        }
    }

    private void AlertToStart()
    {
        
    }

    private void Show(int i)
    {
        images[i].color = showColor;
        
    }
    private void Hide(int i)
    {
        images[i].color = Color.black;
        
    }
    
    IEnumerator HideButton(int i)
    {
        yield return new WaitForSeconds(.35f);
        buttons[i].color = Color.white;
        
    }

    private int answerLevel = 0;
    private bool done;
    public void OnClick(int i)
    {
        if (simonSaying || done) return;
        // number is button pressed
        if (order[answerLevel] == i)
        {
            player_lamps[answerLevel].enabled = true;
            answerLevel += 1;
            buttons[i].color = showColor;
            StartCoroutine(HideButton(i));
            
            if (answerLevel == level)
            {
                if (level == order.Length)
                {
                    // success!
                    StartCoroutine(Success());
                    
                    done = true;
                }
                else
                {
                    level += 1;
                    answerLevel = 0;
                    SetLevelLamps();
                    for (int t = 0; t < ss_lamps.Length; t++)
                    {
                        player_lamps[t].enabled = false;
                    }

                    StartCoroutine(Saying());
                }
            }
        }
        else
        {
            foreach(var I in buttons)
            {
                I.color = Color.black;
                answerLevel = 0;
            }


            StartCoroutine(ErrorColors());
            
            StartCoroutine(Saying());
        }
    }

    IEnumerator ErrorColors()
    {
        foreach (var button in buttons)
        {
            button.color = errColor;
        }

        Color col = Color.white;

        foreach (var lamp in player_lamps)
        {
            col = lamp.color;
            lamp.color = errColor;
        }
        for (int t = 0; t < player_lamps.Length; t++)
        {
            player_lamps[t].enabled = true;
        }
        
        yield return new WaitForSeconds(.5f);
        
        foreach (var button in buttons)
        {
            button.color = errColor;
        }

        
        for (int t = 0; t < player_lamps.Length; t++)
        {
            player_lamps[t].enabled = false;
        }
        
        foreach (var lamp in player_lamps)
        {
            lamp.color = col;
        }
        foreach (var button in buttons)
        {
            button.color = Color.white;
        }
    }

    IEnumerator Success()
    {
        StartCoroutine(Blinking());
        yield return new WaitUntil(NotBlinking);
        yield return new WaitForSeconds(1f);
        FindObjectOfType<GameController>().reportCanvas.enabled=true;
    }

    private bool blinking;

    bool NotBlinking()
    {
        return !blinking;
    }
    
    IEnumerator Blinking()
    {
        blinking = true;
        int nBlinks = 6;
        float dur = 1.5f;
        int i = 0;
        bool up = false;
        Color upColor = Color.green;
        Color downColor = Color.white;
        
        for (int t = 0; t < player_lamps.Length; t++)
        {
            player_lamps[t].enabled = true;
        }
        
        while (i++ < nBlinks*2)
        {
            var col = upColor;
            if (up)
            {
                up = false;
                col = downColor;
            }
            else
            {
                up = true;
                
            }
            
            foreach (var button in buttons)
            {
                button.color = col;
            }

            foreach (var lamp in player_lamps)
            {
                lamp.color = col;
            }
            yield return new WaitForSeconds(dur / (nBlinks*2f));
        }

        blinking = false;
    }
}
