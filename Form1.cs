using System;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32.TaskScheduler;
using System.Threading;

namespace FileMonitor
{
    public partial class Form1 : Form
    {
        private bool isPasswordVisible = false;
        private Image eyeOpenImage;
        private Image eyeClosedImage;
        private FileSystemWatcher fileSystemWatcher;
        private System.Windows.Forms.Timer monitoringTimer;
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;

        private string senderEmail = string.Empty;
        private string password = string.Empty;
        private string recipientEmail = string.Empty;

        public Form1()
        {
            InitializeComponent();

            // Cambiar la cultura a español
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-ES");

            InitializeTrayIcon();
            LogAction("Inicializando componentes.");

            EnsureDatabaseSetup(); // Asegurarse de que la base de datos está configurada correctamente
            LogAction("Verificación de la base de datos completada.");

            // Cargar imágenes
            eyeOpenImage = Image.FromFile("eye_open.png");
            eyeClosedImage = Image.FromFile("eye_closed.png");

            // Establecer imagen inicial del PictureBox
            pictureBoxEye.Image = eyeClosedImage;

            // Inicializar el FileSystemWatcher
            fileSystemWatcher = new FileSystemWatcher();

            // Inicializar el Timer
            monitoringTimer = new System.Windows.Forms.Timer();

            // Verificar si ya hay configuración guardada
            if (HasConfiguration())
            {
                LogAction("Configuración existente encontrada. Iniciando monitoreo en segundo plano.");
                StartMonitoringInBackground();
                this.Hide(); // Ocultar la interfaz
            }

            // Manejar el evento Resize para minimizar a la bandeja del sistema
            this.Resize += new EventHandler(Form1_Resize);

            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Inicializar textos de la interfaz
            InitializeTexts();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                trayIcon.Visible = true;
                trayIcon.ShowBalloonTip(1000, "File Monitor", "La aplicación se está ejecutando en segundo plano.", ToolTipIcon.Info);
            }
        }

        private void InitializeTrayIcon()
        {
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Mostrar", null, ShowForm);
            trayMenu.Items.Add("Salir", null, ExitApplication);

            // Especificar la ruta completa del archivo folder.ico
            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "folder.ico");

            trayIcon = new NotifyIcon
            {
                Text = "File Monitor",
                Icon = new Icon(iconPath),
                ContextMenuStrip = trayMenu,
                Visible = true
            };

