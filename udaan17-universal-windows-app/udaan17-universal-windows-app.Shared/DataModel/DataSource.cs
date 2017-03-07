using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        public int Count { get { return _events.Count + _devs.Count; } }

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
                Uri dataUri = new Uri("ms-appx:///DataModel/Data.json");
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
                string jsonText = await FileIO.ReadTextAsync(file);
                JsonObject Data = JsonObject.Parse(jsonText);
                //Load Tech events
                foreach (JsonValue val in Data["tech"].GetArray())
                {
                    JsonObject obj = val.GetObject();
                    Department d = new Department(obj["name"].GetString(), obj["alias"].GetString());
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
                            e.Rounds.Add(new Iteration() { no = e.Rounds.Count + 1+") ", Value = jval.GetString() });
                        }
                        e.NoOfParticipants = eventobj["participants"].GetString();
                        e.Managers = new List<Manager>();
                        foreach (JsonValue manager in eventobj["managers"].GetArray())
                        {
                            JsonObject mgr = manager.GetObject();
                            e.Managers.Add(new Manager() { name = mgr["name"].GetString(), Contact = mgr["mobile"].GetString() });
                        }
                        foreach (JsonValue prize in eventobj["prizes"].GetArray())
                        {
                            e.Prizes.Add(new Iteration() { no = e.Prizes.Count + 1+") ", Value = prize.GetString() });
                        }
                        d.Events.Add(e);
                    }
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
                        }catch (Exception)
                        {
                            e.OneLiner = e.Description;
                        }
                        foreach (JsonValue mgrs in o["managers"].GetArray())
                        {
                            JsonObject mgrObj = mgrs.GetObject();
                            e.Managers.Add(new Manager() { name = mgrObj["name"].GetString(), Contact = mgrObj["mobile"].GetString() });
                        }
                        foreach (JsonValue round in o["rounds"].GetArray())
                        {
                            e.Rounds.Add(new Iteration() { no = e.Rounds.Count + 1 + ") ", Value = round.GetString() });
                        }
                        foreach (JsonValue prize in o["prizes"].GetArray())
                        {
                            e.Prizes.Add(new Iteration() { no = e.Prizes.Count + 1 + ") ", Value = prize.GetString() });
                        }
                        dep.Events.Add(e);
                    }
                    this.Events.Add(dep);
                }
            }
            catch(KeyNotFoundException ex)
            {

            }
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
                _devs.Add(new Devs(o["name"].GetString(), o["email"].GetString(), o["github"].GetString(), o["mobile"].GetString()));
            }
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
            if (matches.Count() == 1) return matches.First();
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

        public Devs(string n, string e, string g, string c)
        {
            Name = n;
            Email = e;
            Git = g;
            Contact = c;
            Mailto = "mailto:" + e;
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
}