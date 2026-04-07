using System;
using System.Drawing;
using System.Windows.Forms;

namespace TutorSchedule
{
    public class TimeInputControl : UserControl
    {
        private MaskedTextBox timeTextBox;
        private ComboBox amPmComboBox;

        public TimeInputControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(150, 25);

            timeTextBox = new MaskedTextBox
            {
                Mask = "00:00",
                Location = new Point(0, 0),
                Width = 60,
                Font = new Font("Segoe UI", 9.75F),
                TextAlign = HorizontalAlignment.Center
            };
            timeTextBox.Text = "09:00";

            amPmComboBox = new ComboBox
            {
                Location = new Point(65, 0),
                Width = 55,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.75F)
            };
            amPmComboBox.Items.AddRange(new[] { "AM", "PM" });
            amPmComboBox.SelectedIndex = 0;

            this.Controls.Add(timeTextBox);
            this.Controls.Add(amPmComboBox);
        }

        public string GetTime24Hour()
        {
            try
            {
                string[] parts = timeTextBox.Text.Split(':');
                int hour = int.Parse(parts[0]);
                int minute = int.Parse(parts[1]);

                if (amPmComboBox.SelectedItem.ToString() == "PM" && hour != 12)
                    hour += 12;
                else if (amPmComboBox.SelectedItem.ToString() == "AM" && hour == 12)
                    hour = 0;

                return $"{hour:D2}:{minute:D2}";
            }
            catch
            {
                return "09:00";
            }
        }

        public void SetTime24Hour(string time24)
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

                timeTextBox.Text = $"{hour:D2}:{minute:D2}";
                amPmComboBox.SelectedItem = amPm;
            }
            catch
            {
                timeTextBox.Text = "09:00";
                amPmComboBox.SelectedIndex = 0;
            }
        }
    }
}