            trayIcon.DoubleClick += ShowForm;
        }

        private void ShowForm(object sender, EventArgs e)
        {
            LogAction("Mostrando la interfaz de usuario.");
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void ExitApplication(object sender, EventArgs e)
        {
            LogAction("Saliendo de la aplicación.");
            trayIcon.Visible = false;
            Application.Exit();
        }

        private void LogAction(string message)
        {
            string logFilePath = "FileMonitorLog.txt";
            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.WriteLine($"{DateTime.Now}: {message}");
            }
        }

        private bool HasConfiguration()
        {
            LogAction("Verificando si hay configuración guardada.");
            using (SQLiteConnection con = new SQLiteConnection("Data Source=FileMonitor.db;Version=3;"))
            {
                con.Open();
                string query = "SELECT COUNT(*) FROM Configuration";
                using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                {
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    LogAction($"Cantidad de configuraciones encontradas: {count}");
                    return count > 0;
                }
            }
        }

        private void StartMonitoringInBackground()
        {
            LogAction("Iniciando monitoreo en segundo plano.");
            LoadConfiguration(); // Cargar configuración guardada
            LoadMonitoredFolders(); // Cargar carpetas monitoreadas
            LogAction("Configuración y carpetas cargadas. Ocultando la interfaz.");
            btnStartMonitoring_Click(null, null); // Iniciar monitoreo

            // Asegurarse de que la interfaz esté oculta
            if (this.Visible)
            {
                LogAction("La interfaz todavía está visible. Ocultando ahora.");
                this.Hide();
            }

            LogAction("Interfaz oculta. Aplicación en segundo plano.");
        }

        private void LoadConfiguration()
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection("Data Source=FileMonitor.db;Version=3;"))
                {
                    con.Open();
                    LogAction("Conexión a la base de datos abierta para cargar configuración.");

                    string query = "SELECT * FROM Configuration LIMIT 1;";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                    {
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                senderEmail = reader["SenderEmail"]?.ToString() ?? string.Empty;
                                password = reader["Password"]?.ToString() ?? string.Empty;
                                recipientEmail = reader["RecipientEmail"]?.ToString() ?? string.Empty;

                                LogAction($"Datos cargados: SenderEmail={senderEmail}, RecipientEmail={recipientEmail}");

                                // Verificar si la columna 'MonitoringInterval' existe y contiene un valor válido
                                if (reader["MonitoringInterval"] != DBNull.Value)
                                {
                                    LogAction($"MonitoringInterval encontrado: {reader["MonitoringInterval"]}");
                                    if (int.TryParse(reader["MonitoringInterval"].ToString(), out int intervalIndex))
                                    {
                                        if (intervalIndex >= 0 && intervalIndex < comboBoxInterval.Items.Count)
                                        {
                                            comboBoxInterval.SelectedIndex = intervalIndex;
                                            LogAction($"MonitoringInterval establecido: {intervalIndex}");
                                        }
                                        else
                                        {
                                            comboBoxInterval.SelectedIndex = 0; // Valor por defecto
                                            LogAction("Valor de MonitoringInterval fuera de rango. Se estableció el valor por defecto.");
                                        }
                                    }
                                    else
                                    {
                                        comboBoxInterval.SelectedIndex = 0; // Valor por defecto
                                        LogAction("Error al convertir MonitoringInterval a int. Se estableció el valor por defecto.");
                                    }
                                }
                                else
                                {
                                    comboBoxInterval.SelectedIndex = 0; // Valor por defecto
                                    LogAction("No se encontró MonitoringInterval. Se estableció el valor por defecto.");
                                }

                                // Actualizar los campos de la interfaz con los datos cargados
                                txtSenderEmail.Text = senderEmail;
                                txtPassword.Text = password;
                                txtRecipientEmail.Text = recipientEmail;
                                LogAction("Configuración de la interfaz actualizada con los datos cargados.");
                            }
                            else
                            {
                                // No hay datos guardados, mostrar interfaz para ingresar datos
                                LogAction("No se encontraron registros en la tabla Configuration. Mostrando interfaz para ingresar datos.");
                                this.Show();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogAction($"Error al cargar la configuración: {ex.Message}");
                MessageBox.Show($"Error al cargar la configuración: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EnsureDatabaseSetup()
        {
            using (SQLiteConnection con = new SQLiteConnection("Data Source=FileMonitor.db;Version=3;"))
            {
                con.Open();
                LogAction("Conexión a la base de datos abierta para configuración.");

                // Crear la tabla Configuration si no existe
                string createConfigurationTable = @"
                    CREATE TABLE IF NOT EXISTS Configuration (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        SenderEmail TEXT,
                        Password TEXT,
                        RecipientEmail TEXT,
                        MonitoringInterval INTEGER
                    )";
                using (SQLiteCommand cmd = new SQLiteCommand(createConfigurationTable, con))
                {
                    cmd.ExecuteNonQuery();
                    LogAction("Tabla Configuration verificada/creada.");
                }

                // Verificar si la columna MonitoringInterval ya existe antes de añadirla
                bool columnExists = false;
                string queryCheck = "PRAGMA table_info(Configuration)";
                using (SQLiteCommand cmd = new SQLiteCommand(queryCheck, con))
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["name"].ToString() == "MonitoringInterval")
                        {
                            columnExists = true;
                            break;
                        }
                    }
                }

                if (!columnExists)
                {
                    // Añadir la columna MonitoringInterval si no existe
                    string alterTableQuery = "ALTER TABLE Configuration ADD COLUMN MonitoringInterval INTEGER";
                    using (SQLiteCommand cmd = new SQLiteCommand(alterTableQuery, con))
                    {
                        cmd.ExecuteNonQuery();
                        LogAction("Columna MonitoringInterval añadida a la tabla Configuration.");
                    }
                }

                // Crear la tabla MonitoredFolders si no existe
                string createMonitoredFoldersTable = @"
                    CREATE TABLE IF NOT EXISTS MonitoredFolders (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Path TEXT
                    )";
                using (SQLiteCommand cmd = new SQLiteCommand(createMonitoredFoldersTable, con))
                {
                    cmd.ExecuteNonQuery();
                    LogAction("Tabla MonitoredFolders verificada/creada.");
                }
            }
        }

        private void SaveConfiguration()
        {
            if (string.IsNullOrWhiteSpace(txtSenderEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text) ||
                string.IsNullOrWhiteSpace(txtRecipientEmail.Text) ||
                listBoxFolders.Items.Count == 0)
            {
                MessageBox.Show("Por favor, complete todos los campos y agregue al menos una carpeta para monitorear.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            senderEmail = txtSenderEmail.Text;
            password = txtPassword.Text;
            recipientEmail = txtRecipientEmail.Text;

            CloseAllConnections();
            LogAction("Intentando guardar la configuración.");

            int maxRetries = 5;
            int delay = 200; // Milisegundos

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    using (SQLiteConnection con = new SQLiteConnection("Data Source=FileMonitor.db;Version=3;"))
                    {
                        con.Open();
                        LogAction("Conexión a la base de datos abierta para guardar configuración.");

                        string deleteQuery = "DELETE FROM Configuration";
                        using (SQLiteCommand deleteCmd = new SQLiteCommand(deleteQuery, con))
                        {
                            deleteCmd.ExecuteNonQuery();
                            LogAction("Tabla Configuration limpiada.");
                        }

                        string insertQuery = @"INSERT INTO Configuration (SenderEmail, Password, RecipientEmail, MonitoringInterval)
                                               VALUES (@SenderEmail, @Password, @RecipientEmail, @MonitoringInterval)";

                        using (SQLiteCommand cmd = new SQLiteCommand(insertQuery, con))
                        {
                            cmd.Parameters.AddWithValue("@SenderEmail", senderEmail);
                            cmd.Parameters.AddWithValue("@Password", password);
                            cmd.Parameters.AddWithValue("@RecipientEmail", recipientEmail);

                            int monitoringIntervalIndex = comboBoxInterval.SelectedIndex;
                            if (monitoringIntervalIndex < 0 || monitoringIntervalIndex >= comboBoxInterval.Items.Count)
                            {
                                monitoringIntervalIndex = 0; // Valor predeterminado si el índice es inválido
                            }
                            cmd.Parameters.AddWithValue("@MonitoringInterval", monitoringIntervalIndex);
                            cmd.ExecuteNonQuery();
                            LogAction("Configuración guardada en la base de datos.");
                        }
                    }
                    CreateScheduledTask(); // Crear tarea programada al guardar la configuración
                    break; // Salir del bucle si la operación es exitosa
                }
                catch (SQLiteException ex) when (ex.Message.Contains("database is locked"))
                {
                    LogAction($"Intento {i + 1}: La base de datos está bloqueada. Esperando {delay} ms...");
                    System.Threading.Thread.Sleep(delay);
                }
                catch (Exception ex)
                {
                    LogAction($"Error general al guardar configuración: {ex.Message}");
                    MessageBox.Show($"Error general: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break; // Salir del bucle si ocurre un error diferente
                }
            }
        }

        private void CloseAllConnections()
        {
            // Implementar lógica para cerrar todas las conexiones abiertas, si aplica
            LogAction("Cerrando todas las conexiones a la base de datos.");
        }

        private void pictureBoxEye_Click(object sender, EventArgs e)
        {
            LogAction("Icono de ojo clicado para mostrar/ocultar contraseña.");
            if (isPasswordVisible)
            {
                txtPassword.PasswordChar = '*';
                pictureBoxEye.Image = eyeClosedImage;
            }
            else
            {
                txtPassword.PasswordChar = '\0';
                pictureBoxEye.Image = eyeOpenImage;
            }
            isPasswordVisible = !isPasswordVisible;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            LogAction($"Guardando configuración: SenderEmail={senderEmail}, RecipientEmail={recipientEmail}");
            SaveConfiguration();
            MessageBox.Show("Configuración guardada con éxito.");
        }

        private void btnAddFolder_Click(object sender, EventArgs e)
        {
            LogAction("Añadiendo nueva carpeta para monitorear.");
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    listBoxFolders.Items.Add(fbd.SelectedPath);
                    SaveFolderPath(fbd.SelectedPath);
                    LogAction($"Carpeta añadida: {fbd.SelectedPath}");
                }
            }
        }

        private void SaveFolderPath(string path)
        {
            CloseAllConnections();
            LogAction($"Guardando carpeta monitoreada: {path}");

            using (SQLiteConnection con = new SQLiteConnection("Data Source=FileMonitor.db;Version=3;"))
            {
                con.Open();

                string query = @"INSERT INTO MonitoredFolders (Path) VALUES (@Path)";
                using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Path", path);
                    cmd.ExecuteNonQuery();
                    LogAction($"Carpeta guardada en la base de datos: {path}");
                }
            }
            LoadMonitoredFolders(); // Refrescar la lista de carpetas después de guardar
        }

        private void LoadMonitoredFolders()
        {
            LogAction("Cargando carpetas monitoreadas desde la base de datos.");
            listBoxFolders.Items.Clear(); // Asegurarse de limpiar la lista antes de cargar

            using (SQLiteConnection con = new SQLiteConnection("Data Source=FileMonitor.db;Version=3;"))
            {
                con.Open();

                string query = "SELECT Path FROM MonitoredFolders";
                using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string path = reader["Path"].ToString();
                        listBoxFolders.Items.Add(path);
                        LogAction($"Ruta cargada: {path}");
                    }
                }
            }
        }

        private void btnRemoveFolder_Click(object sender, EventArgs e)
        {
            LogAction("Eliminando carpeta seleccionada.");
            if (listBoxFolders.SelectedItem != null)
            {
                string selectedPath = listBoxFolders.SelectedItem.ToString();
                listBoxFolders.Items.Remove(selectedPath);
                RemoveFolderPath(selectedPath);
                LogAction($"Carpeta eliminada: {selectedPath}");
            }
        }

        private void RemoveFolderPath(string path)
        {
            CloseAllConnections();
            LogAction($"Eliminando carpeta de la base de datos: {path}");

            using (SQLiteConnection con = new SQLiteConnection("Data Source=FileMonitor.db;Version=3;"))
            {
                con.Open();

                string query = "DELETE FROM MonitoredFolders WHERE Path = @Path";
                using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Path", path);
                    cmd.ExecuteNonQuery();
                    LogAction($"Carpeta eliminada de la base de datos: {path}");
                }
            }
        }

        private void btnStartMonitoring_Click(object sender, EventArgs e)
        {
            LogAction("Iniciando monitoreo.");
            if (listBoxFolders.Items.Count == 0)
            {
                MessageBox.Show("Por favor, agregue al menos una carpeta para monitorear.");
                return;
            }

            foreach (string folder in listBoxFolders.Items)
            {
                fileSystemWatcher.Path = folder;
                fileSystemWatcher.IncludeSubdirectories = false;
                fileSystemWatcher.Created += OnFileCreated;
                fileSystemWatcher.EnableRaisingEvents = true;
                LogAction($"Monitoreo iniciado para carpeta: {folder}");
            }

            monitoringTimer.Interval = GetIntervalFromComboBox();
            monitoringTimer.Tick += OnMonitoringTimerTick;
            monitoringTimer.Start();

            LogAction($"Intervalo de monitoreo establecido en: {monitoringTimer.Interval / 1000} segundos.");
            LogAction("Monitoreo iniciado.");
            this.Hide(); // Ocultar la interfaz y mostrar solo el icono en la bandeja
        }

        private void btnStopMonitoring_Click(object sender, EventArgs e)
        {
            LogAction("Deteniendo monitoreo.");
            fileSystemWatcher.EnableRaisingEvents = false;
            monitoringTimer.Stop();
            LogAction("Monitoreo detenido.");
            MessageBox.Show("Monitoreo detenido.");
        }

        private void btnClearData_Click(object sender, EventArgs e)
        {
            LogAction("Borrando todos los datos.");
            listBoxFolders.Items.Clear();
            txtSenderEmail.Clear();
            txtPassword.Clear();
            txtRecipientEmail.Clear();
            comboBoxInterval.SelectedIndex = 0;

            ClearAllData();
            DeleteScheduledTask(); // Eliminar tarea programada al borrar los datos
            MessageBox.Show("Datos borrados.");
        }

        private void btnChangeLanguage_Click(object sender, EventArgs e)
        {
            if (btnChangeLanguage.Text == "ES")
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                btnChangeLanguage.Text = "EN";
            }
            else
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-ES");
                btnChangeLanguage.Text = "ES";
            }
            InitializeTexts();
        }

        private void InitializeTexts()
        {
            // Actualizar los textos de los controles
            this.Text = Properties.Resources.AppTitle ?? "File Monitor";
            lblSenderEmail.Text = Properties.Resources.SenderEmailLabel ?? "Correo electrónico del remitente";
            lblPassword.Text = Properties.Resources.PasswordLabel ?? "Contraseña";
            lblRecipientEmail.Text = Properties.Resources.RecipientEmailLabel ?? "Correo electrónico del destinatario";
            lblFolders.Text = Properties.Resources.FoldersLabel ?? "Carpetas monitoreadas";
            lblInterval.Text = Properties.Resources.IntervalLabel ?? "Intervalo de monitoreo";
            chkIncludeSubfolders.Text = Properties.Resources.IncludeSubfolders ?? "Incluir Subcarpetas";
            btnAddFolder.Text = Properties.Resources.AddFolderButton ?? "Agregar Carpeta";
            btnRemoveFolder.Text = Properties.Resources.RemoveFolderButton ?? "Eliminar Carpeta";
            btnStartMonitoring.Text = Properties.Resources.StartMonitoringButton ?? "Iniciar Monitoreo";
            btnStopMonitoring.Text = Properties.Resources.StopMonitoringButton ?? "Detener Monitoreo";
            btnClearData.Text = Properties.Resources.ClearDataButton ?? "Borrar Datos";
            btnSave.Text = Properties.Resources.SaveButton ?? "Guardar";

            // Actualizar los elementos del comboBoxInterval
            comboBoxInterval.Items.Clear();
            comboBoxInterval.Items.Add(Properties.Resources.Interval1Min ?? "1 minuto");
            comboBoxInterval.Items.Add(Properties.Resources.Interval5Min ?? "5 minutos");
            comboBoxInterval.Items.Add(Properties.Resources.Interval10Min ?? "10 minutos");
            comboBoxInterval.Items.Add(Properties.Resources.Interval15Min ?? "15 minutos");
            comboBoxInterval.Items.Add(Properties.Resources.Interval30Min ?? "30 minutos");
            comboBoxInterval.Items.Add(Properties.Resources.Interval1Hour ?? "1 hora");
            comboBoxInterval.Items.Add(Properties.Resources.Interval2Hours ?? "2 horas");
            comboBoxInterval.Items.Add(Properties.Resources.Interval4Hours ?? "4 horas");
            comboBoxInterval.Items.Add(Properties.Resources.Interval8Hours ?? "8 horas");
            comboBoxInterval.Items.Add(Properties.Resources.Interval12Hours ?? "12 horas");
            comboBoxInterval.Items.Add(Properties.Resources.Interval24Hours ?? "24 horas");

            // Establecer el elemento seleccionado por defecto
            comboBoxInterval.SelectedIndex = 0; // Puedes ajustar el índice según sea necesario
        }

        private void ClearAllData()
        {
            CloseAllConnections();
            LogAction("Eliminando toda la información de la base de datos.");

            using (SQLiteConnection con = new SQLiteConnection("Data Source=FileMonitor.db;Version=3;"))
            {
                con.Open();
                string query = "DELETE FROM Configuration";
                using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                {
                    cmd.ExecuteNonQuery();
                    LogAction("Configuración eliminada de la base de datos.");
                }

                query = "DELETE FROM MonitoredFolders";
                using (SQLiteCommand cmd = new SQLiteCommand(query, con))
                {
                    cmd.ExecuteNonQuery();
                    LogAction("Carpetas monitoreadas eliminadas de la base de datos.");
                }
            }
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            LogAction($"Archivo creado: {e.FullPath}");
        }

        private void OnMonitoringTimerTick(object sender, EventArgs e)
        {
            LogAction("Intervalo de monitoreo alcanzado. Enviando correo de resumen.");
            SendSummaryEmail();
        }

        private int GetIntervalFromComboBox()
        {
            LogAction($"Obteniendo intervalo de monitoreo desde comboBoxInterval: {comboBoxInterval.SelectedIndex}");
            switch (comboBoxInterval.SelectedIndex)
            {
                case 0: return 1 * 60 * 1000; // 1 minuto
                case 1: return 5 * 60 * 1000; // 5 minutos
                case 2: return 10 * 60 * 1000; // 10 minutos
                case 3: return 15 * 60 * 1000; // 15 minutos
                case 4: return 30 * 60 * 1000; // 30 minutos
                case 5: return 60 * 60 * 1000; // 1 hora
                case 6: return 2 * 60 * 60 * 1000; // 2 horas
                case 7: return 4 * 60 * 1000; // 4 horas
                case 8: return 8 * 60 * 1000; // 8 horas
                case 9: return 12 * 60 * 1000; // 12 horas
                case 10: return 24 * 60 * 1000; // 24 horas
                default: return 15 * 60 * 1000; // Default 15 minutos
            }
        }

        private void SendSummaryEmail()
        {
            try
            {
                StringBuilder emailBody = new StringBuilder();
                emailBody.AppendLine("Aquí va el resumen de archivos monitoreados.");
                emailBody.AppendLine($"Host: {Environment.MachineName}");
                emailBody.AppendLine($"Hora de envío: {DateTime.Now}");

                // Agregar detalles de los archivos en las carpetas monitoreadas
                foreach (string folder in listBoxFolders.Items)
                {
                    if (Directory.Exists(folder))
                    {
                        emailBody.AppendLine($"Carpeta: {folder}");
                        string[] files = Directory.GetFiles(folder);
                        emailBody.AppendLine($"Total de archivos: {files.Length}");

                        foreach (string file in files)
                        {
                            FileInfo fileInfo = new FileInfo(file);
                            emailBody.AppendLine($"{fileInfo.Name} - {fileInfo.Length} bytes - {fileInfo.LastWriteTime}");
                        }
                    }
                }

                using (MailMessage mail = new MailMessage())
                {
                    using (SmtpClient smtpServer = new SmtpClient("smtp.gmail.com"))
                    {
                        mail.From = new MailAddress(senderEmail);
                        mail.To.Add(recipientEmail);
                        mail.Subject = "Resumen de Archivos Monitoreados";
                        mail.Body = emailBody.ToString();

                        smtpServer.Port = 587;
                        smtpServer.Credentials = new NetworkCredential(senderEmail, password);
                        smtpServer.EnableSsl = true;

                        smtpServer.Send(mail);
                    }
                }

                LogAction("Correo de resumen enviado.");
            }
            catch (Exception ex)
            {
                LogAction($"Error al enviar correo: {ex.Message}");
                MessageBox.Show($"Error al enviar correo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateScheduledTask()
        {
            try
            {
                using (TaskService ts = new TaskService())
                {
                    TaskDefinition td = ts.NewTask();
                    td.RegistrationInfo.Description = "FileMonitor - Tarea programada para iniciar la aplicación en segundo plano.";
                    td.Principal.LogonType = TaskLogonType.ServiceAccount; // Cambiar a ServiceAccount para ejecutar sin necesidad de sesión de usuario
                    td.Principal.UserId = "SYSTEM"; // Ejecutar como SYSTEM
                    td.Principal.RunLevel = TaskRunLevel.Highest; // Ejecutar con permisos elevados

                    // Acción para iniciar la aplicación
                    td.Actions.Add(new ExecAction(Application.ExecutablePath, null, null));

                    // Trigger para iniciar en el inicio del sistema
                    td.Triggers.Add(new BootTrigger());

                    // Crear o actualizar la tarea
                    ts.RootFolder.RegisterTaskDefinition("FileMonitorScheduledTask", td, TaskCreation.CreateOrUpdate, null, null, TaskLogonType.ServiceAccount, null);
                }
                LogAction("Tarea programada creada/actualizada correctamente.");
            }
            catch (Exception ex)
            {
                LogAction($"Error al crear/actualizar la tarea programada: {ex.Message}");
                MessageBox.Show($"Error al crear/actualizar la tarea programada: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteScheduledTask()
        {
            try
            {
                using (TaskService ts = new TaskService())
                {
                    ts.RootFolder.DeleteTask("FileMonitorScheduledTask", false);
                }
                LogAction("Tarea programada eliminada correctamente.");
            }
            catch (Exception ex)
            {
                LogAction($"Error al eliminar la tarea programada: {ex.Message}");
                MessageBox.Show($"Error al eliminar la tarea programada: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
