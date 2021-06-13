using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        int tasksCompleted;
        [SyncVar]
        public int connId;

        [SyncVar(hook = nameof(UpdateReady))]
        public bool ready;

        private List<GameTask> tasks;

        public void Awake()
        {
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
            while (cnm.stationIdToLocation.ContainsKey(gc))
            {
                gc = cnm.GenerateGameCode();
            }
            cnm.AddNewStation(connId,gc,location);
            TargetSetStationData(gc);
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
        
        
        public void SetLastLocation(string newString)
        {
            lastLocation = newString;
        } 
        
        public void SetCurrentLocation(string newString)
        {
            
            currentLocation = newString;
        }
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
            SetLastLocation("None");
            SetCurrentLocation("None");
        }

        public string GetName()
        {
            return playerName;
        }

        [ClientRpc]
        internal virtual void StartGame(int s,int l,string activeTaskList)
        {
            Debug.Log(s+ " < " + l +" < "+activeTaskList);
            switch (type)
            {
                case PlayerType.Player:
                    tasks = GameTask.getGameTasks(s,l,activeTaskList);
                    foreach (var VARIABLE in tasks)
                    {
                        Debug.Log(playerName+ " : "+VARIABLE);
                    }
                    SceneManager.LoadScene("TaskListScene");
                    break;
                case PlayerType.AdminPanel:
                    break;
                // station doesn't really do anything here...
            }
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

        
    }
}