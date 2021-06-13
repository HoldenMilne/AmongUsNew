using System;
using System.Collections;
using System.Linq;
using kcp2k;
using StationsAndHubs.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class HubMenuSelectController : MonoBehaviour
{
    public LoadingController loadingController;
    private byte state = 0; // main menu
    private const byte MAIN_MENU_STATE = 0;
    private const byte HUB_MENU_STATE = 1;
    private const byte STATION_MENU_STATE = 2;
    private const byte SETTING_MENU_STATE = 3;

    public Text backQuitText;
    public Canvas[] canvasMenus;
    public Canvas quitDialog;
    public enum MenuOption
    {
        START_HUB=0,START_STATION=1,SETTINGS=2,BACK=-1,
        START_HUB_GO=3,START_STATION_GO=4
    }
    public Color hoverColor = new Color();

    private Text text;
    private Color defaultColor;

    public SoundController sc;

    public InputField portNumber;
    public InputField stationIP;
    // Start is called before the first frame update
    void Start()
    {
        Loading.LoadAllScreens();
        
        Text _text;
        var b = TryGetComponent(out _text);
        if (b)
        {
            defaultColor = _text.color;
            text = _text;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HoverEnter()
    {
        text.color = hoverColor;
    }
    public void HoverExit()
    {
        text.color = defaultColor;
    }
    public void OnClick(int option)
    {
        switch (option)
        {
            case (int)MenuOption.START_HUB:
                StartHubMenu();
                break;
            case (int)MenuOption.START_STATION:
                StartStationMenu();
                break;
            case (int)MenuOption.SETTINGS:
                SettingsMenu();
                break;
            case (int)MenuOption.BACK:
                Back();
                break;
            case (int)MenuOption.START_HUB_GO:
                StartHub();
                break;
            case (int)MenuOption.START_STATION_GO:
                StartStation();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(option), option, null);
        }
    }
    private void StartStation()
    {
        int port = 0;
        string IP = "";
        bool b = false;
        string text = stationIP.text;
        Debug.Log(text);
        if (text.Contains(":"))
        {

            String[] data = stationIP.text.Split(':');
            IP = data[0];
            b = int.TryParse(data[1],out port);
        }
        if (b && ValidPort(port) && ValidateIP(IP))
        {
            var cont = FindObjectOfType<CustomNetworkManager>();
                cont.GetComponent<KcpTransport>().Port = (ushort) port;
            cont.networkAddress = IP;
            sc.PlayBite("bell");
            //cont.StartClient();
            loadingController.StartLoad(sc,"StationRoom", 2);
            
        }
        else
        {
            sc.PlayBite("error",.15f);
        }
    }

    private bool ValidateIP(string ip)
    {
        int count = Count(ip,'.');//COUNT NUMBER OF "."  if 1 use x.y.{ip}, if 0 use x.y.z.{ip}, if 2 use x.{ip} else use ip

        if (count > 3) return false;
        Debug.Log("IP dots: "+count+ " : "+ip);

        if (count < 3)
            return false; // infuture get ipv4 and partial

        foreach (string s in ip.Split('.'))
        {
            int i = 0;
            if (!int.TryParse(s, out i))
            {
                return false;
            }

            if (i < 0 || i > 255)
                return false;
        }

        return true;

    }

    private string GetPartialIp(string ip, int count)
    {
        switch (count)
        {
            case 0:
                return ip.Substring(0, ip.LastIndexOf(".")+1);
                break;
            case 1:
                return ip.Substring(0, ip.LastIndexOf(".")).Substring(0, ip.LastIndexOf(".")+1);
                break;
            case 2:
                return ip.Substring(0, ip.IndexOf(".") + 1);
                break;
        }

        return ip;
    }

    public int Count(string s, string sub)
    {
        int x = 0;
        while (s.Contains(sub))
        {
            x += 1;
            s = s.Substring(s.IndexOf(sub) + sub.Length);
        }

        return x;
    }
    
    public int Count(string s, char c)
    {
        int x = 0;
        foreach(char q in s)
        {
            if (q == c)
                x += 1;
        }

        return x;
    }
    private void StartHub()
    {
        int port = 0;
        
        
        bool b = int.TryParse(portNumber.text,out port);
        if (b && ValidPort(port))
        {
            FindObjectOfType<KcpTransport>().Port = (ushort) port;
            sc.PlayBite("bell");
            loadingController.StartLoad(sc,"Room", 1);
            
        }
        else
        {
            sc.PlayBite("error",.15f);
        }
    }

    private bool ValidPort(int portNumber)
    {
        return portNumber >= 0 && portNumber < ushort.MaxValue;
    }

    
    private void SettingsMenu()
    {
        backQuitText.text = "Back";
        throw new NotImplementedException();
    }

    private void StartStationMenu()
    {
        backQuitText.text = "Back";
        state = STATION_MENU_STATE;
        canvasMenus[MAIN_MENU_STATE].enabled = false;
        canvasMenus[STATION_MENU_STATE].enabled = true;
        
    }

    private void StartHubMenu()
    {
        backQuitText.text = "Back";
        state = HUB_MENU_STATE;
        canvasMenus[MAIN_MENU_STATE].enabled = false;
        canvasMenus[HUB_MENU_STATE].enabled = true;
    }
    
    private void Back()
    {
        if (state != MAIN_MENU_STATE)
        {
            backQuitText.text = "Quit";
            canvasMenus[state].enabled = false;
            state = MAIN_MENU_STATE;
            canvasMenus[state].enabled = true;
        }
        else
        {
            Quit();
        }
    }

    public void Quit(bool force=false)
    {
        if (force)
        {
            Application.Quit();
        }
        else
        {
            quitDialog.enabled = true;
            canvasMenus[0].enabled = false;
        }
    }
    
    public void CancelQuit()
    {
         quitDialog.enabled = false;
         canvasMenus[0].enabled = true;
    }
}


/*
 *  0 - Menu
 *      1 - Start Hub
 *          Load Scene (no State)
 *      2 - Start Station
 *          Load Scene (no State)
 *      3 - Settings
 *          - Many Panels (no state)
 */
public class Loading
{
    public static Sprite[] images;
    public static void LoadAllScreens()
    {
      images = Resources.LoadAll<Sprite>("/loading");
    }

    public static Sprite GetRandomImage()
    {
        return images[Random.Range(0, images.Length)];
    }
}