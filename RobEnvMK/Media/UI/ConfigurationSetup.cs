// created on 2004*11*10 at 12:40 PM
using System;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace RobEnvMK 
{
	public class ConfigurationForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label labelLightY;
		private System.Windows.Forms.Button buttonMapImageBrowse;
		private System.Windows.Forms.TextBox textBoxRobotSize;
		private System.Windows.Forms.TextBox textBoxPlugInCfgFilePath;
		private System.Windows.Forms.CheckBox checkBoxHWVertexProc;
		private System.Windows.Forms.Label labelRotationZ;
		private System.Windows.Forms.Label labelRotationY;
		private System.Windows.Forms.Label labelRotationX;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Label labelCameraY;
		private System.Windows.Forms.TextBox textBoxRotationZ;
		private System.Windows.Forms.TextBox textBoxLookAtZ;
		private System.Windows.Forms.TextBox textBoxLookAtY;
		private System.Windows.Forms.TextBox textBoxLookAtX;
		private System.Windows.Forms.Label labelRobotStartY;
		private System.Windows.Forms.Button buttonBrowsePlugInCfg;
		private System.Windows.Forms.Label labelCameraZ;
		private System.Windows.Forms.TextBox textBoxImagePath;
		private System.Windows.Forms.TextBox textBoxRobotStartY;
		private System.Windows.Forms.TextBox textBoxRobotStartX;
		private System.Windows.Forms.CheckBox checkBoxVisualizeWorld;
		private System.Windows.Forms.TextBox textBoxSightDistance;
		private System.Windows.Forms.Label labelRobotStartX;
		private System.Windows.Forms.TextBox textBoxLightZ;
		private System.Windows.Forms.TextBox textBoxLightY;
		private System.Windows.Forms.TextBox textBoxLightX;
		private System.Windows.Forms.Label labelLightX;
		private System.Windows.Forms.Label labelCameraX;
		private System.Windows.Forms.Label labelLightZ;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.TextBox textBoxRotationY;
		private System.Windows.Forms.TextBox textBoxRotationX;
		private System.Windows.Forms.CheckBox checkBoxRandomRobotStart;
		private System.Windows.Forms.TextBox textBoxCameraX;
		private System.Windows.Forms.TextBox textBoxCameraY;
		private System.Windows.Forms.TextBox textBoxCameraZ;
		private System.Windows.Forms.Label labelLookAtY;
		private System.Windows.Forms.GroupBox groupBoxRobotStart;
		private System.Windows.Forms.Label labelLookAtZ;
		private System.Windows.Forms.CheckBox checkBoxShowOnScreenInfo;
		private System.Windows.Forms.Label labelRobotSize;
		private System.Windows.Forms.Label labelSightDistance;
		private System.Windows.Forms.TextBox textBoxStep;
		private System.Windows.Forms.Label labelStep;
		private System.Windows.Forms.Label labelPathToPlugInCfg;
		private System.Windows.Forms.Label labelMapImagePath;
		private System.Windows.Forms.Label labelLookAtX;
        private CheckBox checkBoxShowStatus;
        private OpenFileDialog openFileDialog1;
        private OpenFileDialog openFileDialog2;
        private CheckBox checkBoxCloseOnAllBotsQuit;
		const string m_sConfigFile = "./SimpleWorldSetup.ini";
		private System.Windows.Forms.Label labelMaxIdleSteps;
		private System.Windows.Forms.NumericUpDown numericUpDownMaxIdleSteps;
		private System.Windows.Forms.Label labelStuckRadius;
		private System.Windows.Forms.NumericUpDown numericUpDownStuckRadius;
		private System.Windows.Forms.Label labelMaxStuckSteps;
		private System.Windows.Forms.NumericUpDown numericUpDownMaxStuckSteps;
		
		public ConfigurationForm()
		{
			InitializeComponent();
			this.LoadConfig ();
		}
		
		// THIS METHOD IS MAINTAINED BY THE FORM DESIGNER
		// DO NOT EDIT IT MANUALLY! YOUR CHANGES ARE LIKELY TO BE LOST
		void InitializeComponent()
		{
			this.labelLookAtX = new System.Windows.Forms.Label();
			this.labelMapImagePath = new System.Windows.Forms.Label();
			this.labelPathToPlugInCfg = new System.Windows.Forms.Label();
			this.labelStep = new System.Windows.Forms.Label();
			this.textBoxStep = new System.Windows.Forms.TextBox();
			this.labelSightDistance = new System.Windows.Forms.Label();
			this.labelRobotSize = new System.Windows.Forms.Label();
			this.checkBoxShowOnScreenInfo = new System.Windows.Forms.CheckBox();
			this.labelLookAtZ = new System.Windows.Forms.Label();
			this.groupBoxRobotStart = new System.Windows.Forms.GroupBox();
			this.textBoxRobotStartY = new System.Windows.Forms.TextBox();
			this.textBoxRobotStartX = new System.Windows.Forms.TextBox();
			this.labelRobotStartY = new System.Windows.Forms.Label();
			this.labelRobotStartX = new System.Windows.Forms.Label();
			this.checkBoxRandomRobotStart = new System.Windows.Forms.CheckBox();
			this.labelLookAtY = new System.Windows.Forms.Label();
			this.textBoxCameraZ = new System.Windows.Forms.TextBox();
			this.textBoxCameraY = new System.Windows.Forms.TextBox();
			this.textBoxCameraX = new System.Windows.Forms.TextBox();
			this.textBoxRotationX = new System.Windows.Forms.TextBox();
			this.textBoxRotationY = new System.Windows.Forms.TextBox();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.labelLightZ = new System.Windows.Forms.Label();
			this.labelCameraX = new System.Windows.Forms.Label();
			this.labelLightX = new System.Windows.Forms.Label();
			this.textBoxLightX = new System.Windows.Forms.TextBox();
			this.textBoxLightY = new System.Windows.Forms.TextBox();
			this.textBoxLightZ = new System.Windows.Forms.TextBox();
			this.textBoxSightDistance = new System.Windows.Forms.TextBox();
			this.checkBoxVisualizeWorld = new System.Windows.Forms.CheckBox();
			this.textBoxImagePath = new System.Windows.Forms.TextBox();
			this.labelCameraZ = new System.Windows.Forms.Label();
			this.buttonBrowsePlugInCfg = new System.Windows.Forms.Button();
			this.textBoxLookAtX = new System.Windows.Forms.TextBox();
			this.textBoxLookAtY = new System.Windows.Forms.TextBox();
			this.textBoxLookAtZ = new System.Windows.Forms.TextBox();
			this.textBoxRotationZ = new System.Windows.Forms.TextBox();
			this.labelCameraY = new System.Windows.Forms.Label();
			this.buttonOK = new System.Windows.Forms.Button();
			this.labelRotationX = new System.Windows.Forms.Label();
			this.labelRotationY = new System.Windows.Forms.Label();
			this.labelRotationZ = new System.Windows.Forms.Label();
			this.checkBoxHWVertexProc = new System.Windows.Forms.CheckBox();
			this.textBoxPlugInCfgFilePath = new System.Windows.Forms.TextBox();
			this.textBoxRobotSize = new System.Windows.Forms.TextBox();
			this.buttonMapImageBrowse = new System.Windows.Forms.Button();
			this.labelLightY = new System.Windows.Forms.Label();
			this.checkBoxShowStatus = new System.Windows.Forms.CheckBox();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
			this.checkBoxCloseOnAllBotsQuit = new System.Windows.Forms.CheckBox();
			this.labelMaxIdleSteps = new System.Windows.Forms.Label();
			this.numericUpDownMaxIdleSteps = new System.Windows.Forms.NumericUpDown();
			this.labelStuckRadius = new System.Windows.Forms.Label();
			this.numericUpDownStuckRadius = new System.Windows.Forms.NumericUpDown();
			this.labelMaxStuckSteps = new System.Windows.Forms.Label();
			this.numericUpDownMaxStuckSteps = new System.Windows.Forms.NumericUpDown();
			this.groupBoxRobotStart.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxIdleSteps)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownStuckRadius)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxStuckSteps)).BeginInit();
			this.SuspendLayout();
			// 
			// labelLookAtX
			// 
			this.labelLookAtX.Location = new System.Drawing.Point(11, 182);
			this.labelLookAtX.Name = "labelLookAtX";
			this.labelLookAtX.Size = new System.Drawing.Size(89, 16);
			this.labelLookAtX.TabIndex = 13;
			this.labelLookAtX.Text = "Look at X:";
			// 
			// labelMapImagePath
			// 
			this.labelMapImagePath.Location = new System.Drawing.Point(12, 421);
			this.labelMapImagePath.Name = "labelMapImagePath";
			this.labelMapImagePath.Size = new System.Drawing.Size(148, 19);
			this.labelMapImagePath.TabIndex = 27;
			this.labelMapImagePath.Text = "Path to map image:";
			// 
			// labelPathToPlugInCfg
			// 
			this.labelPathToPlugInCfg.Location = new System.Drawing.Point(12, 456);
			this.labelPathToPlugInCfg.Name = "labelPathToPlugInCfg";
			this.labelPathToPlugInCfg.Size = new System.Drawing.Size(217, 23);
			this.labelPathToPlugInCfg.TabIndex = 46;
			this.labelPathToPlugInCfg.Text = "Path to plug-in robots config:";
			// 
			// labelStep
			// 
			this.labelStep.Location = new System.Drawing.Point(241, 30);
			this.labelStep.Name = "labelStep";
			this.labelStep.Size = new System.Drawing.Size(87, 17);
			this.labelStep.TabIndex = 35;
			this.labelStep.Text = "Step:";
			this.labelStep.Click += new System.EventHandler(this.LabelStepClick);
			// 
			// textBoxStep
			// 
			this.textBoxStep.Location = new System.Drawing.Point(345, 25);
			this.textBoxStep.MaxLength = 10;
			this.textBoxStep.Name = "textBoxStep";
			this.textBoxStep.Size = new System.Drawing.Size(114, 20);
			this.textBoxStep.TabIndex = 36;
			this.textBoxStep.Text = "0.6";
			this.textBoxStep.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelSightDistance
			// 
			this.labelSightDistance.Location = new System.Drawing.Point(241, 84);
			this.labelSightDistance.Name = "labelSightDistance";
			this.labelSightDistance.Size = new System.Drawing.Size(86, 18);
			this.labelSightDistance.TabIndex = 42;
			this.labelSightDistance.Text = "Sight distance:";
			// 
			// labelRobotSize
			// 
			this.labelRobotSize.Location = new System.Drawing.Point(241, 57);
			this.labelRobotSize.Name = "labelRobotSize";
			this.labelRobotSize.Size = new System.Drawing.Size(87, 17);
			this.labelRobotSize.TabIndex = 48;
			this.labelRobotSize.Text = "Size:";
			// 
			// checkBoxShowOnScreenInfo
			// 
			this.checkBoxShowOnScreenInfo.Location = new System.Drawing.Point(244, 299);
			this.checkBoxShowOnScreenInfo.Name = "checkBoxShowOnScreenInfo";
			this.checkBoxShowOnScreenInfo.Size = new System.Drawing.Size(174, 18);
			this.checkBoxShowOnScreenInfo.TabIndex = 38;
			this.checkBoxShowOnScreenInfo.Text = global::RobEnvMK.Properties.Resources.SetupChkBoxTextShowOnScreenInfo;
			// 
			// labelLookAtZ
			// 
			this.labelLookAtZ.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelLookAtZ.Location = new System.Drawing.Point(10, 230);
			this.labelLookAtZ.Name = "labelLookAtZ";
			this.labelLookAtZ.Size = new System.Drawing.Size(87, 16);
			this.labelLookAtZ.TabIndex = 19;
			this.labelLookAtZ.Text = "Look at Z:";
			// 
			// groupBoxRobotStart
			// 
			this.groupBoxRobotStart.Controls.Add(this.textBoxRobotStartY);
			this.groupBoxRobotStart.Controls.Add(this.textBoxRobotStartX);
			this.groupBoxRobotStart.Controls.Add(this.labelRobotStartY);
			this.groupBoxRobotStart.Controls.Add(this.labelRobotStartX);
			this.groupBoxRobotStart.Controls.Add(this.checkBoxRandomRobotStart);
			this.groupBoxRobotStart.Location = new System.Drawing.Point(242, 113);
			this.groupBoxRobotStart.Name = "groupBoxRobotStart";
			this.groupBoxRobotStart.Size = new System.Drawing.Size(219, 137);
			this.groupBoxRobotStart.TabIndex = 44;
			this.groupBoxRobotStart.TabStop = false;
			this.groupBoxRobotStart.Text = "Robot start position";
			// 
			// textBoxRobotStartY
			// 
			this.textBoxRobotStartY.Location = new System.Drawing.Point(37, 97);
			this.textBoxRobotStartY.Name = "textBoxRobotStartY";
			this.textBoxRobotStartY.Size = new System.Drawing.Size(100, 20);
			this.textBoxRobotStartY.TabIndex = 4;
			this.textBoxRobotStartY.Text = "10";
			this.textBoxRobotStartY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textBoxRobotStartX
			// 
			this.textBoxRobotStartX.Location = new System.Drawing.Point(37, 61);
			this.textBoxRobotStartX.MaxLength = 3;
			this.textBoxRobotStartX.Name = "textBoxRobotStartX";
			this.textBoxRobotStartX.Size = new System.Drawing.Size(100, 20);
			this.textBoxRobotStartX.TabIndex = 3;
			this.textBoxRobotStartX.Text = "10";
			this.textBoxRobotStartX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelRobotStartY
			// 
			this.labelRobotStartY.Location = new System.Drawing.Point(9, 101);
			this.labelRobotStartY.Name = "labelRobotStartY";
			this.labelRobotStartY.Size = new System.Drawing.Size(27, 23);
			this.labelRobotStartY.TabIndex = 2;
			this.labelRobotStartY.Text = "Y:";
			// 
			// labelRobotStartX
			// 
			this.labelRobotStartX.Location = new System.Drawing.Point(9, 63);
			this.labelRobotStartX.Name = "labelRobotStartX";
			this.labelRobotStartX.Size = new System.Drawing.Size(29, 23);
			this.labelRobotStartX.TabIndex = 1;
			this.labelRobotStartX.Text = "X:";
			// 
			// checkBoxRandomRobotStart
			// 
			this.checkBoxRandomRobotStart.Location = new System.Drawing.Point(13, 28);
			this.checkBoxRandomRobotStart.Name = "checkBoxRandomRobotStart";
			this.checkBoxRandomRobotStart.Size = new System.Drawing.Size(104, 24);
			this.checkBoxRandomRobotStart.TabIndex = 0;
			this.checkBoxRandomRobotStart.Text = global::RobEnvMK.Properties.Resources.SetupChkBoxTextRandom;
			// 
			// labelLookAtY
			// 
			this.labelLookAtY.Location = new System.Drawing.Point(11, 205);
			this.labelLookAtY.Name = "labelLookAtY";
			this.labelLookAtY.Size = new System.Drawing.Size(86, 21);
			this.labelLookAtY.TabIndex = 16;
			this.labelLookAtY.Text = "Look at Y:";
			// 
			// textBoxCameraZ
			// 
			this.textBoxCameraZ.Location = new System.Drawing.Point(113, 154);
			this.textBoxCameraZ.MaxLength = 10;
			this.textBoxCameraZ.Name = "textBoxCameraZ";
			this.textBoxCameraZ.Size = new System.Drawing.Size(114, 20);
			this.textBoxCameraZ.TabIndex = 12;
			this.textBoxCameraZ.Text = "-50.0";
			this.textBoxCameraZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textBoxCameraY
			// 
			this.textBoxCameraY.Location = new System.Drawing.Point(113, 129);
			this.textBoxCameraY.MaxLength = 10;
			this.textBoxCameraY.Name = "textBoxCameraY";
			this.textBoxCameraY.Size = new System.Drawing.Size(114, 20);
			this.textBoxCameraY.TabIndex = 10;
			this.textBoxCameraY.Text = "-20.0";
			this.textBoxCameraY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textBoxCameraX
			// 
			this.textBoxCameraX.Location = new System.Drawing.Point(113, 104);
			this.textBoxCameraX.MaxLength = 10;
			this.textBoxCameraX.Name = "textBoxCameraX";
			this.textBoxCameraX.Size = new System.Drawing.Size(114, 20);
			this.textBoxCameraX.TabIndex = 9;
			this.textBoxCameraX.Text = "10.0";
			this.textBoxCameraX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textBoxRotationX
			// 
			this.textBoxRotationX.Location = new System.Drawing.Point(113, 329);
			this.textBoxRotationX.MaxLength = 10;
			this.textBoxRotationX.Name = "textBoxRotationX";
			this.textBoxRotationX.Size = new System.Drawing.Size(114, 20);
			this.textBoxRotationX.TabIndex = 30;
			this.textBoxRotationX.Text = "0.0";
			this.textBoxRotationX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textBoxRotationY
			// 
			this.textBoxRotationY.Location = new System.Drawing.Point(113, 355);
			this.textBoxRotationY.MaxLength = 10;
			this.textBoxRotationY.Name = "textBoxRotationY";
			this.textBoxRotationY.Size = new System.Drawing.Size(114, 20);
			this.textBoxRotationY.TabIndex = 32;
			this.textBoxRotationY.Text = "0.0";
			this.textBoxRotationY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Location = new System.Drawing.Point(256, 512);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(112, 32);
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = global::RobEnvMK.Properties.Resources.SetupButtonText_Cancel;
			this.buttonCancel.Click += new System.EventHandler(this.ButtonCancelClick);
			// 
			// labelLightZ
			// 
			this.labelLightZ.Location = new System.Drawing.Point(11, 307);
			this.labelLightZ.Name = "labelLightZ";
			this.labelLightZ.Size = new System.Drawing.Size(67, 19);
			this.labelLightZ.TabIndex = 25;
			this.labelLightZ.Text = "Light Z:";
			// 
			// labelCameraX
			// 
			this.labelCameraX.Location = new System.Drawing.Point(8, 109);
			this.labelCameraX.Name = "labelCameraX";
			this.labelCameraX.Size = new System.Drawing.Size(87, 17);
			this.labelCameraX.TabIndex = 7;
			this.labelCameraX.Text = "Camera X:";
			// 
			// labelLightX
			// 
			this.labelLightX.Location = new System.Drawing.Point(10, 257);
			this.labelLightX.Name = "labelLightX";
			this.labelLightX.Size = new System.Drawing.Size(66, 19);
			this.labelLightX.TabIndex = 21;
			this.labelLightX.Text = "Light X:";
			// 
			// textBoxLightX
			// 
			this.textBoxLightX.Location = new System.Drawing.Point(113, 254);
			this.textBoxLightX.MaxLength = 10;
			this.textBoxLightX.Name = "textBoxLightX";
			this.textBoxLightX.Size = new System.Drawing.Size(114, 20);
			this.textBoxLightX.TabIndex = 22;
			this.textBoxLightX.Text = "43.0";
			this.textBoxLightX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textBoxLightY
			// 
			this.textBoxLightY.Location = new System.Drawing.Point(113, 279);
			this.textBoxLightY.MaxLength = 10;
			this.textBoxLightY.Name = "textBoxLightY";
			this.textBoxLightY.Size = new System.Drawing.Size(114, 20);
			this.textBoxLightY.TabIndex = 24;
			this.textBoxLightY.Text = "-80.0";
			this.textBoxLightY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textBoxLightZ
			// 
			this.textBoxLightZ.Location = new System.Drawing.Point(113, 304);
			this.textBoxLightZ.MaxLength = 10;
			this.textBoxLightZ.Name = "textBoxLightZ";
			this.textBoxLightZ.Size = new System.Drawing.Size(114, 20);
			this.textBoxLightZ.TabIndex = 26;
			this.textBoxLightZ.Text = "25.0";
			this.textBoxLightZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textBoxSightDistance
			// 
			this.textBoxSightDistance.Location = new System.Drawing.Point(345, 81);
			this.textBoxSightDistance.MaxLength = 10;
			this.textBoxSightDistance.Name = "textBoxSightDistance";
			this.textBoxSightDistance.Size = new System.Drawing.Size(115, 20);
			this.textBoxSightDistance.TabIndex = 43;
			this.textBoxSightDistance.Text = "6.0";
			this.textBoxSightDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// checkBoxVisualizeWorld
			// 
			this.checkBoxVisualizeWorld.Checked = true;
			this.checkBoxVisualizeWorld.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxVisualizeWorld.Location = new System.Drawing.Point(244, 350);
			this.checkBoxVisualizeWorld.Name = "checkBoxVisualizeWorld";
			this.checkBoxVisualizeWorld.Size = new System.Drawing.Size(156, 24);
			this.checkBoxVisualizeWorld.TabIndex = 52;
			this.checkBoxVisualizeWorld.Text = global::RobEnvMK.Properties.Resources.SetupChkBoxTxt_VisualiseWorld;
			// 
			// textBoxImagePath
			// 
			this.textBoxImagePath.Location = new System.Drawing.Point(238, 418);
			this.textBoxImagePath.MaxLength = 256;
			this.textBoxImagePath.Name = "textBoxImagePath";
			this.textBoxImagePath.Size = new System.Drawing.Size(227, 20);
			this.textBoxImagePath.TabIndex = 28;
			this.textBoxImagePath.Text = "/bin/WorldMap2.bmp";
			// 
			// labelCameraZ
			// 
			this.labelCameraZ.Location = new System.Drawing.Point(8, 156);
			this.labelCameraZ.Name = "labelCameraZ";
			this.labelCameraZ.Size = new System.Drawing.Size(86, 21);
			this.labelCameraZ.TabIndex = 11;
			this.labelCameraZ.Text = "Camera Z:";
			// 
			// buttonBrowsePlugInCfg
			// 
			this.buttonBrowsePlugInCfg.Location = new System.Drawing.Point(488, 451);
			this.buttonBrowsePlugInCfg.Name = "buttonBrowsePlugInCfg";
			this.buttonBrowsePlugInCfg.Size = new System.Drawing.Size(88, 22);
			this.buttonBrowsePlugInCfg.TabIndex = 47;
			this.buttonBrowsePlugInCfg.Text = global::RobEnvMK.Properties.Resources.SetupButtonTxt_Browse;
			this.buttonBrowsePlugInCfg.Click += new System.EventHandler(this.ButtonRobotsFileBrowseClick);
			// 
			// textBoxLookAtX
			// 
			this.textBoxLookAtX.Location = new System.Drawing.Point(113, 179);
			this.textBoxLookAtX.MaxLength = 10;
			this.textBoxLookAtX.Name = "textBoxLookAtX";
			this.textBoxLookAtX.Size = new System.Drawing.Size(114, 20);
			this.textBoxLookAtX.TabIndex = 14;
			this.textBoxLookAtX.Text = "0.0";
			this.textBoxLookAtX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textBoxLookAtY
			// 
			this.textBoxLookAtY.Location = new System.Drawing.Point(113, 204);
			this.textBoxLookAtY.MaxLength = 10;
			this.textBoxLookAtY.Name = "textBoxLookAtY";
			this.textBoxLookAtY.Size = new System.Drawing.Size(114, 20);
			this.textBoxLookAtY.TabIndex = 17;
			this.textBoxLookAtY.Text = "10.0";
			this.textBoxLookAtY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textBoxLookAtZ
			// 
			this.textBoxLookAtZ.Location = new System.Drawing.Point(113, 229);
			this.textBoxLookAtZ.MaxLength = 10;
			this.textBoxLookAtZ.Name = "textBoxLookAtZ";
			this.textBoxLookAtZ.Size = new System.Drawing.Size(114, 20);
			this.textBoxLookAtZ.TabIndex = 20;
			this.textBoxLookAtZ.Text = "15.0";
			this.textBoxLookAtZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textBoxRotationZ
			// 
			this.textBoxRotationZ.Location = new System.Drawing.Point(113, 380);
			this.textBoxRotationZ.MaxLength = 10;
			this.textBoxRotationZ.Name = "textBoxRotationZ";
			this.textBoxRotationZ.Size = new System.Drawing.Size(114, 20);
			this.textBoxRotationZ.TabIndex = 34;
			this.textBoxRotationZ.Text = "0.0";
			this.textBoxRotationZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelCameraY
			// 
			this.labelCameraY.Location = new System.Drawing.Point(9, 131);
			this.labelCameraY.Name = "labelCameraY";
			this.labelCameraY.Size = new System.Drawing.Size(86, 16);
			this.labelCameraY.TabIndex = 8;
			this.labelCameraY.Text = "Camera Y:";
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(92, 512);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(104, 32);
			this.buttonOK.TabIndex = 2;
			this.buttonOK.Text = global::RobEnvMK.Properties.Resources.SetupBtnTxt_OK;
			this.buttonOK.Click += new System.EventHandler(this.ButtonOKClick);
			// 
			// labelRotationX
			// 
			this.labelRotationX.Location = new System.Drawing.Point(11, 335);
			this.labelRotationX.Name = "labelRotationX";
			this.labelRotationX.Size = new System.Drawing.Size(91, 19);
			this.labelRotationX.TabIndex = 29;
			this.labelRotationX.Text = "Rotation X:";
			// 
			// labelRotationY
			// 
			this.labelRotationY.Location = new System.Drawing.Point(11, 361);
			this.labelRotationY.Name = "labelRotationY";
			this.labelRotationY.Size = new System.Drawing.Size(90, 19);
			this.labelRotationY.TabIndex = 31;
			this.labelRotationY.Text = "Rotation Y:";
			// 
			// labelRotationZ
			// 
			this.labelRotationZ.Location = new System.Drawing.Point(12, 385);
			this.labelRotationZ.Name = "labelRotationZ";
			this.labelRotationZ.Size = new System.Drawing.Size(90, 19);
			this.labelRotationZ.TabIndex = 33;
			this.labelRotationZ.Text = "Rotation Z:";
			// 
			// checkBoxHWVertexProc
			// 
			this.checkBoxHWVertexProc.Checked = true;
			this.checkBoxHWVertexProc.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxHWVertexProc.Location = new System.Drawing.Point(244, 323);
			this.checkBoxHWVertexProc.Name = "checkBoxHWVertexProc";
			this.checkBoxHWVertexProc.Size = new System.Drawing.Size(246, 24);
			this.checkBoxHWVertexProc.TabIndex = 51;
			this.checkBoxHWVertexProc.Text = global::RobEnvMK.Properties.Resources.SetupChkBoxTxt_EnableHWVertexProc;
			// 
			// textBoxPlugInCfgFilePath
			// 
			this.textBoxPlugInCfgFilePath.Location = new System.Drawing.Point(238, 453);
			this.textBoxPlugInCfgFilePath.Name = "textBoxPlugInCfgFilePath";
			this.textBoxPlugInCfgFilePath.Size = new System.Drawing.Size(227, 20);
			this.textBoxPlugInCfgFilePath.TabIndex = 45;
			this.textBoxPlugInCfgFilePath.Text = "/bin/RobEnvMK_robots.cfg";
			// 
			// textBoxRobotSize
			// 
			this.textBoxRobotSize.Location = new System.Drawing.Point(345, 53);
			this.textBoxRobotSize.MaxLength = 10;
			this.textBoxRobotSize.Name = "textBoxRobotSize";
			this.textBoxRobotSize.Size = new System.Drawing.Size(114, 20);
			this.textBoxRobotSize.TabIndex = 49;
			this.textBoxRobotSize.Text = "0.6";
			this.textBoxRobotSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// buttonMapImageBrowse
			// 
			this.buttonMapImageBrowse.Location = new System.Drawing.Point(488, 416);
			this.buttonMapImageBrowse.Name = "buttonMapImageBrowse";
			this.buttonMapImageBrowse.Size = new System.Drawing.Size(88, 22);
			this.buttonMapImageBrowse.TabIndex = 39;
			this.buttonMapImageBrowse.Text = global::RobEnvMK.Properties.Resources.SetupBtnTxt_MapImageBrowse;
			this.buttonMapImageBrowse.Click += new System.EventHandler(this.ButtonMapImageBrowseClick);
			// 
			// labelLightY
			// 
			this.labelLightY.Location = new System.Drawing.Point(10, 283);
			this.labelLightY.Name = "labelLightY";
			this.labelLightY.Size = new System.Drawing.Size(66, 19);
			this.labelLightY.TabIndex = 23;
			this.labelLightY.Text = "Light Y:";
			// 
			// checkBoxShowStatus
			// 
			this.checkBoxShowStatus.Location = new System.Drawing.Point(244, 276);
			this.checkBoxShowStatus.Name = "checkBoxShowStatus";
			this.checkBoxShowStatus.Size = new System.Drawing.Size(129, 17);
			this.checkBoxShowStatus.TabIndex = 53;
			this.checkBoxShowStatus.Text = global::RobEnvMK.Properties.Resources.SetupChkBoxTxt_ShowStatusWindow;
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.AddExtension = false;
			this.openFileDialog1.AutoUpgradeEnabled = false;
			this.openFileDialog1.CheckFileExists = false;
			this.openFileDialog1.CheckPathExists = false;
			this.openFileDialog1.DefaultExt = "bmp";
			this.openFileDialog1.FileName = "Map.bmp";
			this.openFileDialog1.Filter = global::RobEnvMK.Properties.Resources.SetupOpenBmpDlg_Filter;
			this.openFileDialog1.InitialDirectory = ".";
			this.openFileDialog1.ReadOnlyChecked = true;
			this.openFileDialog1.RestoreDirectory = true;
			this.openFileDialog1.Title = global::RobEnvMK.Properties.Resources.SetupOpenBmpDlg_Title;
			// 
			// openFileDialog2
			// 
			this.openFileDialog2.AddExtension = false;
			this.openFileDialog2.AutoUpgradeEnabled = false;
			this.openFileDialog2.CheckFileExists = false;
			this.openFileDialog2.CheckPathExists = false;
			this.openFileDialog2.DefaultExt = "cfg";
			this.openFileDialog2.FileName = "robots.cfg";
			this.openFileDialog2.Filter = global::RobEnvMK.Properties.Resources.SetupOpenCfgDlg_Filter;
			this.openFileDialog2.InitialDirectory = ".";
			this.openFileDialog2.ReadOnlyChecked = true;
			this.openFileDialog2.RestoreDirectory = true;
			this.openFileDialog2.Title = global::RobEnvMK.Properties.Resources.SetupOpenCfgDlg_Title;
			// 
			// checkBoxCloseOnAllBotsQuit
			// 
			this.checkBoxCloseOnAllBotsQuit.AutoSize = true;
			this.checkBoxCloseOnAllBotsQuit.Checked = true;
			this.checkBoxCloseOnAllBotsQuit.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxCloseOnAllBotsQuit.Location = new System.Drawing.Point(244, 381);
			this.checkBoxCloseOnAllBotsQuit.Name = "checkBoxCloseOnAllBotsQuit";
			this.checkBoxCloseOnAllBotsQuit.Size = new System.Drawing.Size(137, 17);
			this.checkBoxCloseOnAllBotsQuit.TabIndex = 54;
			this.checkBoxCloseOnAllBotsQuit.Text = global::RobEnvMK.Properties.Resources.SetupChkBoxTxt_CloseWhenAllBotsQuit;
			this.checkBoxCloseOnAllBotsQuit.UseVisualStyleBackColor = true;
			// 
			// labelMaxIdleSteps
			// 
			this.labelMaxIdleSteps.Location = new System.Drawing.Point(11, 27);
			this.labelMaxIdleSteps.Name = "labelMaxIdleSteps";
			this.labelMaxIdleSteps.Size = new System.Drawing.Size(89, 23);
			this.labelMaxIdleSteps.TabIndex = 55;
			this.labelMaxIdleSteps.Text = "Max. idle steps:";
			// 
			// numericUpDownMaxIdleSteps
			// 
			this.numericUpDownMaxIdleSteps.Location = new System.Drawing.Point(113, 26);
			this.numericUpDownMaxIdleSteps.Maximum = new decimal(new int[] {
			1000000,
			0,
			0,
			0});
			this.numericUpDownMaxIdleSteps.Minimum = new decimal(new int[] {
			1000,
			0,
			0,
			0});
			this.numericUpDownMaxIdleSteps.Name = "numericUpDownMaxIdleSteps";
			this.numericUpDownMaxIdleSteps.Size = new System.Drawing.Size(82, 20);
			this.numericUpDownMaxIdleSteps.TabIndex = 56;
			this.numericUpDownMaxIdleSteps.Value = new decimal(new int[] {
			2000,
			0,
			0,
			0});
			// 
			// labelStuckRadius
			// 
			this.labelStuckRadius.Location = new System.Drawing.Point(12, 51);
			this.labelStuckRadius.Name = "labelStuckRadius";
			this.labelStuckRadius.Size = new System.Drawing.Size(91, 20);
			this.labelStuckRadius.TabIndex = 57;
			this.labelStuckRadius.Text = "Stuck radius:";
			// 
			// numericUpDownStuckRadius
			// 
			this.numericUpDownStuckRadius.Location = new System.Drawing.Point(113, 50);
			this.numericUpDownStuckRadius.Maximum = new decimal(new int[] {
			1000,
			0,
			0,
			0});
			this.numericUpDownStuckRadius.Minimum = new decimal(new int[] {
			100,
			0,
			0,
			0});
			this.numericUpDownStuckRadius.Name = "numericUpDownStuckRadius";
			this.numericUpDownStuckRadius.Size = new System.Drawing.Size(82, 20);
			this.numericUpDownStuckRadius.TabIndex = 58;
			this.numericUpDownStuckRadius.Value = new decimal(new int[] {
			300,
			0,
			0,
			0});
			// 
			// labelMaxStuckSteps
			// 
			this.labelMaxStuckSteps.Location = new System.Drawing.Point(11, 76);
			this.labelMaxStuckSteps.Name = "labelMaxStuckSteps";
			this.labelMaxStuckSteps.Size = new System.Drawing.Size(97, 21);
			this.labelMaxStuckSteps.TabIndex = 59;
			this.labelMaxStuckSteps.Text = "Max. stuck steps:";
			// 
			// numericUpDownMaxStuckSteps
			// 
			this.numericUpDownMaxStuckSteps.Location = new System.Drawing.Point(113, 74);
			this.numericUpDownMaxStuckSteps.Maximum = new decimal(new int[] {
			2000000,
			0,
			0,
			0});
			this.numericUpDownMaxStuckSteps.Minimum = new decimal(new int[] {
			2000,
			0,
			0,
			0});
			this.numericUpDownMaxStuckSteps.Name = "numericUpDownMaxStuckSteps";
			this.numericUpDownMaxStuckSteps.Size = new System.Drawing.Size(82, 20);
			this.numericUpDownMaxStuckSteps.TabIndex = 60;
			this.numericUpDownMaxStuckSteps.Value = new decimal(new int[] {
			10000,
			0,
			0,
			0});
			// 
			// ConfigurationForm
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(604, 561);
			this.Controls.Add(this.numericUpDownMaxStuckSteps);
			this.Controls.Add(this.labelMaxStuckSteps);
			this.Controls.Add(this.numericUpDownStuckRadius);
			this.Controls.Add(this.labelStuckRadius);
			this.Controls.Add(this.numericUpDownMaxIdleSteps);
			this.Controls.Add(this.labelMaxIdleSteps);
			this.Controls.Add(this.checkBoxCloseOnAllBotsQuit);
			this.Controls.Add(this.checkBoxShowStatus);
			this.Controls.Add(this.checkBoxVisualizeWorld);
			this.Controls.Add(this.checkBoxHWVertexProc);
			this.Controls.Add(this.textBoxRobotSize);
			this.Controls.Add(this.labelRobotSize);
			this.Controls.Add(this.buttonBrowsePlugInCfg);
			this.Controls.Add(this.labelPathToPlugInCfg);
			this.Controls.Add(this.textBoxPlugInCfgFilePath);
			this.Controls.Add(this.groupBoxRobotStart);
			this.Controls.Add(this.textBoxSightDistance);
			this.Controls.Add(this.labelSightDistance);
			this.Controls.Add(this.buttonMapImageBrowse);
			this.Controls.Add(this.checkBoxShowOnScreenInfo);
			this.Controls.Add(this.textBoxStep);
			this.Controls.Add(this.labelStep);
			this.Controls.Add(this.textBoxRotationZ);
			this.Controls.Add(this.labelRotationZ);
			this.Controls.Add(this.textBoxRotationY);
			this.Controls.Add(this.labelRotationY);
			this.Controls.Add(this.textBoxRotationX);
			this.Controls.Add(this.labelRotationX);
			this.Controls.Add(this.textBoxImagePath);
			this.Controls.Add(this.labelMapImagePath);
			this.Controls.Add(this.textBoxLightZ);
			this.Controls.Add(this.labelLightZ);
			this.Controls.Add(this.textBoxLightY);
			this.Controls.Add(this.labelLightY);
			this.Controls.Add(this.textBoxLightX);
			this.Controls.Add(this.labelLightX);
			this.Controls.Add(this.textBoxLookAtZ);
			this.Controls.Add(this.labelLookAtZ);
			this.Controls.Add(this.textBoxLookAtY);
			this.Controls.Add(this.labelLookAtY);
			this.Controls.Add(this.textBoxLookAtX);
			this.Controls.Add(this.labelLookAtX);
			this.Controls.Add(this.textBoxCameraZ);
			this.Controls.Add(this.labelCameraZ);
			this.Controls.Add(this.textBoxCameraY);
			this.Controls.Add(this.textBoxCameraX);
			this.Controls.Add(this.labelCameraY);
			this.Controls.Add(this.labelCameraX);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.DoubleBuffered = true;
			this.Name = "ConfigurationForm";
			this.Text = "Configuration Setup";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.ConfigurationFormClosing);
			this.groupBoxRobotStart.ResumeLayout(false);
			this.groupBoxRobotStart.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxIdleSteps)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownStuckRadius)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxStuckSteps)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		
		public int MaxIdleSteps {
			
			get { return (int)numericUpDownMaxIdleSteps.Value; }
		}
		
		public int StuckRadius {
			
			get { return (int)numericUpDownStuckRadius.Value; }
		}
		
		public int MaxStuckSteps {
			
			get { return (int)numericUpDownMaxStuckSteps.Value; }
		}
		
		public void SaveConfig ()
		{
			SaveConfig (m_sConfigFile);
		}
		
		public void SaveConfig (string sFileName)
		{
			try
			{
	        	using (var sw = new StreamWriter(sFileName)) 
	        	{
	        		//sw.WriteLine ("UpdateEvery " + this.textBoxUpdateEvery.Text.ToString ());
	        		sw.WriteLine ("MaxIdleSteps " + numericUpDownMaxIdleSteps.Value);
	        		sw.WriteLine ("StuckRadius " + numericUpDownStuckRadius.Value);
	        		sw.WriteLine ("MaxStuckSteps " + numericUpDownMaxStuckSteps.Value);
	        		sw.WriteLine ("CameraX " + textBoxCameraX.Text);
	        		sw.WriteLine ("CameraY " + textBoxCameraY.Text);
	        		sw.WriteLine ("CameraZ " + textBoxCameraZ.Text);
	        		sw.WriteLine ("LookAtX " + textBoxLookAtX.Text);
	        		sw.WriteLine ("LookAtY " + textBoxLookAtY.Text);
	        		sw.WriteLine ("LookAtZ " + textBoxLookAtZ.Text);
	        		sw.WriteLine ("LightX " + textBoxLightX.Text);
	        		sw.WriteLine ("LightY " + textBoxLightY.Text);
	        		sw.WriteLine ("LightZ " + textBoxLightZ.Text);	        		
	        		sw.WriteLine ("MapImagePath " + textBoxImagePath.Text);	  
	        		sw.WriteLine ("PlugInCfgPath " + textBoxPlugInCfgFilePath.Text);
	        		sw.WriteLine ("RotateX " + textBoxRotationX.Text);
	        		sw.WriteLine ("RotateY " + textBoxRotationY.Text);
	        		sw.WriteLine ("RotateZ " + textBoxRotationZ.Text);
	        		sw.WriteLine ("Step " + textBoxStep.Text);
	        		sw.WriteLine ("ShowOnScreenInfo " + checkBoxShowOnScreenInfo.Checked);
	        		sw.WriteLine ("SightDistance " + textBoxSightDistance.Text);
	        		sw.WriteLine ("RandomRobotStart " + checkBoxRandomRobotStart.Checked);
	        		sw.WriteLine ("RobotStartX " + textBoxRobotStartX.Text);
	        		sw.WriteLine ("RobotStartY " + textBoxRobotStartY.Text);
	        		sw.WriteLine ("RobotSize " + textBoxRobotSize.Text);
	        		sw.WriteLine ("HWVertexProc " + checkBoxHWVertexProc.Checked);
	        		sw.WriteLine ("VisualizeWorld " + checkBoxVisualizeWorld.Checked);
                    sw.WriteLine ("ShowStatusWindow " + checkBoxShowStatus.Checked);
                    sw.WriteLine ("CloseWhenAllBotsQuit " + checkBoxCloseOnAllBotsQuit.Checked);
	        	}
			}
			catch (Exception e)
			{
				MessageBox.Show ("Error saving setup file! " + e.ToString (), "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			
		}		
		
		public void LoadConfig ()
		{
			LoadConfig (m_sConfigFile);
		}
		
		public void LoadConfig (string sFileName)
		{
	        try 
	        {
	        	if (File.Exists (sFileName))
	        	{
		            using (var sr = new StreamReader(sFileName)) 
		            {
		                String line;
		            	string [] saSplit = null;
						const string sDelim = " ";
		            	char [] delim = sDelim.ToCharArray ();
		            	const bool bChecked = true;
		                while ((line = sr.ReadLine()) != null) 
		                {
							saSplit = line.Split (delim, 2);
							switch (saSplit[0]) {
								case "MaxIdleSteps":
									numericUpDownMaxIdleSteps.Value = Convert.ToInt16(saSplit[1]);
									break;
								case "StuckRadius":
									numericUpDownStuckRadius.Value = Convert.ToInt16(saSplit[1]);
									break;
								case "MaxStuckSteps":
									numericUpDownMaxStuckSteps.Value = Convert.ToInt16(saSplit[1]);
									break;
								case "CameraX":
									textBoxCameraX.Text = saSplit[1];
									break;
								case "CameraY":
									textBoxCameraY.Text = saSplit[1];
									break;
								case "CameraZ":
									textBoxCameraZ.Text = saSplit[1];
									break;
								case "LookAtX":
									textBoxLookAtX.Text = saSplit[1];
									break;
								case "LookAtY":
									textBoxLookAtY.Text = saSplit[1];
									break;
								case "LookAtZ":
									textBoxLookAtZ.Text = saSplit[1];
									break;
								case "LightX":
									this.textBoxLightX.Text = saSplit[1];
									break;
								case "LightY":
									textBoxLightY.Text = saSplit[1];
									break;
								case "LightZ":
									textBoxLightZ.Text = saSplit[1];
									break;
								case "MapImagePath":
									textBoxImagePath.Text = saSplit[1];
									break;
								case "PlugInCfgPath":
									textBoxPlugInCfgFilePath.Text = saSplit[1];
									break;
								case "RotateX":
									textBoxRotationX.Text = saSplit[1];
									break;
								case "RotateY":
									textBoxRotationY.Text = saSplit[1];
									break;
								case "RotateZ":
									textBoxRotationZ.Text = saSplit[1];
									break;
								case "Step":
									textBoxStep.Text = saSplit[1];
									break;
								case "RobotSize":
									textBoxRobotSize.Text = saSplit[1];
									break;
								case "ShowOnScreenInfo":
									checkBoxShowOnScreenInfo.Checked = (saSplit[1] == bChecked.ToString());
									break;
								case "SightDistance":
									textBoxSightDistance.Text = saSplit[1];
									break;
								case "RandomRobotStart":
									checkBoxRandomRobotStart.Checked = (saSplit[1] == bChecked.ToString());
									break;
								case "HWVertexProc":
									checkBoxHWVertexProc.Checked = (saSplit[1] == bChecked.ToString());
									break;
								case "VisualizeWorld":
									checkBoxVisualizeWorld.Checked = (saSplit[1] == bChecked.ToString());
									break;
								case "ShowStatusWindow":
									checkBoxShowStatus.Checked = (saSplit[1] == bChecked.ToString());
									break;
								case "CloseWhenAllBotsQuit":
									checkBoxCloseOnAllBotsQuit.Checked = (saSplit[1] == bChecked.ToString());
									break;
								case "RobotStartX":
									textBoxRobotStartX.Text = saSplit[1];
									break;
								case "RobotStartY":
									textBoxRobotStartY.Text = saSplit[1];
									break;
								default:
									Console.WriteLine("Unknown parameter: {0}", saSplit[0]);
									break;
							}
		                }
		            }
					Refresh();
	        	}
	        }
	        catch (Exception) 
	        {
				MessageBox.Show ("Error reading setup file! ", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
	        }
		}		
		
		public double GetSightDistance ()
		{
			return Convert.ToDouble(textBoxSightDistance.Text);
		}
		
		public double GetCameraX ()
		{
			return Convert.ToSingle(textBoxCameraX.Text);
		}
		
		public double GetCameraY ()
		{
			return Convert.ToSingle(textBoxCameraY.Text);
		}		
		
		public double GetCameraZ ()
		{
			return Convert.ToSingle(textBoxCameraZ.Text);
		}		
		
		public double GetLookAtX ()
		{
			return Convert.ToSingle(textBoxLookAtX.Text);
		}		
		
		public double GetLookAtY ()
		{
			return Convert.ToSingle(textBoxLookAtY.Text);
		}		
		
		public double GetLookAtZ ()
		{
			return Convert.ToSingle(textBoxLookAtZ.Text);
		}				
		
		public double GetLightX ()
		{
			return Convert.ToSingle(textBoxLightX.Text);
		}		
		
		public double GetLightY ()
		{
			return Convert.ToSingle(textBoxLightY.Text);
		}		
		
		public double GetLightZ ()
		{
			return Convert.ToSingle(textBoxLightZ.Text);
		}						
		
		public string GetMapImagePath ()
		{
			return (textBoxImagePath.Text);
		}		
		
		public double GetRotationX ()
		{
			return Convert.ToSingle(textBoxRotationX.Text);
		}				
		
		public double GetRotationY ()
		{
			return Convert.ToSingle(textBoxRotationY.Text);
		}				
		
		public double GetRotationZ ()
		{
			return Convert.ToSingle(textBoxRotationZ.Text);
		}						
		
		public double GetStep ()
		{
			return Convert.ToSingle(textBoxStep.Text);
		}								
		
		public double GetRobotSize ()
		{
			return Convert.ToSingle(textBoxRobotSize.Text);
		}		
		
		public bool ShowOnScreenInfo ()
		{
			return (checkBoxShowOnScreenInfo.Checked);
		}
		
		public bool IsRobotStartPosRandom ()
		{
			return (checkBoxRandomRobotStart.Checked);
		}
		
		public bool IsHWVertexProcessingOn ()
		{
			return (checkBoxHWVertexProc.Checked);
		}
		
		public bool IsWorldVisible ()
		{
			return (checkBoxVisualizeWorld.Checked);
		}

        public bool IsStatusWindowVisible()
        {
            return (checkBoxShowStatus.Checked);
        }

        public bool CloseWhenAllBotsQuit()
        {
            return (checkBoxCloseOnAllBotsQuit.Checked);
        }
		
		public int GetRobotStartX ()
		{
			return Convert.ToInt16(textBoxRobotStartX.Text);
		}
		
		public int GetRobotStartY ()
		{
			return Convert.ToInt16(textBoxRobotStartY.Text);
		}		
		
		public string GetPlugInCfgPath ()
		{
			return (this.textBoxPlugInCfgFilePath.Text);
		}
		
		public string GetConfigFilePath ()
		{
			return m_sConfigFile;
		}
		
		void ConfigurationFormClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			LoadConfig();
		}
		
		void ButtonOKClick(object sender, EventArgs e)
		{
			SaveConfig();
			Close();
		}
		
		void ButtonCancelClick(object sender, EventArgs e)
		{
			LoadConfig();
			Close();
		}

		void ButtonMapImageBrowseClick(object sender, EventArgs e)
		{
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
				textBoxImagePath.Text = openFileDialog1.FileName;
		}

        void ButtonRobotsFileBrowseClick(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
				textBoxPlugInCfgFilePath.Text = openFileDialog2.FileName;
        }
		void LabelStepClick(object sender, EventArgs e)
		{
	
		}
		
	}
}
