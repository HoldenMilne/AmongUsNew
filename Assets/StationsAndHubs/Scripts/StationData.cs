using Mirror;
using UnityEngine.SceneManagement;

namespace StationsAndHubs.Scripts
{
    public class StationData : PlayerData
    {
        [SyncVar(hook = nameof(SetInspectorVarCode))]
        public string code;
        [SyncVar(hook = nameof(SetInspectorVarLocation))]
        public string location;

        public string visibleCode;
        public string visibleLoc;

        private void SetInspectorVarCode(string oldValue, string newValue)
        {
            visibleCode = newValue;
        }
        private void SetInspectorVarLocation(string oldValue, string newValue)
        {
            visibleLoc = newValue;
        }
        public new void Start()
        {
            base.Start();
        }

        [Command]
        public void RequestStationDataUpdate()
        {
            
        }
        
        //internal override void StartGame()
        //{
         //   SceneManager.LoadScene("StationScene");
        //}

        [TargetRpc]
        public void GiveGameCode(string gc)
        {
            code = gc;
        }
    }
}