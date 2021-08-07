using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Mirror;
using Mirror.Cloud.Example;
using Mirror.Discovery;
using Mirror.Examples.MultipleAdditiveScenes;
using StationsAndHubs.Scripts.GameTasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using Random = System.Random;
using Toggle = UnityEngine.UI.Toggle;

namespace StationsAndHubs.Scripts
{
    public class CustomNetworkManager : NetworkManager
    {

        public bool showPlayers = true;
        public enum ConnectionMethod
        {
            LAN,WAN_PORT_FORWARDING,WAN_SSH_TUNNELLING
        }

        public ConnectionMethod connectionMethod;
        public bool FORCEREMAKE = false;

        private int x = 0;

        private CustomNetworkManager netMan;

        new void Awake()
        {
            base.Awake();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        public new void Start()
        {
            base.Start();
            //netMan = GetComponent<CustomNetworkManager>();
            //netMan.StartServer();
            var ctc = FindObjectOfType<ConnectionTypeController>();
            SetConnMethod(ctc);
            switch (connectionMethod)
            {
                case ConnectionMethod.LAN:
                    networkAddress = GetLocalIPv4();
                    break;
                case ConnectionMethod.WAN_PORT_FORWARDING:
                case ConnectionMethod.WAN_SSH_TUNNELLING:
                    networkAddress = ctc.GetWanAddress();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Initializer.Init();
            rooms = GameTask.LoadLocationsFromFile(AmongUsGoSettings.singleton.taskListName);
            GameTask.LoadGameTasks(AmongUsGoSettings.singleton.taskListName);
            StartCoroutine(UpdatePanel());
        }

        private IEnumerator UpdatePanel()
        {
            while (true)
            {
                if (showPlayers)
                {
                    RemakePlayerPanel();
                    RemakeStationPanel();
                }

                yield return new WaitForSeconds(2f);
                
            }
        }

        private void CheckPlayerNameChanges()
        {
            foreach (var VARIABLE in playerNames)
            {
                
            }
        }

        private void SetConnMethod()
        {
            connectionMethod = FindObjectOfType<ConnectionTypeController>().type;
        }
        
        private void SetConnMethod(ConnectionTypeController c)
        {
            try
            {
                connectionMethod = c.type;
            }
            catch (Exception e)
            {
                connectionMethod = ConnectionMethod.LAN;
            }
        }

        public void Update()
        {
            if (FORCEREMAKE)
            {
                FORCEREMAKE = false;
                string name = "HOLDEN";
                playerNames.Add(name);
                idToPlayer.Add(x,name);
                x++;

                RemakePlayerPanel();
            }
            
            if(Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();

            
        }

        public GameObject playerDisplayObject;
        public GameObject spawnPoint;
        public GameObject locSpawnPoint;
        public GameObject scrollView;
        public GameObject locScrollView;
        public List<string> playerNames = new List<string>();
        public Dictionary<string,NetworkIdentity> players = new Dictionary<string,NetworkIdentity>();
        public Dictionary<int,NetworkIdentity> objects = new Dictionary<int,NetworkIdentity>();
        public Dictionary<int, string> idToPlayer = new Dictionary<int, string>();


        public Dictionary<string, string> stationIdToLocation = new Dictionary<string, string>();
        private Dictionary<int, string> conIdToStationId = new Dictionary<int, string>();

        public bool useAsAdminPanel = false;
        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);
        }


        /**
         * NOTES:
         * We're NOT getting here.  Either one ^ or v
         *
         * Also, data-type... etc isn't going to work.  Instead we'll need to find a
         * way to sync the connection type. 
         */
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            Debug.Log("Conn Opened");
            Transform startPos = GetStartPosition();
            GameObject player = startPos != null ? 
                Instantiate(playerPrefab, startPos.position, startPos.rotation)
                : Instantiate(playerPrefab);

            
            // instantiating a "Player" prefab gives it the name "Player(clone)"
            // => appending the connectionId is WAY more useful for debugging!
            var data = player.GetComponent<PlayerData>();
            data.MakeLocalPlayer();
            data.SetConnId(conn.connectionId);
            Debug.Log("Calling player obj <<< "+conn.connectionId);
            player.name = data.name + " xx "+conn.connectionId;
            
            objects.Add(conn.connectionId, player.GetComponent<NetworkIdentity>());
            NetworkServer.AddPlayerForConnection(conn, player);

        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            
            base.OnClientConnect(conn);
        }

        
        // For consideration: try getting "my" client ID and getting that
        // player data;
        public PlayerData GetDataFromClientID()
        {
            return null;
        }

