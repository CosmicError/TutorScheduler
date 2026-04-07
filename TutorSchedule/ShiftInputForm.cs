using System;
using System.Drawing;
using System.Windows.Forms;

namespace TutorSchedule
{
    public partial class ShiftInputForm : Form
    {
        public string SelectedDay { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        private ComboBox dayComboBox;
        private TimeInputControl startTimePicker;
        private TimeInputControl endTimePicker;
        private Button okButton;

        public ShiftInputForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Add Regular Shift";
            this.Size = new Size(350, 250);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            Label lblDay = new Label { Text = "Day:", Location = new Point(20, 20), AutoSize = true };
            dayComboBox = new ComboBox { Location = new Point(20, 40), Width = 240 };
            dayComboBox.Items.AddRange(new string[] { "mon", "tue", "wed", "thu", "fri", "sat", "sun" });
            dayComboBox.SelectedIndex = 0;

            Label lblStart = new Label { Text = "Start Time:", Location = new Point(20, 80), AutoSize = true };
            startTimePicker = new TimeInputControl { Location = new Point(20, 100) };

            Label lblEnd = new Label { Text = "End Time:", Location = new Point(180, 80), AutoSize = true };
            endTimePicker = new TimeInputControl { Location = new Point(180, 100) };

            okButton = new Button
            {
                Text = "Add Shift",
                DialogResult = DialogResult.OK,
                Location = new Point(100, 160),
                Width = 100,
                BackColor = Color.LightBlue,
                FlatStyle = FlatStyle.Flat
            };
            okButton.Click += (s, e) => {
                SelectedDay = dayComboBox.SelectedItem.ToString();
                StartTime = startTimePicker.GetTime24Hour();
                EndTime = endTimePicker.GetTime24Hour();
                this.DialogResult = DialogResult.OK;
            };

            this.Controls.AddRange(new Control[] { lblDay, dayComboBox, lblStart, startTimePicker, lblEnd, endTimePicker, okButton });
        }

        // Inside ShiftInputForm.cs
        // Inside ShiftInputForm.cs

        // Add this method to pre-fill the form

        public void Prepopulate(string day, string start, string end)
        {
            this.SelectedDay = day.ToLower();
            this.StartTime = start;
            this.EndTime = end;

            // Sync UI Controls
            if (dayComboBox.Items.Contains(SelectedDay))
                dayComboBox.SelectedItem = SelectedDay;

            startTimePicker.SetTime24Hour(StartTime);
            endTimePicker.SetTime24Hour(EndTime);
        }
        public void LoadShiftData(string day, string startTime24, string endTime24)
        {
            // Ensure the day matches one of the items: "mon", "tue", etc.
            dayComboBox.SelectedItem = day.ToLower();
            startTimePicker.SetTime24Hour(startTime24);
            endTimePicker.SetTime24Hour(endTime24);

            okButton.Text = "Update Shift"; // Visual feedback that we are editing
        }

}
}