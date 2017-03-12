using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;

namespace udaan17_universal_windows_app.Data
{
    public class DataSource
    {
        private static DataSource _DataSource = new DataSource();
        private ObservableCollection<Devs> _devs = new ObservableCollection<Devs>();
        public ObservableCollection<Devs> Devs
        {
            get { return this._devs; }
        }
        private ObservableCollection<Department> _events = new ObservableCollection<Department>();
        public ObservableCollection<Department> Events
        {
            get { return this._events; }
        }
        private ObservableCollection<Department> _tevents = new ObservableCollection<Department>();
        public ObservableCollection<Department> TEvents
        {
            get { return this._tevents; }
        }
        private ObservableCollection<Team> _team = new ObservableCollection<Team>();
        public ObservableCollection<Team> TeamUdaan
        {
            get { return this._team; }
        }
        public int Count { get { return _events.Count + _devs.Count + _team.Count; } }

        public static async Task<IEnumerable<Department>> GetDepartmentsAsync()
        {
            await _DataSource.GetDataAsync();
            return _DataSource.Events;
        }

        public static async Task<ObservableCollection<Department>> GetTeventsAsync()
        {
            await _DataSource.GetDataAsync();
            return _DataSource.TEvents;
        }

        private async Task GetDataAsync()
        {
            if (this.Count != 0)
                return;
            try
            {
                StorageFile file;
                try
                {
                    Uri dataUri = new Uri("ms-appdata:///local/Data.json");
                    file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
                }
                catch (Exception)
                {
                    var dataUri = new Uri("ms-appx:///DataModel/Data.json");
                    file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
                }
                string jsonText = await FileIO.ReadTextAsync(file);
                JsonObject Data = JsonObject.Parse(jsonText);
                //Load Tech events
                foreach (JsonValue val in Data["tech"].GetArray())
                {
                    JsonObject obj = val.GetObject();
                    Department d = new Department(obj["name"].GetString().ToUpper(), obj["alias"].GetString());
                    foreach (JsonValue item in obj["events"].GetArray())
                    {
                        JsonObject eventobj = item.GetObject();
                        Event e = new Event(eventobj["name"].GetString());
                        e.Fee = eventobj["fees"].GetString();
                        e.Description = eventobj["description"].GetString();
                        try
                        {
                            e.OneLiner = e.Description.Split('\n')[0];
                        }
                        catch (Exception)
                        {
                            e.OneLiner = e.Description;
                        }
                        foreach (JsonValue jval in eventobj["rounds"].GetArray())
                        {
                            e.Rounds.Add(new Iteration() { no = e.Rounds.Count + 1 + ") ", Value = jval.GetString() });
                        }
                        e.NoOfParticipants = eventobj["participants"].GetString();
                        e.Managers = new List<Manager>();
                        foreach (JsonValue manager in eventobj["managers"].GetArray())
                        {
                            JsonObject mgr = manager.GetObject();
                            var m = new Manager();
                            try
                            {
                                m.name = mgr["name"].GetString();
                                m.Contact = mgr["mobile"].GetString();
                            }
                            catch (Exception) { m.Contact = ""; }
                            e.Managers.Add(m);
                        }
                        foreach (JsonValue prize in eventobj["prizes"].GetArray())
                        {
                            e.Prizes.Add(new Iteration() { no = e.Prizes.Count + 1 + ") ", Value = "₹ " + prize.GetString() });
                        }
                        e.Background = d.Background;
                        d.Events.Add(e);
                    }
                    try
                    {
                        foreach (JsonValue head in obj["heads"].GetArray())
                        {
                            JsonObject hObj = head.GetObject();
                            d.Heads.Add(new Manager() { name = hObj["name"].GetString(), Contact = hObj["mobile"].GetString() });
                        }
                        foreach (JsonValue head in obj["coHeads"].GetArray())
                        {
                            JsonObject hObj = head.GetObject();
                            d.CoHeads.Add(new Manager() { name = hObj["name"].GetString(), Contact = hObj["mobile"].GetString() });
                        }
                    }
                    catch (Exception) { }
                    this.Events.Add(d);
                    this.TEvents.Add(d);
                }
                //load non-tech and cultural events
                foreach (string str in new List<string> { "nonTech", "cultural" })
                {
                    Department dep = new Department(str, str);
                    foreach (JsonValue item in Data[str].GetArray())
                    {
                        JsonObject o = item.GetObject();
                        Event e = new Event(o["name"].GetString());
                        e.NoOfParticipants = o["participants"].GetString();
                        e.Description = o["description"].GetString();
                        e.Fee = o["fees"].GetString();
                        e.Managers = new List<Manager>();
                        try
                        {
                            e.OneLiner = e.Description.Split('\n')[0];
                        }
                        catch (Exception)
                        {
                            e.OneLiner = e.Description;
                        }
                        foreach (JsonValue mgrs in o["managers"].GetArray())
                        {
                            JsonObject mgrObj = mgrs.GetObject();
                            var m = new Manager();
                            try
                            {
                                m.name = mgrObj["name"].GetString();
                                m.Contact = mgrObj["mobile"].GetString();
                            }
                            catch (Exception) { m.Contact = ""; }
                            e.Managers.Add(m);
                        }
                        foreach (JsonValue round in o["rounds"].GetArray())
                        {
                            e.Rounds.Add(new Iteration() { no = e.Rounds.Count + 1 + ") ", Value = round.GetString() });
                        }
                        foreach (JsonValue prize in o["prizes"].GetArray())
                        {
                            e.Prizes.Add(new Iteration() { no = e.Prizes.Count + 1 + ") ", Value = prize.GetString() });
                        }
                        e.Background = dep.Background;
                        dep.Events.Add(e);
                    }
                    this.Events.Add(dep);
                }
                //Load TeamUdaan data
                LoadTeam(Data);
            }
            catch (KeyNotFoundException) { }
            //Load Developers data
            await LoadDevs();
        }
        private async Task LoadDevs()
        {
            Uri dataUri = new Uri("ms-appx:///DataModel/developers.json");
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            string jsonText = await FileIO.ReadTextAsync(file);
            JsonObject Data = JsonObject.Parse(jsonText);
            foreach (JsonValue item in Data["devs"].GetArray())
            {
                JsonObject o = item.GetObject();
                _devs.Add(new Devs(o["name"].GetString(), o["email"].GetString(), o["github"].GetString(), o["mobile"].GetString(), o["title"].GetString(), o["color"].GetString()));
            }
        }
        private void LoadTeam(JsonObject data)
        {
            foreach (JsonValue teamCat in data["teamUdaan"].GetArray())
            {
                Team team = new Team();
                team.Members = new List<Manager>();
                JsonObject category = teamCat.GetObject();
                team.Title = category["category"].GetString();
                foreach (JsonValue item in category["members"].GetArray())
                {
                    JsonObject member = item.GetObject();
                    team.Members.Add(new Manager() { name = member["name"].GetString(), Contact = member["title"].GetString() });
                }
                _team.Add(team);
            }
        }
        public static async Task<List<Team>> GetTeamAsync()
        {
            await _DataSource.GetDataAsync();
            return _DataSource._team.ToList();
        }

