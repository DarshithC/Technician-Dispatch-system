using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace ServiceAppSystem
{
    public class MainForm : Form
    {
        private readonly DispatchManager manager;
        private TabControl tabControl;

        // Tab pages
        private TabPage tabDashboard;
        private TabPage tabCustomers;
        private TabPage tabTechnicians;
        private TabPage tabRequests;
        private TabPage tabReports;

        // Color scheme
        private readonly Color primaryColor = Color.FromArgb(41, 128, 185);      // Blue
        private readonly Color secondaryColor = Color.FromArgb(52, 152, 219);    // Light Blue
        private readonly Color successColor = Color.FromArgb(46, 204, 113);      // Green
        private readonly Color warningColor = Color.FromArgb(241, 196, 15);      // Yellow
        private readonly Color dangerColor = Color.FromArgb(231, 76, 60);        // Red
        private readonly Color lightBg = Color.FromArgb(236, 240, 241);          // Light Gray
        private readonly Color darkText = Color.FromArgb(44, 62, 80);            // Dark Blue-Gray

        public MainForm()
        {
            var ids = new IdGenerator();
            manager = new DispatchManager(ids);
            manager.Load();

            InitializeUI();
        }

        private void InitializeUI()
        {
            // Form properties
            Text = "Service Dispatch Management System";
            Size = new Size(1200, 750);
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1000, 650);
            BackColor = lightBg;
            Icon = SystemIcons.Application;

            // Create tab control
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                Padding = new Point(15, 8)
            };

            // Create tabs
            tabDashboard = new TabPage("🏠 Dashboard") { BackColor = lightBg };
            tabCustomers = new TabPage("👥 Customers") { BackColor = lightBg };
            tabTechnicians = new TabPage("🔧 Technicians") { BackColor = lightBg };
            tabRequests = new TabPage("📋 Service Requests") { BackColor = lightBg };
            tabReports = new TabPage("📊 Reports") { BackColor = lightBg };

            tabControl.TabPages.AddRange(new[] { 
                tabDashboard, tabCustomers, tabTechnicians, tabRequests, tabReports 
            });

            // Initialize each tab
            InitializeDashboard();
            InitializeCustomersTab();
            InitializeTechniciansTab();
            InitializeRequestsTab();
            InitializeReportsTab();

            Controls.Add(tabControl);
        }

        private Button CreateStyledButton(string text, Color bgColor, int width = 130, int height = 38)
        {
            return new Button
            {
                Text = text,
                Size = new Size(width, height),
                BackColor = bgColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(5)
            };
        }

        private Panel CreateCard(int x, int y, int width, int height)
        {
            return new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private void InitializeDashboard()
        {
            var mainPanel = new Panel { Dock = DockStyle.Fill, BackColor = lightBg, Padding = new Padding(30) };
            
            var lblTitle = new Label
            {
                Text = "📊 Dashboard Overview",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = darkText,
                Location = new Point(30, 20),
                AutoSize = true
            };

            // Stats Cards
            var cardCustomers = CreateDashboardCard("👥 Total Customers", "0", primaryColor, 30, 80);
            var cardTechs = CreateDashboardCard("🔧 Total Technicians", "0", secondaryColor, 280, 80);
            var cardOpen = CreateDashboardCard("📋 Open Requests", "0", warningColor, 530, 80);
            var cardCompleted = CreateDashboardCard("✅ Completed Jobs", "0", successColor, 780, 80);
            
            var cardRevenue = new Panel
            {
                Location = new Point(30, 250),
                Size = new Size(480, 120),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            
            var lblRevTitle = new Label
            {
                Text = "💰 Total Revenue",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = darkText,
                Location = new Point(20, 15),
                AutoSize = true
            };
            
            var lblRevAmount = new Label
            {
                Text = "$0.00",
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = successColor,
                Location = new Point(20, 50),
                AutoSize = true,
                Name = "lblRevAmount"
            };
            
            cardRevenue.Controls.AddRange(new Control[] { lblRevTitle, lblRevAmount });

            var btnRefresh = CreateStyledButton("🔄 Refresh Dashboard", primaryColor, 180, 42);
            btnRefresh.Location = new Point(30, 400);
            btnRefresh.Click += (s, e) => UpdateDashboard(cardCustomers, cardTechs, cardOpen, cardCompleted, lblRevAmount);

            mainPanel.Controls.AddRange(new Control[] { 
                lblTitle, cardCustomers, cardTechs, cardOpen, cardCompleted, cardRevenue, btnRefresh 
            });
            
            tabDashboard.Controls.Add(mainPanel);
            UpdateDashboard(cardCustomers, cardTechs, cardOpen, cardCompleted, lblRevAmount);
        }

        private Panel CreateDashboardCard(string title, string value, Color accentColor, int x, int y)
        {
            var card = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(230, 140),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var topBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 8,
                BackColor = accentColor
            };

            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = darkText,
                Location = new Point(15, 25),
                AutoSize = true
            };

            var lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 32F, FontStyle.Bold),
                ForeColor = accentColor,
                Location = new Point(15, 60),
                AutoSize = true,
                Name = "value"
            };

            card.Controls.AddRange(new Control[] { topBar, lblTitle, lblValue });
            return card;
        }

        private void UpdateDashboard(Panel cardCust, Panel cardTech, Panel cardOpen, Panel cardComp, Label lblRev)
        {
            manager.Load();
            
            (cardCust.Controls["value"] as Label).Text = manager.Customers.Count.ToString();
            (cardTech.Controls["value"] as Label).Text = manager.Technicians.Count.ToString();
            (cardOpen.Controls["value"] as Label).Text = manager.Open().Count.ToString();
            (cardComp.Controls["value"] as Label).Text = manager.Completed().Count.ToString();
            lblRev.Text = $"${manager.Revenue():N2}";
        }

        private void InitializeCustomersTab()
        {
            var mainPanel = new Panel { Dock = DockStyle.Fill, BackColor = lightBg, Padding = new Padding(20) };

            var lblTitle = new Label
            {
                Text = "👥 Customer Management",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = darkText,
                Location = new Point(20, 15),
                AutoSize = true
            };

            // Input Card
            var inputCard = new Panel
            {
                Location = new Point(20, 60),
                Size = new Size(1120, 140),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblName = new Label { Text = "Name:", Location = new Point(20, 25), AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var txtName = new TextBox { Location = new Point(20, 50), Width = 300, Height = 30, Font = new Font("Segoe UI", 10F), Name = "txtName" };

            var lblPhone = new Label { Text = "Phone:", Location = new Point(350, 25), AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var txtPhone = new TextBox { Location = new Point(350, 50), Width = 250, Height = 30, Font = new Font("Segoe UI", 10F), Name = "txtPhone" };

            var lblAddress = new Label { Text = "Address:", Location = new Point(630, 25), AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var txtAddress = new TextBox { Location = new Point(630, 50), Width = 450, Height = 30, Font = new Font("Segoe UI", 10F), Name = "txtAddress" };

            var btnAdd = CreateStyledButton("➕ Add Customer", successColor, 150);
            btnAdd.Location = new Point(20, 95);

            var btnClear = CreateStyledButton("🗑️ Clear", dangerColor, 100);
            btnClear.Location = new Point(180, 95);
            btnClear.Click += (s, e) => { txtName.Clear(); txtPhone.Clear(); txtAddress.Clear(); };

            inputCard.Controls.AddRange(new Control[] { lblName, txtName, lblPhone, txtPhone, lblAddress, txtAddress, btnAdd, btnClear });

            // ListView
            var listView = new ListView
            {
                Location = new Point(20, 220),
                Size = new Size(1120, 400),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.White
            };
            listView.Columns.Add("ID", 80);
            listView.Columns.Add("Name", 280);
            listView.Columns.Add("Phone", 200);
            listView.Columns.Add("Address", 540);

            var btnRefresh = CreateStyledButton("🔄 Refresh", primaryColor, 120);
            btnRefresh.Location = new Point(920, 625);

            btnAdd.Click += (s, e) => AddCustomerClick(txtName, txtPhone, txtAddress, listView);
            btnRefresh.Click += (s, e) => RefreshCustomerList(listView);

            mainPanel.Controls.AddRange(new Control[] { lblTitle, inputCard, listView, btnRefresh });
            tabCustomers.Controls.Add(mainPanel);
            RefreshCustomerList(listView);
        }

        private void AddCustomerClick(TextBox txtName, TextBox txtPhone, TextBox txtAddress, ListView listView)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Please enter customer name.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                manager.AddCustomer(txtName.Text, txtPhone.Text, txtAddress.Text);
                
                txtName.Clear();
                txtPhone.Clear();
                txtAddress.Clear();

                RefreshCustomerList(listView);
                MessageBox.Show("✅ Customer added successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshCustomerList(ListView listView)
        {
            listView.Items.Clear();
            manager.Load();
            foreach (var customer in manager.Customers)
            {
                var item = new ListViewItem(customer.CustomerId.ToString());
                item.SubItems.Add(customer.Name);
                item.SubItems.Add(customer.Phone);
                item.SubItems.Add(customer.Address);
                listView.Items.Add(item);
            }
        }

        private void InitializeTechniciansTab()
        {
            var mainPanel = new Panel { Dock = DockStyle.Fill, BackColor = lightBg, Padding = new Padding(20) };

            var lblTitle = new Label
            {
                Text = "🔧 Technician Management",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = darkText,
                Location = new Point(20, 15),
                AutoSize = true
            };

            // Input Card
            var inputCard = new Panel
            {
                Location = new Point(20, 60),
                Size = new Size(1120, 140),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblName = new Label { Text = "Name:", Location = new Point(20, 25), AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var txtName = new TextBox { Location = new Point(20, 50), Width = 280, Height = 30, Font = new Font("Segoe UI", 10F), Name = "txtName" };

            var lblSkills = new Label { Text = "Skills (comma-separated):", Location = new Point(330, 25), AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var txtSkills = new TextBox { Location = new Point(330, 50), Width = 450, Height = 30, Font = new Font("Segoe UI", 10F), Name = "txtSkills" };
            var lblHint = new Label { Text = "e.g., HVAC, Plumbing, Electrical", Location = new Point(330, 80), AutoSize = true, Font = new Font("Segoe UI", 9F), ForeColor = Color.Gray };

            var lblRate = new Label { Text = "Hourly Rate ($):", Location = new Point(810, 25), AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var txtRate = new TextBox { Location = new Point(810, 50), Width = 150, Height = 30, Font = new Font("Segoe UI", 10F), Name = "txtRate" };

            var btnAdd = CreateStyledButton("➕ Add Technician", successColor, 160);
            btnAdd.Location = new Point(20, 95);

            var btnClear = CreateStyledButton("🗑️ Clear", dangerColor, 100);
            btnClear.Location = new Point(190, 95);
            btnClear.Click += (s, e) => { txtName.Clear(); txtSkills.Clear(); txtRate.Clear(); };

            inputCard.Controls.AddRange(new Control[] { lblName, txtName, lblSkills, txtSkills, lblHint, lblRate, txtRate, btnAdd, btnClear });

            // ListView
            var listView = new ListView
            {
                Location = new Point(20, 220),
                Size = new Size(1120, 400),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.White
            };
            listView.Columns.Add("ID", 80);
            listView.Columns.Add("Name", 280);
            listView.Columns.Add("Skills", 580);
            listView.Columns.Add("Hourly Rate", 160);

            var btnRefresh = CreateStyledButton("🔄 Refresh", primaryColor, 120);
            btnRefresh.Location = new Point(920, 625);

            btnAdd.Click += (s, e) => AddTechnicianClick(txtName, txtSkills, txtRate, listView);
            btnRefresh.Click += (s, e) => RefreshTechnicianList(listView);

            mainPanel.Controls.AddRange(new Control[] { lblTitle, inputCard, listView, btnRefresh });
            tabTechnicians.Controls.Add(mainPanel);
            RefreshTechnicianList(listView);
        }

        private void AddTechnicianClick(TextBox txtName, TextBox txtSkills, TextBox txtRate, ListView listView)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Please enter technician name.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtRate.Text, out decimal rate) || rate <= 0)
                {
                    MessageBox.Show("Please enter a valid hourly rate.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var skills = txtSkills.Text.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim()).ToList();

                manager.AddTech(txtName.Text, skills, rate);
                
                txtName.Clear();
                txtSkills.Clear();
                txtRate.Clear();

                RefreshTechnicianList(listView);
                MessageBox.Show("✅ Technician added successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshTechnicianList(ListView listView)
        {
            listView.Items.Clear();
            manager.Load();
            foreach (var tech in manager.Technicians)
            {
                var item = new ListViewItem(tech.TechId.ToString());
                item.SubItems.Add(tech.Name);
                item.SubItems.Add(tech.Skills.Any() ? string.Join(", ", tech.Skills) : "None");
                item.SubItems.Add($"${tech.HourlyRate:F2}");
                listView.Items.Add(item);
            }
        }

        private void InitializeRequestsTab()
        {
            var mainPanel = new Panel { 
                Dock = DockStyle.Fill, 
                BackColor = lightBg,
                Padding = new Padding(20)
            };

            // 1. Create Request Section (Top)
            var lblCreateTitle = new Label
            {
                Text = "📝 Create Service Request",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = darkText,
                Location = new Point(20, 15),
                AutoSize = true
            };

            var inputCard = new Panel
            {
                Location = new Point(20, 60),
                Size = new Size(1120, 140),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20)
            };

            var lblCustomer = new Label { Text = "Customer:", Location = new Point(20, 20), AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var cboCustomer = new ComboBox
            {
                Location = new Point(20, 45),
                Width = 250,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F),
                Name = "cboCustomer"
            };

            var btnRefreshCust = new Button { 
                Text = "🔄", 
                Location = new Point(275, 45), 
                Size = new Size(30, 25),
                Font = new Font("Segoe UI", 9F)
            };

            var lblDesc = new Label { Text = "Description:", Location = new Point(320, 20), AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var txtDesc = new TextBox
            {
                Location = new Point(320, 45),
                Width = 450,
                Height = 60,
                Multiline = true,
                Font = new Font("Segoe UI", 10F),
                Name = "txtDesc"
            };

            var lblStart = new Label { Text = "Start Time:", Location = new Point(20, 85), AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var dtpStart = new DateTimePicker
            {
                Location = new Point(20, 110),
                Width = 250,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "yyyy-MM-dd HH:mm",
                Font = new Font("Segoe UI", 10F),
                Name = "dtpStart"
            };

            var lblSkill = new Label { Text = "Required Skill (Optional):", Location = new Point(320, 85), AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            var txtSkill = new TextBox
            {
                Location = new Point(320, 110),
                Width = 250,
                Height = 30,
                Font = new Font("Segoe UI", 10F),
                Name = "txtSkill"
            };

            var btnCreate = CreateStyledButton("➕ Create Request", successColor, 180, 42);
            btnCreate.Location = new Point(800, 100);

            inputCard.Controls.AddRange(new Control[] { 
                lblCustomer, cboCustomer, btnRefreshCust, 
                lblDesc, txtDesc, 
                lblStart, dtpStart, 
                lblSkill, txtSkill, 
                btnCreate 
            });

            // 2. Service Requests Section (Bottom)
            var lblViewTitle = new Label
            {
                Text = "📋 Service Requests",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = darkText,
                Location = new Point(20, 220),
                AutoSize = true
            };

            // Button panel
            var buttonPanel = new FlowLayoutPanel
            {
                Location = new Point(20, 260),
                Size = new Size(1120, 45),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = lightBg
            };
            
            var btnRefresh = CreateStyledButton("🔄 Refresh", primaryColor, 120);
            var btnAssign = CreateStyledButton("👤 Assign Technician", secondaryColor, 150);
            var btnComplete = CreateStyledButton("✅ Complete Job", successColor, 140);
            
            buttonPanel.Controls.Add(btnRefresh);
            buttonPanel.Controls.Add(btnAssign);
            buttonPanel.Controls.Add(btnComplete);

            // ListView for Service Requests
            var listView = new ListView
            {
                Location = new Point(20, 315),
                Size = new Size(1120, 350),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.White,
                Name = "lvRequests"
            };
            
            // Set column widths - removed Required Skill column
            listView.Columns.Add("ID", 60);
            listView.Columns.Add("Customer", 150);
            listView.Columns.Add("Technician", 150);
            listView.Columns.Add("Description", 250);
            listView.Columns.Add("Start Time", 150);
            listView.Columns.Add("Status", 120);

            // Connect event handlers
            btnRefresh.Click += (s, e) => RefreshRequestsList(listView);
            btnAssign.Click += (s, e) => AssignTechClick(listView);
            btnComplete.Click += (s, e) => CompleteRequestClick(listView);
            btnCreate.Click += (s, e) => CreateRequestClick(cboCustomer, txtDesc, dtpStart, txtSkill, listView);
            btnRefreshCust.Click += (s, e) => LoadCustomersToCombo(cboCustomer);

            // Add all controls to main panel
            mainPanel.Controls.AddRange(new Control[] { 
                lblCreateTitle, inputCard, 
                lblViewTitle, buttonPanel, listView 
            });
            
            tabRequests.Controls.Add(mainPanel);

            // Initial load
            LoadCustomersToCombo(cboCustomer);
            RefreshRequestsList(listView);
        }

        private void LoadCustomersToCombo(ComboBox combo)
        {
            manager.Load();
            combo.Items.Clear();
            combo.Items.Add("-- Select Customer --");
            foreach (var c in manager.Customers)
                combo.Items.Add($"{c.CustomerId} - {c.Name}");
            combo.SelectedIndex = 0;
        }

        private void CreateRequestClick(ComboBox cbo, TextBox txtDesc, DateTimePicker dtp, TextBox txtSkill, ListView listView)
        {
            try
            {
                if (cbo.SelectedIndex <= 0)
                {
                    MessageBox.Show("Please select a customer.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtDesc.Text))
                {
                    MessageBox.Show("Please enter a description.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var custId = int.Parse(cbo.Text.Split('-')[0].Trim());
                var skill = string.IsNullOrWhiteSpace(txtSkill.Text) ? null : txtSkill.Text.Trim();

                manager.AddRequest(custId, txtDesc.Text, dtp.Value, skill);
                
                txtDesc.Clear();
                txtSkill.Clear();
                cbo.SelectedIndex = 0;

                RefreshRequestsList(listView);
                MessageBox.Show("✅ Service request created successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshRequestsList(ListView listView)
        {
            listView.Items.Clear();
            manager.Load();
            foreach (var req in manager.Requests)
            {
                var customer = manager.GetCustomer(req.CustomerId);
                var tech = req.TechId.HasValue ? manager.GetTechnician(req.TechId.Value) : null;

                var item = new ListViewItem(req.RequestId.ToString());
                item.SubItems.Add(customer?.Name ?? "Unknown");
                item.SubItems.Add(tech?.Name ?? "Unassigned");
                item.SubItems.Add(req.Description);
                item.SubItems.Add(req.ScheduledStart.ToString("yyyy-MM-dd HH:mm"));
                item.SubItems.Add(req.CurrentStatus.ToString());
                
                // Color code by status
                if (req.CurrentStatus == Status.Completed)
                    item.BackColor = Color.FromArgb(230, 247, 236);
                else if (req.CurrentStatus == Status.Dispatched)
                    item.BackColor = Color.FromArgb(232, 244, 253);
                else if (req.CurrentStatus == Status.New)
                    item.BackColor = Color.FromArgb(255, 250, 230);
                
                item.Tag = req;
                listView.Items.Add(item);
            }
        }

        private void AssignTechClick(ListView listView)
        {
            if (listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a request.", "Selection Required", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var req = listView.SelectedItems[0].Tag as ServiceRequest;
            if (req == null) return;

            var form = new AssignTechForm(manager, req.RequestId);
            if (form.ShowDialog() == DialogResult.OK)
            {
                RefreshRequestsList(listView);
            }
        }

        private void CompleteRequestClick(ListView listView)
        {
            if (listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a request.", "Selection Required", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var req = listView.SelectedItems[0].Tag as ServiceRequest;
            if (req == null) return;

            if (req.CurrentStatus == Status.Completed)
            {
                MessageBox.Show("This request is already completed.", "Already Completed", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!req.TechId.HasValue)
            {
                MessageBox.Show("Please assign a technician first.", "No Technician", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var form = new CompleteJobForm(manager, req.RequestId);
            if (form.ShowDialog() == DialogResult.OK)
            {
                RefreshRequestsList(listView);
            }
        }
private void InitializeReportsTab()
{
    var mainPanel = new Panel { 
        Dock = DockStyle.Fill, 
        BackColor = lightBg, 
        Padding = new Padding(20) 
    };
    
    var lblTitle = new Label
    {
        Text = "📊 Revenue Report",
        Font = new Font("Segoe UI", 18F, FontStyle.Bold),
        ForeColor = darkText,
        Location = new Point(20, 15),
        AutoSize = true
    };

    // Create a container for better layout
    var reportContainer = new Panel
    {
        Location = new Point(0, 50),
        Size = new Size(mainPanel.Width, mainPanel.Height - 50),
        BackColor = lightBg,
        Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
    };

    var listView = new ListView
    {
        Location = new Point(20, 0),
        Size = new Size(reportContainer.Width - 40, 480),
        View = View.Details,
        FullRowSelect = true,
        GridLines = true,
        Font = new Font("Segoe UI", 10F),
        BackColor = Color.White,
        Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
    };
    listView.Columns.Add("Request ID", 100);
    listView.Columns.Add("Customer", 180);
    listView.Columns.Add("Technician", 180);
    listView.Columns.Add("Hours Worked", 120);
    listView.Columns.Add("Parts Cost", 120);
    listView.Columns.Add("Total Cost", 120);
    listView.Columns.Add("Date", 140);

    var revenueCard = new Panel
    {
        Location = new Point(20, 500),
        Size = new Size(reportContainer.Width - 40, 80),
        BackColor = Color.White,
        BorderStyle = BorderStyle.FixedSingle,
        Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
    };

    var lblTotalLabel = new Label
    {
        Text = "💰 TOTAL REVENUE:",
        Location = new Point(20, 20),
        AutoSize = true,
        Font = new Font("Segoe UI", 14F, FontStyle.Bold),
        ForeColor = darkText
    };

    var lblTotal = new Label
    {
        Location = new Point(220, 20),
        AutoSize = true,
        Font = new Font("Segoe UI", 24F, FontStyle.Bold),
        ForeColor = successColor,
        Name = "lblTotal"
    };

    // Refresh button positioned properly
    var btnRefresh = CreateStyledButton("🔄 Refresh Report", primaryColor, 160, 42);
    btnRefresh.Location = new Point(reportContainer.Width - 200, 510);
    btnRefresh.Click += (s, e) => RefreshRevenueReport(listView, lblTotal);
    
    // Add auto-refresh when tab is selected
    tabControl.Selected += (s, e) => {
        if (e.TabPage == tabReports)
        {
            RefreshRevenueReport(listView, lblTotal);
        }
    };

    revenueCard.Controls.AddRange(new Control[] { lblTotalLabel, lblTotal });

    // Add controls to report container
    reportContainer.Controls.AddRange(new Control[] { listView, revenueCard, btnRefresh });
    
    // Add all to main panel
    mainPanel.Controls.AddRange(new Control[] { lblTitle, reportContainer });
    tabReports.Controls.Add(mainPanel);
    
    // Handle resizing
    reportContainer.Resize += (s, e) => {
        listView.Width = reportContainer.Width - 40;
        revenueCard.Width = reportContainer.Width - 40;
        btnRefresh.Location = new Point(reportContainer.Width - 200, 510);
    };

    // Initial load
    RefreshRevenueReport(listView, lblTotal);
}

        private void RefreshRevenueReport(ListView listView, Label lblTotal)
        {
            listView.Items.Clear();
            manager.Load();
            
            foreach (var req in manager.Completed())
            {
                var customer = manager.GetCustomer(req.CustomerId);
                var tech = req.TechId.HasValue ? manager.GetTechnician(req.TechId.Value) : null;

                var item = new ListViewItem(req.RequestId.ToString());
                item.SubItems.Add(customer?.Name ?? "Unknown");
                item.SubItems.Add(tech?.Name ?? "Unknown");
                item.SubItems.Add(req.HoursWorked?.ToString("F2") ?? "0");
                item.SubItems.Add($"${req.PartsCost ?? 0:F2}");
                item.SubItems.Add($"${req.TotalCost ?? 0:F2}");
                item.SubItems.Add(req.ScheduledStart.ToString("yyyy-MM-dd"));
                item.BackColor = Color.FromArgb(230, 247, 236);
                listView.Items.Add(item);
            }

            lblTotal.Text = $"${manager.Revenue():N2}";
        }
    }
}