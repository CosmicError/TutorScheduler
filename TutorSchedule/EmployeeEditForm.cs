using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TutorSchedule
{
    public partial class EmployeeEditForm : Form
    {
        private Employee employee;
        private bool isNew;

        // controls we’ll dynamically add
        private ListBox availabilityListBox;
        private ComboBox dayCombo;
        private TimeInputControl startBox;
        private TimeInputControl endBox;
        private Button addAvailabilityButton;
        private Button deleteAvailabilityButton;

        private TextBox nameTextBox;
        private NumericUpDown maxHoursNumeric;
        private ComboBox preferenceComboBox;
        private ComboBox distanceComboBox;
        private CheckBox isNewCheckBox;
        private Button okButton;
        private Button cancelButton;

        public EmployeeEditForm(Employee emp, bool isNew = false)
        {
            this.employee = emp;
            this.isNew = isNew;
            InitializeComponent();
            LoadEmployeeData();
            BuildAvailabilitySection();
        }

        private void InitializeComponent()
        {
            this.Text = "Edit Employee";
            this.Size = new Size(620, 680);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var lblWidth = 100;
            var left = lblWidth + 20;
            var width = 350;
            var baseY = 20;
            int rowH = 35;

            // Name
            AddLabel("Name:", 10, baseY, lblWidth);
            nameTextBox = new TextBox { Location = new Point(left, baseY), Width = width };
            Controls.Add(nameTextBox);

            // Max hours
            AddLabel("Max Hours:", 10, baseY + rowH, lblWidth);
            maxHoursNumeric = new NumericUpDown
            {
                Location = new Point(left, baseY + rowH),
                Minimum = 0,
                Maximum = 40,
                Width = 60
            };
            Controls.Add(maxHoursNumeric);

            // Preference
            AddLabel("Preference:", 10, baseY + rowH * 2, lblWidth);
            preferenceComboBox = new ComboBox
            {
                Location = new Point(left, baseY + rowH * 2),
                Width = 120,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            preferenceComboBox.Items.AddRange(new[] { "morning", "day", "evening" });
            Controls.Add(preferenceComboBox);

            // Distance
            AddLabel("Distance:", 10, baseY + rowH * 3, lblWidth);
            distanceComboBox = new ComboBox
            {
                Location = new Point(left, baseY + rowH * 3),
                Width = 120,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            distanceComboBox.Items.AddRange(new[] { "near", "average", "far" });
            Controls.Add(distanceComboBox);

            // New employee checkbox
            isNewCheckBox = new CheckBox
            {
                Text = "Is New",
                Location = new Point(left, baseY + rowH * 4),
                AutoSize = true
            };
            Controls.Add(isNewCheckBox);

            // OK / Cancel
            okButton = new Button
            {
                Text = "OK",
                Location = new Point(320, 560), // Nudged down slightly to accommodate larger listbox
                Width = 100,
                DialogResult = DialogResult.OK
            };
            okButton.Click += OkButton_Click;

            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(430, 560),
                Width = 100,
                DialogResult = DialogResult.Cancel
            };

            Controls.Add(okButton);
            Controls.Add(cancelButton);

            AcceptButton = okButton;
            CancelButton = cancelButton;
        }

        private void BuildAvailabilitySection()
        {
            // Position Settings adjusted to start AFTER the 'Is New' checkbox
            int labelY = 200;
            int inputY = 225;
            int listY = 280;

            // Add Header Labels
            AddSimpleLabel("Day", 20, labelY);
            AddSimpleLabel("Start Time", 130, labelY);
            AddSimpleLabel("End Time", 290, labelY);

            // Day Selection
            dayCombo = new ComboBox
            {
                Location = new Point(20, inputY + 2),
                Width = 100,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            dayCombo.Items.AddRange(new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" });
            dayCombo.SelectedIndex = 0;

            // Time Inputs
            startBox = new TimeInputControl { Location = new Point(130, inputY), Width = 150 };
            endBox = new TimeInputControl { Location = new Point(290, inputY), Width = 150 };

            startBox.SetTime24Hour("09:00");
            endBox.SetTime24Hour("17:00");

            // Add Button
            addAvailabilityButton = new Button
            {
                Text = "Add",
                Location = new Point(450, inputY - 1),
                Width = 80,
                Height = 28,
                BackColor = Color.FromArgb(240, 240, 240)
            };
            addAvailabilityButton.Click += AddAvailabilityButton_Click;

            // Availability ListBox
            availabilityListBox = new ListBox
            {
                Location = new Point(20, listY),
                Size = new Size(420, 220), // Increased height for better visibility
                Font = new Font("Consolas", 9F)
            };

            // Delete Button
            deleteAvailabilityButton = new Button
            {
                Text = "Delete Selected",
                Location = new Point(450, listY),
                Width = 110,
                Height = 30,
                BackColor = Color.MistyRose
            };
            deleteAvailabilityButton.Click += DeleteAvailabilityButton_Click;

            // Add all to form
            this.Controls.AddRange(new Control[] {
                dayCombo, startBox, endBox, addAvailabilityButton,
                availabilityListBox, deleteAvailabilityButton
            });

            RefreshAvailabilityList();
        }

        private void AddSimpleLabel(string text, int x, int y)
        {
            var lbl = new Label { Text = text, Location = new Point(x, y), AutoSize = true };
            this.Controls.Add(lbl);
        }

        private void AddAvailabilityButton_Click(object sender, EventArgs e)
        {
            string day = dayCombo.SelectedItem.ToString().ToLower();
            string start = startBox.GetTime24Hour();
            string end = endBox.GetTime24Hour();

            if (!employee.Availabilities.ContainsKey(day))
                employee.Availabilities[day] = new List<List<string>>();

            employee.Availabilities[day].Add(new List<string> { start, end });
            RefreshAvailabilityList();
        }

        private void DeleteAvailabilityButton_Click(object sender, EventArgs e)
        {
            if (availabilityListBox.SelectedItem == null) return;

            string selectedText = availabilityListBox.SelectedItem.ToString();

            foreach (var day in employee.Availabilities.Keys.ToList())
            {
                var list = employee.Availabilities[day];
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    string currentItemText = $"{day}: {TimeHelper.ConvertTo12Hour(list[i][0])} - {TimeHelper.ConvertTo12Hour(list[i][1])}";

                    if (currentItemText == selectedText)
                    {
                        list.RemoveAt(i);
                        if (list.Count == 0) employee.Availabilities.Remove(day);

                        RefreshAvailabilityList();
                        return;
                    }
                }
            }
        }

        private void RefreshAvailabilityList()
        {
            availabilityListBox.Items.Clear();
            foreach (var day in employee.Availabilities)
            {
                foreach (var window in day.Value)
                {
                    string displayStart = TimeHelper.ConvertTo12Hour(window[0]);
                    string displayEnd = TimeHelper.ConvertTo12Hour(window[1]);
                    availabilityListBox.Items.Add($"{day.Key}: {displayStart} - {displayEnd}");
                }
            }
        }

        private void AddLabel(string text, int x, int y, int width)
        {
            var lbl = new Label
            {
                Text = text,
                Location = new Point(x, y + 4),
                Width = width,
                TextAlign = ContentAlignment.MiddleRight
            };
            Controls.Add(lbl);
        }

        private void LoadEmployeeData()
        {
            nameTextBox.Text = employee.Name;
            maxHoursNumeric.Value = employee.MaxHours;
            preferenceComboBox.SelectedItem = employee.Preference;
            distanceComboBox.SelectedItem = employee.Distance;
            isNewCheckBox.Checked = employee.IsNew;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            employee.Name = nameTextBox.Text;
            employee.MaxHours = (int)maxHoursNumeric.Value;
            employee.Preference = preferenceComboBox.SelectedItem?.ToString() ?? "day";
            employee.Distance = distanceComboBox.SelectedItem?.ToString() ?? "average";
            employee.IsNew = isNewCheckBox.Checked;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}