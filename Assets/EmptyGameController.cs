using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyGameController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Success());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Success()
    {
        yield return new WaitForSeconds(1f);
        FindObjectOfType<GameController>().reportCanvas.enabled = true;
    }
}
