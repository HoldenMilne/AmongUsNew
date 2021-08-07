using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace.Scripts;
using Mirror;
using StationsAndHubs.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class DeathPanelController : MonoBehaviour
{

    public Button button;
    
    [Range(.1f, 15f)] public float speed; // TODO: test on build
    private bool isOpen = false;
    private bool opening = false;

    private Vector2 closedPosition;
    private Vector2 openPosition;

    private RectTransform rect;

    public Canvas deadCanvas;
    public CanvasGroup youAreDeadPanel;
    private float width;

    public static bool iamDead = false;

    public Text timerText;
    // Start is called before the first frame update
    void Start()
    {
        isOpen = false;
        rect = this.GetComponent<RectTransform>();
        closedPosition = rect.anchoredPosition;
        openPosition = rect.anchoredPosition;
        openPosition.x -= rect.sizeDelta.x-button.GetComponent<RectTransform>().sizeDelta.x*1.25f;
        if (iamDead)
        {
            IamDead();
            StartCoroutine(TaskCooldown());
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private PlayerRoleCanvas prc;
    public void RevealRole()
    {
        if (prc == null)
            prc = FindObjectOfType<PlayerRoleCanvas>();
        Debug.Log(prc);
        if (prc == null) return;
        StartCoroutine(prc.Show());

    }

    public void ChangePanelState()
    {
        if (isOpen)
        {
            StartCoroutine(ClosePanel());
        }
        else
        {
            StartCoroutine(OpenPanel());
        }
    }

    IEnumerator OpenPanel()
    {
        StopCoroutine(ClosePanel());
        opening = true;

        float a = 0;
        while (opening && rect.anchoredPosition.x > openPosition.x)
        {
            var anc = rect.anchoredPosition;
            var x = anc.x;
            x -= Time.deltaTime * a * speed*70;
            x = Math.Max(openPosition.x, x);
            anc.x = x;
            rect.anchoredPosition = anc;
            a += Time.deltaTime;
            a = Mathf.Min(a, 1);
            yield return null;
        }
        
        if(opening)
            rect.anchoredPosition = openPosition;
        isOpen = true;
        StartCoroutine(RotateButton(true));
    }
    
    IEnumerator ClosePanel()
    {
        StopCoroutine(OpenPanel());
        opening = false;

        float a = 0;
        while (!opening && rect.anchoredPosition.x < closedPosition.x)
        {
            var anc = rect.anchoredPosition;
            var x = anc.x;
            x += Time.deltaTime * a * speed*70;
            x = Math.Min(closedPosition.x, x);
            anc.x = x;
            rect.anchoredPosition = anc;
            a += Time.deltaTime;
            a = Mathf.Min(a, 1);
            yield return null;
        }

        if(!opening)
            rect.anchoredPosition = closedPosition;

        isOpen = false;

        StartCoroutine(RotateButton(false));
    }

    IEnumerator RotateButton(bool toClosePosition)
    {
        RectTransform rect = button.GetComponent<RectTransform>();
        
        var eang = rect.eulerAngles;
        float z = eang.z;
        
        if(toClosePosition)
            while (rect.eulerAngles.z>90f)
            {
                z -= 720 * Time.deltaTime;
                z = Mathf.Max(90, z);
                eang.z = z;
                rect.rotation = Quaternion.Euler(eang);
                yield return null;
            }
        else
            while (rect.eulerAngles.z<270f)
            {
                z += 720 * Time.deltaTime;
                eang.z = z;
                z = Mathf.Min(270, z);
                rect.rotation = Quaternion.Euler(eang);
                yield return null;
            }
    }

    private GameController gc;

    public void IamDead()
    {
        Debug.Log("I died :(");
        if (gc == null)
        {
            gc = FindObjectOfType<GameController>();
        }

        if (gc.ghostsUseStationCode)
        {
            youAreDeadPanel.alpha = 0f;
            youAreDeadPanel.interactable=false;
            youAreDeadPanel.blocksRaycasts = false;
        }else{
            youAreDeadPanel.alpha = 1f;
            youAreDeadPanel.interactable=true;
            youAreDeadPanel.blocksRaycasts = true;
            
        }
        deadCanvas.enabled = true;
        
        if (iamDead) return;
        iamDead = true;
        timer = cooldown;
        StartCoroutine(TaskCooldown());
        try
        {
            gc.isDead = true;
            
        }
        catch (Exception e)
        {
            
            Debug.Log("GameController not found...");
            
        }

        PlayerData.FindLocalPlayer().IAmDead();
    }

    
    public int cooldown =10;
    public int timer = 0;
    public void GameEnd()
    {
        FindObjectOfType<GameController>().GameEndStart("New Game");
    }

    IEnumerator TaskCooldown()
    {
        timer = cooldown;
        SetTimerText(timer);
        while (timer > 0)
        {
            yield return new WaitForSecondsRealtime(1f);
            timer -= 1;
            // update timer text;
            SetTimerText(timer);
        }

        timer = 0;
        SetTimerText(timer);
    }

    private void SetTimerText(int timer)
    {
        timerText.text = timer+"";
    }
}