        public NetworkIdentity GetPlayer(string name)
        {
            if(players.ContainsKey(name))
                return players[name];
            return null;
        }
        
        public NetworkIdentity GetPlayer(int id)
        {
            string name = idToPlayer[id];
            if(players.ContainsKey(name))
                return players[name];
            return null;
        }

        
        public void RemakePlayerPanel()
        {
            if (SceneManager.GetActiveScene().name != "Room") return;
            Debug.Log(spawnPoint +" <ISNULL");
            if(spawnPoint==null) return;
                foreach (var g in spawnPoint.GetComponentsInChildren<Transform>())
                {
                    if(g.gameObject!=spawnPoint)
                        Destroy(g.gameObject);
                }

            
            int i = 0;
            foreach (var p in playerNames)
            {
                Debug.Log(p+ " <PLAYER");
                var go = GameObject.Instantiate(playerDisplayObject,spawnPoint.transform) as GameObject;
                go.GetComponentInChildren<Text>().text=p;
                var anc = go.GetComponent<RectTransform>().anchoredPosition;
                anc.y = - 60 * i;
                i += 1;
                go.GetComponent<RectTransform>().anchoredPosition = anc;
                
                var img = go.GetComponent<Image>().color;
                img.a = ((i & 1) + 1) * .1f;
                go.GetComponent<Image>().color = img;
            }

            var size = scrollView.GetComponent<RectTransform>().sizeDelta;
            size.y = i * 60;
            scrollView.GetComponent<RectTransform>().sizeDelta = size;
            
            
            var anc2 = scrollView.GetComponent<RectTransform>().anchoredPosition;
            anc2.y = 0;
            scrollView.GetComponent<RectTransform>().anchoredPosition = anc2;
        }

        private void RemakeStationPanel()
        {
            if (SceneManager.GetActiveScene().name != "Room") return;
            foreach (var g in locSpawnPoint.GetComponentsInChildren<Transform>())
            {
                if(g.gameObject!=locSpawnPoint)
                    Destroy(g.gameObject);
            }

            
            int i = 0;
            foreach (var p in stationIdToLocation.Values)
            {
                var go = GameObject.Instantiate(playerDisplayObject,locSpawnPoint.transform) as GameObject;
                go.GetComponentInChildren<Text>().text=p;
                var anc = go.GetComponent<RectTransform>().anchoredPosition;
                anc.y = - 60 * i;
                i += 1;
                go.GetComponent<RectTransform>().anchoredPosition = anc;
                
                var img = go.GetComponent<Image>().color;
                img.a = ((i & 1) + 1) * .1f;
                go.GetComponent<Image>().color = img;
            }

            var size = locScrollView.GetComponent<RectTransform>().sizeDelta;
            size.y = i * 60;
            locScrollView.GetComponent<RectTransform>().sizeDelta = size;
            
            
            var anc2 = locScrollView.GetComponent<RectTransform>().anchoredPosition;
            anc2.y = 0;
            locScrollView.GetComponent<RectTransform>().anchoredPosition = anc2;
        }

        public List<string> rooms;

        public List<GameTask> tasks;

        public override void OnStartServer()
        {
            base.OnStartServer();
            
        }
        

        // GONNA NEED conn.connectionID TO location or locCode...
        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            if (!objects.ContainsKey(conn.connectionId)) return;
            var obj = objects[conn.connectionId];
            /*var pd = obj.GetComponent<PlayerData>();
            switch (pd.type)
            {
                
            }*/
            if (players.ContainsValue(objects[conn.connectionId]))
            {
                var pname = idToPlayer[conn.connectionId];
                players.Remove(pname);
                playerNames.Remove(pname);
                idToPlayer.Remove(conn.connectionId);
                RemakePlayerPanel();
            }else if (stationIdToLocation.Values.Contains(conIdToStationId[conn.connectionId]))
            {
                    
                var stationId = conIdToStationId[conn.connectionId];
                stationIdToLocation.Remove(stationId);
                conIdToStationId.Remove(conn.connectionId);
                RemakeStationPanel();
            }
            objects.Remove(conn.connectionId);
        }

