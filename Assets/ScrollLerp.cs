using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollLerp : MonoBehaviour
{
    [Tooltip("Goes down to this from positive")]
    public float ymin = 0f;
    public float speed = 1f;
    private float y = 0f;

    private float target = 0f;
    private RectTransform rect;
    public CanvasGroup cgroup;
    public Canvas[] uiCanvasRoots;
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        y = rect.anchoredPosition.y;
        //running = false;
        StartCoroutine(LookUpCoroutine());
    }

    private IEnumerator LookUpCoroutine()
    {
        yield return new WaitForSeconds(1f);
        var coroutactive = false;
        while (running)
        {
            var anc = rect.anchoredPosition;
            var _y = anc.y; //current position
            _y = Mathf.Lerp(_y, ymin, Time.deltaTime * _trueSpeed);
            anc.y = _y;
            rect.anchoredPosition = anc;
            if (Mathf.Abs(_y - ymin) < 60f && !coroutactive)
            {
                StartCoroutine(TurnOnUI());
                coroutactive = true;
            };


            if (Mathf.Abs(_y - ymin) < 10f)
            {
                yield break;
                
            }
            _trueSpeed = Mathf.Lerp(_trueSpeed, speed, Time.deltaTime);
            yield return null;
        }
    }

    private bool running = true;

    private float _trueSpeed = 0f;
    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Space) && !running)
        {
            FindObjectOfType<SoundController>()._music_source.Play();
            running = true;
            StartCoroutine(LookUpCoroutine());
        }
    }

    private IEnumerator TurnOnUI()
    {
        cgroup.alpha = 0;
        foreach (var c in uiCanvasRoots)
        {
            c.enabled = true;
        }
        var a = 0f;

        while (a < 1f) 
        {
            a += Time.deltaTime * speed;
            cgroup.alpha = a;

            yield return null;
        }

        FindObjectOfType<TitleController>().enabled = true;
        
    }
}
