using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using DefaultNamespace.IP;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameController : MonoBehaviour
{

    public bool isDead;
    public Text gameCodeTextBox;
    public Text nameTextBox;

    private Socket client;
    //private TcpClient client;

    public Canvas reportCanvas;
    public Canvas winPanel;
    public Text winPanelText;
    public Canvas endPanel;
    public Text endPanelText;

    public Image blackOut;
    Dictionary<string,string> taskMap = new Dictionary<string, string>();
    public List<Task> tasks = new List<Task>();
    public const string EXIT_CODE = "__EXIT2617__";
    private const string MAIN_SCENE_KEY = "Main";

    public Task currentTask;

    [HideInInspector] public bool ghostsUseStationCode=true;

    public Toggle checkbox;
    // Start is called before the first frame update
    void Start()
    {
        Application.runInBackground = true;
        DontDestroyOnLoad(gameObject);
        
        if (popupImage != null)
        {
            DontDestroyOnLoad(popupImage.gameObject);
            DontDestroyOnLoad(popupImage.transform.parent.gameObject);
        }

        if (popupText != null)
            DontDestroyOnLoad(popupText.gameObject);
        if(reportCanvas!=null)
            DontDestroyOnLoad(reportCanvas);
        if(winPanel!=null)
            DontDestroyOnLoad(winPanel);
        if(endPanel!=null)
            DontDestroyOnLoad(endPanel);
        if (gameCodeTextBox == null)
            gameCodeTextBox = GetGameCodeTextBox();
        if (nameTextBox == null)
            gameCodeTextBox = GetNameTextBox();

        if(checkbox!=null)
            ghostsUseStationCode = checkbox.isOn;
        LoadTaskMap();
        
    }

    private void LoadTaskMap()
    {
        // for each task code, map to a scene.
        
        
        
    }

    private Text GetGameCodeTextBox()
    {
        foreach (var text in GameObject.FindObjectsOfType<Text>())
        {
            if (text.tag.Equals("GameCodeTextBox")) return text;
        }

        return null;
    }
    private Text GetNameTextBox()
    {
        foreach (var text in GameObject.FindObjectsOfType<Text>())
        {
            if (text.tag.Equals("NameTextBox")) return text;
        }

        return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private NetworkStream stream;
    StreamWriter writer;
    StreamReader reader;

    bool socketReady;
    public void Connectx()
    {
        /*
        //StartCoroutine(co());
        IPAddress ip = null;
        int port = 0;
        string gameCode = gameCodeTextBox.text.ToUpper();
        //StartCoroutine(ShowPopup((gameCodeTextBox==null)+""));

        int[] ints = IPCoder.codeToBytes(gameCode,DateTime.Now);
    
        byte[] ips = IPCoder.getIP();
        string ipstring = ips[0] + "." + ips[1] + "." + (((ints[0]+1) & (127 << 8)) >> 8) + "." + (ints[0] & (127));
        Debug.Log(ipstring);

        //StartCoroutine(ShowPopup(ips + ""));
        ip = new IPAddress(new byte[]{Convert.ToByte(ips[0]),Convert.ToByte(ips[1]),
            Convert.ToByte(((ints[0]&(127<<8))>>8)),Convert.ToByte((ints[0]&(127)))});
    
        port  = ints[1];

        //Security.PrefetchSocketPolicy();
        Debug.Log(port);
        client = new TcpClient(ipstring, port);
           stream = client.GetStream();
        
        writer =  new StreamWriter(stream);
        reader = new StreamReader(stream);
        socketReady = true;
        ///client = new .NetworkClient();
        //client.RegisterHandler(MsgType.Connect, OnConnected);     
        //client.Connect("127.0.0.1", port);2VBG6N1
        
        
        String s = "Socket connected to " + client.Client.RemoteEndPoint.ToString();
        
        Debug.Log(s);
        //StartCoroutine(ShowPopup(s));
        
        ghostsUseStationCode = checkbox.isOn;
        
        SendToServer(nameTextBox.text);
       
        
        // Receive the response from the remote device.  
        RecieveTasks();
        
        LoadScene("TaskListScene"); //RemoveAssetOptions this later*/

    }
    

    /**
     * Connect to the JAVA client (hub)
     */
    private IPAddress ip;
    private int port;
    private bool closePop = false;

    public void Connect()
    {
        connectingCover.enabled=true;
        StartCoroutine(ConnectCo());
    }
    
    
    public IEnumerator ConnectCo()
    {
        yield return null;
        //StartCoroutine(co());
        ip = null;
        port = 0;
        
        //StartCoroutine(ShowPopup("Connecting",5f,true));
        try
        {
            string gameCode = gameCodeTextBox.text.ToUpper();
            Debug.Log("GameCode:"+gameCode);
    
            int[] ints = IPCoder.codeToBytes(gameCode,DateTime.Now);
            
            byte[] ips = IPCoder.getIP();
            Debug.Log(ips[0]+"."+ips[1]+"."+((ints[0]&(127<<8))>>8)+"."+(ints[0]&(127)));
                AddToDebug(+ips[0] + "." + ips[1] + "." + ((ints[0] & (127 << 8)) >> 8) + "." + (ints[0] & (127)));
    
            //StartCoroutine(ShowPopup(ips + ""));
            ip = new IPAddress(new byte[]{Convert.ToByte(ips[0]),Convert.ToByte(ips[1]),
                Convert.ToByte(((ints[0]&(127<<8))>>8)),Convert.ToByte((ints[0]&(127)))});
            port  = ints[1];
            Debug.Log(port);
            AddToDebug(port+"");}
        catch (Exception e)
        {
            AddToDebug(ip+":"+port);
            AddToDebug(e.InnerException?.Message ?? "" + " : " + e.Message + " " + e.Source);

            yield break;
        }

        yield return null;
        try
        {
            OpenClient(ip, port);
        }
        catch (Exception e)
        {
            AddToDebug(ip+":"+port);
            AddToDebug(e.InnerException?.Message ?? "" + " : " + e.Message + " " + e.Source);
    
            yield break;
        }
        

        if (client?.RemoteEndPoint == null)
        {
            AddToDebug("Null Client");
            yield break;
        }
        String s = "Socket connected to " + client.RemoteEndPoint.ToString();
        
        Debug.Log(s);
        //StartCoroutine(ShowPopup(s));
        
        ghostsUseStationCode = checkbox.isOn;
        
        SendToServer(nameTextBox.text);
       
        
        // Receive the response from the remote device.  
        RecieveTasks();
        
        LoadScene("TaskListScene");//RemoveAssetOptions this later
        
    }

    private void AddToDebug(string s)
    {
        String st = debug.text;
        debug.text = s + "\n" + st;
    }

    public Canvas connectingCover;
    private void OpenClient(IPAddress ip, int port)
    {
        // open client
        client = new Socket(ip.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp );  
        IPEndPoint remoteEP = new IPEndPoint(ip,port); // server adr  
        //return;

        Debug.Log("Waiting to Connect");
        if (!client.ConnectAsync(remoteEP).Wait(10000)) // connect
        {
            Debug.Log("Connection Timed Out");
            AddToDebug("Client Timed Out");
            connectingCover.enabled = false;
            //StartCoroutine(ShowPopup("Connection Timed Out "+ip+ " : " + port));
            
            Debug.Log("Hella");
            return;
        }
        else
        {
            
            connectingCover.enabled=false;
            Debug.Log("Connected");
        }
        
        Debug.Log("DONE");
        
        // 
    }

    private void RecieveTasksx()
    {
        Debug.Log("RECIEVE");
        if(this.tasks.Count>0)this.tasks.Clear();
        if (!socketReady)
            return;
        Debug.Log("READY");
        if (!stream.DataAvailable)
            return;
        Debug.Log("DATA");
        string s = reader.ReadLine();

        Debug.Log(s);
        if (s != null)
        {
            Debug.Log("NOT NULL");
            string[] _tasks = s.Split(';');


            foreach (var t in _tasks)
            {
                this.tasks.Add(ParseTask(t));
            }
        }
    }

    private void RecieveTasks()
    {
        Debug.Log("Getting Tasks");
        if(this.tasks.Count>0)this.tasks.Clear();
        
        byte[] bytes = new byte[1024];
        int bytesRec = client.Receive(bytes); 
        String s = Encoding.ASCII.GetString(bytes,0,bytesRec);

        Debug.Log(s+ " <STRING");
        string[] tasks = s.Split(';');


        foreach (var t in tasks)
        {
            this.tasks.Add(ParseTask(t));
        }
    }

    private Task ParseTask(string t)
    {
        
        Debug.Log(t);
        string[] t_data = t.Split(':');
        Task T = new Task(t_data[0],t_data[1],t_data[2]);
        Debug.Log(T);
        return T;
    }

    public bool RequestLocationName(string s)
    {
        if (winPanel.enabled) return true;
        SendToServer("LOC--"+s); // tell the server the station code
        string resp = AwaitResponse(); // server tells you the name of that location
        Debug.Log(resp+"<<");
        foreach (var task in tasks)
        {
            if (!task.isComplete && task.location.Equals(resp))/* && (task.name.Equals("Fix Wiring", StringComparison.InvariantCultureIgnoreCase)||
                                                                   task.name.Equals("Submit Scan", StringComparison.InvariantCultureIgnoreCase)||
                                                                   task.name.Equals("Chart Course", StringComparison.InvariantCultureIgnoreCase)||
                                                                   task.name.Equals("Swipe Card", StringComparison.InvariantCultureIgnoreCase)||
                task.name.Equals("Clear Asteroids", StringComparison.InvariantCultureIgnoreCase))) // find the first incomplete task at that location.
            */{
                currentTask = task;
                LoadScene(task.name);
                return true;
            }
        }
        return false;

    }

    private string lastScene = "Intro Scene";

    public void LoadScene(String scene)
    {
        //throw new NotImplementedException();
        
        SceneManager.LoadScene(scene);

    }

    private string AwaitResponsex()
    {
        if(this.tasks.Count>0)this.tasks.Clear();
        if (!socketReady)
            return "";
        if (!stream.DataAvailable)
            return "";
        string s = reader.ReadLine();

        if (s != null)
        {
            return s;
        }

        return "";
    }

    private string AwaitResponse()
    {
        byte[] bytes = new byte[1024];  
        
        int bytesRec = client.Receive(bytes); 
        return Encoding.ASCII.GetString(bytes,0,bytesRec);
        
        //return ""; // remove this later
    }

    public void SendToServerx(string s)
    {
        if (!socketReady)
            return;
        String foo = s + "\r\n";
        writer.Write(foo);
        writer.Flush();
    }

    public void SendToServer(string s)
    {
        
        Debug.Log("Sending Messgae:"+s);
        // Encode the data string into a byte array.  
        byte[] msg = Encoding.ASCII.GetBytes(s+"\n");
  
        // Send the data through the socket.  
        int bytesSent = client.Send(msg);
         

    }

    public void AlertToJobSucess(string s)
    {
        SendToServer("Complete--"+s);
        // get main scene and load
        var v = AwaitResponse(); // TODO: use this to get next task if chain
        currentTask.isComplete = true;
        if (!v.Equals("nul", StringComparison.InvariantCultureIgnoreCase) && !v.Equals("win", StringComparison.InvariantCultureIgnoreCase))
        {
            
            Debug.Log("NOT NUL");
            Task t = ParseTask(v);
            Task remove = null;
            foreach (var T in tasks)
            {
                if (T.name.StartsWith(s, StringComparison.InvariantCultureIgnoreCase))
                {
                    remove = T;
                    break;
                }
            }

            if (remove != null)
            {
                tasks.Remove(remove);
                tasks.Add(t);
            }
            else
            {
                foreach (var tas in tasks)
                {
                    if (tas.name.Equals(currentTask.name))
                    {
                        Debug.Log("TASK MATCH -- "+tas);
                        tas.isComplete = true;
                        break;
                    }
                }
            }
        }
        else
        {
            foreach (var tas in tasks)
            {
                if (tas.name.Equals(currentTask.name))
                {
                    Debug.Log("TASK MATCH -- "+tas);
                    tas.isComplete = true;
                    break;
                }
            }

            
        } // test with GR1, CL, Funk station (fix wiring)

        string scene = "TaskListScene";
        LoadScene(scene);
        if (v.Equals("win", StringComparison.InvariantCultureIgnoreCase))
        {

            StartCoroutine(Delay(.5f, OpenWinPanel));
            
        }
    }

    IEnumerator Delay(float f, Action func)
    {
        yield return new WaitForSeconds(f);
        func();
    }
    
    private void OpenWinPanel()
    {
        winPanel.enabled = true; // on start, close panel, reload game settings.
    }

    public void Closex()
    {
        
        SendToServer(EXIT_CODE);
        //client.Shutdown(SocketShutdown.Both);
        client.Close();
    }

    public void Close()
    {
        SendToServer(EXIT_CODE);
        //client.Shutdown(SocketShutdown.Both);
        client.Close();
    }
    
    private void OnApplicationQuit()
    {
        Close();
    }

    public void TaskSuccess(bool report)
    {
        var text = blackOut.GetComponentInChildren<Text>();
        text.text = RandBlackOutText();
        text.enabled = true;
        blackOut.enabled = true;
        
        if (report)
        {
            AlertToJobSucess(currentTask.name);
        }
        else
        {
            AlertToJobSucess(currentTask.name+"-NOREPORT");
        }
        reportCanvas.enabled = false;
        text.enabled = false;
        blackOut.enabled = false;
    }

    private string RandBlackOutText()
    {

        string[] texts = new[] {"You are the imposter.\n\nI dunno I'm just guessing.","Everything is Sex", "You slut.", "You Simp", 
            "We get it, you own a Patagonia ski jacket and make Kombucha", "Cr3wm@t3",
            "Voldemort was kind of a really shitty wizard.  His Avadakedavra spell lost to an expelliarmus for fucks sake.",
            "Star wars is leathal weapon for people who wanna be smart.", "Batman's a simp"
        };
        
        var r = new System.Random().Next(0,texts.Length);
        return texts[r];
    }
    
    
    /*
     * FOR REUSE
     */

    private bool testing = true;
    public Vector2 GetTouch(bool toWorld = true)
    {
        if (!testing)
            return GetTouchPosition(toWorld);
        return GetMousePosition(toWorld);
    }
    
    public Vector2 GetTouch(bool toWorld, bool testing = false)
    {
        if (!testing)
            return GetTouchPosition(toWorld);
        return GetMousePosition(toWorld);
    }
    
    Vector2 nul = new Vector2(-1,-1);
    private Vector2 GetTouchPosition(bool toWorld = true)
    {
        
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 p = touch.position;
            p.x += Screen.width/2f;
            p.y += Screen.height/2f;
            p = Camera.main.ScreenToWorldPoint(p);
            Debug.Log(p);
            return p;

        }

        return nul;
    }
    
    private Vector2 GetMousePosition(bool toWorld=true)
    {
        
        if (Input.GetMouseButton(0))
        {
            Vector2 p = Input.mousePosition;
            //p.x -= Screen.width/2f;
            //p.y -= Screen.height/2f;
            // p.x -= Screen.width/2f;
            // p.y -= Screen.height/2f;
            if(toWorld)
                p = Camera.main.ScreenToWorldPoint(p);
            
            return p;

        }

        return nul;
    }

    public static GameController CreateGC()
    {
        return new GameObject().AddComponent<GameController>();
    }

    private bool doneFade = false;

    
    public IEnumerator GameEndCoroutine(string s)
    {
        endPanelCoOn = true;
        endPanelText.text = "Waiting on Hub...";
        StartCoroutine(FadeCanvas(endPanel));
        yield return new WaitUntil(DoneFade);
        yield return new WaitForSeconds(.5f);
        doneFade = false;
        GameEnd(s);
        StartCoroutine(FadeCanvas(endPanel,false));
        yield return new WaitUntil(DoneFade);
        endPanel.enabled = false;
        endPanelCoOn = false;
        doneFade = false;

    }

    public bool DoneFade()
    {
        return doneFade;
    }

    public IEnumerator FadeCanvas(Canvas c,bool fadeIn=true,float rate =2f)
    {
        doneFade = false;
        var cg = c.GetComponent<CanvasGroup>();
        var a = (fadeIn?0f:1f);
        cg.alpha = a;
        endPanel.enabled = true;
        while (a <= 1f && a>=0f)
        {
            if (fadeIn)
                a += Time.deltaTime * rate;
            else
                a -= Time.deltaTime * rate;
            cg.alpha = a;

            yield return null;
            
            

        }
        doneFade = true;
        
    }

    public void GameEndStart(string s)
    {
        StartCoroutine(GameEndCoroutine(s));
    }

    public void GameEnd(string s)
    {
        switch (s.ToLowerInvariant())
        {
            case "new game":
                SendToServer("new--game");
                RecieveTasks();
                isDead = false;
                DeathPanelController.iamDead = false;
                LoadScene("TaskListScene");
                break;
        }
    }

    public void WinPanelOnClick()
    {
        if(!winPanelCoOn)
            StartCoroutine(WinPanelCoroutine());
    }

    private bool winPanelCoOn = false;
    private bool endPanelCoOn = false;
    IEnumerator WinPanelCoroutine()
    {
        winPanelCoOn = true;
        winPanelText.text = "Waiting on Hub...";
        var cg = winPanel.GetComponentInChildren<CanvasGroup>();
        cg.alpha= 0f;
        cg.interactable = false;
        yield return new WaitForSeconds(.5f);
        GameEnd("new game");
        winPanel.enabled = false;
        cg.interactable = transform;
        cg.alpha= 1f;
        winPanelText.text = "Crew Mates Win";
        winPanelCoOn = false;
    }
    
    
    public Image popupImage;
    public Text popupText;
    public Text debug;

    bool ClosePop()
    {
        return closePop;
    }

    bool CheckPopOpen()
    {
        return !popupText.enabled;
    }
    public IEnumerator ShowPopup(string msg, float dur = 5f, bool b = false)
    {
        yield return new WaitUntil(CheckPopOpen);
        popupText.text = msg;
        popupImage.enabled = true;
        popupText.enabled = true;
        
        if(b)
            yield return new WaitForSeconds(dur);
        else
        {
            yield return new WaitUntil(ClosePop);
        }
        
        
        popupImage.enabled = false;
        popupText.enabled = false;
    }

    
    // Pause state

    private AddressFamily save_ip;
    private int save_port;
    private bool is_paused = false;
    private void OnApplicationPause(bool pauseStatus)
    {
        return;
        Debug.Log("PAUSED");
        is_paused = true;
        Close();
        AwaitResponse();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        return;
        Debug.Log("FOUCS");
        OpenClient(ip,port);
        AwaitResponse();
        
    }
}


public class Task
{

    public string name;
    public string id;
    public string location;
    public bool isComplete;

    public Task(string name, string id, string location, bool isComplete=false)
    {
        this.name = name;
        this.id = id;
        this.isComplete = isComplete;
        this.location = location;
    }

    public override string ToString()
    {
        return name+ " : " + id + " : "+location+ " : "+isComplete;
    }
}