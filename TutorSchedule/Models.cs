using System;
using System.Collections.Generic;

namespace TutorSchedule
{
    public class EmployeeData
    {
        public List<Employee> Employees { get; set; }
    }

    public class Employee
    {
        public string Name { get; set; }
        public Dictionary<string, List<List<string>>> Availabilities { get; set; }
        // Stores [Day, Start, End]
        public Dictionary<string, List<string>> Labs { get; set; }
        // New: Stores the credit/hours for each lab code
        public Dictionary<string, double> LabHours { get; set; } = new();
        public bool IsNew { get; set; }
        public string Preference { get; set; }
        public Dictionary<string, List<List<string>>> SetShifts { get; set; }
        public int MaxHours { get; set; }
        public string Distance { get; set; }
        public Dictionary<string, double> Grading { get; set; } = new();
    }

    public class AvailabilityResult
    {
        public string EmployeeName { get; set; }
        public List<string> Available { get; set; }
        public List<string> LabConflicts { get; set; }
        public List<string> ShiftConflicts { get; set; }

        public AvailabilityResult()
        {
            Available = new List<string>();
            LabConflicts = new List<string>();
            ShiftConflicts = new List<string>();
        }
    }
}