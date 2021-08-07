using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace StationsAndHubs.Scripts.GameTasks
{
    public abstract class GameTask : Task // I forgot I had Task already so I should really fix that...
    {
    private const int ID = 0;
    private const int NAME = 1;
    private const int LOCATION = 2;
    private const int TYPE = 3;
    private const int NEXT = 4;
    public static List<LongTask> longGameTasks;
    public static List<ShortTask> shortGameTasks;
    public static List<ChainTask> chainGameTasks;
    public static List<string> locations = new List<string>();

    public bool isRandomRoom = false;
    public string next;
    public static string[] taskFiles;
    private string taskType;

    public GameTask(string id, string name, string location, string next="", string taskType = "short") : base(id,name,location){
        this.id=id;
        this.name=name;
        if(location.Equals("RANDOM ROOM",StringComparison.InvariantCultureIgnoreCase))
            isRandomRoom = true;
        this.location = location;
        this.next = next;
        this.taskType = taskType;
    }

    public static bool removeLocation(string line) {
        string loc = null;
        foreach(string l in locations)
        {
            if(l.Equals(line,StringComparison.InvariantCultureIgnoreCase))
            {
                loc = l;
                break;
            }
        }
        if(loc!=null)
            locations.Remove(loc);
        else
        {
            foreach(string l in locations)
            {
                if(l.Equals("RANDOM ROOM#"+line,StringComparison.InvariantCultureIgnoreCase))
                {
                    loc = l;
                    break;
                }
            }
        }

        if(loc==null) return false;


        locations.Remove(loc);

        return true;
    }

    public void Completed()
    {

    }

    public bool isLong()
    {
        return this.GetType()==typeof(LongTask);
    }

    public static void LoadGameTasks(string taskFileName)
    {

        shortGameTasks = new List<ShortTask>();
        longGameTasks = new List<LongTask>();
        chainGameTasks = new List<ChainTask>();

        // TODO: Fix scanner usage.  This will need to get replaced with a different kind of stream reader probably.
        //File f = new File(taskFileName);
        StreamReader sc = null;
        
        try {
            sc = new StreamReader(Initializer.workingDirectory+"/"+Initializer.taskFilesDir+"/"+taskFileName+ ".agt");
            
            string line;
            while((line = sc.ReadLine())!=null)
            {
                while(line[0]==' ')
                    line = line.Substring(1);

                if(line[0] == '#')
                    continue;

                Debug.Log("load: "+line);
                GameTask task = parseGameTaskFromLine(line);

                if(!locations.Contains(task.location) && !task.isRandomRoom) //Uncomment this to check if locations are valid.
                    continue;

                if(task.isRandomRoom)
                    task.location = GetRandomLocation();

                if(task.GetType() ==  typeof(ShortTask))
                {
                    shortGameTasks.Add((ShortTask) task);
                }else if(task.GetType() ==  typeof(LongTask))
                {
                    longGameTasks.Add((LongTask) task);
                }else {
                    chainGameTasks.Add((ChainTask) task);
                }
            }
            

            ClearChainTasksWithInvalidLocations(); // must come first
            ClearShortTasksWithInvalidLocations();
            ClearLongTasksWithInvalidLocations();
        } catch (FileNotFoundException e) {
            Debug.Log("ERROR Occured on Game Task Loading from "+taskFileName);
            Debug.Log(e.Message);
            Debug.Log(e.StackTrace);
            Debug.Log(e.Source);
        }

        sc?.Close();

    }
    
    public static void LoadGameTasksFromArray(string[] taskFileName, string[] locs)
    {

        locations.Clear();
        
        foreach (var l in locs)
        {
            locations.Add(l);
        }
        shortGameTasks = new List<ShortTask>();
        longGameTasks = new List<LongTask>();
        chainGameTasks = new List<ChainTask>();

        // TODO: Fix scanner usage.  This will need to get replaced with a different kind of stream reader probably.
        //File f = new File(taskFileName);
            
        foreach(string l in taskFileName)
        {
            string line = l;
            while(line[0]==' ')
                line = line.Substring(1);

            if(line[0] == '#')
                continue;

            Debug.Log("load: "+line);
            GameTask task = parseGameTaskFromLine(line);

            if(!locations.Contains(task.location) && !task.isRandomRoom) //Uncomment this to check if locations are valid.
                continue;

            if(task.isRandomRoom)
                task.location = GetRandomLocation();

            if(task.GetType() ==  typeof(ShortTask))
            {
                shortGameTasks.Add((ShortTask) task);
            }else if(task.GetType() ==  typeof(LongTask))
            {
                longGameTasks.Add((LongTask) task);
            }else {
                chainGameTasks.Add((ChainTask) task);
            }
        }
        

        ClearChainTasksWithInvalidLocations(); // must come first
        ClearShortTasksWithInvalidLocations();
        ClearLongTasksWithInvalidLocations();
       

    }

    private static string GetRandomLocation()
    {
        List<string> rrlocs = new List<string>();
        List<string> deflocs = new List<string>();

        Debug.Log("ERR SIZE"+locations.Count);
        foreach(string loc in locations)
        {
            if(loc.StartsWith("RANDOM ROOM#"))
            {
                rrlocs.Add(loc.Substring(loc.IndexOf("#")+1));
            }else{
                deflocs.Add(loc);
            }
        }

        if(rrlocs.Count == 0)
        {
            return getRandomElement(deflocs);
        }else{
            int r = (int)(UnityEngine.Random.Range(0,3));
            if(r==0)
            {
                return getRandomElement(deflocs);
            }
        }
        return getRandomElement(rrlocs);

    }

    static string getRandomElement(List<string> l)
    {
        int r = (int)(UnityEngine.Random.Range(0,l.Count));
        return l[r];
    }

    private static List<ShortTask> ClearShortTasksWithInvalidLocations() {
        List<GameTask> removetasks = new List<GameTask>();
        foreach(GameTask t in shortGameTasks)
        {
            if(!t.next.Equals("nul",StringComparison.InvariantCultureIgnoreCase))
            {
                GameTask t2 = findGameTaskByID(t.next);

                if(t2!= null && (locations.Contains(t2.location) || locations.Contains("RANDOM ROOM#"+t2.location)))//&& chainGameTasks.Contains(t2))
                    continue;

                if(t2!=null)
                    removetasks.Add(t2);
                removetasks.Add(t);
            }
        }
        foreach(var gameTask in removetasks)
        {
            var t = (ShortTask) gameTask;
            if(shortGameTasks.Contains(t))
                shortGameTasks.Remove(t);
        }
        return shortGameTasks;
    }
    private static List<LongTask> ClearLongTasksWithInvalidLocations() {
        List<GameTask> removetasks = new List<GameTask>();
        foreach(GameTask t in longGameTasks)
        {
            if(!t.next.Equals("nul",StringComparison.InvariantCultureIgnoreCase))
            {
                GameTask t2 = findGameTaskByID(t.next);
                if(t2!= null && (locations.Contains(t2.location)||locations.Contains("RANDOM ROOM#"+t2.location)))// && chainGameTasks.Contains(t2))
                    continue;
                if(t2!=null)
                    removetasks.Add(t2);
                removetasks.Add(t);
            }
        }
        foreach(var gameTask in removetasks)
        {
            var t = (LongTask) gameTask;
            if(longGameTasks.Contains(t))
                longGameTasks.Remove(t);
        }
        return longGameTasks;
    }
    private static List<ChainTask> ClearChainTasksWithInvalidLocations() {
        List<GameTask> removetasks = new List<GameTask>();
        foreach(var t in chainGameTasks)
        {
            Debug.Log(t+ " <CHAINTS");
            if(!t.next.Equals("nul",StringComparison.InvariantCultureIgnoreCase))
            {
                GameTask t2 = findGameTaskByID(t.next);
                if(t2!= null && (locations.Contains(t2.location)||locations.Contains("RANDOM ROOM#"+t2.location)))
                    continue;
                if(t2!=null)
                    removetasks.Add(t2);
                removetasks.Add(t);
            }
        }
        foreach(var gameTask in removetasks)
        {
            var t = (ChainTask) gameTask;
            if(chainGameTasks.Contains(t))
               chainGameTasks.Remove(t);
        }
        return chainGameTasks;
    }

    public static GameTask parseGameTaskFromLine(string line) {
        line = line.Replace(", ",",");
        Debug.Log(line);
        string[] tokens = line.Split(',');

        return GameTask.GameTaskFactory(tokens[GameTask.ID],tokens[GameTask.NAME],tokens[GameTask.LOCATION],tokens[GameTask.TYPE],tokens[GameTask.NEXT]);
    }

    private static GameTask GameTaskFactory(string ID, string name, string location, string type, string next) {
        if(type.Equals("Short Task",StringComparison.InvariantCultureIgnoreCase))
        {
            return new ShortTask(ID,name,location,next);
        }else if(type.Equals("Long Task",StringComparison.InvariantCultureIgnoreCase))
        {
            return new LongTask(ID,name,location,next);

        }

        return new ChainTask(ID,name,location,next);

    }

    public static string[] LoadTaskStringArray(string taskFileName)
    {
        List<string> tasks = new List<string>();
        StreamReader sc = null;

        try
        {
            sc = new StreamReader(Initializer.workingDirectory + "/" + Initializer.taskFilesDir + "/" + taskFileName+".agt");

            string line;
            while ((line = sc.ReadLine()) != null)
            {
                while (line[0] == ' ')
                    line = line.Substring(1);

                if (line[0] == '#')
                    continue;

                Debug.Log("load: " + line);
                tasks.Add(line);
            }

            sc.Close();

        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            Debug.Log(e.Data);
            Debug.Log(e.StackTrace);
        }

        string[] strings = new string[tasks.Count] ;
        int i = 0;
        foreach (var t in tasks)
        {
            strings[i] = t;
            i += 1;
        }

        return strings;
    }
    // todo: this always returns all_tasks.count = 0 for some reason...
    // todo: !! BECAUSE this is called on the client, but the client doesn't have any tasks loaded.  Instead, we'll need to get the tasks and
    // todo: pass the ToString value of each task into an array and send that into the StartGame clientRpc.
    public static List<GameTask> getGameTasks(int s, int l,string file)
    {
        if(longGameTasks==null || shortGameTasks == null || chainGameTasks == null) LoadGameTasks(file);
        List<LongTask> l_GameTasks = new List<LongTask>();
        foreach(GameTask t in longGameTasks)
        {
            l_GameTasks.Add((LongTask) t);
        }
        List<ShortTask> s_GameTasks = new List<ShortTask>();
        foreach(GameTask t in shortGameTasks)
        {
            s_GameTasks.Add((ShortTask) t);
        }

        List<GameTask> all_tasks = new List<GameTask>();
        while(s>0 && s_GameTasks.Count>0)
        {
            ShortTask t = s_GameTasks[UnityEngine.Random.Range(0, s_GameTasks.Count)];
            s_GameTasks.Remove(t);
            List<ShortTask> toRemove = new List<ShortTask>();
            foreach(ShortTask ta in s_GameTasks)
            {

                if(t.name.Equals(ta.name,StringComparison.InvariantCultureIgnoreCase))
                    toRemove.Add(ta);
            }
            foreach(ShortTask ta in toRemove)
            {
                if(t.name.Equals(ta.name,StringComparison.InvariantCultureIgnoreCase))
                    s_GameTasks.Remove(ta);
            }

            all_tasks.Add(t);
            s-=1;
        }

        while(l>0 && l_GameTasks.Count>0)
        {
            LongTask t = l_GameTasks[UnityEngine.Random.Range(0, l_GameTasks.Count)];
            l_GameTasks.Remove(t);
            List<LongTask> toRemove = new List<LongTask>();
            foreach(LongTask ta in l_GameTasks)
            {
                if(t.name.Equals(ta.name,StringComparison.InvariantCultureIgnoreCase))
                    toRemove.Add(ta);
            }
            foreach(LongTask ta in toRemove)
            {
                if(t.name.Equals(ta.name,StringComparison.InvariantCultureIgnoreCase))
                    l_GameTasks.Remove(ta);
            }
            all_tasks.Add(t);
            l-=1;
        }
        Debug.Log(all_tasks.Count);
        return all_tasks;
    }

    public static List<GameTask> getGameTasksForArray(int s, int l,string[] file = null)
    {

        if (longGameTasks == null || shortGameTasks == null || chainGameTasks == null)
        {
            if (file == null)
            {
                Debug.Log("A task list is null but no fallback string array was passed");
                return new List<GameTask>();
            }
            LoadGameTasksFromArray(file, new string[0]);
        }
        List<LongTask> l_GameTasks = new List<LongTask>();
        foreach(GameTask t in longGameTasks)
        {
            l_GameTasks.Add((LongTask) t);
        }
        List<ShortTask> s_GameTasks = new List<ShortTask>();
        foreach(GameTask t in shortGameTasks)
        {
            s_GameTasks.Add((ShortTask) t);
        }

        List<GameTask> all_tasks = new List<GameTask>();
        while(s>0 && s_GameTasks.Count>0)
        {
            ShortTask t = s_GameTasks[UnityEngine.Random.Range(0, s_GameTasks.Count)];
            s_GameTasks.Remove(t);
            List<ShortTask> toRemove = new List<ShortTask>();
            foreach(ShortTask ta in s_GameTasks)
            {

                if(t.name.Equals(ta.name,StringComparison.InvariantCultureIgnoreCase))
                    toRemove.Add(ta);
            }
            foreach(ShortTask ta in toRemove)
            {
                if(t.name.Equals(ta.name,StringComparison.InvariantCultureIgnoreCase))
                    s_GameTasks.Remove(ta);
            }

            all_tasks.Add(t);
            s-=1;
        }

        while(l>0 && l_GameTasks.Count>0)
        {
            
            LongTask t = l_GameTasks[UnityEngine.Random.Range(0, l_GameTasks.Count)];
            l_GameTasks.Remove(t);
            List<LongTask> toRemove = new List<LongTask>();
            foreach(LongTask ta in l_GameTasks)
            {
                if(t.name.Equals(ta.name,StringComparison.InvariantCultureIgnoreCase))
                    toRemove.Add(ta);
            }
            foreach(LongTask ta in toRemove)
            {
                if(t.name.Equals(ta.name,StringComparison.InvariantCultureIgnoreCase))
                    l_GameTasks.Remove(ta);
            }
            all_tasks.Add(t);
            l-=1;
        }
        Debug.Log(all_tasks.Count);
        return all_tasks;
    }
    public static GameTask findGameTaskByID(string id)
    {
        if(id[1]=='S')
        {
            foreach(GameTask t in shortGameTasks)
            {
                if(t.id.Equals(id))
                {
                    return t;
                }
            }
        }else
        {
            foreach(GameTask t in longGameTasks)
            {
                if(t.id.Equals(id))
                {
                    return t;
                }
            }
        }

        foreach(GameTask t in chainGameTasks)
        {
            if(t.id.Equals(id))
            {
                return t;
            }
        }
        return null;
    }

        public override string ToString() {
            return this.name+" : " +this.id+ " : " +this.location + " : " + this.next+ " : "+this.taskType ;
        }

        public static List<string> LoadRooms(bool forceUpdate = false)
        {
            if (locations.Count > 0 && !forceUpdate) return locations;
            
            locations.Clear();
            locations = new List<string>();
            
            foreach (var s in shortGameTasks)
            {
                var loc = s.location;
                if (locations.Contains(loc)) continue;
                Debug.Log(loc);
                locations.Add(loc);
            }
            foreach (var s in longGameTasks)
            {
                var loc = s.location;
                if (locations.Contains(loc)) continue;
                Debug.Log(loc);
                locations.Add(loc);
            }
            foreach (var s in chainGameTasks)
            {
                var loc = s.location;
                if (locations.Contains(loc)) continue;
                Debug.Log(loc);
                locations.Add(loc);
            }

            return locations;
        }

        public static List<string> LoadLocationsFromFile(string activeTasksFileName)
        {
            locations.Clear();
            StreamReader sr = new StreamReader(Initializer.workingDirectory + "/" + Initializer.taskFilesDir + "/" +
                                               activeTasksFileName+".agt");
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (line.Contains("#") && !line.Substring(0,line.IndexOf("#")).Contains(",")) continue;
                string s = parseGameTaskFromLine(line).location;
                if(!locations.Contains(s))
                    locations.Add(s);
            }

            return locations;
        }

    }
    
    public class ShortTask : GameTask{
        public ShortTask(string id, string name, string location, string next) 
            : base(id,name,location,next,"short"){}
    }
    
    public class LongTask : GameTask {
        public LongTask(String id, String name, String location, String next)
            : base(id, name, location, next,"long")
        {
            
        }
    }

    public class ChainTask : GameTask{
        public ChainTask(String id, String name, String location, String next) 
            : base(id,name,location,next, "chain"){}
    }

    public class GameTaskFactory
    {

        public static GameTask create(string name, string id, string location, string next,string type)
        {
            switch (type.ToLower())
            {
                case "short":
                    return new ShortTask(id, name, location,next);
                    break;
                case "long":
                    return new LongTask(id, name, location,next);
                    break;
                case "chain":
                    return new ChainTask(id, name, location,next);
                    break;
            }

            return null;
        }
    }
}