        public static async Task<Department> GetDepartmentAsync(string uniqueId)
        {
            await _DataSource.GetDataAsync();
            var matches = _DataSource.Events.Where((group) => group.Alias.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }
        public static async Task<Event> GetEventAsync(string uniqueId)
        {
            await _DataSource.GetDataAsync();
            var matches = _DataSource.Events.SelectMany(group => group.Events).Where((item) => item.name.Equals(uniqueId));
            if (matches.Count() >= 1) return matches.First();
            return null;
        }
        public static async Task<List<Event>> GetUpcomingAsync()
        {
            await _DataSource.GetDataAsync();
            var matches = _DataSource.Events.SelectMany(group => group.Events).ToList();
            var ls = new List<Event>();
            Random r = new Random();
            for (int i = 0; i < 20; i++)
            {
                ls.Add(matches[r.Next() % matches.Count]);
            }
            return ls;
        }
        public static async Task<List<Devs>> GetDevsAsync()
        {
            await _DataSource.GetDataAsync();
            return _DataSource.Devs.ToList();
        }

        public static async Task<bool> FileExists(StorageFolder folder, string fileName)
        {
            try { StorageFile file = await folder.GetFileAsync(fileName); }
            catch { return false; }
            return true;
        }

        public static async Task CheckDataAsync()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            bool flag = await FileExists(localFolder, "Data.json");
            Uri dataUri;
            if (!flag)
                dataUri = new Uri("ms-appx:///DataModel/Data.json");
            else
                dataUri = new Uri("ms-appdata:///local/Data.json");
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            var props = await file.GetBasicPropertiesAsync();
            DateTime local = props.DateModified.DateTime;
            try
            {
                HttpWebRequest req = WebRequest.CreateHttp("http://udaan17.in:8080/api/data.json");
                var res = (HttpWebResponse)await req.GetResponseAsync();
                var str = res.Headers["Last-Modified"].ToString();
                DateTime remote = DateTime.Parse(str);
                if (remote.CompareTo(local) >= 0)
                {
                    if (flag)
                    {
                        using (StreamReader sr = new StreamReader(res.GetResponseStream()))
                        using (StreamWriter sw = new StreamWriter(await file.OpenStreamForWriteAsync()))
                        {
                            sw.WriteLine(sr.ReadToEnd());
                        }
                    }
                    else
                    {
                        StorageFile f = await localFolder.CreateFileAsync("Data.json");
                        using (StreamReader sr = new StreamReader(res.GetResponseStream()))
                        using (StreamWriter sw = new StreamWriter(await f.OpenStreamForWriteAsync()))
                        {
                            sw.WriteLine(sr.ReadToEnd());
                        }
                    }
                }
            }
            catch (Exception) { }
        }
    }
    public class Department
    {
        public string Alias { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public List<Event> Events { get; set; }
        public List<Manager> Heads { get; set; }
        public List<Manager> CoHeads { get; set; }
        public string Background { get; set; }

        public Department(string name, string alias)
        {
            this.Title = name;
            this.Alias = alias;
            this.Image = "/Assets/DepartmentLogos/" + alias + ".png";
            this.Background = "/Assets/DepartmentBackgrounds/" + alias + ".png";
            Events = new List<Event>();
            Heads = new List<Manager>();
            CoHeads = new List<Manager>();
        }
    }
    public class Devs
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Git { get; set; }
        public string Contact { get; set; }
        public string Mailto { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }

