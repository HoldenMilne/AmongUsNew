using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DownloadUploadController : MonoBehaviour
{
    public bool download = true;

    public Canvas defaultCanvas;
    public Canvas evilCanvas;

    [Range(5f, 10f)] private float uploadTime = 8.7f;

    private bool evil = false;

    private Image runner_img;
    // Start is called before the first frame update
    void Start()
    {
        runner_img = runner.gameObject.GetComponent<Image>();
        if (!download)
        {
            var v = Random.Range(0, 1000);
            if (v == 666)
            {
                evil = true;
                
                var e = runner.eulerAngles;
                e.z = 180;
                runner.eulerAngles = e;
            }
        }

        if (ColorizeGhost.color != null) runner_img.color = (Color) ColorizeGhost.color;
        else
        {
            ColorizeGhost.SetColor();
            if (ColorizeGhost.color != null) runner_img.color = (Color) ColorizeGhost.color;
            else runner_img.color = ColorizeGhost.GetColor(0);
        }
    }

    public Text jobtext;
    public Text percText;
    private bool running = false;

    private float timer = 0;
    // Update is called once per frame
    void Update()
    {
        if (running)
        {
            timer += Time.deltaTime;

            if (timer >= uploadTime)
            {
                success = true;
                
                running = false;
                percText.text = "100 %";
                
                StartCoroutine(Success());
            }
            else
            {
                var percent = (int) ((timer / uploadTime) * 100f);
                percText.text = percent + "%";
            }

        }
    }

    private bool success = false;
    
    IEnumerator Success()
    {
        float time = Time.time;
        yield return new WaitUntil(IsDone);
        float t = Time.time - time;
        yield return new WaitForSeconds(Mathf.Max(0f, t));
        runner.gameObject.GetComponent<GifAnimator>().running = false;

        var gc = FindObjectOfType<GameController>();

        gc.reportCanvas.enabled = true;
    }

    private bool IsDone()
    {
        return done;
    }

    public void OnClick()
    {
        if (running || done || success) return;
        if (evil)
        {
            evilCanvas.enabled = true;
            defaultCanvas.enabled = false;
            evilCanvas.GetComponentInChildren<GifAnimator>().running = true;
        }
        running = true;
        timer = 0;
        StartCoroutine(UpdateText());
        StartCoroutine(Runner());
    }

    public RectTransform runner;
    public Image littlePage;

    public float runnerSpeed = 100;
    public RectTransform runnerStart;
    public RectTransform runnerEnd;

    private bool done = false;

    public Image lf1, lf2, rf1, rf2, pg1,pg2;
    IEnumerator Runner()
    {
        
        lf2.enabled = true;
        lf1.enabled = false;
        rf2.enabled = false;
        rf1.enabled = true;
        var rect = pg1.gameObject.GetComponent<RectTransform>();
        var ac = rect.anchoredPosition;
        ac.x = lf2.gameObject.GetComponent<RectTransform>().anchoredPosition.x;
        rect.anchoredPosition = ac;
        runner.anchoredPosition = runnerStart.anchoredPosition;
        runner.gameObject.GetComponent<Image>().enabled = true;
        
        littlePage.enabled = true;
        runner.gameObject.GetComponent<GifAnimator>().running = true;
        var pos = runner.anchoredPosition;
        while (true)
        {
            pos += Vector2.right * runnerSpeed * Time.deltaTime;
            runner.anchoredPosition = pos;
            if (pos.x > runnerEnd.anchoredPosition.x)
            {
                pos = runnerStart.anchoredPosition;

                if (evil)
                {
                
                    var e = runner.eulerAngles;
                    e.z = Random.Range(0,4)*90;
                    runner.eulerAngles = e;
                }
                runner.anchoredPosition = pos;
                if (!running)
                    break;
            }
            yield return null;
        }

        lf2.enabled = false;
        lf1.enabled = true;
        rf2.enabled = true;
        rf1.enabled = false;
        pg2.enabled = true;
        pg1.enabled = false;
        done = true;
    }

    IEnumerator UpdateText()
    {
        while (running)
        {
            jobtext.text = GetRandomFileName();
            var r1 = Random.Range(.5f, 2f);
            var r2 = Random.Range(.1f, r1);
            yield return new WaitForSeconds(r2);
            
        }

        jobtext.text = (download?"Download":"Upload")+" Complete";
    }

    public string[] words1 = new string[]
    {
        "dog","cat","red","pain","stock","beef","case","texture","sprite","love","gun",
        "weeb","port","IP","manifest","ferret","pow","101011","to","eat","pat","meat",
        "vegan","west","east","north","norbert","paul","wan","not_a_virus","true","real"
        ,"product","last","version","420","how_to","readme","fart","all","graph","everything",
        "architecture","not","porn"
    };
    public string[] words2 = new string[]
    {
        "dog","cat","red","pain","stock","beef","case","texture","sprite","love","gun",
        "weeb","port","IP","manifest","ferret","pow","101011","to","eat","pat","meat",
        "vegan","west","east","north","norbert","paul","wan","not_a_virus","true","real"
        ,"product","last","version","1","2","3","666","69","420","how_to","readme","fart",
        "SINs","accounts","1","2","3","666","69","erdős","is_sex","farts","controller","animator",
        "server","client","architecture","porn","collection"
    };
    
    public string[] separators = new string[]
    {
        "_","-"
    };
    
    public string[] extensions = new string[]
    {
        ".docx","",".xlsx",".csv",".png",".gay",".xcf",".dot",".ppt",".asm",".c",".class",".cpp",".cs",
        ".py",".3ds",".scm",".sh",".bat",".jpg",".jif",".tiff",".yiff",".gif",".jpeg",".mp4",".mp3",".wav",".ogg",
        ".mkv",".iso",".zip",".tar.gz",".simp",".cal",".rpm",".h"
    };
    
    private string GetRandomFileName()
    {
        var v = Random.Range(0, 2);
        var v1 = Random.Range(0, words1.Length);
        string s = words1[v1];
        if (v == 0)
        {
            var v2 = Random.Range(0, words2.Length);
            var v3 = Random.Range(0, separators.Length);
            s += separators[v3] + words2[v2];
        }

        var v4 = Random.Range(0,extensions.Length);
        s += extensions[v4];
        return s;
    }
}
