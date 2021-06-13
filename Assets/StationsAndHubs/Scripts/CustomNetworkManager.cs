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
            Debug.Log("CONN TIME");
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
            data.SetConnId(conn.connectionId);
            Debug.Log("Calling player obj <<< "+conn.connectionId);
            name = data.name;
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
            FindObjectOfType<PlayerData>().StartGame(AmongUsGoSettings.singleton.shortTasks,AmongUsGoSettings.singleton.longTasks,AmongUsGoSettings.singleton.taskListName);
            if (true) return;
            var nids = FindObjectsOfType<NetworkIdentity>();

            foreach (var n in nids)
            {
                PlayerData pd;
                if ((pd = n.GetComponent<PlayerData>()) != null)
                {
                    pd.StartGame(0,0,"");
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
        
        public string GetLocalIPv4()
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
    }
}