/*
 * Created by SharpDevelop.
 * User: mkarcz
 * Date: 2007*04*17
 * Time: 5:32 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace RobEnvMK
{
	/// <summary>
	/// Description of FormRobotStatus.
	/// </summary>
	public class FormRobotStatus : System.Windows.Forms.Form
	{
		private Label labelComa;
		private TextBox textBoxCurrCmd;
		private TextBox textBoxLog;
		private Label labelAngle;
		private TextBox textBoxAngle;
		private TextBox textBoxPosY;
		private TextBox textBoxPosX;
		private Label labelPosition;
		private Label labelRobotNum;
		private NumericUpDown numericUpDownRobot;
		private Button buttonKill;
		private Label labelCurrCmd;
		private int m_nNumOfRobots = 1;
		private int m_nCurrRobotNum = 0;
		private double [] m_dXCoordTbl = null;
		private double [] m_dYCoordTbl = null;
		private double [] m_daAngleTbl = null;
        private double [] m_daRadarTbl = null; // radar readings in front of bot for each bot.
        private CheckBox checkBoxHideLog;
        private Label labelStep;
        private TextBox textBoxStep;
        private Button buttonManage;
		private bool [] m_baRequestedKill = null;
        private FormManageRobots m_RobotsManager = new FormManageRobots();
        private RadioButton radioButtonLightXEnabled;
        private RadioButton radioButtonLightYEnabled;
        private RadioButton radioButtonLightZEnabled;
        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
        private Label label1;
        private string m_RobotsConfigFile = "./robots.cfg";
        private bool m_bLightXEnable = false;
        private bool m_bLightYEnable = false;
        private bool m_bLightZEnable = false;
        private Label labelRadar;
        private TextBox textBoxRadar;
        private bool m_bLightingSetupChanged = true;
        private System.Windows.Forms.CheckBox checkBoxShowAllBotsLog;
        bool m_bShowAllBotsLog = false;

        public bool ShowAllBotsLog {
        	
        	get { return m_bShowAllBotsLog; }
        }
        
        public int SelectedBot {
        	
        	get { return m_nCurrRobotNum; }
        }
        
        public bool LightingChanged
        {
            get 
            {
                bool bRet = m_bLightingSetupChanged;
                m_bLightingSetupChanged = false;
                return bRet; 
            }
        }

        public bool LightSwitchX
        {
            get
            {
                return m_bLightXEnable;
            }
        }

        public bool LightSwitchY
        {
            get
            {
                return m_bLightYEnable;
            }
        }

        public bool LightSwitchZ
        {
            get
            {
                return m_bLightZEnable;
            }
        }
		
		public FormRobotStatus()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			AllocateCoordTbl (m_nNumOfRobots);
		}

        public void SetLightingEnablemendStatus(bool bx, bool by, bool bz)
        {
            m_bLightXEnable = bx;
            m_bLightYEnable = by;
            m_bLightZEnable = bz;
            radioButtonLightXEnabled.Checked = bx;
            radioButtonLightYEnabled.Checked = by;
            radioButtonLightZEnabled.Checked = bz;
			Refresh();
        }

		public bool IsToBeKilled (int n)
		{
			if (n >= 0 && n < m_baRequestedKill.Length)
				return (m_baRequestedKill[n]);
			return (false);

			//throw new Exception("Wrong index: " + n.ToString());
		}
		
		public void KillRequestCompleted (int n)
		{
            if (n >= 0 && n < m_baRequestedKill.Length)
				m_baRequestedKill[n] = false;
		}
		
		void AllocateCoordTbl (int nSize)
		{
			m_dXCoordTbl = new double [nSize];
			m_dYCoordTbl = new double [nSize];
			m_daAngleTbl = new double [nSize];
            m_daRadarTbl = new double [nSize];
			m_baRequestedKill = new bool [nSize];
			for (int i = 0; i < nSize; i++)
				m_baRequestedKill [i] = false;
		}
		
		public FormRobotStatus (int nNumOfRobots)
		{
			m_nNumOfRobots = nNumOfRobots;			
			InitializeComponent ();
			AllocateCoordTbl (m_nNumOfRobots);
		}

        public FormRobotStatus(int nNumOfRobots, string cfgpath)
        {
            m_RobotsConfigFile = cfgpath;
            m_RobotsManager = new FormManageRobots(m_RobotsConfigFile);
            m_nNumOfRobots = nNumOfRobots;
            InitializeComponent();
            AllocateCoordTbl(m_nNumOfRobots);
        }
		
		public FormRobotStatus (int nNumOfRobots, double [] daX, double [] daY)
		{
			m_nNumOfRobots = nNumOfRobots;			
			InitializeComponent ();
			AllocateCoordTbl (m_nNumOfRobots);
			for (int i = 0; i < m_nNumOfRobots; i++)
			{
				m_dXCoordTbl [i] = daX [i];
				m_dYCoordTbl [i] = daY [i];
			}
		}

        public FormRobotStatus(int nNumOfRobots, double[] daX, double[] daY, string cfgpath)
        {
            m_RobotsConfigFile = cfgpath;
            m_RobotsManager = new FormManageRobots(m_RobotsConfigFile);
            m_nNumOfRobots = nNumOfRobots;
            InitializeComponent();
            AllocateCoordTbl(m_nNumOfRobots);
            for (int i = 0; i < m_nNumOfRobots; i++)
            {
                m_dXCoordTbl[i] = daX[i];
                m_dYCoordTbl[i] = daY[i];
            }
        }
		
		public void SetPos (double dPosX, double dPosY, int nRobotNum)
		{
            if (nRobotNum >= 0 && nRobotNum < m_dXCoordTbl.Length)
            {
                m_dXCoordTbl[nRobotNum] = dPosX;
                m_dYCoordTbl[nRobotNum] = dPosY;
                if (m_nCurrRobotNum == nRobotNum)
                {
					textBoxPosX.Text = m_dXCoordTbl[m_nCurrRobotNum].ToString();
					textBoxPosY.Text = m_dYCoordTbl[m_nCurrRobotNum].ToString();
					textBoxPosX.Update();
					textBoxPosY.Update();
                }
            }
		}
		
		public void SetAngle (double dAngle, int nRobotNum)
		{
            if (nRobotNum >= 0 && nRobotNum < m_daAngleTbl.Length)
            {
                m_daAngleTbl[nRobotNum] = dAngle;
                if (m_nCurrRobotNum == nRobotNum)
                {
					textBoxAngle.Text = m_daAngleTbl[m_nCurrRobotNum].ToString();
					textBoxAngle.Update();
                }
            }
		}

        public void SetStep(int step)
        {
            textBoxStep.Text = step.ToString();
            textBoxStep.Update();
        }

        public void SetRadar(double rdr, int nRobotNum)
        {
            if (nRobotNum >= 0 && nRobotNum < m_daRadarTbl.Length)
            {
                m_daRadarTbl[nRobotNum] = rdr;
                if (m_nCurrRobotNum == nRobotNum)
                {
                    textBoxRadar.Text = m_daRadarTbl[m_nCurrRobotNum].ToString();
                    textBoxRadar.Update();
                }
            }
        }
		
		public void AppendLog (string sText)
		{
			string sTmpBuf = string.Empty;
			
			if (textBoxLog.Text.Length + sText.Length >= 10000)
			{
				sTmpBuf = textBoxLog.Text;
				if (sTmpBuf.Length > sText.Length)
				{
					sTmpBuf = sTmpBuf.Remove (0, 5000);
				}
				else
				{
					sTmpBuf = string.Empty;
				}
				textBoxLog.Text = "\r\n" + sTmpBuf;
			}
			textBoxLog.AppendText (sText + "\r\n");
			
			string sRobotID = "Robot #" + m_nCurrRobotNum;
			
			if (sText.StartsWith(sRobotID, StringComparison.Ordinal)) {
				
				const string sep = "|";
				char [] delim = sep.ToCharArray ();
				string [] sShortInfo = sText.Split(delim);
				textBoxCurrCmd.Text = sShortInfo[0];
			}
			
			textBoxLog.Update ();
			textBoxCurrCmd.Update ();
		}

		public void UpdateStatus ()
		{
			Refresh();
		}
		
		public void UpdateStatus (double [] daX, double [] daY)
		{
			for (int i = 0; i < m_nNumOfRobots; i++)
			{
				SetPos (daX [i], daY [i], i);
			}	
			Refresh();
		}

        public void UpdateStatus(double[] daX, double[] daY, int step)
        {
			textBoxStep.Text = step.ToString();
            UpdateStatus(daX, daY);
        }
        
        public void ShowTextLog()
        {
        	checkBoxHideLog.Checked = false;
        }


		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.labelCurrCmd = new System.Windows.Forms.Label();
			this.buttonKill = new System.Windows.Forms.Button();
			this.numericUpDownRobot = new System.Windows.Forms.NumericUpDown();
			this.labelRobotNum = new System.Windows.Forms.Label();
			this.labelPosition = new System.Windows.Forms.Label();
			this.textBoxPosX = new System.Windows.Forms.TextBox();
			this.textBoxPosY = new System.Windows.Forms.TextBox();
			this.textBoxAngle = new System.Windows.Forms.TextBox();
			this.labelAngle = new System.Windows.Forms.Label();
			this.textBoxLog = new System.Windows.Forms.TextBox();
			this.textBoxCurrCmd = new System.Windows.Forms.TextBox();
			this.labelComa = new System.Windows.Forms.Label();
			this.checkBoxHideLog = new System.Windows.Forms.CheckBox();
			this.labelStep = new System.Windows.Forms.Label();
			this.textBoxStep = new System.Windows.Forms.TextBox();
			this.buttonManage = new System.Windows.Forms.Button();
			this.radioButtonLightXEnabled = new System.Windows.Forms.RadioButton();
			this.radioButtonLightYEnabled = new System.Windows.Forms.RadioButton();
			this.radioButtonLightZEnabled = new System.Windows.Forms.RadioButton();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.labelRadar = new System.Windows.Forms.Label();
			this.textBoxRadar = new System.Windows.Forms.TextBox();
			this.checkBoxShowAllBotsLog = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownRobot)).BeginInit();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelCurrCmd
			// 
			this.labelCurrCmd.Location = new System.Drawing.Point(11, 140);
			this.labelCurrCmd.Name = "labelCurrCmd";
			this.labelCurrCmd.Size = new System.Drawing.Size(146, 23);
			this.labelCurrCmd.TabIndex = 7;
			this.labelCurrCmd.Text = "Current command:";
			// 
			// buttonKill
			// 
			this.buttonKill.Location = new System.Drawing.Point(516, 58);
			this.buttonKill.Name = "buttonKill";
			this.buttonKill.Size = new System.Drawing.Size(75, 23);
			this.buttonKill.TabIndex = 11;
			this.buttonKill.Text = "Kill";
			this.buttonKill.Click += new System.EventHandler(this.ButtonKillClick);
			// 
			// numericUpDownRobot
			// 
			this.numericUpDownRobot.Location = new System.Drawing.Point(540, 17);
			this.numericUpDownRobot.Name = "numericUpDownRobot";
			this.numericUpDownRobot.Size = new System.Drawing.Size(52, 20);
			this.numericUpDownRobot.TabIndex = 5;
			this.numericUpDownRobot.ValueChanged += new System.EventHandler(this.NumericUpDownRobotValueChanged);
			// 
			// labelRobotNum
			// 
			this.labelRobotNum.Location = new System.Drawing.Point(467, 20);
			this.labelRobotNum.Name = "labelRobotNum";
			this.labelRobotNum.Size = new System.Drawing.Size(72, 23);
			this.labelRobotNum.TabIndex = 6;
			this.labelRobotNum.Text = "Robot #:";
			// 
			// labelPosition
			// 
			this.labelPosition.Location = new System.Drawing.Point(22, 22);
			this.labelPosition.Name = "labelPosition";
			this.labelPosition.Size = new System.Drawing.Size(105, 23);
			this.labelPosition.TabIndex = 0;
			this.labelPosition.Text = "Position X, Y:";
			// 
			// textBoxPosX
			// 
			this.textBoxPosX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxPosX.Location = new System.Drawing.Point(145, 19);
			this.textBoxPosX.MaxLength = 10;
			this.textBoxPosX.Name = "textBoxPosX";
			this.textBoxPosX.ReadOnly = true;
			this.textBoxPosX.Size = new System.Drawing.Size(144, 20);
			this.textBoxPosX.TabIndex = 1;
			// 
			// textBoxPosY
			// 
			this.textBoxPosY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxPosY.Location = new System.Drawing.Point(313, 19);
			this.textBoxPosY.MaxLength = 10;
			this.textBoxPosY.Name = "textBoxPosY";
			this.textBoxPosY.ReadOnly = true;
			this.textBoxPosY.Size = new System.Drawing.Size(143, 20);
			this.textBoxPosY.TabIndex = 2;
			// 
			// textBoxAngle
			// 
			this.textBoxAngle.Location = new System.Drawing.Point(145, 57);
			this.textBoxAngle.Name = "textBoxAngle";
			this.textBoxAngle.ReadOnly = true;
			this.textBoxAngle.Size = new System.Drawing.Size(144, 20);
			this.textBoxAngle.TabIndex = 10;
			// 
			// labelAngle
			// 
			this.labelAngle.Location = new System.Drawing.Point(24, 60);
			this.labelAngle.Name = "labelAngle";
			this.labelAngle.Size = new System.Drawing.Size(106, 23);
			this.labelAngle.TabIndex = 9;
			this.labelAngle.Text = "Angle [rad]:";
			// 
			// textBoxLog
			// 
			this.textBoxLog.Location = new System.Drawing.Point(8, 248);
			this.textBoxLog.MaxLength = 0;
			this.textBoxLog.Multiline = true;
			this.textBoxLog.Name = "textBoxLog";
			this.textBoxLog.ReadOnly = true;
			this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBoxLog.Size = new System.Drawing.Size(583, 373);
			this.textBoxLog.TabIndex = 4;
			// 
			// textBoxCurrCmd
			// 
			this.textBoxCurrCmd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxCurrCmd.Location = new System.Drawing.Point(145, 138);
			this.textBoxCurrCmd.MaxLength = 100;
			this.textBoxCurrCmd.Name = "textBoxCurrCmd";
			this.textBoxCurrCmd.ReadOnly = true;
			this.textBoxCurrCmd.Size = new System.Drawing.Size(281, 20);
			this.textBoxCurrCmd.TabIndex = 8;
			// 
			// labelComa
			// 
			this.labelComa.Location = new System.Drawing.Point(294, 21);
			this.labelComa.Name = "labelComa";
			this.labelComa.Size = new System.Drawing.Size(15, 23);
			this.labelComa.TabIndex = 3;
			this.labelComa.Text = ",";
			// 
			// checkBoxHideLog
			// 
			this.checkBoxHideLog.Location = new System.Drawing.Point(313, 58);
			this.checkBoxHideLog.Name = "checkBoxHideLog";
			this.checkBoxHideLog.Size = new System.Drawing.Size(100, 19);
			this.checkBoxHideLog.TabIndex = 12;
			this.checkBoxHideLog.Text = "Hide text log";
			this.checkBoxHideLog.CheckedChanged += new System.EventHandler(this.checkBoxHideLog_CheckedChanged);
			// 
			// labelStep
			// 
			this.labelStep.AutoSize = true;
			this.labelStep.Location = new System.Drawing.Point(442, 140);
			this.labelStep.Name = "labelStep";
			this.labelStep.Size = new System.Drawing.Size(32, 13);
			this.labelStep.TabIndex = 13;
			this.labelStep.Text = "Step:";
			// 
			// textBoxStep
			// 
			this.textBoxStep.Location = new System.Drawing.Point(491, 137);
			this.textBoxStep.MaxLength = 10;
			this.textBoxStep.Name = "textBoxStep";
			this.textBoxStep.ReadOnly = true;
			this.textBoxStep.Size = new System.Drawing.Size(100, 20);
			this.textBoxStep.TabIndex = 14;
			this.textBoxStep.Text = "0";
			this.textBoxStep.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// buttonManage
			// 
			this.buttonManage.Location = new System.Drawing.Point(435, 58);
			this.buttonManage.Name = "buttonManage";
			this.buttonManage.Size = new System.Drawing.Size(75, 23);
			this.buttonManage.TabIndex = 15;
			this.buttonManage.Text = "Manage";
			this.buttonManage.UseVisualStyleBackColor = true;
			this.buttonManage.Click += new System.EventHandler(this.buttonManage_Click);
			// 
			// radioButtonLightXEnabled
			// 
			this.radioButtonLightXEnabled.AutoSize = true;
			this.radioButtonLightXEnabled.Location = new System.Drawing.Point(3, 5);
			this.radioButtonLightXEnabled.Name = "radioButtonLightXEnabled";
			this.radioButtonLightXEnabled.Size = new System.Drawing.Size(32, 17);
			this.radioButtonLightXEnabled.TabIndex = 16;
			this.radioButtonLightXEnabled.TabStop = true;
			this.radioButtonLightXEnabled.Text = "X";
			this.radioButtonLightXEnabled.UseVisualStyleBackColor = true;
			this.radioButtonLightXEnabled.Click += new System.EventHandler(this.radioButtonLightXEnabled_Click);
			// 
			// radioButtonLightYEnabled
			// 
			this.radioButtonLightYEnabled.AutoSize = true;
			this.radioButtonLightYEnabled.Location = new System.Drawing.Point(3, 5);
			this.radioButtonLightYEnabled.Name = "radioButtonLightYEnabled";
			this.radioButtonLightYEnabled.Size = new System.Drawing.Size(32, 17);
			this.radioButtonLightYEnabled.TabIndex = 17;
			this.radioButtonLightYEnabled.TabStop = true;
			this.radioButtonLightYEnabled.Text = "Y";
			this.radioButtonLightYEnabled.UseVisualStyleBackColor = true;
			this.radioButtonLightYEnabled.Click += new System.EventHandler(this.radioButtonLightYEnabled_Click);
			// 
			// radioButtonLightZEnabled
			// 
			this.radioButtonLightZEnabled.AutoSize = true;
			this.radioButtonLightZEnabled.Location = new System.Drawing.Point(3, 5);
			this.radioButtonLightZEnabled.Name = "radioButtonLightZEnabled";
			this.radioButtonLightZEnabled.Size = new System.Drawing.Size(32, 17);
			this.radioButtonLightZEnabled.TabIndex = 18;
			this.radioButtonLightZEnabled.TabStop = true;
			this.radioButtonLightZEnabled.Text = "Z";
			this.radioButtonLightZEnabled.UseVisualStyleBackColor = true;
			this.radioButtonLightZEnabled.Click += new System.EventHandler(this.radioButtonLightZEnabled_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.radioButtonLightXEnabled);
			this.panel1.Location = new System.Drawing.Point(14, 185);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(39, 26);
			this.panel1.TabIndex = 20;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.radioButtonLightYEnabled);
			this.panel2.Location = new System.Drawing.Point(59, 185);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(39, 26);
			this.panel2.TabIndex = 21;
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.radioButtonLightZEnabled);
			this.panel3.Location = new System.Drawing.Point(104, 185);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(38, 26);
			this.panel3.TabIndex = 22;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(11, 163);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 13);
			this.label1.TabIndex = 23;
			this.label1.Text = "Lighting enabled:";
			// 
			// labelRadar
			// 
			this.labelRadar.Location = new System.Drawing.Point(22, 101);
			this.labelRadar.Name = "labelRadar";
			this.labelRadar.Size = new System.Drawing.Size(106, 23);
			this.labelRadar.TabIndex = 24;
			this.labelRadar.Text = "Radar:";
			// 
			// textBoxRadar
			// 
			this.textBoxRadar.Location = new System.Drawing.Point(145, 98);
			this.textBoxRadar.Name = "textBoxRadar";
			this.textBoxRadar.ReadOnly = true;
			this.textBoxRadar.Size = new System.Drawing.Size(144, 20);
			this.textBoxRadar.TabIndex = 25;
			// 
			// checkBoxShowAllBotsLog
			// 
			this.checkBoxShowAllBotsLog.Location = new System.Drawing.Point(14, 218);
			this.checkBoxShowAllBotsLog.Name = "checkBoxShowAllBotsLog";
			this.checkBoxShowAllBotsLog.Size = new System.Drawing.Size(143, 24);
			this.checkBoxShowAllBotsLog.TabIndex = 26;
			this.checkBoxShowAllBotsLog.Text = "Show log from all bots.";
			this.checkBoxShowAllBotsLog.UseVisualStyleBackColor = true;
			this.checkBoxShowAllBotsLog.CheckedChanged += new System.EventHandler(this.CheckBoxShowAllBotsLogCheckedChanged);
			// 
			// FormRobotStatus
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(599, 633);
			this.ControlBox = false;
			this.Controls.Add(this.checkBoxShowAllBotsLog);
			this.Controls.Add(this.textBoxRadar);
			this.Controls.Add(this.labelRadar);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.panel3);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.buttonManage);
			this.Controls.Add(this.textBoxStep);
			this.Controls.Add(this.labelStep);
			this.Controls.Add(this.checkBoxHideLog);
			this.Controls.Add(this.buttonKill);
			this.Controls.Add(this.textBoxAngle);
			this.Controls.Add(this.labelAngle);
			this.Controls.Add(this.textBoxCurrCmd);
			this.Controls.Add(this.labelCurrCmd);
			this.Controls.Add(this.labelRobotNum);
			this.Controls.Add(this.numericUpDownRobot);
			this.Controls.Add(this.textBoxLog);
			this.Controls.Add(this.labelComa);
			this.Controls.Add(this.textBoxPosY);
			this.Controls.Add(this.textBoxPosX);
			this.Controls.Add(this.labelPosition);
			this.Name = "FormRobotStatus";
			this.Opacity = 0.9D;
			this.Text = "Robots Status/Control";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormRobotStatus_FormClosing);
			this.Resize += new System.EventHandler(this.FormRobotStatusResize);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownRobot)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
		void NumericUpDownRobotValueChanged(object sender, EventArgs e)
		{
			if ((int)((NumericUpDown)sender).Value >= m_nNumOfRobots)
				((NumericUpDown)sender).Value = m_nNumOfRobots - 1;
			m_nCurrRobotNum = (int) ((NumericUpDown)sender).Value;
			Update();
			Refresh();
		}
		
		void ButtonKillClick(object sender, EventArgs e)
		{
			m_baRequestedKill [(int)numericUpDownRobot.Value] = true;
		}
		
		void FormRobotStatusResize(object sender, EventArgs e)
		{
			int width = ((Form)sender).Size.Width - 20;
			int height = ((Form)sender).Size.Height - textBoxLog.Bounds.Y - 60;
			
			this.textBoxLog.SetBounds (textBoxLog.Bounds.X, textBoxLog.Bounds.Y, width, height);
		}

        void checkBoxHideLog_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxHideLog.Checked)
				textBoxLog.Visible = false;
            else
				textBoxLog.Visible = true;
        }

        void buttonManage_Click(object sender, EventArgs e)
        {
            //m_RobotsManager.ShowDialog();
            if (null == m_RobotsManager || m_RobotsManager.IsDisposed)
            {
                m_RobotsManager = new FormManageRobots(m_RobotsConfigFile);
            }
            m_RobotsManager.Show();
        }

        void FormRobotStatus_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_RobotsManager.Close();
        }

        void radioButtonLightXEnabled_Click(object sender, EventArgs e)
        {
            var btn = (RadioButton)sender;

			m_bLightXEnable = !m_bLightXEnable;
            btn.Checked = m_bLightXEnable;
            m_bLightingSetupChanged = true;
        }

        void radioButtonLightYEnabled_Click(object sender, EventArgs e)
        {
            var btn = (RadioButton)sender;

			m_bLightYEnable = !m_bLightYEnable;
            btn.Checked = m_bLightYEnable;
            m_bLightingSetupChanged = true;
        }

        void radioButtonLightZEnabled_Click(object sender, EventArgs e)
        {
            var btn = (RadioButton)sender;

			m_bLightZEnable = !m_bLightZEnable;
            btn.Checked = m_bLightZEnable;
            m_bLightingSetupChanged = true;
        }
        
		void CheckBoxShowAllBotsLogCheckedChanged(object sender, EventArgs e)
		{
			var chkBox = (CheckBox)sender;
			
			m_bShowAllBotsLog = chkBox.Checked;
		}
		
	}
}
