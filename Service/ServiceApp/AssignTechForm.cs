using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ServiceAppSystem
{
    public class AssignTechForm : Form
    {
        private readonly DispatchManager manager;
        private readonly int requestId;
        private ComboBox cboTechnician;

        public AssignTechForm(DispatchManager mgr, int reqId)
        {
            manager = mgr;
            requestId = reqId;
            InitializeUI();
        }

        private void InitializeUI()
        {
            Text = "Assign Technician";
            Size = new Size(400, 200);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var lblInfo = new Label
            {
                Text = $"Assign technician to Request #{requestId}",
                Location = new Point(20, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 10F)
            };

            var lblTech = new Label
            {
                Text = "Technician:",
                Location = new Point(20, 60),
                AutoSize = true
            };

            cboTechnician = new ComboBox
            {
                Location = new Point(110, 57),
                Width = 250,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            LoadTechnicians();

            var btnAssign = new Button
            {
                Text = "Assign",
                Location = new Point(200, 110),
                Size = new Size(80, 30),
                DialogResult = DialogResult.OK
            };
            btnAssign.Click += BtnAssign_Click;

            var btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(290, 110),
                Size = new Size(80, 30),
                DialogResult = DialogResult.Cancel
            };

            Controls.AddRange(new Control[] { lblInfo, lblTech, cboTechnician, btnAssign, btnCancel });
            AcceptButton = btnAssign;
            CancelButton = btnCancel;
        }

        private void LoadTechnicians()
        {
            cboTechnician.Items.Clear();
            cboTechnician.Items.Add("-- Select Technician --");
            
            foreach (var tech in manager.Technicians)
            {
                cboTechnician.Items.Add($"{tech.TechId} - {tech.Name}");
            }
            
            cboTechnician.SelectedIndex = 0;
        }

        private void BtnAssign_Click(object? sender, EventArgs e)
        {
            if (cboTechnician.SelectedIndex <= 0)
            {
                MessageBox.Show("Please select a technician.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            try
            {
                var techId = int.Parse(cboTechnician.Text.Split('-')[0].Trim());
                
                if (manager.AssignManual(requestId, techId))
                {
                    MessageBox.Show("Technician assigned successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to assign technician. The technician may be busy at that time.", 
                        "Assignment Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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