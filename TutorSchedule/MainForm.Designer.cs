namespace TutorSchedule
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button editShiftsButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openJSONToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveJSONToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.queryTab = new System.Windows.Forms.TabPage();
            this.employeeTab = new System.Windows.Forms.TabPage();
            this.deleteEmployeeButton = new System.Windows.Forms.Button();
            this.addEmployeeButton = new System.Windows.Forms.Button();
            this.editButton = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.employeeDetailsTextBox = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.employeeListBox = new System.Windows.Forms.ListBox();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.employeeTab.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();

            // 1. Instantiate the button
            this.editShiftsButton = new System.Windows.Forms.Button();

            // 2. Configure its appearance and position
            this.editShiftsButton.BackColor = System.Drawing.Color.FromArgb(255, 128, 0); // Orange color to distinguish it
            this.editShiftsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.editShiftsButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.editShiftsButton.ForeColor = System.Drawing.Color.White;
            // Placing it to the left of the existing "Edit Employee" button
            this.editShiftsButton.Location = new System.Drawing.Point(335, 560);
            this.editShiftsButton.Size = new System.Drawing.Size(150, 35);
            this.editShiftsButton.Text = "Edit Shifts/Labs";
            this.editShiftsButton.UseVisualStyleBackColor = false;

            // 3. Attach the click event (Logic in MainForm.cs)
            this.editShiftsButton.Click += new System.EventHandler(this.EditShiftsMenuItem_Click);

            // 4. Add it to the employeeTab controls
            this.employeeTab.Controls.Add(this.editShiftsButton);

            // MENU STRIP
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.fileToolStripMenuItem });
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Size = new System.Drawing.Size(984, 24);
            this.menuStrip1.TabIndex = 0;

            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.openJSONToolStripMenuItem,
                this.saveJSONToolStripMenuItem,
                this.toolStripSeparator1,
                this.exitToolStripMenuItem
            });
            this.fileToolStripMenuItem.Text = "File";

            this.openJSONToolStripMenuItem.Text = "Open JSON...";
            this.openJSONToolStripMenuItem.Click += new System.EventHandler(this.openJSONToolStripMenuItem_Click);

            this.saveJSONToolStripMenuItem.Text = "Save JSON...";
            this.saveJSONToolStripMenuItem.Click += new System.EventHandler(this.saveJSONToolStripMenuItem_Click);

            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);

            // TAB CONTROL
            this.tabControl1.Controls.Add(this.queryTab);
            this.tabControl1.Controls.Add(this.employeeTab);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Size = new System.Drawing.Size(984, 637);
            this.tabControl1.SelectedIndex = 0;



            // ====================
            // QUERY TAB
            // ====================
            this.queryTab.Text = "Query Availability";
            this.queryTab.UseVisualStyleBackColor = true;

            var splitContainer = new System.Windows.Forms.SplitContainer();
            splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer.Orientation = System.Windows.Forms.Orientation.Vertical;
            splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            splitContainer.IsSplitterFixed = false;
            this.queryTab.Controls.Add(splitContainer);

            // ---------- LEFT FILTER PANEL ----------
            System.Windows.Forms.GroupBox filterGroup = new System.Windows.Forms.GroupBox();
            filterGroup.Text = "Search Options";
            filterGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            filterGroup.Padding = new System.Windows.Forms.Padding(10);
            splitContainer.Panel1.Controls.Add(filterGroup);

            int lblX = 20;
            int ctrlX = 120;
            int y = 30;
            int spacing = 35;

            // Day
            System.Windows.Forms.Label lblDay = new System.Windows.Forms.Label();
            lblDay.Text = "Day:";
            lblDay.AutoSize = true;
            lblDay.Location = new System.Drawing.Point(lblX, y + 3);
            filterGroup.Controls.Add(lblDay);

            this.dayComboBox = new System.Windows.Forms.ComboBox();
            this.dayComboBox.Location = new System.Drawing.Point(ctrlX, y);
            this.dayComboBox.Width = 110;
            this.dayComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.dayComboBox.Items.AddRange(new object[] { "mon", "tue", "wed", "thu", "fri", "sat" });
            this.dayComboBox.SelectedItem = "mon";
            filterGroup.Controls.Add(this.dayComboBox);

            // Start
            y += spacing;
            System.Windows.Forms.Label lblStart = new System.Windows.Forms.Label();
            lblStart.Text = "Start:";
            lblStart.AutoSize = true;
            lblStart.Location = new System.Drawing.Point(lblX, y + 3);
            filterGroup.Controls.Add(lblStart);

            this.startTimeInput = new TutorSchedule.TimeInputControl();
            this.startTimeInput.Location = new System.Drawing.Point(ctrlX, y);
            this.startTimeInput.Size = new System.Drawing.Size(120, 25);
            filterGroup.Controls.Add(this.startTimeInput);

            // End
            y += spacing;
            System.Windows.Forms.Label lblEnd = new System.Windows.Forms.Label();
            lblEnd.Text = "End:";
            lblEnd.AutoSize = true;
            lblEnd.Location = new System.Drawing.Point(lblX, y + 3);
            filterGroup.Controls.Add(lblEnd);

            this.endTimeInput = new TutorSchedule.TimeInputControl();
            this.endTimeInput.Location = new System.Drawing.Point(ctrlX, y);
            this.endTimeInput.Size = new System.Drawing.Size(120, 25);
            filterGroup.Controls.Add(this.endTimeInput);

            // Preference
            y += spacing;
            System.Windows.Forms.Label lblPref = new System.Windows.Forms.Label();
            lblPref.Text = "Preference:";
            lblPref.AutoSize = true;
            lblPref.Location = new System.Drawing.Point(lblX, y + 3);
            filterGroup.Controls.Add(lblPref);

            this.prefComboBox = new System.Windows.Forms.ComboBox();
            this.prefComboBox.Location = new System.Drawing.Point(ctrlX, y);
            this.prefComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.prefComboBox.Items.AddRange(new object[] { "", "morning", "day", "evening" });
            filterGroup.Controls.Add(this.prefComboBox);

            // Distance
            y += spacing;
            System.Windows.Forms.Label lblDist = new System.Windows.Forms.Label();
            lblDist.Text = "Distance:";
            lblDist.AutoSize = true;
            lblDist.Location = new System.Drawing.Point(lblX, y + 3);
            filterGroup.Controls.Add(lblDist);

            this.distComboBox = new System.Windows.Forms.ComboBox();
            this.distComboBox.Location = new System.Drawing.Point(ctrlX, y);
            this.distComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.distComboBox.Items.AddRange(new object[] { "", "near", "average", "far" });
            filterGroup.Controls.Add(this.distComboBox);

            // Is New
            y += spacing;
            System.Windows.Forms.Label lblNew = new System.Windows.Forms.Label();
            lblNew.Text = "Is New:";
            lblNew.AutoSize = true;
            lblNew.Location = new System.Drawing.Point(lblX, y + 3);
            filterGroup.Controls.Add(lblNew);

            this.isNewComboBox = new System.Windows.Forms.ComboBox();
            this.isNewComboBox.Location = new System.Drawing.Point(ctrlX, y);
            this.isNewComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.isNewComboBox.Items.AddRange(new object[] { "", "True", "False" });
            filterGroup.Controls.Add(this.isNewComboBox);

            // Max Hours
            y += spacing;
            System.Windows.Forms.Label lblMax = new System.Windows.Forms.Label();
            lblMax.Text = "Max Hours:";
            lblMax.AutoSize = true;
            lblMax.Location = new System.Drawing.Point(lblX, y + 3);
            filterGroup.Controls.Add(lblMax);

            this.maxHoursNumeric = new System.Windows.Forms.NumericUpDown();
            this.maxHoursNumeric.Location = new System.Drawing.Point(ctrlX, y);
            this.maxHoursNumeric.Width = 120;
            this.maxHoursNumeric.Maximum = 40;
            this.maxHoursNumeric.Value = 0;
            filterGroup.Controls.Add(this.maxHoursNumeric);

            // Search Button
            y += spacing + 10;
            this.searchButton = new System.Windows.Forms.Button();
            this.searchButton.Text = "Search";
            this.searchButton.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.searchButton.ForeColor = System.Drawing.Color.White;
            this.searchButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.searchButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.searchButton.Size = new System.Drawing.Size(130, 35);
            this.searchButton.Location = new System.Drawing.Point(65, y);
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            filterGroup.Controls.Add(this.searchButton);

            // ---------- RIGHT RESULTS PANEL ----------
            var resultsLayout = new System.Windows.Forms.TableLayoutPanel();
            resultsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            resultsLayout.ColumnCount = 1;
            resultsLayout.RowCount = 3;
            resultsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            resultsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            resultsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            splitContainer.Panel2.Controls.Add(resultsLayout);

            var rightFont = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);

            // Available
            gbAvail = new System.Windows.Forms.GroupBox();
            gbAvail.Text = "Available";
            gbAvail.Font = rightFont;
            gbAvail.Dock = System.Windows.Forms.DockStyle.Fill;

            this.availableListView = new System.Windows.Forms.ListView();
            this.availableListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.availableListView.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.availableListView.View = System.Windows.Forms.View.Details;
            this.availableListView.FullRowSelect = true;
            this.availableListView.Columns.Add("Employee", 200, System.Windows.Forms.HorizontalAlignment.Left);
            this.availableListView.Columns.Add("Time Availabile", 200, System.Windows.Forms.HorizontalAlignment.Left);
            gbAvail.Controls.Add(this.availableListView);
            resultsLayout.Controls.Add(gbAvail, 0, 0);

            // Lab Conflicts
            gbLab = new System.Windows.Forms.GroupBox();
            gbLab.Text = "Lab Conflicts";
            gbLab.Font = rightFont;
            gbLab.Dock = System.Windows.Forms.DockStyle.Fill;

            this.labConflictListView = new System.Windows.Forms.ListView();
            this.labConflictListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labConflictListView.Font = new System.Drawing.Font("Consolas", 9F);
            this.labConflictListView.View = System.Windows.Forms.View.Details;
            this.labConflictListView.FullRowSelect = true;
            // Inside InitializeComponent in MainForm.Designer.cs
            this.labConflictListView.Columns.Add("Employee", 150, System.Windows.Forms.HorizontalAlignment.Left);
            this.labConflictListView.Columns.Add("Lab & Time", 250, System.Windows.Forms.HorizontalAlignment.Left);
            gbLab.Controls.Add(this.labConflictListView);
            resultsLayout.Controls.Add(gbLab, 0, 1);

            // Shift Conflicts
            gbShift = new System.Windows.Forms.GroupBox();
            gbShift.Text = "Shift Conflicts";
            gbShift.Font = rightFont;
            gbShift.Dock = System.Windows.Forms.DockStyle.Fill;

            this.shiftConflictListView = new System.Windows.Forms.ListView();
            this.shiftConflictListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.shiftConflictListView.Font = new System.Drawing.Font("Consolas", 9F);
            this.shiftConflictListView.View = System.Windows.Forms.View.Details;
            this.shiftConflictListView.FullRowSelect = true;
            this.shiftConflictListView.Columns.Add("Employee", 200, System.Windows.Forms.HorizontalAlignment.Left);
            this.shiftConflictListView.Columns.Add("Conflict Time", 200, System.Windows.Forms.HorizontalAlignment.Left);
            gbShift.Controls.Add(this.shiftConflictListView);
            resultsLayout.Controls.Add(gbShift, 0, 2);

            // HandleCreated event safely applies SplitterDistance
            splitContainer.HandleCreated += (s, e) =>
            {
                int desiredLeftWidth = 360; // move divider right
                if (desiredLeftWidth < splitContainer.Width)
                    splitContainer.SplitterDistance = desiredLeftWidth;

                splitContainer.PerformLayout();
                splitContainer.Panel2.PerformLayout();
            };

            // =====================
            // EMPLOYEE TAB
            // =====================
            this.employeeTab.Text = "Employee Data";
            this.employeeTab.UseVisualStyleBackColor = true;
            this.employeeTab.Controls.Add(this.deleteEmployeeButton);
            this.employeeTab.Controls.Add(this.addEmployeeButton);
            this.employeeTab.Controls.Add(this.editButton);
            this.employeeTab.Controls.Add(this.groupBox5);
            this.employeeTab.Controls.Add(this.groupBox4);

            this.deleteEmployeeButton.BackColor = System.Drawing.Color.Firebrick;
            this.deleteEmployeeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deleteEmployeeButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.deleteEmployeeButton.ForeColor = System.Drawing.Color.White;
            this.deleteEmployeeButton.Location = new System.Drawing.Point(800, 560);
            this.deleteEmployeeButton.Size = new System.Drawing.Size(150, 35);
            this.deleteEmployeeButton.Text = "Delete Employee";
            this.deleteEmployeeButton.UseVisualStyleBackColor = false;
            this.deleteEmployeeButton.Click += new System.EventHandler(this.deleteEmployeeButton_Click);

            this.addEmployeeButton.BackColor = System.Drawing.Color.ForestGreen;
            this.addEmployeeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addEmployeeButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.addEmployeeButton.ForeColor = System.Drawing.Color.White;
            this.addEmployeeButton.Location = new System.Drawing.Point(645, 560);
            this.addEmployeeButton.Size = new System.Drawing.Size(150, 35);
            this.addEmployeeButton.Text = "Add Employee";
            this.addEmployeeButton.UseVisualStyleBackColor = false;
            this.addEmployeeButton.Click += new System.EventHandler(this.addEmployeeButton_Click);

            this.editButton.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.editButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.editButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.editButton.ForeColor = System.Drawing.Color.White;
            this.editButton.Location = new System.Drawing.Point(490, 560);
            this.editButton.Size = new System.Drawing.Size(150, 35);
            this.editButton.Text = "Edit Employee";
            this.editButton.UseVisualStyleBackColor = false;
            this.editButton.Click += new System.EventHandler(this.editButton_Click);

            this.groupBox5.Controls.Add(this.employeeDetailsTextBox);
            this.groupBox5.Location = new System.Drawing.Point(323, 13);
            this.groupBox5.Size = new System.Drawing.Size(640, 535);
            this.groupBox5.Text = "Employee Details";

            this.employeeDetailsTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.employeeDetailsTextBox.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.employeeDetailsTextBox.Multiline = true;
            this.employeeDetailsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.employeeDetailsTextBox.ReadOnly = true;

            this.groupBox4.Controls.Add(this.employeeListBox);
            this.groupBox4.Location = new System.Drawing.Point(13, 13);
            this.groupBox4.Size = new System.Drawing.Size(300, 582);
            this.groupBox4.Text = "Employees";

            this.employeeListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.employeeListBox.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.employeeListBox.ItemHeight = 15;
            this.employeeListBox.SelectedIndexChanged += new System.EventHandler(this.employeeListBox_SelectedIndexChanged);

            // MAIN FORM
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.ClientSize = new System.Drawing.Size(984, 661);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "KSU Tutoring Center - Schedule Manager";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.employeeTab.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openJSONToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveJSONToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage queryTab;
        private System.Windows.Forms.TabPage employeeTab;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox employeeDetailsTextBox;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ListBox employeeListBox;
        private System.Windows.Forms.Button editButton;
        private System.Windows.Forms.Button addEmployeeButton;
        private System.Windows.Forms.Button deleteEmployeeButton;
        private System.Windows.Forms.ComboBox dayComboBox;
        private TutorSchedule.TimeInputControl startTimeInput;
        private TutorSchedule.TimeInputControl endTimeInput;
        private System.Windows.Forms.ComboBox prefComboBox;
        private System.Windows.Forms.ComboBox distComboBox;
        private System.Windows.Forms.ComboBox isNewComboBox;
        private System.Windows.Forms.NumericUpDown maxHoursNumeric;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.ListView availableListView;
        private System.Windows.Forms.ListView labConflictListView;
        private System.Windows.Forms.ListView shiftConflictListView;
        private System.Windows.Forms.GroupBox gbAvail;
        private System.Windows.Forms.GroupBox gbLab;
        private System.Windows.Forms.GroupBox gbShift;
    }
}