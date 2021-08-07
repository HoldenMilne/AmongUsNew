using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Cryptography;
using kcp2k;
using Mirror;
using StationsAndHubs.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class GameCodeGenerator : MonoBehaviour
{
    private CustomNetworkManager conn;

    public Text portText;
    // Start is called before the first frame update
    void Start()
    {
        conn = FindObjectOfType<CustomNetworkManager>();
        GenerateCode();
    }

    private void GenerateCode()
    {
        switch (conn.connectionMethod)
        {
            case CustomNetworkManager.ConnectionMethod.LAN:
                GenerateLANCode();
                break;
            case CustomNetworkManager.ConnectionMethod.WAN_PORT_FORWARDING:
            case CustomNetworkManager.ConnectionMethod.WAN_SSH_TUNNELLING:
                // Use google db?
                // Use domain?
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /**
     * For now, use the IP Encoded into the gamecode.  Later we'll get a port.  Also gonna add a static number variable.
     */
    private void GenerateLANCode()
    {
        var id = conn.networkAddress;
        var gc = MakeLANCode(id);
//        GetComponent<Text>().text = gc;
        GetComponent<Text>().text = CustomNetworkManager.GetLocalIPv4();
    }

    private string MakeLANCode(string id)
    {
        id = "192.168.2.76";
        var port = conn.gameObject.GetComponent<KcpTransport>().Port;
        portText.text = port.ToString();
        var sum = 0;
        var x = 0;
        var spl = id.Substring(id.IndexOf(".", StringComparison.Ordinal) + 1).Split('.');
        
        foreach (var i in spl)
        {
            Debug.Log(i);
            sum+=(int)(Int32.Parse(i)*Mathf.Pow(35,spl.Length-x));
        }

        sum += port;
        var chars = 5;
        var code = "";
        var t_sum = 0f;
        Debug.Log(sum);
        for (int i = 0; i < chars; i++)
        {
            var mod = sum % 35;
            mod += i;
            code = GetChar(mod)+""+code;
            sum -= mod;
            sum /= 35;
            t_sum += mod*Mathf.Pow(35,i); 
            Debug.Log(sum);
            Debug.Log(code);
        }
        
        Debug.Log(t_sum);
        Debug.Log(code);
        

        return code;
    }
    
    private string MakeLANCode2(string id)
    {
        var port = conn.gameObject.GetComponent<KcpTransport>().Port;
        var sum = 0;
        var x = 0;
        var spl = id.Substring(id.IndexOf(".", StringComparison.Ordinal) + 1).Split('.');
        
        foreach (var i in spl)
        {
            Debug.Log(i);
            sum+=(int)(Int32.Parse(i)*Mathf.Pow(16,spl.Length-x));
        }

        var chars = 6;
        var code = "";
        var t_sum = 0f;
        Debug.Log(sum);
        for (int i = 0; i < chars; i++)
        {
            var mod = sum % 16;
            //mod += i;
            code = GetChar(mod)+""+code;
            sum -= mod;
            sum /= 16;
            t_sum += mod*Mathf.Pow(16,i); 
            Debug.Log(sum);
            Debug.Log(code);
        }
        
        Debug.Log(t_sum);
        Debug.Log(code);
        

        return code;
    }

    private char GetChar(int mod)
    {
        if (mod < 26)
        {
            return (char)(mod + 'A');
        }

        mod -= 26;
        return (char)(mod + '1');
    }
    private char GetCharHex(int mod)
    {
        if (mod < 10)
        {
            return (char)(mod + '0');
        }

        mod -= 10;
        return (char)(mod + 'A');
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