        public void ToggleUseAsAdminPanel()
        {
            useAsAdminPanel = !useAsAdminPanel;
        }
        public void SetUseAsAdminPanelToCheckbox(Toggle toggle)
        {
            useAsAdminPanel = toggle.isOn;
        }
        /* Load the Correct Scene for Each */
        public void StartGame()
        {
            var locations = new string[stationIdToLocation.Count];
            int i = 0;
            GameTask.locations.Clear();
            foreach (var k in stationIdToLocation.Values)
            {
                locations[i] = k;
                GameTask.locations.Add(k);
                i++;
            }
            Debug.Log(locations.Length);
            FindObjectOfType<SettingsManager>().LoadSettingsFromPanel();
            GameTask.LoadGameTasks(AmongUsGoSettings.singleton.taskListName);
            

            string[] imposterNames = null;
            if (AmongUsGoSettings.singleton.assignImposters)
            {
                Debug.Log("ASSIGN IMPOSTERS"+AmongUsGoSettings.singleton.assignImposters);
                foreach (var _name in players.Keys)
                {
                    var go = GameObject.Find(_name);
                    Debug.Log(go);
                    try
                    {
                        Debug.Log(go.name);
                        go.GetComponent<PlayerData>().isImposter = false;
                    }
                    catch (Exception e)
                    {
                        Debug.Log("ERROR2");
                        Debug.Log(e.Data);
                        Debug.Log(e.Message);
                        Debug.Log(e.Source);
                        Debug.Log(e.StackTrace);
                    }
                }
                imposterNames = AssignImposters(AmongUsGoSettings.singleton.numImposters,out imposters);
                foreach (var _name in imposters)
                {
                    Debug.Log("imposters");
                    var go = GameObject.Find(_name);
                    Debug.Log(go);
                    try
                    {
                        Debug.Log(go.name);
                        go.GetComponent<PlayerData>().isImposter =true;
                    }
                    catch (Exception e)
                    {
                        Debug.Log("ERROR");
                        Debug.Log(e.Data);
                        Debug.Log(e.Message);
                        Debug.Log(e.Source);
                        Debug.Log(e.StackTrace);
                    }
                }
            }

            foreach (var pd in FindObjectsOfType<PlayerData>())
            {
                var _tasks = GameTask.getGameTasks(AmongUsGoSettings.singleton.shortTasks,
                    AmongUsGoSettings.singleton.longTasks, AmongUsGoSettings.singleton.taskListName);

                Debug.Log("TASKSLENGTH:" + _tasks.Count);
                string[] tasksStrings = new string [_tasks.Count];

                i = 0;
                foreach (var t in _tasks)
                {
                    tasksStrings[i] = t.ToString();
                    i += 1;
                }

                if (pd.type == PlayerData.PlayerType.Player)
                {
                    pd.SetCurrentLocation("Unknown3");
                    pd.SetLastLocation("Unknown3");
                    pd.StartGameThin(AmongUsGoSettings.singleton.shortTasks, AmongUsGoSettings.singleton.longTasks,
                        tasksStrings, AmongUsGoSettings.singleton.assignImposters,
                        AmongUsGoSettings.singleton.ghostsVisitStations,
                        AmongUsGoSettings.singleton.crewWinsOnTaskCompletion, imposterNames);
                }
            }

            if (true) return;
            var nids = FindObjectsOfType<NetworkIdentity>();

            foreach (var n in nids)
            {
                PlayerData pd;
                if ((pd = n.GetComponent<PlayerData>()) != null)
                {
                    pd.StartGame(0,0,null,null);
                }
            }

            if (useAsAdminPanel)
            {
                SceneManager.LoadScene("AdminScene");
            }
            else
            {
                SceneManager.LoadScene("VotingScene");
            }
        }
        private List<String> imposters = new List<string>();
        private string[] AssignImposters(int nImposters, out List<string> imposters)
        {
            imposters = new List<string>();
            //if (nImposters >= players.Count) return false;
            var names = players.Keys.ToList();
            for (int i = 0; i < nImposters; i++)
            {
                if (names.Count == 0) break;
                var n = names[UnityEngine.Random.Range(0, names.Count)];// get the name
                names.Remove(n);
                imposters.Add(n);
            }

            return imposters.ToArray();
        }


