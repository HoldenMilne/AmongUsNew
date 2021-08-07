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
using Mirror;
using StationsAndHubs.Scripts;
using StationsAndHubs.Scripts.GameTasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameController : MonoBehaviour
{
    
    //TODO: May want to rewrite completely
    public bool isDead;
    public Text gameCodeTextBox;
    public Text nameTextBox;

    private PlayerData client;
    //private TcpClient client;

    public Canvas reportCanvas;
    public Canvas winPanel;
    public Canvas imposterWinPanel;
    public Text winPanelText;
    public Canvas endPanel;
    public Text endPanelText;

    public Image blackOut;
    Dictionary<string,string> taskMap = new Dictionary<string, string>();
    public List<GameTask> tasks = new List<GameTask>();
    public const string EXIT_CODE = "__EXIT2617__";
    private const string MAIN_SCENE_KEY = "Main";

    public GameTask currentTask;

    [HideInInspector] public bool ghostsUseStationCode=true;

    public PlayerData playerData;
    public Toggle checkbox;
    // Start is called before the first frame update
    private void Start()
    {
        singleton = this;
        Application.runInBackground = true;
        DontDestroyOnLoad(gameObject);
        //if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
/*#if UNITY_ANDROID
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
        }
        new Action(() => {
            var permission = Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead);
            if (permission)
            {
                Permission.RequestUserPermission(Permission.ExternalStorageRead);
            }
        }).Invoke();
#endif//*/
        //SendActivity.PluginClass.CallStatic("Run");
        
        if (popupImage != null)
        {
            DontDestroyOnLoad(popupImage.transform.parent.gameObject);
        }

        if(reportCanvas!=null)
            DontDestroyOnLoad(reportCanvas);
        if(winPanel!=null)
            DontDestroyOnLoad(winPanel);
        if(imposterWinPanel!=null)
            DontDestroyOnLoad(imposterWinPanel);
        if(endPanel!=null)
            DontDestroyOnLoad(endPanel);
        if (gameCodeTextBox == null)
            gameCodeTextBox = GetGameCodeTextBox();
        if (nameTextBox == null)
            gameCodeTextBox = GetNameTextBox();

        if(checkbox!=null)
            ghostsUseStationCode = checkbox.isOn;
        LoadTaskMap();

        //playerData = PlayerData.FindLocalPlayer();
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

    //TODO: This will likely be dead
    public void Connect()
    {
        connectingCover.enabled=true;
        StartCoroutine(ConnectCo());
    }
    
    
    //TODO: This will likely be dead
    public IEnumerator ConnectCo()
    {
        yield return null;
        /*
        
        //StartCoroutine(co());
        ip = null;
        port = 0;
        
        //StartCoroutine(ShowPopup("Connecting",5f,true));
        try
        {
            string gameCode = gameCodeTextBox.text.ToUpper();

            if (gameCode.StartsWith("_DEBUG_"))
            {
                var g = gameCode.Substring("_DEBUG_".Length);
                Debug.Log(g);
                switch (g)
                {
                    case "AST":
                    case "ASTEROIDS":
                        LoadScene("Clear Asteroids");
                        break;
                    case "CHART":
                    case "CHT":
                        LoadScene("Chart Course");
                        break;
                    case "DOWN":
                    case "DOWNLOAD":
                        LoadScene("Download Data");//RemoveAssetOptions this later
                        break;
                }
                
            }
            else if (gameCode.Contains(".") && gameCode.Contains(":"))
            {
                var spl = gameCode.Split(':');
                var spl2 = spl[0].Split('.');

                ip = new IPAddress(new byte[]
                {
                    Convert.ToByte(spl2[0]), Convert.ToByte(spl2[1]),
                    Convert.ToByte(spl2[2]), Convert.ToByte(spl2[3])
                });
                port = Int32.Parse(spl[1]);
                
                AddToDebug(spl[0] + "");
                AddToDebug(port + "");
            }
            else
            {
                Debug.Log("GameCode:" + gameCode);

                int[] ints = IPCoder.codeToBytes(gameCode, DateTime.Now);

                byte[] ips = IPCoder.getIP();
                Debug.Log(ips[0] + "." + ips[1] + "." + ((ints[0] & (127 << 8)) >> 8) + "." + (ints[0] & (127)));
                AddToDebug(+ips[0] + "." + ips[1] + "." + ((ints[0] & (127 << 8)) >> 8) + "." + (ints[0] & (127)));

                //StartCoroutine(ShowPopup(ips + ""));
                ip = new IPAddress(new byte[]
                {
                    Convert.ToByte(ips[0]), Convert.ToByte(ips[1]),
                    Convert.ToByte(((ints[0] & (127 << 8)) >> 8)), Convert.ToByte((ints[0] & (127)))
                });
                port = ints[1];
                Debug.Log(port);
                AddToDebug(port + "");
            }
        }

        catch (Exception e)
        {
            AddToDebug(ip + ":" + port);
            AddToDebug(e.InnerException?.Message ?? "" + " : " + e.Message + " " + e.Source);

            //connectingCover.enabled=true;
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
    
            //connectingCover.enabled=true;
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
        try
        {
            SendActivity.StartActivity(nameTextBox.text);
            SendToServer(nameTextBox.text);
            // Receive the response from the remote device.  
            RecieveTasks();

            LoadScene("TaskListScene"); //RemoveAssetOptions this later
        }
        catch (Exception e)
        {
            AddToDebug(e.Message);
            AddToDebug(SendActivity.activityName);
            Debug.Log(e.StackTrace);
            //connectingCover.enabled=true;
        }
        
        */
    }

    public void AddToDebug(string s)
    {
        String st = debug.text;
        var now = System.DateTime.Now;
        debug.text = now.Hour+":"+now.Minute+":"+now.Second+": "+s + "\n" + st;
    }

    public Canvas connectingCover;
    
    //TODO: This will likely be dead
    private void OpenClient(IPAddress ip, int port)
    {
        /*
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
        
        // */
    }

    //TODO: This will likely be dead
    private void RecieveTasksx()
    {
        /*
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
        }*/
    }

    //TODO: This will likely be dead
    private void RecieveTasks()
    {
        /*
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
        }*/
    }

    private GameTask ParseTask(string t)
    {
        
        Debug.Log(t);
        string[] t_data = t.Split(':');
        GameTask T = GameTaskFactory.create(t_data[0],t_data[1],t_data[2],t_data[3],t_data[4]);
        Debug.Log(T);
        return T;
    }

    /*
     * TODO:
     * This function should be simply "SendToServer" as a command.
     * Then, the server should make the "AwaitResponse" into a callback as "TellLocation" Target RPC
     * That "TellLocation" RPC should do the rest of the code with LoadScene and what not.
     */
    public bool RequestLocationName(string s)
    {
        if (winPanel.enabled) return true;
        if (client == null)
            client = FindClient();
        if (client == null) return true;
        client.SendToServer("LOC--"+s); // tell the server the station code
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
                Debug.Log("WHAT");
                currentTask = task;
                //client.UpdatePlayerLocation(client.connId,resp);
                LoadScene(task.name);
                return true;
            }
        }
        return false;

    }

    private PlayerData FindClient()
    {
        foreach (var pd in FindObjectsOfType<PlayerData>())
        {
            if (pd.isLocalPlayer) return pd;
        }

        return null;
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
        return "";
        /*
        byte[] bytes = new byte[1024];  
        
        int bytesRec = client.Receive(bytes); 
        return Encoding.ASCII.GetString(bytes,0,bytesRec);
        
        //return ""; // remove this later*/
    }

    public void SendToServerx(string s)
    {
        if (!socketReady)
            return;
        String foo = s + "\r\n";
        writer.Write(foo);
        writer.Flush();
    }

    /*
     * TODO: SendToServer should become a [command], and that should call a function callback
     * TODO: Each call to SendToServer must be a different [command]
     */
    public void SendToServer(string s)
    {
        
        Debug.Log("Sending Messgae:"+s);
        // Encode the data string into a byte array.  
        byte[] msg = Encoding.ASCII.GetBytes(s+"\n");

        try
        {
            // Send the data through the socket.  
            PlayerData bytesSent = client;
        }
        catch (Exception e)
        {Debug.Log("Send to server error.");
        }

    }

    public void AlertToJobSucess(string s)
    {
        client.TaskComplete("Complete--"+s);
        // get main scene and load
       
    }

    IEnumerator Delay(float f, Action func)
    {
        yield return new WaitForSeconds(f);
        func();
    }
    
    public void OpenWinPanel()
    {
        winPanel.enabled = true; // on start, close panel, reload game settings.
    }
    public void OpenIWinPanel()
    {
        imposterWinPanel.enabled = true; // on start, close panel, reload game settings.
    }

    public void Closex()
    {
        
        SendToServer(EXIT_CODE);
        //client.Shutdown(SocketShutdown.Both);
        //client.Close();
    }

    public void Close()
    {
        SendToServer(EXIT_CODE);
        //client.Shutdown(SocketShutdown.Both);
        try
        {
            //client.Close();
        }
        catch(Exception e){}
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
            AlertToJobSucess(currentTask.id);
        }
        else
        {
            AlertToJobSucess(currentTask.id+"-NOREPORT");
        }

        client.UpdatePlayerLocation(client.connId,"Unknown");
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
        winPanel.enabled = false;
        imposterWinPanel.enabled = false;
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
    public static GameController singleton;

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

    private void getFIles()
    {
        
    }

    protected virtual void ReturnToMenu()
    {
        SceneManager.LoadScene("Intro Scene");
    }

    // NEW:this is a test
    public string GetResponse(string s)
    { 
        Debug.Log(s+ " RESPONSE");
        string[] spl = s.Split(new string[]{"--"}, StringSplitOptions.RemoveEmptyEntries);
        string resp = "NONE--NUL";
        Debug.Log(spl[0]+ " :: "+spl[1]);
        switch (spl[0].ToUpper())
        {
            case "LOC":
                LoadTaskGame(spl[1]);
                client.UpdatePlayerLocation(client.connId,spl[1]);
                break;
            case "COMPLETE":
                Debug.Log("COMPLETE");
                string x = MarkTaskComplete(spl[1]);
                
            break;
            case "WIN":
                // ALERT TO WIN!
                winPanel.enabled = true;
                break;
        }

        return resp;
    }

    private string MarkTaskComplete(string s)
    {
        currentTask.isComplete = true;
        if (!s.Equals("nul", StringComparison.InvariantCultureIgnoreCase) && !s.Equals("win", StringComparison.InvariantCultureIgnoreCase))
        {
    
            Debug.Log("NOT NUL");
            string[] parts = s.Split(new string[]{">>"},StringSplitOptions.None);
            s = parts[1];
            string[] rtask = parts[0].Split(':');
            GameTask remove = GameTask.findGameTaskByID(rtask[1]);
            Debug.Log("PARTS");
            Debug.Log(remove);
            Debug.Log(parts[0]);
            GameTask t = ParseTask(s);
            if(remove==null) // try finding by name
                foreach (var T in tasks)
                {
                    if (T.name.StartsWith(rtask[0], StringComparison.InvariantCultureIgnoreCase))
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

        if (playerData == null)
        {
            Debug.Log("NULL DATA");
            playerData = PlayerData.FindLocalPlayer();
            //playerData = PlayerData.FindLocalPlayer();
        }
        if (playerData == null) return "ERR--no player data found";
        
        playerData.ReloadTaskListScene(tasks);
        if (s.Equals("win", StringComparison.InvariantCultureIgnoreCase))
        {

            StartCoroutine(Delay(.5f, OpenWinPanel));
    
        }
         //*/
         return "";
    }
    private void LoadTaskGame(string tname)
    {
        foreach (var task in tasks)
        {
            Debug.Log("task : " + task.name+ " :loc :" + task.location + "<"+tname);
            if (task.isComplete || !task.location.Equals(tname,StringComparison.InvariantCultureIgnoreCase)) continue;
            currentTask = task;
            LoadScene(task.name);
            break;
        }
    }

    public void Success()
    {
        if (AmongUsGoSettings.singleton.assignImposters)
        {
            if (playerData == null)
            {
                playerData = PlayerData.FindLocalPlayer();
            }
            TaskSuccess(!playerData.isImposter);
        }
        else
            reportCanvas.enabled = true;
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