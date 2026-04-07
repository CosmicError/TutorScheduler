using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TutorSchedule
{
    public partial class ShiftManagementForm : Form
    {
        private Employee employee;
        private ListBox workListBox;
        private Button btnAddShift, btnAddLab, btnAddGrading, btnRemove, btnOk;

        public ShiftManagementForm(Employee emp)
        {
            this.employee = emp;
            InitializeComponent();
            RefreshWorkList();
        }

        private void InitializeComponent()
        {
            this.Text = $"Manage Shifts - {employee.Name}";
            this.Size = new Size(520, 550); // Increased height to fit the OK button
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // 1. Initialize the ListBox
            workListBox = new ListBox
            {
                Location = new Point(20, 20),
                Size = new Size(460, 300),
                Font = new Font("Consolas", 9F)
            };

            // 2. Initialize Action Buttons
            btnAddShift = new Button { Text = "Add Shift", Location = new Point(20, 330), Width = 110 };
            btnAddLab = new Button { Text = "Add Lab", Location = new Point(135, 330), Width = 110 };
            btnAddGrading = new Button { Text = "Add Grading", Location = new Point(250, 330), Width = 110 };
            btnRemove = new Button { Text = "Remove Selected", Location = new Point(365, 330), Width = 115, BackColor = Color.MistyRose };

            // 3. Initialize the New OK Button
            btnOk = new Button
            {
                Text = "OK",
                Location = new Point(380, 460), // Positioned at the bottom right
                Width = 100,
                Height = 30,
                DialogResult = DialogResult.OK
            };

            // 4. Attach Event Handlers
            btnAddShift.Click += btnAddShift_Click;
            btnAddLab.Click += btnAddLab_Click;
            btnAddGrading.Click += AddGrading_Click;
            btnRemove.Click += RemoveSelected_Click;

            // 5. CRITICAL: Add all controls to the Form
            this.Controls.Add(workListBox);
            this.Controls.Add(btnAddShift);
            this.Controls.Add(btnAddLab);
            this.Controls.Add(btnAddGrading);
            this.Controls.Add(btnRemove);
            this.Controls.Add(btnOk);

            // Set the OK button as the default "Enter" key action
            this.AcceptButton = btnOk;

            // Inside InitializeComponent in ShiftManagementForm.cs
            workListBox.DoubleClick += WorkListBox_DoubleClick;
        }

        private void RefreshWorkList()
        {
            workListBox.Items.Clear();

            // 1. Display Regular Shifts (Formatted to 12-hour time)
            if (employee.SetShifts != null)
            {
                foreach (var day in employee.SetShifts)
                    foreach (var s in day.Value)
                        workListBox.Items.Add($"[SHIFT] {day.Key.ToUpper()}: {TimeHelper.ConvertTo12Hour(s[0])} - {TimeHelper.ConvertTo12Hour(s[1])}");
            }

            // 2. Display Labs
            if (employee.Labs != null)
            {
                foreach (var lab in employee.Labs)
                    workListBox.Items.Add($"[LAB] {lab.Key}: {lab.Value[0].ToUpper()} {TimeHelper.ConvertTo12Hour(lab.Value[1])} - {TimeHelper.ConvertTo12Hour(lab.Value[2])}");
            }

            // 3. Display Grading
            if (employee.Grading != null)
            {
                foreach (var g in employee.Grading)
                    workListBox.Items.Add($"[GRADING] {g.Key}: {g.Value} hrs");
            }
        }

        private void AddShift_Click(object sender, EventArgs e)
        {
            using var f = new ShiftInputForm();
            if (f.ShowDialog() == DialogResult.OK)
            {
                employee.SetShifts ??= new();
                if (!employee.SetShifts.ContainsKey(f.SelectedDay)) employee.SetShifts[f.SelectedDay] = new();
                employee.SetShifts[f.SelectedDay].Add(new List<string> { f.StartTime, f.EndTime });
                RefreshWorkList();
            }
        }


        // Updated AddGrading_Click logic
        private void AddGrading_Click(object sender, EventArgs e)
        {
            string code = Microsoft.VisualBasic.Interaction.InputBox("Enter Course Code:", "Add Grading");
            if (string.IsNullOrEmpty(code)) return;

            // New input field for hours
            string hoursStr = Microsoft.VisualBasic.Interaction.InputBox("Enter Weekly Grading Hours:", "Grading Hours", "1.0");
            if (double.TryParse(hoursStr, out double hours))
            {
                employee.Grading[code] = hours;
                RefreshWorkList();
            }
        }

        private void RemoveSelected_Click(object sender, EventArgs e)
        {
            if (workListBox.SelectedItem == null) return;
            string item = workListBox.SelectedItem.ToString();

            if (item.StartsWith("[GRADING]"))
            {
                string key = item.Split(':')[0].Replace("[GRADING] ", "").Trim();
                employee.Grading.Remove(key);
            }
            else if (item.StartsWith("[LAB]"))
            {
                string key = item.Split(':')[0].Replace("[LAB] ", "").Trim();
                employee.Labs.Remove(key);
                if (employee.LabHours != null && employee.LabHours.ContainsKey(key))
                    employee.LabHours.Remove(key);
            }
            else if (item.StartsWith("[SHIFT]"))
            {
                // 1. Get everything after the first colon to get the full time range
                // Original string: "[SHIFT] MON: 09:00 AM - 05:00 PM"
                int firstColonIndex = item.IndexOf(':');
                if (firstColonIndex == -1) return;

                // Extract day (e.g., "mon")
                string day = item.Substring(0, firstColonIndex).Replace("[SHIFT] ", "").Trim().ToLower();

                // Extract the exact time range string (e.g., "09:00 AM - 05:00 PM")
                string timeRange = item.Substring(firstColonIndex + 1).Trim();

                if (employee.SetShifts.ContainsKey(day))
                {
                    var dayShifts = employee.SetShifts[day];

                    // 2. Find the match by comparing the reconstructed 12-hour strings
                    var match = dayShifts.FirstOrDefault(s =>
                        $"{TimeHelper.ConvertTo12Hour(s[0])} - {TimeHelper.ConvertTo12Hour(s[1])}" == timeRange);

                    if (match != null)
                    {
                        dayShifts.Remove(match);

                        // 3. Clean up empty day keys
                        if (dayShifts.Count == 0) employee.SetShifts.Remove(day);
                    }
                }
            }
            RefreshWorkList();
        }

    

        private void WorkListBox_DoubleClick(object sender, EventArgs e)
        {
            if (workListBox.SelectedItem == null) return;
            string selectedItem = workListBox.SelectedItem.ToString();

            if (selectedItem.StartsWith("[SHIFT]"))
                EditExistingShift(selectedItem);
            else if (selectedItem.StartsWith("[LAB]"))
                EditExistingLab(selectedItem);
            else if (selectedItem.StartsWith("[GRADING]"))
                EditExistingGrading(selectedItem);
        }

        private void EditExistingShift(string item)
        {
            // 1. Extract the original day and time range from the list item string
            int firstColon = item.IndexOf(':');
            if (firstColon == -1) return;

            string originalDay = item.Substring(0, firstColon).Replace("[SHIFT] ", "").Trim().ToLower();
            string timeRange = item.Substring(firstColon + 1).Trim();

            if (employee.SetShifts.ContainsKey(originalDay))
            {
                var dayShifts = employee.SetShifts[originalDay];

                // Find the exact shift in the list that matches the string you clicked
                var match = dayShifts.FirstOrDefault(s =>
                    $"{TimeHelper.ConvertTo12Hour(s[0])} - {TimeHelper.ConvertTo12Hour(s[1])}" == timeRange);

                if (match != null)
                {
                    using (var f = new ShiftInputForm())
                    {
                        // 2. Pre-populate the form with the MATCHED data
                        f.LoadShiftData(originalDay, match[0], match[1]);

                        if (f.ShowDialog() == DialogResult.OK)
                        {
                            // 3. REMOVE the old record
                            dayShifts.Remove(match);
                            if (dayShifts.Count == 0) employee.SetShifts.Remove(originalDay);

                            // 4. ADD the new record (from the form properties)
                            if (!employee.SetShifts.ContainsKey(f.SelectedDay))
                                employee.SetShifts[f.SelectedDay] = new List<List<string>>();

                            employee.SetShifts[f.SelectedDay].Add(new List<string> { f.StartTime, f.EndTime });

                            RefreshWorkList();
                        }
                    }
                }
            }
        }

        private void EditExistingLab(string item)
        {
            // Get the lab code from the string "[LAB] CSC101: mon ..."
            string labCode = item.Split(':')[0].Replace("[LAB] ", "").Trim();

            if (employee.Labs.ContainsKey(labCode))
            {
                var details = employee.Labs[labCode]; // [day, start, end]
                double duration = employee.LabHours.ContainsKey(labCode) ? employee.LabHours[labCode] : 1.0;

                using (var f = new LabInputForm())
                {
                    // PRE-POPULATE HERE
                    f.LoadLabData(labCode, details[0], details[1], details[2], duration);

                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        // Remove old key in case the Lab Code was changed
                        employee.Labs.Remove(labCode);
                        employee.LabHours.Remove(labCode);

                        // Add updated data
                        employee.Labs[f.LabCode] = new List<string> { f.SelectedDay, f.StartTime, f.EndTime };
                        employee.LabHours[f.LabCode] = f.LabDuration;

                        RefreshWorkList();
                    }
                }
            }
        }

        private void EditExistingGrading(string item)
        {
            // 1. Extract the Class Code from the list item string: "[GRADING] CSC101: 5 hrs"
            // Split by ':' then remove the prefix "[GRADING] "
            string classCode = item.Split(':')[0].Replace("[GRADING] ", "").Trim();

            if (employee.Grading.ContainsKey(classCode))
            {
                // 2. Get the current value to pre-populate the input box
                double currentHours = employee.Grading[classCode];

                // 3. Show the InputBox with the current value already filled in
                string hoursStr = Microsoft.VisualBasic.Interaction.InputBox(
                    $"Edit Weekly Grading Hours for {classCode}:",
                    "Edit Grading Entry",
                    currentHours.ToString() // This pre-populates the text field
                );

                // 4. If the user didn't hit cancel (empty string), update the value
                if (!string.IsNullOrWhiteSpace(hoursStr))
                {
                    if (double.TryParse(hoursStr, out double newHours))
                    {
                        employee.Grading[classCode] = newHours;
                        RefreshWorkList();
                    }
                    else
                    {
                        MessageBox.Show("Please enter a valid number for hours.", "Invalid Input");
                    }
                }
            }
        }

        private void btnAddShift_Click(object sender, EventArgs e)
        {
            using (var shiftForm = new ShiftInputForm())
            {
                if (shiftForm.ShowDialog() == DialogResult.OK)
                {
                    string day = shiftForm.SelectedDay.ToLower();

                    // Ensure dictionaries exist
                    employee.SetShifts ??= new Dictionary<string, List<List<string>>>();
                    if (!employee.SetShifts.ContainsKey(day))
                        employee.SetShifts[day] = new List<List<string>>();

                    // Add the new shift [Start, End]
                    employee.SetShifts[day].Add(new List<string> { shiftForm.StartTime, shiftForm.EndTime });

                    RefreshWorkList(); // Refresh the ListBox in the management window
                }
            }
        }

        private void btnAddLab_Click(object sender, EventArgs e)
        {
            using (var labForm = new LabInputForm())
            {
                if (labForm.ShowDialog() == DialogResult.OK)
                {
                    employee.Labs ??= new Dictionary<string, List<string>>();
                    employee.LabHours ??= new Dictionary<string, double>();

                    employee.Labs[labForm.LabCode] = new List<string> {
                labForm.SelectedDay.ToLower(),
                labForm.StartTime,
                labForm.EndTime
            };
                    employee.LabHours[labForm.LabCode] = labForm.LabDuration;

                    RefreshWorkList();
                }
            }
        }


    }
}