        public static string GetLocalIPv4()
        {
            return Dns.GetHostEntry(Dns.GetHostName())
                .AddressList.First(
                    f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .ToString();
        }

        public void UpdatePlayerLists()
        {
            playerNames.Clear();
            foreach (var v in idToPlayer.Values)
            {
                playerNames.Add(v);
            }
        }

        public void ChangePlayerName(int connID, string newString)
        {
            
                var curname = idToPlayer[connID];
                var ind = playerNames.IndexOf(curname);
                var keyval = players[curname];
                players.Remove(curname);
                players.Add(newString,keyval);//removal doesn't work...
                objects.Remove(connID);
                objects.Add(connID,keyval);
                objects[connID].gameObject.GetComponent<PlayerData>().playerName = newString;
                playerNames[ind] = newString;
                idToPlayer[connID] = newString;
                
                RemakePlayerPanel();
                    
            
            
        }

        /*
         * This assumes the "object" has been added to objects
         */
        public void AddNewPlayer(int connID, string _name)
        {
            Debug.Log("ADD NEW PLAYER");
            if (idToPlayer.Keys.Contains(connID))
            {
                ChangePlayerName(connID,_name);
                return;
                
            }
            Debug.Log("ADD NEW PLAYER2");
        
            if (players.ContainsKey(_name))
            {
                Debug.Log("CONTAINS KEY: "+_name);
                return;
            }
            int i = 1;
            while (players.ContainsKey(_name))
            {
                _name = _name + $" ({i})";
                i += 1;
            }
            
            players.Add(_name,objects[connID]);
            playerNames.Add(_name);//
            idToPlayer.Add(connID, _name);
            objects[connID].name = _name;
            objects[connID].gameObject.GetComponent<PlayerData>().playerName = _name;
            RemakePlayerPanel();
        }

        public void ChangeStation(int connID, string newString)
        {
            stationIdToLocation[conIdToStationId[connID]] = newString;
            RemakeStationPanel();
        }
        
        public void AddNewStation(int connID, string gc, string location)
        {
            if (conIdToStationId.ContainsKey(connID))
            {
                ChangeStation(connID, location);
                return;
            }

            if (stationIdToLocation.ContainsValue(location))
            {
                // TODO: Make a "failed" callback
                return;
            }
            
            int i = 1;
            
            
            stationIdToLocation.Add(gc,location);
            conIdToStationId.Add(connID,gc);
            
            //objects[connID].GetComponent<StationData>().GiveGameCode(gc);
            var stationData = objects[connID].GetComponent<PlayerData>();
            stationData.playerName = gc;
            stationData.name = gc;
            stationData.type = PlayerData.PlayerType.Station;
            stationData.currentLocation = location;
            stationData.connId = connID;
            // set up a station call back that tells it a gamecode
            RemakeStationPanel();
            
        }

        public string GenerateGameCode(int l = 6)
        {
            string code = "";
            for (var i = 0; i < l; i++)
            {
                code += GetRandomChar(UnityEngine.Random.Range(0, 35));
            }

            return code;
        }

        private char GetRandomChar(int v)
        {
            if (v < 26)
            {
                return (char) (v + 'A');
            }

            return (char) (v + '1');
        }


        void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.name == "Room")
            {
                spawnPoint = GameObject.FindWithTag("playerDisplaySpawnPoint");
                scrollView = spawnPoint.transform.parent.parent.gameObject;
                locSpawnPoint = GameObject.FindWithTag("locDisplaySpawnPoint");
                locScrollView = locSpawnPoint.transform.parent.parent.gameObject;
            }
        }

        public string SendToServer(string s)
        {
            string[] spl = s.Split(new string[]{"--"}, System.StringSplitOptions.RemoveEmptyEntries);
            string resp = "NONE--NUL";
            Debug.Log(spl[0]+ " <LOG");
            switch (spl[0].ToUpper())
            {
                case "LOC" :
                    try
                    {
                        resp = spl[0] + "--" + stationIdToLocation[spl[1]];
                    }
                    catch
                    {
                        return "ERR--NULL";
                    }

                    break;
                case "COMPLETE":
                    // Todo: Check if is mafia
                    // Todo: Increment victory total if not
                    bool report = true;
                    
                    int index;
                    if ((index = spl[1].IndexOf("-NOREPORT", StringComparison.Ordinal)) >= 0)
                    {
                        spl[1] = spl[1].Substring(0,index);
                        report = false;
                    }
                    var complete = GameTask.findGameTaskByID(spl[1]);
                    Debug.Log(complete.next);
                    if (!complete.next.Equals("NUL",StringComparison.InvariantCultureIgnoreCase))
                    {
                        resp = spl[0]+ "--"+GameTask.findGameTaskByID(complete.next);
                    }
                    else
                    {
                        resp = spl[0] + "--NUL";
                    }

                    return resp;
                    if (tasks == null) return "ERR--NUL";
                    GameTask addTask=null, removeTask=null;
                    bool addremove = false;
                    foreach (var task in tasks)
                    {
                        if(task.name.Equals(spl[1],StringComparison.InvariantCultureIgnoreCase))// if has a new task
                        {
                            Debug.Log("TASK>"+task.name+"<>"+task.next);
                            if(!task.next.Equals("nul",StringComparison.InvariantCultureIgnoreCase))
                            {
                                addTask = GameTask.findGameTaskByID(task.next);
                                if(addTask!=null)
                                {
                                    removeTask=task;
                                    addremove=true;
                                }
                            }
                            break;
                        }
                    }

                    resp = spl[0] + "--nul";
                    if(addremove)
                    {
                        resp = spl[0] + "--" + removeTask.name+":"+removeTask.id + ">>"+addTask.name+":"+addTask.id+":"+addTask.location;
                    }

                    break;
            }

            return resp;
        }

        /**
         * Returns true if win
         */
        public bool CheckWin()
        {
            var settings = AmongUsGoSettings.singleton;

            var count = 0;
            var total = 0;
            foreach (var pd in FindObjectsOfType<PlayerData>())
            {
                if (pd.type == PlayerData.PlayerType.Player)
                {
                    count += 1;
                    total += pd.tasksCompleted;
                }
            }
            Debug.Log(total+  ": :: :"+(settings.longTasks + settings.shortTasks) * (count-settings.numImposters));
            // true nTasks >= max nTasks 
            return total>=(settings.longTasks + settings.shortTasks) * (count-settings.numImposters) && settings.crewWinsOnTaskCompletion;
        }

        /**
         * Used to alert to the end of game
         */
        public void GameEnd()
        {
            foreach (var pd in GameObject.FindObjectsOfType<PlayerData>())
            {
                switch (pd.type)
                {
                    case PlayerData.PlayerType.Player:
                        // return to menu area with an initial load (name should be already set)
                        pd.AlertToGameEnd();
                        break;
                }
            }
        }

        public void CallForResume()
        {
            foreach (var pd in GameObject.FindObjectsOfType<PlayerData>())
            {
                switch (pd.type)
                {
                    case PlayerData.PlayerType.Player:
                        // return to menu area with an initial load (name should be already set)
                        pd.ResumeGame();
                        break;
                }
            }
        }

        private int deadImposters;
        private int deadCrewmates;

        
        public void IAmDead(int connId)
        {
            Debug.Log("I AM DEAD");
            var pd = objects[connId].gameObject.GetComponent<PlayerData>();
            pd.isDead = true;
            if (AmongUsGoSettings.singleton.assignImposters)
            {
                if (pd.isImposter)
                {
                    deadImposters += 1;
                    var nImposter = AmongUsGoSettings.singleton.numImposters;
                    Debug.Log(deadImposters+ " <DI");
                    if (deadImposters >= nImposter || (nImposter >= playerNames.Count && playerNames.Count <= deadImposters))
                    {
                        //crewmates win
                        CrewmatesWin();
                    }
                }
                else
                {
                    deadCrewmates += 1;
                    Debug.Log(deadCrewmates+ " <DC");
                    if (deadCrewmates >= objects.Count - AmongUsGoSettings.singleton.numImposters)
                    {
                        ImpostersWin();
                    }
                }
                
            }
        }

        void CrewmatesWin()
        {
            foreach (var pd in GameObject.FindObjectsOfType<PlayerData>())
            {
                if(pd.type.Equals(PlayerData.PlayerType.Player))
                    pd.CrewmatesWin();
            }

            StartCoroutine(DelayGameEnd());
        }

        public IEnumerator DelayGameEnd()
        {
            var gsm = FindObjectOfType<GameStateManager>();
            gsm.votingCanvas.enabled = false;
            gsm.playingCanvas.enabled = false;
            gsm.StopAllCoroutines();
            
            yield return new WaitForSeconds(2);
            GameEnd();
            yield return new WaitForSeconds(5);
            foreach (var pd in GameObject.FindObjectsOfType<PlayerData>())
            {
                if(pd.type.Equals(PlayerData.PlayerType.Player))
                    pd.ForceClosedWinPanel();
            }
        }

        void ImpostersWin()
        {
            foreach (var pd in GameObject.FindObjectsOfType<PlayerData>())
            {
                if(pd.type.Equals(PlayerData.PlayerType.Player))
                    pd.ImpostersWin();
            }
            StartCoroutine(DelayGameEnd());
        }

        public PlayerData FindByConnId(int connId)
        {
            foreach (var pd in GameObject.FindObjectsOfType<PlayerData>())
            {
                if (pd.connId == connId)
                {
                    return pd;
                }
            }

            return null;
        }
    }
}