using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBackgroundUpdater : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var screenHeight = Screen.height;
        var screenWidth = Screen.width;
        var size = GetComponent<RectTransform>().sizeDelta;
        var scale = GetComponentInParent<Canvas>().GetComponent<RectTransform>().localScale;
        Debug.Log(scale);
        var div = size.y / screenHeight;
        var _size = new Vector2(size.x,size.y);
        _size.y = screenHeight/scale.y;
        _size.x = size.x / (div);

        if (_size.x < screenWidth)
        {
            Debug.Log("SIZE SUCKS");
            div = size.x / screenWidth;
            size.x = screenWidth/scale.x;
            size.y = size.y / (div*scale.y);
        }
        else
        {
            _size.x /= scale.x;
            size = _size;
        }
        GetComponent<RectTransform>().sizeDelta = size;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
