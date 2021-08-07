using System;
using System.Collections.Generic;
using System.IO;
using StationsAndHubs.Scripts.GameTasks;
using UnityEngine;

namespace StationsAndHubs.Scripts
{
    public class Initializer
    {
        public const string DEFAULT_TASKS = "Default.agt";
        public const string workingDirName = "AmongUs_Go";
        public const string taskFilesDir = "tasks";
        public const string audioFilesDir = "sounds";
        public const string settingsFile = ".settings";
        public static string workingDirectory = Environment.GetFolderPath(
            Environment.SpecialFolder.UserProfile)+"/"+workingDirName;
        public static void Init()
        {
            MakeDirectory();
            new AmongUsGoSettings();
            AmongUsGoSettings.singleton.LoadSettings();
            GameTask.taskFiles = GetTaskFiles();
        }

        private static void MakeDirectory()
        {
            string[] paths = new string[]
            {
                workingDirectory,
                workingDirectory + "/" + taskFilesDir,
                workingDirectory + "/" + audioFilesDir,
            };

            Debug.Log("WORKING DIR"+workingDirectory);
            foreach (var path in paths)
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            
            string[] files = new string[]
            {
                workingDirectory+"/"+taskFilesDir+"/"+DEFAULT_TASKS,
                workingDirectory+"/"+settingsFile,
            };
            foreach (var path in files)
            {
                Debug.Log("newFile to create : "+path);
                if (!File.Exists(path))
                    CreateFile(path);
            }
            
        }

        private static void CreateFile(string path)
        {
            
            if (File.Exists(path)) return;
            FileStream fs = File.Create(path);
            
            if (path.Contains("/"))
            {
                string[] spl = path.Split('/');
                path = spl[spl.Length - 1];
            }


            switch (path)
            {
                case settingsFile:
                    //if (AmongUsGoSettings.singleton == null)
                    //    new AmongUsGoSettings();
                    break;
                case DEFAULT_TASKS:
                    CopyDefaultTaskFileFromAssets(fs);
                    break;
            }
            fs.Close();
        }

        private static void CopyDefaultTaskFileFromAssets(FileStream fileStream)
        {
            string content = Resources.Load<TextAsset>("Docs/Default").text;
            StreamWriter sw = new StreamWriter(fileStream);
            sw.Write(content);
            
            
            foreach (var line in content.Split('\n'))
            {
                Debug.Log(line+"< defaultFile");
            }
            
            sw.Close();
        }

        private static string[] GetTaskFiles()
        {
            string path = workingDirectory+"/"+taskFilesDir;//Hub.WORKING_DIRECTORY+File.separator+Hub._DEFAULT_TASK_FILE_LOCATION+File.separator;

            var taskFileNames = Directory.GetFiles(path, "*.agt");

            for (int i = 0; i<taskFileNames.Length; i++)
            {
                string fullPath = taskFileNames[i];
                fullPath = fullPath.Substring(fullPath.LastIndexOf("/", StringComparison.Ordinal) + 1);
                taskFileNames[i] = fullPath.Substring(0,fullPath.LastIndexOf(".", StringComparison.Ordinal));
            }
            
            return taskFileNames;
        }

        public static void LoadTasks(string taskFileName)
        {
            string path = workingDirectory + "/"+taskFilesDir +"/"+taskFileName;
            
            //Read the text from directly from the test.txt file
            StreamReader reader = new StreamReader(path); 
            Debug.Log(reader.ReadToEnd());
            reader.Close();
        }

        public static List<string> LoadRooms()
        {
            List<string> rooms = new List<string>();
            
            
            
            return rooms;
        }
    }

    public class AmongUsGoSettings
    {

        public static AmongUsGoSettings singleton;

        public AmongUsGoSettings()
        {
            singleton = this;
        }
        
        public int shortTasks = 4;
        public int longTasks = 2;
        public int numImposters = 2;
        public int votingTime = 120;
        public string alarm = "Echo";
        public string taskListName = "Default";
        public bool assignImposters = false;
        public bool ghostsVisitStations = false;
        public bool crewWinsOnTaskCompletion = false;
        public bool useAsAdminPanel = false;
        public bool showRoleOnDead = false;
        public bool adminPanelOnlyShowAtStations = true;

