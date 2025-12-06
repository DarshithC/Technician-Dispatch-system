using System;
using System.Collections.Generic;

namespace ServiceAppSystem
{
    public enum Status { New, Dispatched, Completed, Cancelled }

    public class Customer
    {
        public int CustomerId { get; set; }
        public string Name { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
        
        public override string ToString() => 
            $"[{CustomerId}] {Name} | {Phone} | {Address}";
    }

    public class Technician
    {
        public int TechId { get; set; }
        public string Name { get; set; } = "";
        public List<string> Skills { get; set; } = new();
        public decimal HourlyRate { get; set; }
        
        public override string ToString() => 
            $"[{TechId}] {Name} | Rate: ${HourlyRate:F2}/hr | Skills: {(Skills.Any() ? string.Join(", ", Skills) : "None")}";
    }

    public class ServiceRequest
    {
        public int RequestId { get; set; }
        public int CustomerId { get; set; }
        public int? TechId { get; set; }
        public string Description { get; set; } = "";
        public DateTime ScheduledStart { get; set; }
        public DateTime ScheduledEnd { get; set; }
        public Status CurrentStatus { get; set; } = Status.New;
        public decimal? HoursWorked { get; set; }
        public decimal? PartsCost { get; set; }
        public decimal? TotalCost { get; set; }
        
        public override string ToString()
        {
            string tech = TechId?.ToString() ?? "Unassigned";
            return $"[{RequestId}] Customer: {CustomerId} | Tech: {tech} | " +
                   $"{ScheduledStart:yyyy-MM-dd HH:mm} | {CurrentStatus} | {Description}";
        }
    }
}