using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TutorSchedule
{
    public partial class RemoveLabForm : Form
    {
        private ComboBox labComboBox;
        private TextBox newLabTextBox;
        private Button okButton;
        private Button cancelButton;

        public string SelectedLab { get; private set; }
        public string NewLabCode { get; private set; }

        public RemoveLabForm(
            List<KeyValuePair<string, List<string>>> conflictingLabs
        )
        {
            InitializeComponent(conflictingLabs);
        }

        private void InitializeComponent(
            List<KeyValuePair<string, List<string>>> conflictingLabs
        )
        {
            this.Text = "Remove Lab & Add New";
            this.Size = new Size(400, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var label1 = new Label
            {
                Text = "Select lab to remove:",
                Location = new Point(20, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.75F)
            };

            labComboBox = new ComboBox
            {
                Location = new Point(20, 45),
                Width = 340,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.75F)
            };
            foreach (var lab in conflictingLabs)
            {
                labComboBox.Items.Add(lab.Key);
            }
            if (labComboBox.Items.Count > 0)
                labComboBox.SelectedIndex = 0;

            var label2 = new Label
            {
                Text = "New lab code (e.g., 1321-W01):",
                Location = new Point(20, 80),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.75F)
            };

            newLabTextBox = new TextBox
            {
                Location = new Point(20, 105),
                Width = 340,
                Font = new Font("Segoe UI", 9.75F)
            };

            okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(180, 140),
                Width = 80,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            okButton.Click += OkButton_Click;

            cancelButton = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(270, 140),
                Width = 80
            };

            this.Controls.AddRange(
                new Control[]
                {
                    label1,
                    labComboBox,
                    label2,
                    newLabTextBox,
                    okButton,
                    cancelButton
                }
            );
            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (
                labComboBox.SelectedItem == null
                || string.IsNullOrWhiteSpace(newLabTextBox.Text)
            )
            {
                MessageBox.Show(
                    "Please select a lab to remove and enter a new lab code",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                this.DialogResult = DialogResult.None;
                return;
            }

            SelectedLab = labComboBox.SelectedItem.ToString();
            NewLabCode = newLabTextBox.Text.Trim();
        }
    }
}