using System;
using System.Collections.Generic;
using System.Linq;
using TutorSchedule;

namespace TutorSchedule
{
    public static class ScheduleHelper
    {
        // Update this method in ScheduleHelper.cs
        public static int ParseTime(string timeStr)
        {
            // 1. Clean the string in case it contains AM/PM
            string cleanTime = timeStr.Replace("AM", "").Replace("PM", "").Trim();

            // 2. Split by the colon
            var parts = cleanTime.Split(':');
            int hours = int.Parse(parts[0]);
            int minutes = int.Parse(parts[1]);

            // 3. Handle 12-hour logic if PM was present
            if (timeStr.ToUpper().Contains("PM") && hours < 12)
            {
                hours += 12;
            }
            else if (timeStr.ToUpper().Contains("AM") && hours == 12)
            {
                hours = 0;
            }

            return hours * 60 + minutes;
        }

        public static bool TimesOverlap(
            string start1,
            string end1,
            string start2,
            string end2
        )
        {
            int start1Min = ParseTime(start1);
            int end1Min = ParseTime(end1);
            int start2Min = ParseTime(start2);
            int end2Min = ParseTime(end2);

            return start1Min < end2Min && start2Min < end1Min;
        }

        public static bool HasLabConflict(
            Employee employee,
            string day,
            string startTime,
            string endTime
        )
        {
            if (employee.Labs == null || employee.Labs.Count == 0)
                return false;

            foreach (var lab in employee.Labs)
            {
                var labInfo = lab.Value;
                if (labInfo.Count == 3)
                {
                    string labDay = labInfo[0];
                    string labStart = labInfo[1];
                    string labEnd = labInfo[2];

                    if (labDay == day)
                    {
                        if (TimesOverlap(startTime, endTime, labStart, labEnd))
                            return true;
                    }
                }
            }
            return false;
        }

        public static bool HasShiftConflict(
            Employee employee,
            string day,
            string startTime,
            string endTime
        )
        {
            if (employee.SetShifts == null || employee.SetShifts.Count == 0)
                return false;

            if (employee.SetShifts.ContainsKey(day))
            {
                foreach (var shift in employee.SetShifts[day])
                {
                    if (shift.Count >= 2)
                    {
                        string shiftStart = shift[0];
                        string shiftEnd = shift[1];

                        if (TimesOverlap(startTime, endTime, shiftStart, shiftEnd))
                            return true;
                    }
                }
            }
            return false;
        }

        public static (string status, string detail) CheckAvailability(Employee employee, string day, string startTime, string endTime)
        {
            if (employee.Availabilities == null || !employee.Availabilities.ContainsKey(day))
                return ("unavailable", null);

            int reqStart = ParseTime(startTime);
            int reqEnd = ParseTime(endTime);

            bool hasFullCoverage = false;
            string partialRange = null;

            foreach (var window in employee.Availabilities[day])
            {
                int winStart = ParseTime(window[0]);
                int winEnd = ParseTime(window[1]);

                // Full match check
                if (winStart <= reqStart && winEnd >= reqEnd)
                {
                    hasFullCoverage = true;
                    break;
                }

                // Partial overlap check
                if (reqStart < winEnd && winStart < reqEnd)
                {
                    partialRange = $"{window[0]}-{window[1]}";
                }
            }

            if (!hasFullCoverage && partialRange == null)
                return ("unavailable", null);

            // Conflicts take precedence over availability
            if (HasLabConflict(employee, day, startTime, endTime)) return ("conflict", "lab");
            if (HasShiftConflict(employee, day, startTime, endTime)) return ("conflict", "shift");

            return hasFullCoverage ? ("available", null) : ("partial", partialRange);
        }

        // In ScheduleHelper.cs
        public static List<AvailabilityResult> SearchAvailableEmployees(List<Employee> employees, string day, string startTime, string endTime)
        {
            var finalResults = new List<AvailabilityResult>();

            foreach (var employee in employees)
            {
                var res = new AvailabilityResult { EmployeeName = employee.Name };
                var (status, info) = CheckAvailability(employee, day, startTime, endTime);

                double currentHours = GetTotalWorkHours(employee);
                string hourStatus = $"{currentHours:0.##}/{employee.MaxHours}";

                if (status == "available")
                {
                    // Format available time as AM/PM using TimeHelper
                    string timeRange = $"{TimeHelper.ConvertTo12Hour(startTime)} - {TimeHelper.ConvertTo12Hour(endTime)}";

                    if (currentHours >= employee.MaxHours)
                        res.ShiftConflicts.Add($"{timeRange} (At max: {hourStatus})");
                    else
                        res.Available.Add($"{timeRange} ({hourStatus})");
                }
                else if (status == "conflict")
                {
                    if (info == "lab")
                    {
                        // Iterate through labs to find the specific conflict details
                        foreach (var lab in employee.Labs)
                        {
                            var labInfo = lab.Value;
                            if (labInfo[0] == day && TimesOverlap(startTime, endTime, labInfo[1], labInfo[2]))
                            {
                                string labTime = $"{TimeHelper.ConvertTo12Hour(labInfo[1])} - {TimeHelper.ConvertTo12Hour(labInfo[2])}";
                                res.LabConflicts.Add($"{lab.Key} ({labTime})"); // Class Number and Time
                            }
                        }
                    }
                    else // shift conflict
                    {
                        if (employee.SetShifts.ContainsKey(day))
                        {
                            foreach (var shift in employee.SetShifts[day])
                            {
                                if (TimesOverlap(startTime, endTime, shift[0], shift[1]))
                                {
                                    string shiftTime = $"{TimeHelper.ConvertTo12Hour(shift[0])} - {TimeHelper.ConvertTo12Hour(shift[1])}";
                                    res.ShiftConflicts.Add(shiftTime);
                                }
                            }
                        }
                    }
                }
                // ... (handle partial status if needed)

                if (res.Available.Any() || res.LabConflicts.Any() || res.ShiftConflicts.Any())
                    finalResults.Add(res);
            }
            return finalResults;
        }

        public static double GetCurrentShiftHours(Employee employee)
        {
            if (employee.SetShifts == null || employee.SetShifts.Count == 0)
                return 0.0;

            double total = 0;
            foreach (var day in employee.SetShifts)
            {
                foreach (var shift in day.Value)
                {
                    if (shift.Count < 2) continue;

                    int start = ParseTime(shift[0]);
                    int end = ParseTime(shift[1]);

                    total += (end - start) / 60.0; // convert minutes to hours
                }
            }
            return total;
        }

        public static double GetTotalWorkHours(Employee employee)
        {
            double total = 0;

            // 1. Sum Grading Hours from Dictionary
            if (employee.Grading != null)
            {
                foreach (var entry in employee.Grading)
                {
                    total += entry.Value;
                }
            }

            // 2. Add Lab Hours (Using the specific hours entered for each lab)
            // FIX: Removed (employee.Labs.Count * 1.75)
            if (employee.LabHours != null)
            {
                foreach (var labEntry in employee.LabHours)
                {
                    total += labEntry.Value; // Adds the specific hours (e.g., 1.0, 2.5) stored for that lab code
                }
            }

            // 3. Add Shift Hours (Calculated from [Start, End] lists)
            if (employee.SetShifts != null)
            {
                foreach (var dayShifts in employee.SetShifts.Values)
                {
                    foreach (var shift in dayShifts)
                    {
                        if (shift.Count >= 2)
                        {
                            int start = ParseTime(shift[0]);
                            int end = ParseTime(shift[1]);
                            total += (end - start) / 60.0;
                        }
                    }
                }
            }

            return total;
        }
    }
}