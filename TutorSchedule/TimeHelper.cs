using System;

namespace TutorSchedule
{
    public static class TimeHelper
    {
        public static string ConvertTo12Hour(string time24)
        {
            try
            {
                string[] parts = time24.Split(':');
                int hour = int.Parse(parts[0]);
                int minute = int.Parse(parts[1]);

                string amPm = "AM";
                if (hour >= 12)
                {
                    amPm = "PM";
                    if (hour > 12)
                        hour -= 12;
                }
                else if (hour == 0)
                {
                    hour = 12;
                }

                return $"{hour}:{minute:D2} {amPm}";
            }
            catch
            {
                return time24;
            }
        }

        public static string ConvertTo24Hour(string time12)
        {
            try
            {
                // Remove any extra spaces
                time12 = time12.Trim();

                // Split time and AM/PM
                string[] parts = time12.Split(' ');
                string[] timeParts = parts[0].Split(':');

                int hour = int.Parse(timeParts[0]);
                int minute = int.Parse(timeParts[1]);
                string amPm = parts[1].ToUpper();

                if (amPm == "PM" && hour != 12)
                    hour += 12;
                else if (amPm == "AM" && hour == 12)
                    hour = 0;

                return $"{hour:D2}:{minute:D2}";
            }
            catch
            {
                return time12;
            }
        }
    }
}