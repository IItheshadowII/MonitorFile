namespace FileMonitor
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        private void InitializeComponent()
        {
            txtSenderEmail = new TextBox();
            txtPassword = new TextBox();
            txtRecipientEmail = new TextBox();
            listBoxFolders = new ListBox();
            comboBoxInterval = new ComboBox();
            lblSenderEmail = new Label();
            lblPassword = new Label();
            lblRecipientEmail = new Label();
            lblFolders = new Label();
            lblInterval = new Label();
            chkIncludeSubfolders = new CheckBox();
            btnAddFolder = new Button();
            btnRemoveFolder = new Button();
            btnStartMonitoring = new Button();
            btnStopMonitoring = new Button();
            btnSave = new Button();
            btnClearData = new Button();
            btnChangeLanguage = new Button();
            pictureBoxLogo = new PictureBox();
            pictureBoxEye = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBoxLogo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxEye).BeginInit();
            SuspendLayout();
            // 
            // txtSenderEmail
            // 
            txtSenderEmail.Location = new Point(12, 95);
            txtSenderEmail.Name = "txtSenderEmail";
            txtSenderEmail.Size = new Size(260, 23);
            txtSenderEmail.TabIndex = 0;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(12, 135);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '*';
            txtPassword.Size = new Size(230, 23);
            txtPassword.TabIndex = 1;
            // 
            // txtRecipientEmail
            // 
            txtRecipientEmail.Location = new Point(12, 175);
            txtRecipientEmail.Name = "txtRecipientEmail";
            txtRecipientEmail.Size = new Size(260, 23);
            txtRecipientEmail.TabIndex = 2;
            // 
            // listBoxFolders
            // 
            listBoxFolders.FormattingEnabled = true;
            listBoxFolders.ItemHeight = 15;
            listBoxFolders.Location = new Point(12, 215);
            listBoxFolders.Name = "listBoxFolders";
            listBoxFolders.Size = new Size(260, 79);
            listBoxFolders.TabIndex = 3;
            // 
            // comboBoxInterval
            // 
            comboBoxInterval.FormattingEnabled = true;
            comboBoxInterval.Items.AddRange(new object[] { "1 minuto", "5 minutos", "10 minutos", "15 minutos", "30 minutos", "1 hora", "2 horas", "4 horas", "8 horas", "12 horas", "24 horas" });
            comboBoxInterval.Location = new Point(12, 319);
            comboBoxInterval.Name = "comboBoxInterval";
            comboBoxInterval.Size = new Size(121, 23);
            comboBoxInterval.TabIndex = 4;
            // 
            // lblSenderEmail
            // 
            lblSenderEmail.AutoSize = true;
            lblSenderEmail.Location = new Point(12, 79);
            lblSenderEmail.Name = "lblSenderEmail";
            lblSenderEmail.Size = new Size(178, 15);
            lblSenderEmail.TabIndex = 5;
            lblSenderEmail.Text = "Correo electrónico del remitente";
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Location = new Point(12, 119);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(67, 15);
            lblPassword.TabIndex = 6;
            lblPassword.Text = "Contraseña";
            // 
            // lblRecipientEmail
            // 
            lblRecipientEmail.AutoSize = true;
            lblRecipientEmail.Location = new Point(12, 159);
            lblRecipientEmail.Name = "lblRecipientEmail";
            lblRecipientEmail.Size = new Size(189, 15);
            lblRecipientEmail.TabIndex = 7;
            lblRecipientEmail.Text = "Correo electrónico del destinatario";
            // 
            // lblFolders
            // 
            lblFolders.AutoSize = true;
            lblFolders.Location = new Point(12, 199);
            lblFolders.Name = "lblFolders";
            lblFolders.Size = new Size(129, 15);
            lblFolders.TabIndex = 8;
            lblFolders.Text = "Carpetas monitoreadas";
            // 
            // lblInterval
            // 
            lblInterval.AutoSize = true;
            lblInterval.Location = new Point(12, 303);
            lblInterval.Name = "lblInterval";
            lblInterval.Size = new Size(128, 15);
            lblInterval.TabIndex = 9;
            lblInterval.Text = "Intervalo de monitoreo";
            // 
            // chkIncludeSubfolders
            // 
            chkIncludeSubfolders.AutoSize = true;
            chkIncludeSubfolders.Location = new Point(12, 350);
            chkIncludeSubfolders.Name = "chkIncludeSubfolders";
            chkIncludeSubfolders.Size = new Size(126, 19);
            chkIncludeSubfolders.TabIndex = 10;
            chkIncludeSubfolders.Text = "Incluir Subcarpetas";
            chkIncludeSubfolders.UseVisualStyleBackColor = true;
            // 
            // btnAddFolder
            // 
            btnAddFolder.Location = new Point(140, 317);
            btnAddFolder.Name = "btnAddFolder";
            btnAddFolder.Size = new Size(62, 23);
            btnAddFolder.TabIndex = 11;
            btnAddFolder.Text = "Agregar Carpeta";
            btnAddFolder.UseVisualStyleBackColor = true;
            btnAddFolder.Click += btnAddFolder_Click;
            // 
            // btnRemoveFolder
            // 
            btnRemoveFolder.Location = new Point(210, 317);
            btnRemoveFolder.Name = "btnRemoveFolder";
            btnRemoveFolder.Size = new Size(62, 23);
            btnRemoveFolder.TabIndex = 12;
            btnRemoveFolder.Text = "Eliminar Carpeta";
            btnRemoveFolder.UseVisualStyleBackColor = true;
            btnRemoveFolder.Click += btnRemoveFolder_Click;
            // 
            // btnStartMonitoring
            // 
            btnStartMonitoring.Location = new Point(12, 380);
            btnStartMonitoring.Name = "btnStartMonitoring";
            btnStartMonitoring.Size = new Size(120, 23);
            btnStartMonitoring.TabIndex = 13;
            btnStartMonitoring.Text = "Iniciar Monitoreo";
            btnStartMonitoring.UseVisualStyleBackColor = true;
            btnStartMonitoring.Click += btnStartMonitoring_Click;
            // 
            // btnStopMonitoring
            // 
            btnStopMonitoring.Location = new Point(150, 380);
            btnStopMonitoring.Name = "btnStopMonitoring";
            btnStopMonitoring.Size = new Size(120, 23);
            btnStopMonitoring.TabIndex = 14;
            btnStopMonitoring.Text = "Detener Monitoreo";
            btnStopMonitoring.UseVisualStyleBackColor = true;
            btnStopMonitoring.Click += btnStopMonitoring_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(12, 410);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(120, 23);
            btnSave.TabIndex = 15;
            btnSave.Text = "Guardar";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnClearData
            // 
            btnClearData.Location = new Point(150, 410);
            btnClearData.Name = "btnClearData";
            btnClearData.Size = new Size(120, 23);
            btnClearData.TabIndex = 16;
            btnClearData.Text = "Borrar Datos";
            btnClearData.UseVisualStyleBackColor = true;
            btnClearData.Click += btnClearData_Click;
            // 
            // btnChangeLanguage
            // 
            btnChangeLanguage.Location = new Point(12, 12);
            btnChangeLanguage.Name = "btnChangeLanguage";
            btnChangeLanguage.Size = new Size(38, 23);
            btnChangeLanguage.TabIndex = 17;
            btnChangeLanguage.Text = "ES";
            btnChangeLanguage.UseVisualStyleBackColor = true;
            btnChangeLanguage.Click += btnChangeLanguage_Click;
            // 
            // pictureBoxLogo
            // 
            pictureBoxLogo.Image = Properties.Resources.praxis_emr_logo;
            pictureBoxLogo.Location = new Point(70, 13);
            pictureBoxLogo.Name = "pictureBoxLogo";
            pictureBoxLogo.Size = new Size(192, 46);
            pictureBoxLogo.TabIndex = 18;
            pictureBoxLogo.TabStop = false;
            // 
            // pictureBoxEye
            // 
            pictureBoxEye.Image = Properties.Resources.eye_closed;
            pictureBoxEye.Location = new Point(250, 135);
            pictureBoxEye.Name = "pictureBoxEye";
            pictureBoxEye.Size = new Size(22, 20);
            pictureBoxEye.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxEye.TabIndex = 19;
            pictureBoxEye.TabStop = false;
            pictureBoxEye.Click += pictureBoxEye_Click;
            // 
            // Form1
            // 
            ClientSize = new Size(284, 451);
            Controls.Add(pictureBoxEye);
            Controls.Add(pictureBoxLogo);
            Controls.Add(btnChangeLanguage);
            Controls.Add(btnClearData);
            Controls.Add(btnSave);
            Controls.Add(btnStopMonitoring);
            Controls.Add(btnStartMonitoring);
            Controls.Add(btnRemoveFolder);
            Controls.Add(btnAddFolder);
            Controls.Add(chkIncludeSubfolders);
            Controls.Add(lblInterval);
            Controls.Add(lblFolders);
            Controls.Add(lblRecipientEmail);
            Controls.Add(lblPassword);
            Controls.Add(lblSenderEmail);
            Controls.Add(comboBoxInterval);
            Controls.Add(listBoxFolders);
            Controls.Add(txtRecipientEmail);
            Controls.Add(txtPassword);
            Controls.Add(txtSenderEmail);
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "File Monitor";
            ((System.ComponentModel.ISupportInitialize)pictureBoxLogo).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxEye).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox txtSenderEmail;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtRecipientEmail;
        private System.Windows.Forms.ListBox listBoxFolders;
        private System.Windows.Forms.ComboBox comboBoxInterval;
        private System.Windows.Forms.Label lblSenderEmail;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblRecipientEmail;
        private System.Windows.Forms.Label lblFolders;
        private System.Windows.Forms.Label lblInterval;
        private System.Windows.Forms.CheckBox chkIncludeSubfolders;
        private System.Windows.Forms.Button btnAddFolder;
        private System.Windows.Forms.Button btnRemoveFolder;
        private System.Windows.Forms.Button btnStartMonitoring;
        private System.Windows.Forms.Button btnStopMonitoring;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClearData;
        private System.Windows.Forms.Button btnChangeLanguage;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.PictureBox pictureBoxEye;
    }
}
