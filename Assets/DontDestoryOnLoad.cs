using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestoryOnLoad : MonoBehaviour
{
    public bool dontDestroyOnLoad = true;
    // Start is called before the first frame update
    void Start()
    {
        if(dontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
