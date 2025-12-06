using System;
using System.Drawing;
using System.Windows.Forms;

namespace ServiceAppSystem
{
    public class CompleteJobForm : Form
    {
        private readonly DispatchManager manager;
        private readonly int requestId;
        private TextBox txtHours;
        private TextBox txtParts;

        public CompleteJobForm(DispatchManager mgr, int reqId)
        {
            manager = mgr;
            requestId = reqId;
            InitializeUI();
        }

        private void InitializeUI()
        {
            Text = "Complete Job";
            Size = new Size(400, 230);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var lblInfo = new Label
            {
                Text = $"Complete Request #{requestId}",
                Location = new Point(20, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            var lblHours = new Label
            {
                Text = "Hours Worked:",
                Location = new Point(20, 60),
                AutoSize = true
            };

            txtHours = new TextBox
            {
                Location = new Point(140, 57),
                Width = 150
            };

            var lblParts = new Label
            {
                Text = "Parts Cost ($):",
                Location = new Point(20, 100),
                AutoSize = true
            };

            txtParts = new TextBox
            {
                Location = new Point(140, 97),
                Width = 150
            };

            var btnComplete = new Button
            {
                Text = "Complete",
                Location = new Point(180, 150),
                Size = new Size(90, 30),
                DialogResult = DialogResult.OK
            };
            btnComplete.Click += BtnComplete_Click;

            var btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(280, 150),
                Size = new Size(90, 30),
                DialogResult = DialogResult.Cancel
            };

            Controls.AddRange(new Control[] { 
                lblInfo, lblHours, txtHours, lblParts, txtParts, btnComplete, btnCancel 
            });
            
            AcceptButton = btnComplete;
            CancelButton = btnCancel;
        }

        private void BtnComplete_Click(object? sender, EventArgs e)
        {
            if (!decimal.TryParse(txtHours.Text, out decimal hours) || hours < 0)
            {
                MessageBox.Show("Please enter valid hours worked.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            if (!decimal.TryParse(txtParts.Text, out decimal parts) || parts < 0)
            {
                MessageBox.Show("Please enter valid parts cost.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            try
            {
                if (manager.Complete(requestId, hours, parts))
                {
                    MessageBox.Show("Job completed successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to complete job.", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DialogResult = DialogResult.None;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
            }
        }
    }
}