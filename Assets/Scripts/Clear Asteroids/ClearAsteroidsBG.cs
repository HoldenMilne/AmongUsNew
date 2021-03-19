using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class ClearAsteroidsBG : MonoBehaviour
{
    private RectTransform rect;
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        StartCoroutine(Rotate());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool toggle = true;
    IEnumerator Rotate()
    {
        var z = 0;
        while (toggle)
        {
            yield return new WaitForSecondsRealtime(.12f);
            z += 90;
            z = z % 360;
            rect.eulerAngles = new Vector3(0, 0, z);
        }
    }
}