        public Devs(string name, string email, string git, string contact, string title, string cat)
        {
            Name = name;
            Email = email;
            Git = git;
            Contact = contact;
            Mailto = "mailto:" + email;
#if WINDOWS_PHONE_APP
            Mailto = "mailto:?to="+email;
#endif
            Category = "/Assets/DevLogos/" + cat + ".png";
            Title = title;
        }
    }
    public class Event
    {
        public string name { get; set; }
        public List<Iteration> Rounds { get; set; }
        public List<Iteration> Prizes { get; set; }
        public string Description { get; set; }
        public string NoOfParticipants { get; set; }
        public List<Manager> Managers { get; set; }
        public string Fee { get; set; }
        public string Image { get; set; }
        public string Background { get; set; }
        public string OneLiner { get; set; }

        public Event(string s)
        {
            Rounds = new List<Iteration>();
            Prizes = new List<Iteration>();
            name = s;
            Image = "Assets/img/" + name + ".png";
            if (Image.Contains(" ")) Image = Image.Replace(" ", "-");
        }

    }
    public class Manager
    {
        public string name { get; set; }
        public string Contact { get; set; }
    }

    public class Iteration
    {
        public string no { get; set; }
        public string Value { get; set; }
    }

    public class Team
    {
        public string Title { get; set; }
        public List<Manager> Members { get; set; }
    }
}