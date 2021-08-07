using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GCCaller : MonoBehaviour
{
    
    public void StaticInvokeMethod(string s)
    {
        FindObjectOfType<GameController>().Invoke(s,0);
    }
}
