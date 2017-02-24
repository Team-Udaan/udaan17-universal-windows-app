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
            Uri dataUri = new Uri("ms-appx:///DataModel/Data.json");
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            string jsonText = await FileIO.ReadTextAsync(file);
            JsonObject Data = JsonObject.Parse(jsonText);
            //Load Tech events
            JsonArray Categories = Data["departments"].GetArray();
            foreach (JsonValue val in Categories)
            {
                JsonObject obj = val.GetObject();
                Department d = new Department(obj["name"].GetString(), obj["alias"].GetString());
                foreach (JsonValue item in obj["events"].GetArray())
                {
                    JsonObject eventobj = item.GetObject();
                    Event e = new Event(eventobj["name"].GetString());
                    e.Fee = eventobj["fees"].GetString();
                    try
                    {
                        e.Description = eventobj["eventDescription"].GetString();
                        if (eventobj["round1Description"].GetString() != "")
                            e.Description += "\r\n\nRound 1 : \r\n" + eventobj["round1Description"].GetString();
                        if (eventobj["round2Description"].GetString() != "")
                            e.Description += "\r\n\nRound 2 : \r\n" + eventobj["round2Description"].GetString();
                        if (eventobj["round3Description"].GetString() != "")
                            e.Description += "\r\n\nRound 3 : \r\n" + eventobj["round3Description"].GetString();
                    }
                    catch (KeyNotFoundException)
                    { }
                    e.NoOfParticipants = eventobj["participants"].GetString();
                    e.Managers = new List<Manager>();
                    e.email = eventobj["email"].GetString();
                    foreach (JsonValue manager in eventobj["managers"].GetArray())
                    {
                        JsonObject mgr = manager.GetObject();
                        Manager m = new Manager();
                        m.name = mgr["name"].GetString();
                        m.Contact = mgr["mobile"].GetString();
                        e.Managers.Add(m);
                    }
                    d.Events.Add(e);
                }
                this.Events.Add(d);
                this.TEvents.Add(d);
            }
            //load non-tech events
            foreach (JsonValue item in Data["categories"].GetArray())
            {
                JsonObject o = item.GetObject();
                if (o["name"].GetString() != "Tech")
                {
                    Department dep = new Department(o["name"].GetString(), o["alias"].GetString());
                    //if (dep.Title != "Nights")
                    try
                    {
                        foreach (JsonValue v in Data[o["alias"].GetString()].GetArray())
                        {
                            JsonObject jobj = v.GetObject();
                            Event e = new Event(jobj["name"].GetString());
                            e.Description = jobj["eventDescription"].GetString();
                            e.email = jobj["email"].GetString();
                            try
                            {
                                if (jobj["round1Description"].GetString() != "")
                                    e.Description += "\r\n\nRound 1 : \r\n" + jobj["round1Description"].GetString();
                                if (jobj["round2Description"].GetString() != "")
                                    e.Description += "\r\n\nRound 2 : \r\n" + jobj["round2Description"].GetString();
                                if (jobj["round3Description"].GetString() != "")
                                    e.Description += "\r\n\nRound 3 : \r\n" + jobj["round3Description"].GetString();
                                e.NoOfParticipants = jobj["participants"].GetString();
                                e.Fee = jobj["fees"].GetString();
                            }
                            catch (Exception)
                            {
                                e.NoOfParticipants = e.Fee = "N/A";
                            }
                            e.Managers = new List<Manager>();
                            foreach (JsonValue manager in jobj["managers"].GetArray())
                            {
                                JsonObject mgr = manager.GetObject();
                                Manager m = new Manager();
                                m.name = mgr["name"].GetString();
                                m.Contact = mgr["mobile"].GetString();
                                e.Managers.Add(m);
                            }
                            dep.Events.Add(e);
                        }
                    }
                    catch (Exception) { }
                    this.Events.Add(dep);
                }
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
            ls.Add(matches[r.Next() % matches.Count]);
            ls.Add(matches[r.Next() % matches.Count]);
            ls.Add(matches[r.Next() % matches.Count]);
            ls.Add(matches[r.Next() % matches.Count]);
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

        public Department(string name, string alias)
        {
            this.Title = name;
            this.Alias = alias;
            this.Image = "/Assets/DepartmentLogos/" + alias + ".png";
            Events = new List<Event>();
        }
    }
    public class Devs
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Git { get; set; }
        public string Contact { get; set; }

        public Devs(string n, string e, string g, string c)
        {
            Name = n;
            Email = e;
            Git = g;
            Contact = c;
        }
    }
    public class Event
    {
        public string name { get; set; }
        public string email { get; set; }
        public string Description { get; set; }
        public string NoOfParticipants { get; set; }
        public List<Manager> Managers { get; set; }
        public string Fee { get; set; }

        public Event(string s)
        {
            name = s;
        }

    }
    public class Manager
    {
        public string name { get; set; }
        public string Contact { get; set; }
    }
}
