using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class AsteroidController : MonoBehaviour
{
    public Image explosion;

    public Image brokenSprite;

    [Range(45,360)] public int minAngle;
    [Range(45,360)] public int maxAngle;
    private int degPerSec = 90;
    [Range(0,10)] public float minSpeed;
    [Range(0,10)] public float maxSpeed;
    private float pixelsPerSec = 90;

    private Vector2 dir;
    public Vector2 target;

    public bool start = false;

    private RectTransform rect;
    // Start is called before the first frame update
    void Start()
    {
        // get various speeds randomly
        rect = GetComponent<RectTransform>();
        
        CalculateDirection();
        degPerSec = new System.Random().Next(minAngle,maxAngle);
        pixelsPerSec = (float)(new System.Random().NextDouble()*(maxSpeed-minSpeed)+minSpeed);
        
        start = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            if(Camera.main.WorldToScreenPoint(rect.position).x<target.x-Screen.width/4f)
            {
                Destroy(gameObject);
            }

            var pos = (Vector2)rect.position;
            pos += dir * pixelsPerSec*Time.deltaTime;
            rect.position = pos;

            var rot = rect.eulerAngles;
            var z = rot.z;
            z += degPerSec * Time.deltaTime;
            z = z % 360;
            rot.z = z;
            rect.eulerAngles = rot;
        }
    }

    public void CalculateDirection()
    {
        dir = (target - (Vector2)rect.position).normalized;
        
    }

    public void OnClick()
    {
        FindObjectOfType<ClearAsteroidsController>().OnClick(gameObject);
    }
}
