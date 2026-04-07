using System;
using System.Drawing;
using System.Windows.Forms;

namespace TutorSchedule
{
    public partial class LabInputForm : Form
    {
        private TextBox labCodeTextBox;
        private ComboBox dayComboBox;
        private TimeInputControl startTimeInput;
        private TimeInputControl endTimeInput;
        private NumericUpDown hoursNumeric;
        private Button okButton;

        public string LabCode { get; set; }
        public string SelectedDay { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public double LabDuration { get; set; }

        public LabInputForm()
        {
            // THIS LINE IS REQUIRED TO RENDER THE UI
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Add Lab";
            this.Size = new Size(350, 320);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Lab Code
            var label = new Label { Text = "Lab Code:", Location = new Point(20, 10), AutoSize = true };
            labCodeTextBox = new TextBox { Location = new Point(20, 30), Width = 280 };

            // Day Selection
            var dayLabel = new Label { Text = "Day of Week:", Location = new Point(20, 60), AutoSize = true };
            dayComboBox = new ComboBox { Location = new Point(20, 80), Width = 280, DropDownStyle = ComboBoxStyle.DropDownList };
            dayComboBox.Items.AddRange(new string[] { "mon", "tue", "wed", "thu", "fri", "sat", "sun" });
            dayComboBox.SelectedIndex = 0;

            // Start Time
            var startLabel = new Label { Text = "Start Time:", Location = new Point(20, 110), AutoSize = true };
            startTimeInput = new TimeInputControl { Location = new Point(20, 130) };

            // End Time
            var endLabel = new Label { Text = "End Time:", Location = new Point(170, 110), AutoSize = true };
            endTimeInput = new TimeInputControl { Location = new Point(170, 130) };

            // Duration
            var hoursLabel = new Label { Text = "Lab Hours (Duration):", Location = new Point(20, 170), AutoSize = true };
            hoursNumeric = new NumericUpDown
            {
                Location = new Point(20, 190),
                Width = 80,
                DecimalPlaces = 1,
                Increment = 0.5M,
                Value = 1.0M
            };

            // OK Button
            okButton = new Button
            {
                Text = "OK",
                Location = new Point(120, 240),
                Width = 80,
                DialogResult = DialogResult.OK
            };
            okButton.Click += OkButton_Click;

            this.Controls.AddRange(new Control[] {
                label, labCodeTextBox, dayLabel, dayComboBox,
                startLabel, startTimeInput, endLabel, endTimeInput,
                hoursLabel, hoursNumeric, okButton
            });
        }

        // Inside LabInputForm.cs

        // Update these properties to be public so MainForm can write to them


        // Add this method to refresh the UI after properties are set
        public void Prepopulate(string day, string start, string end)
        {
            this.SelectedDay = day.ToLower();
            this.StartTime = start;
            this.EndTime = end;

            // This updates the dropdown and the custom time picker controls
            if (dayComboBox.Items.Contains(SelectedDay))
                dayComboBox.SelectedItem = SelectedDay;

            startTimeInput.SetTime24Hour(StartTime);
            endTimeInput.SetTime24Hour(EndTime);
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(labCodeTextBox.Text))
            {
                MessageBox.Show("Please enter a lab code", "Error");
                this.DialogResult = DialogResult.None;
                return;
            }

            // Ensure the properties match what is currently in the boxes before closing
            LabCode = labCodeTextBox.Text.Trim();
            SelectedDay = dayComboBox.SelectedItem.ToString();
            StartTime = startTimeInput.GetTime24Hour();
            EndTime = endTimeInput.GetTime24Hour();
            LabDuration = (double)hoursNumeric.Value;

            this.DialogResult = DialogResult.OK;
        }

        // Inside LabInputForm.cs
        public void LoadLabData(string labCode, string day, string startTime24, string endTime24, double duration)
        {
            labCodeTextBox.Text = labCode;
            dayComboBox.SelectedItem = day.ToLower();
            startTimeInput.SetTime24Hour(startTime24);
            endTimeInput.SetTime24Hour(endTime24);
            hoursNumeric.Value = (decimal)duration;

            okButton.Text = "Update Lab";
        }

    }
}