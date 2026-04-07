using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace TutorSchedule
{
    public partial class MainForm : Form
    {
        private EmployeeData employeeData = new EmployeeData { Employees = new System.Collections.Generic.List<Employee>() };
        private string currentFilePath = string.Empty;
        private ContextMenuStrip availableContextMenu;
        private ContextMenuStrip labConflictContextMenu;

        public MainForm()
        {
            InitializeComponent();
            dayComboBox.SelectedIndex = 0;
            SetupContextMenus();
            SetupSelectionBehavior();
            SetupEmployeeListContextMenu();
        }

        // ------------------------------------------------------
        // Context Menu Setup & Selection Rules
        // ------------------------------------------------------
        private void SetupContextMenus()
        {
            // Available list context
            availableContextMenu = new ContextMenuStrip();
            availableContextMenu.Items.Add("Set as Lab Time", null, AddLabMenuItem_Click);
            availableContextMenu.Items.Add("Set as Shift Time", null, AddShiftMenuItem_Click);
            availableListView.ContextMenuStrip = availableContextMenu;

            // Lab conflicts
            labConflictContextMenu = new ContextMenuStrip();
            labConflictContextMenu.Items.Add(
                "Remove Lab Conflict & Set New Time",
                null,
                RemoveLabMenuItem_Click
            );
            labConflictListView.ContextMenuStrip = labConflictContextMenu;
        }

        private void SetupSelectionBehavior()
        {
            void ClearOthers(params ListView[] lists)
            {
                foreach (var lv in lists) lv.SelectedItems.Clear();
            }

            availableListView.SelectedIndexChanged += (s, e) =>
            {
                if (availableListView.Focused)
                    ClearOthers(labConflictListView, shiftConflictListView);
            };
            labConflictListView.SelectedIndexChanged += (s, e) =>
            {
                if (labConflictListView.Focused)
                    ClearOthers(availableListView, shiftConflictListView);
            };
            shiftConflictListView.SelectedIndexChanged += (s, e) =>
            {
                if (shiftConflictListView.Focused)
                    ClearOthers(availableListView, labConflictListView);
            };
            // Wire up double-click events
            availableListView.DoubleClick += SearchList_DoubleClick;
            labConflictListView.DoubleClick += SearchList_DoubleClick;
            shiftConflictListView.DoubleClick += SearchList_DoubleClick;
        }

        // ------------------------------------------------------
        // Context Menu Actions
        // ------------------------------------------------------

        private void AddLabMenuItem_Click(object sender, EventArgs e)
        {
            var emp = GetSelectedEmployeeFromSearch(); // Custom helper to find employee in search results
            if (emp == null) return;

            using (var labForm = new LabInputForm())
            {
                // Pre-populate with current search filters
                labForm.Prepopulate(dayComboBox.SelectedItem.ToString(),
                                   startTimeInput.GetTime24Hour(),
                                   endTimeInput.GetTime24Hour());

                if (labForm.ShowDialog() == DialogResult.OK)
                {
                    emp.Labs ??= new Dictionary<string, List<string>>();
                    emp.LabHours ??= new Dictionary<string, double>();

                    emp.Labs[labForm.LabCode] = new List<string> {
                labForm.SelectedDay.ToLower(),
                labForm.StartTime,
                labForm.EndTime
            };
                    emp.LabHours[labForm.LabCode] = labForm.LabDuration;

                    // CRITICAL: Refresh the UI and re-run search to show the conflict/status change
                    searchButton_Click(null, null);
                    MessageBox.Show($"Lab {labForm.LabCode} added to {emp.Name}");
                }
            }
        }

        private void AddShiftMenuItem_Click(object sender, EventArgs e)
        {
            var emp = GetSelectedEmployeeFromSearch();
            if (emp == null) return;

            // 1. Capture the search parameters
            string selectedDay = dayComboBox.SelectedItem.ToString().ToLower();
            string start = startTimeInput.GetTime24Hour();
            string end = endTimeInput.GetTime24Hour();

            using (var shiftForm = new ShiftInputForm())
            {
                // 2. Pre-populate the form UI
                shiftForm.Prepopulate(selectedDay, start, end);

                if (shiftForm.ShowDialog() == DialogResult.OK)
                {
                    // 3. Ensure the Shift Dictionary and Day List exist
                    emp.SetShifts ??= new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<System.Collections.Generic.List<string>>>();

                    string finalDay = shiftForm.SelectedDay.ToLower();
                    if (!emp.SetShifts.ContainsKey(finalDay))
                    {
                        emp.SetShifts[finalDay] = new System.Collections.Generic.List<System.Collections.Generic.List<string>>();
                    }

                    // 4. Add the shift as [ "Start", "End" ]
                    emp.SetShifts[finalDay].Add(new System.Collections.Generic.List<string> { shiftForm.StartTime, shiftForm.EndTime });

                    // 5. CRITICAL: Update the UI
                    RefreshUI(emp, $"Shift added for {emp.Name} on {finalDay}");

                    // Re-run the search automatically so they move to the "Shift Conflict" list
                    searchButton_Click(sender, e);
                }
            }
        }

        private void RemoveLabMenuItem_Click(object sender, EventArgs e)
        {
            if (labConflictListView.SelectedItems.Count == 0) return;

            var item = labConflictListView.SelectedItems[0];
            string name = item.SubItems[0].Text;
            var emp = employeeData.Employees.FirstOrDefault(x => x.Name == name);
            if (emp?.Labs == null) return;

            string day = dayComboBox.SelectedItem.ToString();
            string start = startTimeInput.GetTime24Hour();
            string end = endTimeInput.GetTime24Hour();

            var conflicts = emp.Labs
                .Where(l =>
                    l.Value.Count == 3 &&
                    l.Value[0] == day &&
                    ScheduleHelper.TimesOverlap(start, end, l.Value[1], l.Value[2]))
                .ToList();

            if (conflicts.Count == 0)
            {
                MessageBox.Show("No conflicting labs found.", "Info");
                return;
            }

            using var removeForm = new RemoveLabForm(conflicts);
            if (removeForm.ShowDialog() != DialogResult.OK) return;

            emp.Labs.Remove(removeForm.SelectedLab);
            emp.Labs[removeForm.NewLabCode] = new System.Collections.Generic.List<string> { day, start, end };
            MessageBox.Show($"Replaced lab {removeForm.SelectedLab} with {removeForm.NewLabCode}", "Lab Updated");
            searchButton_Click(sender, e);
        }

        // ------------------------------------------------------
        // File Menu
        // ------------------------------------------------------
        private void openJSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog();
            dlg.Filter = "JSON files (*.json)|*.json";
            if (dlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                string json = File.ReadAllText(dlg.FileName);
                employeeData = JsonConvert.DeserializeObject<EmployeeData>(json)
                    ?? new EmployeeData { Employees = new System.Collections.Generic.List<Employee>() };

                currentFilePath = dlg.FileName;
                LoadEmployeeList();
                MessageBox.Show("Data loaded successfully.", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading file: {ex.Message}", "Load Error");
            }
        }

        private void saveJSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var dlg = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                FileName = Path.GetFileName(currentFilePath) ?? "shift_data.json"
            };

            if (dlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                string json = JsonConvert.SerializeObject(employeeData, Formatting.Indented);
                File.WriteAllText(dlg.FileName, json);
                currentFilePath = dlg.FileName;
                MessageBox.Show("Data saved successfully!", "Saved");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file: {ex.Message}", "Save Error");
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => this.Close();

        // ------------------------------------------------------
        // Query Search
        // ------------------------------------------------------
        // In MainForm.cs
        private void searchButton_Click(object sender, EventArgs e)
        {
            if (employeeData?.Employees?.Any() != true) return;

            string day = dayComboBox.SelectedItem.ToString();
            string start = startTimeInput.GetTime24Hour();
            string end = endTimeInput.GetTime24Hour();

            var searchResults = ScheduleHelper.SearchAvailableEmployees(employeeData.Employees, day, start, end);

            availableListView.Items.Clear();
            labConflictListView.Items.Clear();
            shiftConflictListView.Items.Clear();

            foreach (var res in searchResults)
            {
                foreach (string avail in res.Available)
                    availableListView.Items.Add(new ListViewItem(new[] { res.EmployeeName, avail }));

                // MODIFIED: List the lab name and the time details
                foreach (string lab in res.LabConflicts)
                {
                    // Assuming res.LabConflicts contains strings like "CS1321 (10:00-11:00)" 
                    // from your ScheduleHelper logic.
                    labConflictListView.Items.Add(new ListViewItem(new[] { res.EmployeeName, lab }));
                }

                foreach (string shift in res.ShiftConflicts)
                    shiftConflictListView.Items.Add(new ListViewItem(new[] { res.EmployeeName, shift }));
            }

            // Ensure these names (gbAvail, etc.) are defined in MainForm.Designer.cs 
            // as private fields, not local variables.
            gbAvail.Text = $"Available ({availableListView.Items.Count})";
            gbLab.Text = $"Lab Conflicts ({labConflictListView.Items.Count})";
            gbShift.Text = $"Shift Conflicts ({shiftConflictListView.Items.Count})";
        }

        // ------------------------------------------------------
        // Employee Data Tab
        // ------------------------------------------------------
        private void LoadEmployeeList()
        {
            employeeListBox.Items.Clear();
            if (employeeData?.Employees == null) return;
            foreach (var emp in employeeData.Employees.OrderBy(e => e.Name))
                employeeListBox.Items.Add(emp.Name);
        }

        private void employeeListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (employeeListBox.SelectedItem == null) return;
            string name = employeeListBox.SelectedItem.ToString();
            var emp = employeeData.Employees.FirstOrDefault(x => x.Name == name);
            if (emp != null) DisplayEmployeeDetails(emp);
        }

        private void DisplayEmployeeDetails(Employee emp)
        {
            if (emp == null) return;

            var sb = new StringBuilder();
            sb.AppendLine($"Name: {emp.Name}");
            sb.AppendLine($"Max Hours: {emp.MaxHours}");
            sb.AppendLine($"Total Hours: {ScheduleHelper.GetTotalWorkHours(emp):0.##}");
            sb.AppendLine($"Preference: {emp.Preference}");
            sb.AppendLine($"Distance: {emp.Distance}");
            sb.AppendLine($"New Hire: {(emp.IsNew ? "Yes" : "No")}");
            sb.AppendLine();

            // SECTION 1: AVAILABILITY
            sb.AppendLine("--- AVAILABILITY ---");
            if (emp.Availabilities != null)
            {
                foreach (var day in emp.Availabilities)
                {
                    foreach (var window in day.Value)
                    {
                        // Convert start and end times to AM/PM format
                        string start = TimeHelper.ConvertTo12Hour(window[0]);
                        string end = TimeHelper.ConvertTo12Hour(window[1]);
                        sb.AppendLine($"{day.Key.ToUpper()}: {start} - {end}");
                    }
                }
            }
            sb.AppendLine();

            // SECTION 2: EXISTING WORK (Labs, Grading, Shifts)
            sb.AppendLine("--- ASSIGNED WORK ---");

            // Labs with Section/Class Numbers
            sb.AppendLine("[Labs]");
            if (emp.Labs != null && emp.Labs.Count > 0)
            {
                // For Labs
                foreach (var lab in emp.Labs)
                {
                    string day = lab.Value[0];
                    string start = TimeHelper.ConvertTo12Hour(lab.Value[1]); // Added conversion
                    string end = TimeHelper.ConvertTo12Hour(lab.Value[2]);   // Added conversion
                    sb.AppendLine($"{lab.Key}: {day} {start}-{end}");
                }
            }
            else sb.AppendLine("None");

            // Grading details
            sb.AppendLine("\r\n[Grading]");
            if (emp.Grading != null && emp.Grading.Count > 0)
            {
                foreach (var g in emp.Grading)
                {
                    sb.AppendLine($"{g.Key}: {g.Value} hrs");
                }
            }
            else sb.AppendLine("None");

            // Regular Shifts
            sb.AppendLine("\r\n[Regular Shifts]");
            if (emp.SetShifts != null && emp.SetShifts.Count > 0)
            {
                foreach (var day in emp.SetShifts)
                {
                    foreach (var shift in day.Value)
                    {
                        string start = TimeHelper.ConvertTo12Hour(shift[0]); // Added conversion
                        string end = TimeHelper.ConvertTo12Hour(shift[1]);   // Added conversion
                        sb.AppendLine($"{day.Key.ToUpper()}: {start} - {end}");
                    }
                }
            }
            else sb.AppendLine("None");

            employeeDetailsTextBox.Text = sb.ToString();
        }

        // ------------------------------------------------------
        // Edit / Add / Delete
        // ------------------------------------------------------
        private void editButton_Click(object sender, EventArgs e)
        {
            if (employeeListBox.SelectedItem == null) return;

            int selectedIndex = employeeListBox.SelectedIndex; // Save the current position
            string name = employeeListBox.SelectedItem.ToString();
            var emp = employeeData.Employees.FirstOrDefault(x => x.Name == name);
            if (emp == null) return;

            using var editForm = new EmployeeEditForm(emp);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadEmployeeList(); // Refresh the names

                // RE-SELECT the employee to trigger the UI update
                if (selectedIndex >= 0 && selectedIndex < employeeListBox.Items.Count)
                {
                    employeeListBox.SelectedIndex = selectedIndex;
                }

                DisplayEmployeeDetails(emp); // Final refresh of the text details
            }
        }

        private void addEmployeeButton_Click(object sender, EventArgs e)
        {
            var newEmp = new Employee
            {
                Name = "New Employee",
                MaxHours = 20,
                Preference = "day",
                Distance = "average",
                IsNew = true,
                Availabilities = new(),
                Labs = new(),
                SetShifts = new()
            };

            using var form = new EmployeeEditForm(newEmp, isNew: true);
            if (form.ShowDialog() == DialogResult.OK)
            {
                employeeData.Employees.Add(newEmp);
                LoadEmployeeList();
                DisplayEmployeeDetails(newEmp);
            }
        }

        private void deleteEmployeeButton_Click(object sender, EventArgs e)
        {
            if (employeeListBox.SelectedItem == null)
            {
                MessageBox.Show("Select someone to delete.", "Warning");
                return;
            }

            string name = employeeListBox.SelectedItem.ToString();
            var emp = employeeData.Employees.FirstOrDefault(x => x.Name == name);
            if (emp == null) return;

            if (MessageBox.Show($"Delete {name}?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                employeeData.Employees.Remove(emp);
                LoadEmployeeList();
                employeeDetailsTextBox.Clear();
            }
        }

        // Inside MainForm.cs

        // 1. Add this call to your MainForm() constructor:
        // SetupEmployeeListContextMenu();

        private void SetupEmployeeListContextMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Add("Add Regular Shift", null, (s, e) => {
                var emp = GetSelectedEmployee(); // Helper to get employee from the ListBox
                if (emp == null) return;

                using (var shiftForm = new ShiftInputForm())
                {
                    if (shiftForm.ShowDialog() == DialogResult.OK)
                    {
                        string day = shiftForm.SelectedDay.ToLower();
                        emp.SetShifts ??= new Dictionary<string, List<List<string>>>();
                        if (!emp.SetShifts.ContainsKey(day))
                            emp.SetShifts[day] = new List<List<string>>();

                        emp.SetShifts[day].Add(new List<string> { shiftForm.StartTime, shiftForm.EndTime });

                        DisplayEmployeeDetails(emp); // Refresh the details text box
                        MessageBox.Show($"Shift added for {emp.Name}");
                    }
                }
            });

            menu.Items.Add("Add Lab", null, (s, e) => {
                var emp = GetSelectedEmployee();
                if (emp == null) return;

                using (var labForm = new LabInputForm())
                {
                    if (labForm.ShowDialog() == DialogResult.OK)
                    {
                        emp.Labs ??= new Dictionary<string, List<string>>();
                        emp.LabHours ??= new Dictionary<string, double>();

                        emp.Labs[labForm.LabCode] = new List<string> {
                    labForm.SelectedDay.ToLower(),
                    labForm.StartTime,
                    labForm.EndTime
                };
                        emp.LabHours[labForm.LabCode] = labForm.LabDuration;

                        DisplayEmployeeDetails(emp);
                        MessageBox.Show($"Lab {labForm.LabCode} added to {emp.Name}");
                    }
                }
            });

            menu.Items.Add("Add Grading", null, (s, e) => {
                var emp = GetSelectedEmployee();
                if (emp == null) return;

                // Use a simple input box to get the class code
                string classCode = Microsoft.VisualBasic.Interaction.InputBox(
                    "Enter Class Code (e.g., CSC101):",
                    "Add Grading Entry",
                    "");

                if (!string.IsNullOrWhiteSpace(classCode))
                {
                    // Get the hours
                    string hoursStr = Microsoft.VisualBasic.Interaction.InputBox(
                        $"Enter weekly hours for {classCode}:",
                        "Grading Hours",
                        "1.0");

                    if (double.TryParse(hoursStr, out double hours))
                    {
                        emp.Grading ??= new Dictionary<string, double>();
                        emp.Grading[classCode.ToUpper()] = hours;

                        DisplayEmployeeDetails(emp);
                        MessageBox.Show($"Added {hours} grading hours for {classCode}");
                    }
                }
            });

            employeeListBox.ContextMenuStrip = menu;
        }

        private void ManageExistingWork_Click(object sender, EventArgs e)
        {
            var emp = GetSelectedEmployee();
            if (emp == null) return;

            // This opens the existing Edit Form where users can manage 
            // availabilities and shifts directly.
            using var form = new EmployeeEditForm(emp);
            if (form.ShowDialog() == DialogResult.OK)
            {
                DisplayEmployeeDetails(emp);
            }
        }

        private void AddRegularWork_Click(string type)
        {
            if (employeeListBox.SelectedItem == null) return;

            string name = employeeListBox.SelectedItem.ToString();
            var emp = employeeData.Employees.FirstOrDefault(x => x.Name == name);
            if (emp == null) return;

            switch (type)
            {
                case "Lab":
                    // Updated to use properties from LabInputForm instead of main window inputs
                    using (var labForm = new LabInputForm())
                    {
                        if (labForm.ShowDialog() == DialogResult.OK)
                        {
                            emp.Labs ??= new();
                            // Day comes from the main window dropdown, times come from the lab popup
                            emp.Labs[labForm.LabCode] = new List<string>
                    {
                        dayComboBox.SelectedItem.ToString(),
                        labForm.StartTime,
                        labForm.EndTime
                    };
                            MessageBox.Show($"Lab {labForm.LabCode} added to {emp.Name}.");
                        }
                    }
                    break;

                case "Shift":
                    // Updated to use a popup form so it's not stuck on Monday 9am
                    using (var shiftForm = new ShiftInputForm())
                    {
                        if (shiftForm.ShowDialog() == DialogResult.OK)
                        {
                            string day = shiftForm.SelectedDay;
                            emp.SetShifts ??= new();
                            if (!emp.SetShifts.ContainsKey(day))
                                emp.SetShifts[day] = new List<List<string>>();

                            emp.SetShifts[day].Add(new List<string> { shiftForm.StartTime, shiftForm.EndTime });
                            MessageBox.Show($"Regular shift added to {emp.Name} on {day}.");
                        }
                    }
                    break;

                case "Grading":
                    string classNum = Microsoft.VisualBasic.Interaction.InputBox("Enter Class Number for Grading:", "Add Grading", "1321");
                    if (!string.IsNullOrWhiteSpace(classNum))
                    {
                        emp.Grading ??= new();
                        // Defaulting to 1.0 hour; user can edit this in EmployeeEditForm
                        emp.Grading[classNum] = 1.0;
                        MessageBox.Show($"Grading for {classNum} added to {emp.Name}.");
                    }
                    break;
            }

            DisplayEmployeeDetails(emp); // Refresh the details text box
        }

        private void AddRegularShift_Click()
        {
            var emp = GetSelectedEmployee();
            if (emp == null) return;

            // Use current UI inputs (dayComboBox/startTimeInput) for the shift details
            string day = dayComboBox.SelectedItem.ToString();
            string start = startTimeInput.GetTime24Hour();
            string end = endTimeInput.GetTime24Hour();

            emp.SetShifts ??= new();
            if (!emp.SetShifts.ContainsKey(day))
                emp.SetShifts[day] = new System.Collections.Generic.List<System.Collections.Generic.List<string>>();

            emp.SetShifts[day].Add(new System.Collections.Generic.List<string> { start, end });
            RefreshUI(emp, $"Added regular shift for {emp.Name} on {day}");
        }

        private void AddLabShift_Click()
        {
            var emp = GetSelectedEmployee();
            if (emp == null) return;

            using var labForm = new LabInputForm();
            if (labForm.ShowDialog() == DialogResult.OK)
            {
                emp.Labs ??= new();
                // Now using the StartTime and EndTime provided by the LabInputForm
                emp.Labs[labForm.LabCode] = new System.Collections.Generic.List<string>
        {
            dayComboBox.SelectedItem.ToString(),
            labForm.StartTime,
            labForm.EndTime
        };
                RefreshUI(emp, $"Added lab {labForm.LabCode} for {emp.Name}");
            }
        }

        private void AddGrading_Click()
        {
            var emp = GetSelectedEmployee();
            if (emp == null) return;

            string classCode = Microsoft.VisualBasic.Interaction.InputBox("Enter Class Number:", "Add Grading", "");
            if (string.IsNullOrWhiteSpace(classCode)) return;

            // New: Prompt for hours
            string hoursStr = Microsoft.VisualBasic.Interaction.InputBox("Enter Weekly Grading Hours:", "Grading Hours", "1.0");
            if (double.TryParse(hoursStr, out double hours))
            {
                emp.Grading ??= new();
                emp.Grading[classCode] = hours;
                RefreshUI(emp, $"Added {hours} grading hours for {classCode}");
            }
        }

        private void RefreshUI(Employee emp, string message)
        {
            DisplayEmployeeDetails(emp);
            MessageBox.Show(message, "Success");
        }

        private void EditShiftsMenuItem_Click(object sender, EventArgs e)
        {
            // Get the employee currently selected in the list box
            var emp = GetSelectedEmployee();
            if (emp == null)
            {
                MessageBox.Show("Please select an employee first.");
                return;
            }

            // Launch the ShiftManagementForm to manage Shifts, Labs, and Grading
            using (var shiftMgr = new ShiftManagementForm(emp))
            {
                shiftMgr.ShowDialog();

                // Refresh the details text box to show updated shifts in AM/PM format
                RefreshUI(emp, "Shifts and work assignments updated successfully.");
            }
        }

        private Employee GetSelectedEmployee()
        {
            if (employeeListBox.SelectedItem == null) return null;
            return employeeData.Employees.FirstOrDefault(x => x.Name == employeeListBox.SelectedItem.ToString());
        }

        private Employee GetSelectedEmployeeFromSearch()
        {
            ListView activeList = null;
            if (availableListView.SelectedItems.Count > 0) activeList = availableListView;
            else if (labConflictListView.SelectedItems.Count > 0) activeList = labConflictListView;
            else if (shiftConflictListView.SelectedItems.Count > 0) activeList = shiftConflictListView;

            if (activeList == null) return null;

            string empName = activeList.SelectedItems[0].Text;
            return employeeData.Employees.FirstOrDefault(x => x.Name == empName);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            AutoSaveData();
        }

        private void AutoSaveData()
        {
            try
            {
                // If a file was never opened/saved, use the Documents folder instead of the App folder
                string savePath = currentFilePath;
                if (string.IsNullOrEmpty(savePath))
                {
                    string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    savePath = Path.Combine(docPath, "TutorSchedule_Autosave.json");
                }

                string json = JsonConvert.SerializeObject(employeeData, Formatting.Indented);
                File.WriteAllText(savePath, json);

                // Optional: Logs the path to the Output window in Visual Studio
                MessageBox.Show("Saved to: " + savePath);
                System.Diagnostics.Debug.WriteLine($"Data autosaved to: {savePath}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Autosave failed: {ex.Message}");
            }
        }

        private void SearchList_DoubleClick(object sender, EventArgs e)
        {
            var emp = GetSelectedEmployeeFromSearch();
            if (emp == null) return;

            using (var shiftMgr = new ShiftManagementForm(emp))
            {
                if (shiftMgr.ShowDialog() == DialogResult.OK)
                {
                    DisplayEmployeeDetails(emp);
                    searchButton_Click(sender, e); // Refresh search results to show new status
                }
            }
        }

    }
}