using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Mirror;
using Mirror.Discovery;
using Mirror.Examples.Chat;
using StationsAndHubs.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PhoneToServerConnectionController : MonoBehaviour
{
    private CustomNetworkManager manager;

    private InputField field;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var VARIABLE in FindObjectsOfType<NetworkManager>())
        {
            Debug.Log(VARIABLE);
            Debug.Log(VARIABLE.gameObject.activeSelf);
            Debug.Log("");
        }

        if (NetworkManager.singleton != null)
        {
            manager = (CustomNetworkManager)NetworkManager.singleton;
        }
        
        //field = GameObject.FindWithTag("nameField").GetComponent<InputField>();

    }

    
    public void OnClick(Text text)
    {
        Debug.Log(c);
        var playerName = "";

        if (false && playerName == "")
        {
            StartCoroutine(SetFieldErrorColor());
        }
        else
        {
            var pd = FindObjectOfType<DataSync>();
            Debug.Log(pd.name);
            pd.PlayerName = playerName;
            pd.PlayerType = PlayerData.PlayerType.Player;
            var uri = GetComponent<DiscoveredServerController>().Server.uri;
            Debug.Log(uri);
            FindObjectOfType<CustomNetworkDiscoveryHUD>().Connect(uri);

            SceneManager.LoadScene("ClientRoom");

        }

        

    }

    public float errSpeed = 2f;
    public float plusMinusX;
    private bool setFieldColorRunning;
    private Color c;
    private IEnumerator SetFieldErrorColor()
    {
        if (setFieldColorRunning) yield break;
        setFieldColorRunning = true;
        
        var transform = field.gameObject.GetComponent<RectTransform>();
        var anc = transform.anchoredPosition;
        var initX = anc.x;
        var x = initX;
        for (int i = 0; i < 8; i++)
        {
            if ((i & 1) == 0)
            {
                while (x < initX + plusMinusX) 
                {
                    x += Time.deltaTime * errSpeed * plusMinusX;
                    anc.x = x;
                    transform.anchoredPosition = anc;
                    yield return null;
                }
            }
            else
            {
                while (x > initX - plusMinusX) 
                {
                    x -= Time.deltaTime * errSpeed * plusMinusX;
                    anc.x = x;
                    transform.anchoredPosition = anc;
                    yield return null;
                }
            }

            yield return null;
        }

        Debug.Log("HERE");
        var d = (initX > x ? 1 : -1);
        while ((d==1?x < initX:x>initX) )
        {
            x += Time.deltaTime * errSpeed * plusMinusX * d;
            anc.x = x;
            transform.anchoredPosition = anc;
            yield return null;
        }
        
        anc.x = initX;
        transform.anchoredPosition = anc;
        yield return new WaitForSeconds(2f);
        setFieldColorRunning = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
