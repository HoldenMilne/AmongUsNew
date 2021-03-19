using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scaler : MonoBehaviour
{
    // Start is called before the first frame update
    public float defWidth;
    void Start()
    {
        var w = Screen.width;
        var sprw = GetComponent<SpriteMask>().bounds.size.x;
        Debug.Log(sprw);
        defWidth = 900;
        

        var percent =  w/defWidth;

        transform.localScale = (Vector2.right+Vector2.up)*percent;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
