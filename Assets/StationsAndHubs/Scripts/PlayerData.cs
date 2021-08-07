using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Scripts;
using Mirror;
using StationsAndHubs.Scripts.GameTasks;
using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace StationsAndHubs.Scripts
{
    
    public class PlayerData : NetworkBehaviour
    {
        
        public enum PlayerType
        {
            Player, Station, AdminPanel
        }

        public PlayerType type;
        [SyncVar(hook = nameof(UpdatePlayerName))]
        public string playerName = "Unnamed";
        [SyncVar(hook = nameof(UpdateLastLocation))]
        public string lastLocation = "Unknown";
        [SyncVar(hook = nameof(UpdateCurrentLocation))]
        public string currentLocation = "Unknown";
        [SyncVar(hook = nameof(UpdateCompletionCount))]
        public int tasksCompleted;
        [SyncVar]
        public int connId;
        [SyncVar]
        public bool isDead;

        [SyncVar(hook = nameof(updateImposter))]
        public bool isImposter = false; // default crewmate
        
        
        void updateImposter(bool old, bool newVal)
        {
            isImposter = newVal;
            Debug.Log(newVal);
        }
        [SyncVar(hook = nameof(UpdateReady))]
        public bool ready;

        private List<GameTask> tasks;

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Debug.Log("AWAKE");
            if (isLocalPlayer)
            {
                var sceneName = SceneManager.GetActiveScene().name;
                Debug.Log("SCENE:"+sceneName);
                if (sceneName == "ClientRoom")
                {
                    type = PlayerType.Player;
                }
                else if (sceneName == "StationRoom")
                {
                    type = PlayerType.Station;
                }
            }
        }

        [Command]
        public void CmdRequestGameCode(int connId, string location)
        {
            Debug.Log("REQUEST: GC");
            var cnm = (CustomNetworkManager) NetworkManager.singleton;
            //if (cnm.stationIdToLocation.ContainsValue(location)) // location exists
            //    return;
            var gc = cnm.GenerateGameCode();
            //gc = "PLPMGL";
            while (cnm.stationIdToLocation.ContainsKey(gc))
            {
                gc = cnm.GenerateGameCode();
                
            }
            cnm.AddNewStation(connId,gc,location);
            TargetSetStationData(gc);
        }
        [Command]
        public void IAmDead()
        {
            ((CustomNetworkManager) NetworkManager.singleton).IAmDead(connId);
        }

        public void Start()
        {
            //if(isLocalPlayer)
                //StartCoroutine(TEMP_printRooms());
                Debug.Log("START");
                if (isLocalPlayer)
                {
                    var sceneName = SceneManager.GetActiveScene().name;
                    Debug.Log("SCENE:"+sceneName);
                    if (sceneName == "ClientRoom")
                    {
                        type = PlayerType.Player;
                    }
                    else if (sceneName == "StationRoom")
                    {
                        type = PlayerType.Station;
                        CmdRequestRooms();
                    }
                }
            if (false && GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                var ds=FindObjectOfType<DataSync>();
                if (ds != null)
                {
                    playerName = ds.PlayerName;
                    type = ds.PlayerType;
                    if (type == PlayerType.Player)
                    {
                        Debug.Log("CALL OT SYUERE");
                        Debug.Log(connectionToServer.connectionId);
                        CmdSetPlayerName(connectionToServer.connectionId, playerName);
                    }
                }
            }
        }

        void UpdatePlayerName(string oldString,string newString)
        {
            playerName = newString;
            this.name = playerName;
        }

        public void SetConnId(int id)
        {
            connId = id;
        }
        void UpdateReady(bool oldBool,bool newBool)
        {
            ready = newBool;
        } 
        
        void UpdateLastLocation(string oldString,string newString)
        {
            lastLocation = newString;
        } 
        
        void UpdateCurrentLocation(string oldString,string newString)
        {
            currentLocation = newString;
            lastLocation = oldString;
            timeOfUpdateLocation = DateTime.Now.ToShortTimeString();
        }
        
        void UpdateCompletionCount(int oldInt,int newInt)
        {
            tasksCompleted = newInt;
        }

        private bool attemptedSet = false;
        public bool isDataFirstSent;


        /*
         * THIS I STH REAL ONE
         */
        [Command]
        public void CmdSetPlayerName(int connID,string newString)
        {
            var cnm = FindObjectOfType<CustomNetworkManager>();
            Debug.Log(connID);
            if (cnm.players.ContainsKey(newString))
            { // dupl name
                TargetNameFailedCallback();
                return;
            }
            if (!cnm.players.ContainsKey(newString) && !attemptedSet)
            {
                Debug.Log("connIDHEREX1");
                cnm.AddNewPlayer(connID, newString);
                attemptedSet = true;
            }
            else
            {
                Debug.Log("connIDHEREX2");
                cnm.ChangePlayerName(connID, newString);

            }
            
            TargetNameSuccessCallback();
        }
        
        
        [Command]
        public void CmdRequestRooms()
        {
            var cnm = FindObjectOfType<CustomNetworkManager>();
            var list = cnm.rooms;
            string[] strings = new string[list.Count];
            int i = 0;
            foreach (var e in list)
            {
                strings[i] = e;
                Debug.Log(strings[i]);
                i += 1;
            }
            TargetRoomDataCallback(strings);
        }

        [TargetRpc]
        private void TargetRoomDataCallback(string[] strings)
        {
            var dropdown = GameObject.FindWithTag("RoomsDropdown").GetComponent<Dropdown>();
            var opts = dropdown.options;//.Clear();
            opts.Clear();
            foreach (var s in strings)
            {
                
                opts.Add(new Dropdown.OptionData(s));
            }

            dropdown.RefreshShownValue();
        }

        [Command]
        public void CmdSetPlayerNamex(string newString)
        {
            var cnm = FindObjectOfType<CustomNetworkManager>();
            if (!cnm.playerNames.Contains(newString))
            {
                Debug.Log(cnm);
                Debug.Log(name);
                Debug.Log(name.GetType());
                playerName = newString;
                cnm.idToPlayer[int.Parse(this.name+"")] = this.playerName;
                cnm.UpdatePlayerLists();
                cnm.RemakePlayerPanel();
            }
        }

        public void SetPlayerName(string newString)
        {
            playerName = newString;
            this.name = playerName;
        }
        
        public void SetCurrentLocation(string newString = "Unknown")
        {
            SetLastLocation(currentLocation);
            currentLocation = newString;
            timeOfUpdateLocation = DateTime.Now.ToShortTimeString();
        }
        public void SetLastLocation(string newString = "Unknown")
        {
            lastLocation = newString;
        }

        public string timeOfUpdateLocation = "";
        
        void SetCompletionCount(int i)
        {
            tasksCompleted = i;
        }
        
        public void SetCompletionCount(int i,int max)
        {
            if(i<=max && i>=0)
                tasksCompleted = i;
        }

        public void Reset()
        {
            SetCompletionCount(0);
            currentLocation = "Unknown";
        }

        public string GetName()
        {
            return playerName;
        }

        private AsyncOperation sceneLoading;
        
        [ClientRpc]
        internal virtual void StartGame(int s, int l, string[] activeTaskList, string[] locations)
        {
            if(!isLocalPlayer) return;
            Debug.Log("StartGame");
            Debug.Log("SIZE:"+locations.Length);
            Debug.Log(name + " :: "+ type);
            if (type == PlayerType.Station) return; // maybe later we do something else
            if (type == PlayerType.AdminPanel) return; // maybe later we do something else
            foreach (var st in activeTaskList)
            {
                
                Debug.Log(s+ " < " + l +" < "+st);
            }
            switch (type)
            {
                case PlayerType.Player:
                    Debug.Log("LOCATIONS NOT NULL");
                    foreach (var loc in locations)
                    {
                        GameTask.locations.Add(loc);
                    }
                    Debug.Log("Attempt game task load");
                    GameTask.LoadGameTasksFromArray(activeTaskList, locations);
                    
                    sceneLoading=SceneManager.LoadSceneAsync("TaskListScene");
                    StartCoroutine(OnTaskListSceneLoaded(s,l));
                    
                    break;
                case PlayerType.AdminPanel:
                    break;
                // station doesn't really do anything here...
            }
        }
        
        [TargetRpc]
        internal virtual void StartGameThin(int s, int l, string[] activeTaskList,bool assignImposters, bool ghostsVisitStations,bool crewWinOnTasks, string[] imposterNames)
        {
            var settings = new AmongUsGoSettings
            {
                shortTasks = s,
                longTasks = l,
                assignImposters = assignImposters,
                ghostsVisitStations = ghostsVisitStations,
                crewWinsOnTaskCompletion = crewWinOnTasks
            };

            AmongUsGoSettings.singleton = settings;
            Debug.Log("StartGame");
            Debug.Log(name + " :: "+ type);
            if (type == PlayerType.Station) return; // maybe later we do something else
            if (type == PlayerType.AdminPanel) return; // maybe later we do something else
            if(!isLocalPlayer) 
                if(GameController.singleton!=null && GameController.singleton.playerData!=null)
                    GameController.singleton.playerData.StartGameThin(s,l,activeTaskList,assignImposters,ghostsVisitStations,crewWinOnTasks,imposterNames);
            /*foreach (var st in activeTaskList)
            {
                Debug.Log(s+ " < " + l +" < "+st);
            }//*/
            switch (type)
            {
                case PlayerType.Player:
                    Debug.Log("LOCATIONS IS NULL");
                    Debug.Log("< PLAYER TASK LIST >");
                    var tasks = new List<GameTask>();
                    this.imposterNames = imposterNames;
                    foreach (var t in activeTaskList)
                    {
                        Debug.Log("TASK: -- "+t);
                        var task = ParseTask(t); // this uses a different approach since ToString isn't the same as the file format (which is bad)
                        
                        tasks.Add(task);
                    }
                    Debug.Log("SIZE ?? "+tasks.Count);
                    firstTaskLoad = true;
                    
                    sceneLoading=SceneManager.LoadSceneAsync("TaskListScene");
                    StartCoroutine(OnTaskListSceneLoaded(tasks,imposterNames));
                    
                    break;
                case PlayerType.AdminPanel:
                    break;
                // station doesn't really do anything here...
            }
        }

        [Command]
        public void UpdatePlayerLocation(int id, string location)
        {
            var pd = ((CustomNetworkManager) NetworkManager.singleton).FindByConnId(id);
            if (pd != null)
            {
                Debug.Log("curLocation"+location);
                pd.currentLocation = location;
            }
            else
            {
                Debug.Log("Update Location Failed, player null");
            }
        }
        private GameTask ParseTask(string t)
        {
        
            Debug.Log(t);
            string[] t_data = t.Split(new string[]{" : "},StringSplitOptions.None);
            
            GameTask T = GameTaskFactory.create(t_data[0],t_data[1],t_data[2],t_data[3],t_data[4]);
            Debug.Log(T);
            return T;
        }

        private IEnumerator OnTaskListSceneLoaded(int s,int l)
        {
            yield return new WaitUntil(TaskSceneLoaded);
            
            var tasks = GameTask.getGameTasksForArray(s,l);
            var gc = FindObjectOfType<GameController>();
            gc.tasks = tasks;
            FindObjectOfType<TaskAdder>().AddTasks();
        }
        
        private IEnumerator OnTaskListSceneLoaded(List<GameTask> tasks,string[] imposterNames)
        {
            string iNames = "";
            if(imposterNames!=null)
            {
                bool twoCol = (imposterNames == null ? false : imposterNames.Length > 10);
                bool left = true;
                foreach (var n in imposterNames)
                {
                    if (n.Equals(playerName)) continue;
                    if (twoCol)
                    {
                        if (left)
                        {
                            left = false;
                            iNames += n + "   ";
                        }
                        else
                        {
                            left = true;
                            iNames += n + "\n";
                        }
                    }
                    else
                    {
                        iNames += n + "\n";
                    }
                }

                if (iNames == "") iNames = "No one :(";
            }
            yield return new WaitUntil(TaskSceneLoaded);
            var playerRoleCanvas = FindObjectOfType<PlayerRoleCanvas>();
            playerRoleCanvas.Set(isImposter,iNames);
            if(firstTaskLoad && AmongUsGoSettings.singleton.assignImposters)
                StartCoroutine(playerRoleCanvas.Show());
            firstTaskLoad = false;
            var gc = FindObjectOfType<GameController>();
            gc.tasks = tasks;
            FindObjectOfType<TaskAdder>().AddTasks();
        }

        public void ReloadTaskListScene(List<GameTask> tasks)
        {
            FindObjectOfType<GameController>().tasks = tasks;
            sceneLoading=SceneManager.LoadSceneAsync("TaskListScene");
            StartCoroutine(OnTaskListSceneLoaded(tasks,imposterNames));
        }

        public static PlayerData FindLocalPlayer()
        {
            foreach (var pd in FindObjectsOfType<PlayerData>())
            {
                if (pd.isLocalPlayer) return pd;
            }

            return null;
        }
        private IEnumerator OnTaskListSceneReloaded(List<GameTask> tasks)
        {
            yield return new WaitUntil(TaskSceneLoaded);
            
            var gc = FindObjectOfType<GameController>();
            gc.tasks = tasks;
            FindObjectOfType<TaskAdder>().AddTasks();
        }
        public bool TaskSceneLoaded()
        {
            return sceneLoading.isDone;
        }
        public string GetCurrentLocation()
        {
            return currentLocation;
        }
        
        public string GetLastLocation()
        {
            return lastLocation;
        }

        [TargetRpc]
        public void TargetSetPlayerDatax(NetworkConnection conn)
        {
            Debug.Log("CALLED");
            var ds = FindObjectOfType<DataSync>();

            playerName = ds.PlayerName;
            type = ds.PlayerType;

            CMDValidatePlayerData(conn.connectionId,playerName,type);

        }
        [TargetRpc]
        public void TargetSetPlayerData(int conn)
        {
            Debug.Log("CALLED ID");


            CMDValidatePlayerData(conn,playerName,type);

        }

        [Command]
        public void CMDValidatePlayerData(int conn,string name,PlayerType type)
        {

            var man = ((CustomNetworkManager) NetworkManager.singleton);
            if (man.players.Keys.Contains(name))
            {
                TargetTellClientAboutName(false);
            }
            else
            {
                TargetTellClientAboutName(true);
                var old = man.idToPlayer[conn];
                man.playerNames.Remove(old);
                man.players[name] = man.players[old];
                man.idToPlayer[conn] = name;
                man.players[name].GetComponent<PlayerData>().playerName = name;
                man.players[name].GetComponent<PlayerData>().type = type;
                man.UpdatePlayerLists();
                Debug.Log("Server done with update");
            }

            
        }

        [TargetRpc]
        public void TargetTellClientAboutName(bool b)
        {
            if (b)
            {
                // success, carry on and load scene
                Debug.Log("Name did not have a match! :: Success");
                
            }

            else
            {
                // failed, show error
                Debug.Log("Name had a match... :: failed");
                
            }
        }
        public override void OnStartClient()
        {
            if (isLocalPlayer)
            {
                if (connectionToClient != null)
                {
                    Debug.Log(connectionToClient);
                    connId = connectionToClient.connectionId;
                    playerName = "Player [" + connId + "]";
                    
                }else if (connectionToServer != null)
                {
                    Debug.Log(connectionToServer);
                    connId = connectionToServer.connectionId;
                    playerName = "Player [" + connId + "]";
                }
                else
                {
                    Debug.Log("No connection to either client or server on Start");
                }
            }
        }
        [TargetRpc]
        public void TargetNameFailedCallback()
        {
            var ccf = GetComponent<CustomCanvasFlipper>();
            ccf.ShowCanvas1ForSeconds(4);
        }
        [TargetRpc]
        public void TargetNameSuccessCallback()
        {
            var ccf = GetComponent<CustomCanvasFlipper>();
            ccf.Close(GameObject.FindWithTag("SetNameCanvas").GetComponent<Canvas>());
            ccf.Open(GameObject.FindWithTag("WaitingForGameCanvas").GetComponent<Canvas>());
        }

        [TargetRpc]
        public void TargetSetStationData(string gamecode)
        {
            playerName = gamecode;
            name = gamecode;
            isDataFirstSent = true;
            // update the station code text
            var gct = GameObject.FindWithTag("GameCodeText").GetComponent<Text>();
            var nogct = GameObject.FindWithTag("NoGameCodeText").GetComponent<Text>();
            gct.text=gamecode;
            gct.enabled = true;
            nogct.enabled = false;
            Debug.Log("GAMECODE" + gamecode);
        }
        
        [TargetRpc]
        public void TargetSetStationInitialData(NetworkIdentity conn,string gameCode)
        {
        }
        
        /*
         * Game Controller stuff
         */

        [Command]
        public void TaskComplete(string s)
        {
            tasksCompleted = Mathf.Min(tasksCompleted + 1,
                AmongUsGoSettings.singleton.longTasks + AmongUsGoSettings.singleton.shortTasks);
            var cnm = ((CustomNetworkManager) NetworkManager.singleton);
            
            var resp = cnm.SendToServer(s);
            if (cnm.CheckWin())
                resp = "WIN--C";
            
            if (!resp.StartsWith("NONE--"))
                RespondCall(resp);
        }
        [Command]
        public void SendToServer(string s)
        {
            
            var resp = ((CustomNetworkManager)NetworkManager.singleton).SendToServer(s);
            
            if (!resp.StartsWith("NONE--"))
                RespondCall(resp);
        }

        [TargetRpc]
        public void RespondCall(string s)
        {
            string resp = FindObjectOfType<GameController>().GetResponse(s);
            if (!resp.StartsWith("NONE--"))
                SendToServer(resp);
        }

        [TargetRpc]
        public void MakeLocalPlayer()
        {
            GameController.singleton.playerData = this;
        }

        private Canvas clientVotingCanvas;
        private bool firstTaskLoad;
        private string[] imposterNames;

        [TargetRpc]
        public void CallVote()
        {
            if(!isLocalPlayer) return;
            
            // Show the panel
            if(clientVotingCanvas==null)
                clientVotingCanvas = GameObject.FindWithTag("ClientVotingCanvas").GetComponent<Canvas>();
            clientVotingCanvas.enabled = true;
            
            ReloadTaskListScene(FindObjectOfType<GameController>().tasks);
        }

        [TargetRpc]
        public void AlertToGameEnd()
        {
            sceneLoading=SceneManager.LoadSceneAsync("ClientRoom");
            
            StartCoroutine(OnClientRoomSceneLoaded());
            // must update client room with 
        }

        IEnumerator OnClientRoomSceneLoaded()
        {
            yield return new WaitUntil(TaskSceneLoaded);
            if(clientVotingCanvas!=null)
                clientVotingCanvas.enabled = false;
            if (connId != 0)
            {
                GameObject.FindWithTag("SetNameCanvas").GetComponent<Canvas>().enabled = false;
                GameObject.FindWithTag("WaitingForGameCanvas").GetComponent<Canvas>().enabled = true;
            }
            // set the correct thing so that the player isn't prompted to enter a name again
        }

        [TargetRpc]
        public void ResumeGame()
        {
            if(clientVotingCanvas!=null)
                clientVotingCanvas.enabled = false;
        }


        [TargetRpc]
        public void ImpostersWin()
        {
            FindObjectOfType<GameController>().OpenIWinPanel();
        }


        [TargetRpc]
        public void CrewmatesWin()
        {
            FindObjectOfType<GameController>().OpenWinPanel();
        }

        [TargetRpc]
        public void ForceClosedWinPanel()
        {
            FindObjectOfType<GameController>().WinPanelOnClick();
        }
    }
}