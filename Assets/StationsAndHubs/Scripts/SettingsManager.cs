using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace StationsAndHubs.Scripts
{
    public class SettingsManager : MonoBehaviour
    {
        public InputField nImposters;
        public InputField voteTime;
        public InputField shortTasks;
        public InputField longTasks;
        public Toggle assignImposters;
        public Toggle ghostsVisitStations;
        public Toggle crewWinOnTasks;
        public Toggle useAsAdmin;
        public Toggle showRoleOnVoteOut;
        public Dropdown alarm;
        public Dropdown tasklist;
        
        private void SetCurrentValues()
        {
            var sing = AmongUsGoSettings.singleton;
            nImposters.text = sing.numImposters + "";
            voteTime.text = sing.votingTime + "";
            shortTasks.text = sing.shortTasks + "";
            longTasks.text = sing.longTasks + "";
            assignImposters.isOn = sing.assignImposters;
            ghostsVisitStations.isOn = sing.ghostsVisitStations;
            crewWinOnTasks.isOn = sing.crewWinsOnTaskCompletion;
            useAsAdmin.isOn = sing.useAsAdminPanel;
            showRoleOnVoteOut.isOn = sing.showRoleOnDead;
            var opts = (alarm.options.ToArray());
            int i = 0;
            foreach (var o in  opts)
            {
                if (o.text.Equals(sing.alarm,StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }

                i += 1;
            }

            alarm.value = i;
            alarm.RefreshShownValue();
            opts = (tasklist.options.ToArray());
            i = 0;
            foreach (var o in  opts)
            {
                if (o.text.Equals(sing.taskListName,StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }

                i += 1;
            }

            tasklist.value = i;
            tasklist.RefreshShownValue();
        }

        private bool firstLoad = true;
        private void Start()
        {
            SetPanel();
        }

        public void SaveSettings()
        {
            LoadSettingsFromPanel();
            AmongUsGoSettings.singleton.SaveSettings();
        }

        private void SetPanel()
        {
            var sing = AmongUsGoSettings.singleton;
            nImposters.text = sing.numImposters+"";
            voteTime.text = sing.votingTime+"";
            shortTasks.text = sing.shortTasks+"";
            longTasks.text = sing.longTasks + "";
            assignImposters.isOn = sing.assignImposters;
            ghostsVisitStations.isOn = sing.ghostsVisitStations;
            crewWinOnTasks.isOn = sing.crewWinsOnTaskCompletion;
            useAsAdmin.isOn = sing.useAsAdminPanel;
            showRoleOnVoteOut.isOn = sing.showRoleOnDead;
            
            // get alarm position
            int i = 0;
            var opts = alarm.options;
            for(i=0;i<opts.Count; i++)
            {
                if (opts[i].text.Equals(sing.alarm))
                {
                    break;
                }
            }
            alarm.value = i;
            alarm.RefreshShownValue();
            i = 0;
            opts = tasklist.options;
            for(i=0;i<opts.Count; i++)
            {
                if (opts[i].text.Equals(sing.taskListName))
                {
                    break;
                }
            }
            tasklist.value = i;
            tasklist.RefreshShownValue();
        }

        public void SetAssignImposters(Toggle b)
        {
            Debug.Log(b.isOn);
            AmongUsGoSettings.singleton.assignImposters = b;
        }
        
        public void SetAlarm(Dropdown d)
        {
            AmongUsGoSettings.singleton.alarm = d.options[d.value].text;
        }
        public void SetGhostsVisitStations(Toggle b)
        {
            AmongUsGoSettings.singleton.ghostsVisitStations = b;
        }
        
        public void SetAsAdminPanel(Toggle b)
        {
            AmongUsGoSettings.singleton.useAsAdminPanel = b;
        }
        
        public void SetLongTasks(Text text)
        {
            int l;
            if(int.TryParse(text.text,out l))
            {
                AmongUsGoSettings.singleton.longTasks = l;
            }
            else
            {
                // throw error, maybe tell the user or reset the value in the text
                text.text = AmongUsGoSettings.singleton.longTasks+"";
            }
            
        }
        public void SetShortTasks(Text text)
        {
            int l;
            if(int.TryParse(text.text,out l))
            {
                AmongUsGoSettings.singleton.shortTasks = l;
            }
            else
            {
                // throw error, maybe tell the user or reset the value in the text
                text.text = AmongUsGoSettings.singleton.shortTasks+"";
            }
            
        }
        
        public void SetCrewWinsOnCompletion(Toggle b)
        {
            AmongUsGoSettings.singleton.crewWinsOnTaskCompletion = b;
        }
        public void SetNumImposters(Text text)
        {
            int n;
            if(int.TryParse(text.text,out n))
            {
                AmongUsGoSettings.singleton.numImposters = n;
            }
            else
            {
                // throw error, maybe tell the user or reset the value in the text
                text.text = AmongUsGoSettings.singleton.numImposters+"";
            }
        }
        public void SetVotingTime(Text text)
        {
            int n;
            if(int.TryParse(text.text,out n))
            {
                AmongUsGoSettings.singleton.votingTime = n;
            }
            else
            {
                // throw error, maybe tell the user or reset the value in the text
                text.text = AmongUsGoSettings.singleton.votingTime+"";
            }
        }
        public void SetTaskListName(Dropdown dropdown)
        {
            var cnm = (CustomNetworkManager) NetworkManager.singleton;
            if (cnm.stationIdToLocation.Keys.Count > 0)
            {
                // TODO: probably want to show alert here...
                // Fails because we don't want to change the task list
                // after stations have connected (though we could disconnect them with
                // an alert if the location is not in the task list file
                // thats way more complicated though...
                dropdown.value = GetDropdownIndex(dropdown,AmongUsGoSettings.singleton.taskListName);
            }
            else
            {
                AmongUsGoSettings.singleton.taskListName = dropdown.options[dropdown.value].text;
            }
        }

        private int GetDropdownIndex(Dropdown options, string singletonTaskListName)
        {
            List<Dropdown.OptionData> opts = options.options;
            int i = 0;
            foreach (var o in opts)
            {
                if (o.text.Equals(singletonTaskListName, StringComparison.InvariantCultureIgnoreCase))
                    return i;
                i += 1;

            }

            return options.value;
        }

        public void LoadSettingsFromPanel()
        {
            AmongUsGoSettings.singleton.alarm=alarm.options[alarm.value].text;
            AmongUsGoSettings.singleton.taskListName=tasklist.options[tasklist.value].text;
            int i;
            if (int.TryParse(longTasks.text, out i))
                AmongUsGoSettings.singleton.longTasks = i;
            if (int.TryParse(shortTasks.text, out i))
                AmongUsGoSettings.singleton.shortTasks = i;
            if (int.TryParse(voteTime.text, out i))
                AmongUsGoSettings.singleton.votingTime = i;
            if (int.TryParse(nImposters.text, out i))
                AmongUsGoSettings.singleton.numImposters = i;
            AmongUsGoSettings.singleton.assignImposters = assignImposters.isOn;
            AmongUsGoSettings.singleton.ghostsVisitStations = ghostsVisitStations.isOn;
            AmongUsGoSettings.singleton.showRoleOnDead = showRoleOnVoteOut.isOn;
            AmongUsGoSettings.singleton.useAsAdminPanel = useAsAdmin.isOn;
            AmongUsGoSettings.singleton.crewWinsOnTaskCompletion = crewWinOnTasks.isOn;

        }
    }
}