        // putting the saveable settings in here.
        public void LoadSettings()
        {
            string filePath = Initializer.workingDirectory + "/" + Initializer.settingsFile;
            // read this from file.
            if (!File.Exists(filePath))
            {
                File.Create(filePath);
                
                SaveSettings();
                return;
            }
            StreamReader sr = new StreamReader(filePath);
            
            string line;
            
            int i;
            bool b;
            
            while ((line = sr.ReadLine()) != null)
            {
                string[] spl = line.Split(':');
                string val = spl[1];
                switch (spl[0])
                {
                    case "shortTasks":
                        if (int.TryParse(val,out i))
                        {
                            shortTasks = i;
                        }
                        break;
                    case "longTasks":
                        if (int.TryParse(val,out i))
                        {
                            longTasks = i;
                        }
                        break;
                    case "numImposters":
                        if (int.TryParse(val,out i))
                        {
                            numImposters = i;
                        }
                        break;
                    case "votingTime":
                        if (int.TryParse(val,out i))
                        {
                            votingTime = i;
                        }
                        break;
                    case "alarm":
                        alarm = val;
                        break;
                    case "taskListName":
                        taskListName = val;
                        break;
                    case "assignImposters":
                        if (bool.TryParse(val,out b))
                        {
                            assignImposters = b;
                        }
                        break;
                    case "ghostsVisitStations":
                        if (bool.TryParse(val,out b))
                        {
                            ghostsVisitStations = b;
                        }
                        break;
                    case "crewWinsOnTaskCompletion":
                        if (bool.TryParse(val,out b))
                        {
                            crewWinsOnTaskCompletion = b;
                        }
                        break;
                    case "useAsAdminPanel":
                        if (bool.TryParse(val,out b))
                        {
                            useAsAdminPanel = b;
                        }
                        break;
                    case "showRoleOnDead":
                        if (bool.TryParse(val,out b))
                        {
                            showRoleOnDead = b;
                        }
                        break;
                }
            }
            sr.Close();
        }
        
        public void SaveSettings()
        {
            string text = "shortTasks:"+shortTasks+"\n"+
                          "longTasks:"+longTasks+"\n"+
                          "numImposters:"+numImposters+"\n"+
                          "votingTime:"+votingTime+"\n"+
                          "alarm:"+alarm+"\n"+
                          "taskListName:"+taskListName+"\n"+
                          "assignImposters:"+assignImposters+"\n"+
                          "ghostsVisitStations:"+ghostsVisitStations+"\n"+
                          "crewWinsOnTaskCompletion:"+crewWinsOnTaskCompletion+"\n"+
                          "useAsAdminPanel:"+useAsAdminPanel+"\n"+
                          "showRoleOnDead:"+showRoleOnDead+"\n";

            var sw = new StreamWriter(Initializer.workingDirectory + "/" + Initializer.settingsFile);
            sw.Write(text);
            sw.Flush();
            sw.Close();
        }
        
        public void SetSettings(int shortTasks=-1, int longTasks=-1,int numImposters=-1, int? votingTime=null, string alarm="", string taskListName="",
            bool? assignImposters=null,bool? ghostsVisitStations=null,bool? crewWinsOnTaskCompletion=null,bool? useAsAdminPanel=null)
        {
            if (shortTasks > -1)
            {
                this.shortTasks = shortTasks;
            }
            if (longTasks > -1)
            {
                this.longTasks = longTasks;
            }
            if (numImposters > -1)
            {
                this.numImposters = numImposters;
            }

            if (votingTime != null)
            {
                this.votingTime = (int)votingTime;
            }

            if (alarm != "")
            {
                this.alarm = alarm;
            }
            
            if (taskListName != "")
            {
                this.taskListName = taskListName;
            }

            if (assignImposters!=null)
            {
                this.assignImposters = (bool)assignImposters;
            }
        
            if (ghostsVisitStations!=null)
            {
                this.ghostsVisitStations = (bool)ghostsVisitStations;
            }
        
            if (crewWinsOnTaskCompletion!=null)
            {
                this.crewWinsOnTaskCompletion = (bool)crewWinsOnTaskCompletion;
            }
        
            if (useAsAdminPanel!=null)
            {
                this.useAsAdminPanel = (bool)useAsAdminPanel;
            }
            
        }

        
    }
}
