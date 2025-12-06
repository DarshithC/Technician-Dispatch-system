using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ServiceAppSystem
{
    public class IdGenerator
    {
        public int C = 1, T = 1, R = 1;
        
        public void Sync(List<Customer> c, List<Technician> t, List<ServiceRequest> r)
        {
            if (c.Any()) C = c.Max(x => x.CustomerId) + 1;
            if (t.Any()) T = t.Max(x => x.TechId) + 1;
            if (r.Any()) R = r.Max(x => x.RequestId) + 1;
        }
        
        public int NextC() => C++;
        public int NextT() => T++;
        public int NextR() => R++;
    }

    public class DispatchManager
    {
        public List<Customer> Customers { get; private set; } = new();
        public List<Technician> Technicians { get; private set; } = new();
        public List<ServiceRequest> Requests { get; private set; } = new();

        private readonly IdGenerator ids;
        private readonly TimeSpan duration = TimeSpan.FromHours(2);
        private const string fc = "customers.json";
        private const string ft = "technicians.json";
        private const string fr = "requests.json";

        public DispatchManager(IdGenerator g) 
        { 
            ids = g; 
        }

        public void Load()
        {
            try
            {
                if (File.Exists(fc)) 
                    Customers = JsonSerializer.Deserialize<List<Customer>>(File.ReadAllText(fc)) ?? new();
                if (File.Exists(ft)) 
                    Technicians = JsonSerializer.Deserialize<List<Technician>>(File.ReadAllText(ft)) ?? new();
                if (File.Exists(fr)) 
                    Requests = JsonSerializer.Deserialize<List<ServiceRequest>>(File.ReadAllText(fr)) ?? new();
            }
            catch 
            { 
                Customers = new(); 
                Technicians = new(); 
                Requests = new(); 
            }

            ids.Sync(Customers, Technicians, Requests);
        }

        public void Save()
        {
            var opt = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(fc, JsonSerializer.Serialize(Customers, opt));
            File.WriteAllText(ft, JsonSerializer.Serialize(Technicians, opt));
            File.WriteAllText(fr, JsonSerializer.Serialize(Requests, opt));
        }

        public Customer AddCustomer(string n, string p, string a)
        {
            var c = new Customer { CustomerId = ids.NextC(), Name = n, Phone = p, Address = a };
            Customers.Add(c); 
            Save(); 
            return c;
        }

        public Technician AddTech(string n, List<string> s, decimal r)
        {
            var t = new Technician { TechId = ids.NextT(), Name = n, Skills = s, HourlyRate = r };
            Technicians.Add(t); 
            Save(); 
            return t;
        }

        public ServiceRequest AddRequest(int cid, string desc, DateTime start, string? skill)
        {
            var r = new ServiceRequest
            {
                RequestId = ids.NextR(),
                CustomerId = cid,
                Description = desc,
                ScheduledStart = start,
                ScheduledEnd = start.Add(duration)
            };
            Requests.Add(r);
            AssignAuto(r, skill);
            Save();
            return r;
        }

        private bool Overlap(DateTime a1, DateTime a2, DateTime b1, DateTime b2) 
            => a1 < b2 && b1 < a2;

        private void AssignAuto(ServiceRequest r, string? skill)
        {
            var pool = string.IsNullOrWhiteSpace(skill)
                ? Technicians
                : Technicians.Where(t => t.Skills.Any(s => 
                    s.Contains(skill, StringComparison.OrdinalIgnoreCase)));

            foreach (var t in pool)
            {
                bool busy = Requests.Any(x =>
                    x.TechId == t.TechId &&
                    x.CurrentStatus != Status.Completed &&
                    x.CurrentStatus != Status.Cancelled &&
                    Overlap(x.ScheduledStart, x.ScheduledEnd, r.ScheduledStart, r.ScheduledEnd));

                if (!busy)
                {
                    r.TechId = t.TechId;
                    r.CurrentStatus = Status.Dispatched;
                    return;
                }
            }
        }

        public bool AssignManual(int rid, int tid)
        {
            var r = Requests.FirstOrDefault(x => x.RequestId == rid);
            var t = Technicians.FirstOrDefault(x => x.TechId == tid);
            if (r == null || t == null) return false;

            bool busy = Requests.Any(x =>
                x.TechId == tid &&
                x.RequestId != rid &&
                x.CurrentStatus != Status.Completed &&
                x.CurrentStatus != Status.Cancelled &&
                Overlap(x.ScheduledStart, x.ScheduledEnd, r.ScheduledStart, r.ScheduledEnd));

            if (busy) return false;

            r.TechId = tid;
            if (r.CurrentStatus == Status.New) r.CurrentStatus = Status.Dispatched;
            Save();
            return true;
        }

        public bool Complete(int rid, decimal hrs, decimal parts)
        {
            var r = Requests.FirstOrDefault(x => x.RequestId == rid);
            if (r == null || !r.TechId.HasValue) return false;
            var t = Technicians.First(x => x.TechId == r.TechId);

            r.HoursWorked = hrs;
            r.PartsCost = parts;
            r.TotalCost = hrs * t.HourlyRate + parts;
            r.CurrentStatus = Status.Completed;
            Save();
            return true;
        }

        public List<ServiceRequest> Open() 
            => Requests.Where(r => r.CurrentStatus == Status.New || r.CurrentStatus == Status.Dispatched).ToList();
        
        public List<ServiceRequest> Completed() 
            => Requests.Where(r => r.CurrentStatus == Status.Completed).ToList();
        
        public decimal Revenue() 
            => Completed().Sum(r => r.TotalCost ?? 0);

        public Customer? GetCustomer(int id) 
            => Customers.FirstOrDefault(c => c.CustomerId == id);
        
        public Technician? GetTechnician(int id) 
            => Technicians.FirstOrDefault(t => t.TechId == id);
    }
}