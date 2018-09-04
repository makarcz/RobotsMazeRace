//#define TRACE
#define SHOWARRAYCALCINFO

using System;
using System.Collections.Generic;
using System.Threading;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
	
namespace RobEnvMK
{
	/// <summary>
	/// This is the main class of my Direct3D application
	/// </summary>
	public class MainClass : Form
	{
		/// <summary>
		/// The rendering device
		/// </summary>
		Device device;
		Bitmap m_WorldBmp;    // bmp source of world map
		public int [,] m_anWorldMap; // internal format of world map
		const String m_NewLine = "\n\r";
		VertexBuffer m_VertexBuffer; // vertex buffer of the world map
		int m_nVertices;
		Vector3 m_Eye;
		double m_EyeX, m_EyeY, m_EyeZ;
		Vector3 m_LookAt;
		Vector3 m_Up;
		double m_RotateX, m_RotateY, m_RotateZ;
		double m_LightX, m_LightY, m_LightZ;
		int m_LowPointColor = Color.Green.ToArgb ();
		int m_HighPointColor = Color.FromArgb (179, 179, 128).ToArgb ();
		const int m_nBmpTreshold = -127;
		double m_fLookAtX;
		double m_fLookAtY;
		double m_fLookAtZ;
		Robot [] m_aRobots;
		int m_nNumOfRobots = 10;
		double m_fStep = 0.6f;
		double m_fRobotSize = 0.6f;
		Random m_Random;
		//private System.Timers.Timer m_Timer = null;
		//double m_fUpdateEvery = 1f; // ms
		public bool m_bScreenSaver;
		public ConfigurationForm m_Config = null;
		string m_sMapImagePath = "/bin/WorldMap.bmp";
		private Microsoft.DirectX.Direct3D.Font m_Font = null;
		private System.Drawing.Font m_SysFont = null;
		public bool m_bTimerLock = false;
		Sphere3D [] m_aRobotSphereShape;// = null;
		Sphere3D [] m_aRobotsNose;// = null;
        Sphere3D m_RobotGoalShape;
		Vector3 [] m_avRobotsNosePos = new Vector3 [10];
		Material m_MaterialScene;
		Material m_MaterialRobotBody;
		Material m_MaterialRobotNose;
		Material m_MaterialSightBall;
        int m_nStep;
		string [] m_saRobotsList;
		ProcessPriorityClass [] m_aRobotsPriorities;
		Process [] m_aProcessList;
		Queue<string> [] m_asqRobotOutput;
		RadarPoint [,] m_aRadar;
		RadarInfo m_RadarInfo;
		int m_nCurrRobotNum;
		bool m_bRender = true; // flag that is unset at each frame beginning,
							   // set to true when robot or scene moves / changes
		double [] m_faSinTbl = new double [Constants.RADAR_RESN];
		double [] m_faCosTbl = new double [Constants.RADAR_RESN];	
		double [] m_faSinRobBody = new double [Constants.BODY_EDGE_SCAN_STEPS];
		double [] m_faCosRobBody = new double [Constants.BODY_EDGE_SCAN_STEPS];
		double [,] m_faRadarSinRays;
		double [,] m_faRadarCosRays;
		//private int m_nTimerLockedCount = 0;
		FormRobotStatus m_RobStat = null;
 	    string m_sInitStatText = string.Empty;
 	    Texture m_Texture;
 	    Texture m_TextureRobotBody;
 	    //private Surface m_TargetSurface = null;
 	    //private Surface m_SourceSurface = null;
 	    bool [] m_baLightSwitches = new bool [3];
 	    string [] m_saRobotBodyTextures;
 	    Texture [] m_aRobotBodyTextures;
        Vector2 m_RobotGoalLocation = new Vector2 (3f, 3f);
        float m_fRobotGoalSize = 3f;
        bool m_bExit;
        bool m_bRun = true;
        bool m_bRestart;
        List<string> m_lsRobRaceResults = new List<string>();
        int m_nPos = 0;
        int [] m_anRobotsIdleSteps;	// each robot has a counter which is incremented
        							// when robot sends no command in the loop frame
        							// and zeroed when command is received.
        							
		struct RobotStuckCounters {
        	public double x;
        	public double y;
        	public int steps;
        	
        }; 
        
        RobotStuckCounters [] m_aRobotsStuckCounters;	// each robot has a counter which is incremented
        												// when robot doesn't change position in the last
        												// step and is zeroed when position changes

        public bool ExitProgram
        {
            get { return m_bExit; }
        }
		
		public MainClass()
		{
            //var resources = new ComponentResourceManager(typeof(MainClass));
			this.ClientSize = new Size(Constants.SCR_RES_X, Constants.SCR_RES_Y);
            Text = global::RobEnvMK.Properties.Resources.MyAppName;
			StartValues ();
		}
		
		public MainClass(bool bScrSaver)
		{
            //var resources = new ComponentResourceManager(typeof(MainClass));
			this.ClientSize = new Size(Constants.SCR_RES_X, Constants.SCR_RES_Y);
            // disable once DoNotCallOverridableMethodsInConstructor
            this.Text = global::RobEnvMK.Properties.Resources.MyAppName;
            //this.Icon = ((Icon)(resources.GetObject("$this.Icon")));
			    
			m_bScreenSaver = bScrSaver;

			StartValues ();
		}	
		
		void InitializeSinCosTable ()
		{
			double angle = 0.0f;
			for (int i = 0; i < Constants.RADAR_RESN; i++, angle += Constants.ANGLE_STEP)
			{
				m_faSinTbl [i] = Math.Sin (angle);
				m_faCosTbl [i] = Math.Cos (angle);
			}
			Trace.WriteLine ("Trigonometric tables initialized.");
		}	
		
		void InitializeRobBodyArrays ()
		{
			const int nStep = Constants.RADAR_RESN / Constants.BODY_EDGE_SCAN_STEPS; // robot's body edge scan resolution step
			double r = m_Config.GetRobotSize () / 2.0f;
			for (int i = 0, j = 0; Constants.BODY_EDGE_SCAN_STEPS > j; i += nStep, j++)
			{
				m_faSinRobBody [j] = r * GetSinFromTbl (i);
				m_faCosRobBody [j] = r * GetCosFromTbl (i);
			}
		}
		
		void InitializeRadarRaysArrays ()
		{
			int i = 0, j = 0;
			double fSightDist = m_Config.GetSightDistance ();
			const double fScanStep = Constants.RADAR_SCAN_STEP; // radar scan step (each map pixel is scanned with this accuracy).
			int nAngleIndex = Constants.RADAR_RESN2;
			int nRadarRaysLen = (int) (fSightDist / fScanStep);
			double step = 0.0f;
			double fSin = 0f;
			double fCos = 0f;
			m_faRadarSinRays = new double [Constants.RADAR_RESN, nRadarRaysLen];
			m_faRadarCosRays = new double [Constants.RADAR_RESN, nRadarRaysLen];
			for (j = 0, nAngleIndex = Constants.RADAR_RESN2; 
			     Constants.RADAR_RESN > j; 
			     j++, nAngleIndex++)
			{
				step = 0.0f;
				if (Constants.RADAR_RESN <= nAngleIndex)
					nAngleIndex = 0;
				fCos = GetCosFromTbl (nAngleIndex);
				fSin = GetSinFromTbl (nAngleIndex);
				for (i = 0; nRadarRaysLen > i; i++, step += fScanStep)
				{
					m_faRadarCosRays [nAngleIndex, i] = step * fCos;
					m_faRadarSinRays [nAngleIndex, i] = step * fSin;
				}
			}			
		}
		
		double GetDistance (double x1, double y1, double x2, double y2)
		{
			double ret = Math.Sqrt(Math.Pow(Math.Abs(x2 - x1), 2) + Math.Pow(Math.Abs(y2 - y1), 2));
			
			return ret;
		}
		
		/*
		public double GetRadarRayValue (int nAngleIndex, int nStep, string sFun)
		{
			if (sFun.StartsWith("Sin", StringComparison.Ordinal))
				return (m_faRadarSinRays[nAngleIndex, nStep]);
			
			return (m_faRadarCosRays [nAngleIndex, nStep]);
		}*/
		
		public double GetRadarRayValueX (int nAngleIndex, int nStep)
		{
			return (m_faRadarCosRays[nAngleIndex, nStep]);
		}
		
		public double GetRadarRayValueY (int nAngleIndex, int nStep)
		{
			return (m_faRadarSinRays[nAngleIndex, nStep]);
		}		
		
		public double GetRobBodyValue (int nStep, string sFun)
		{
			return sFun.StartsWith("Sin", StringComparison.Ordinal) ? (m_faSinRobBody[nStep]) : (m_faCosRobBody[nStep]);
			
		}
		
		public double GetSinFromTbl (int i)
		{
			return (m_faSinTbl [i]);
		}
		
		public double GetCosFromTbl (int i)
		{
			return (m_faCosTbl [i]);
		}		
		
		void StartValues ()
		{
			m_bTimerLock = false;
			m_RotateX = 0f; m_RotateY = 0f; m_RotateZ = 0f;
			m_EyeX = 10.0f; m_EyeY = -20.0f; m_EyeZ = -50.0f;
			m_fLookAtX = 0.0f; m_fLookAtY = 10.0f; m_fLookAtZ = 15.0f;
			m_Eye = new Vector3 ((float)m_EyeX, (float)m_EyeY, (float)m_EyeZ);
			m_LookAt = new Vector3 ((float)(m_EyeX + m_fLookAtX), (float) (m_EyeY+m_fLookAtY), (float) (m_EyeZ+m_fLookAtZ));
			m_Up = new Vector3 (0.0f, 1.0f, 0.0f);
			m_LightX = 43.0f; m_LightY = -80.0f; m_LightZ = 25.0f;			
			m_Random = new Random (unchecked ((int) DateTime.Now.Ticks));
			m_nStep = 0;
			if (m_bScreenSaver) {
				ControlBox = false;
				CenterToParent();
				FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
				BackColor = Color.Black;
				MouseLeave += MainClassMouseLeave;
			}

			InitializeSinCosTable ();
			Setup ();
		}
		
		void InitLightSwitches ()
		{
			m_baLightSwitches [0] = true;
			m_baLightSwitches [1] = true;
			m_baLightSwitches [2] = true;
		}
		
		void Setup ()
		{
			InitLightSwitches ();
	    	if (null != m_RobStat && !m_RobStat.IsDisposed)
	    	{
	    		m_RobStat.Close ();
                while (m_RobStat.Disposing)
                    Thread.Sleep(100);
                m_RobStat = null;
	    	}
			bool bRestartTimer = StopTimer ();
			bool  bReloadWorld = false;

			if (!m_bRestart) {
				if (null != m_Config) {
					m_Config.ShowDialog();
					bReloadWorld = (m_sMapImagePath != m_Config.GetMapImagePath());
					m_Config.Close();
					while (m_Config.Disposing)
						Thread.Sleep(100);
				}
				m_Config = null;
				m_Config = new ConfigurationForm();
			} else
				m_bRestart = false;
			m_EyeX = m_Config.GetCameraX ();
			m_EyeY = m_Config.GetCameraY ();
			m_EyeZ = m_Config.GetCameraZ ();
			m_Eye = new Vector3 ((float)m_EyeX, (float)m_EyeY, (float)m_EyeZ);
			m_fLookAtX = m_Config.GetLookAtX ();
			m_fLookAtY = m_Config.GetLookAtY ();
			m_fLookAtZ = m_Config.GetLookAtZ ();
			m_LookAt = new Vector3 ((float) (m_EyeX + m_fLookAtX), (float) (m_EyeY+m_fLookAtY), (float) (m_EyeZ+m_fLookAtZ));
	    	// = m_Config.GetUpdateEvery ();
			m_LightX = m_Config.GetLightX (); 
			m_LightY = m_Config.GetLightY ();
			m_LightZ = m_Config.GetLightZ (); 
			m_RotateX = m_Config.GetRotationX (); 
			m_RotateY = m_Config.GetRotationY (); 
			m_RotateZ = m_Config.GetRotationZ ();
			m_fStep = m_Config.GetStep ();
			m_fRobotSize = m_Config.GetRobotSize ();
			m_sMapImagePath = m_Config.GetMapImagePath ();	    				
			m_WorldBmp = new Bitmap (m_sMapImagePath, false);
			m_nVertices = CountVerticesNumberFromBmp (m_WorldBmp);
            m_fRobotGoalSize = (float)m_Config.GetRobotSize();
			
			if (RadarInfo.IsRadarFileValid (m_sMapImagePath, Constants.RADAR_RESN, m_Config.GetSightDistance ()))
			{
				RadarInfo RadarInfoObj = RadarInfo.DeserializeRadar (m_sMapImagePath);
				m_RadarInfo = RadarInfoObj;
				m_aRadar = m_RadarInfo.RadarArray;
			}
			else
				m_RadarInfo = null;
			
			int nMapSizeX = m_WorldBmp.Width;
			int nMapSizeY = m_WorldBmp.Height;
			
			m_anWorldMap = new int[nMapSizeX,nMapSizeY];
			
			ConvertMapToInt ();	
			
			int nRobotStartX, nRobotStartY;
			
			if (m_Config.IsRobotStartPosRandom ())
			{
                int nRobSize = (int) m_fRobotSize;
				nRobotStartX = m_Random.Next (nMapSizeX - 2 - nRobSize);
				nRobotStartY = m_Random.Next (nMapSizeY - 2 - nRobSize);
				while (nRobotStartX < 0 || nRobotStartX + nRobSize >= nMapSizeX
				       || nRobotStartY < 0 || nRobotStartY + nRobSize >= nMapSizeY
				       || nRobotStartX - nRobSize < 0 || nRobotStartY - nRobSize < 0
					   || 0 != m_anWorldMap [nRobotStartX, nRobotStartY]
				       || 0 != m_anWorldMap [nRobotStartX + nRobSize, nRobotStartY]
				       || 0 != m_anWorldMap [nRobotStartX, nRobotStartY + nRobSize]
				       || 0 != m_anWorldMap [nRobotStartX + nRobSize, nRobotStartY + nRobSize]
				       || 0 != m_anWorldMap [nRobotStartX - nRobSize, nRobotStartY]
				       || 0 != m_anWorldMap [nRobotStartX, nRobotStartY - nRobSize]
				       || 0 != m_anWorldMap [nRobotStartX - nRobSize, nRobotStartY - nRobSize]
				       || 0 != m_anWorldMap [nRobotStartX - nRobSize, nRobotStartY + nRobSize]
				       || 0 != m_anWorldMap [nRobotStartX + nRobSize, nRobotStartY - nRobSize]
				      )
				{
					nRobotStartX = m_Random.Next (nMapSizeX);
					nRobotStartY = m_Random.Next (nMapSizeY);				
				}
			}
			else
			{
				nRobotStartX = m_Config.GetRobotStartX ();
				nRobotStartY = m_Config.GetRobotStartY ();
				int nCount = 0;
				while (0 != m_anWorldMap [nRobotStartX, nRobotStartY])
				{
					nRobotStartX++;
					nRobotStartY++;
					if (nRobotStartX >= nMapSizeX)
						nRobotStartX = 0;
					if (nRobotStartY >= nMapSizeY)
						nRobotStartY = 0;					
					nCount++;
					if (nCount >= nMapSizeX)
						throw new SystemException ();
				}				
			}
			GetRobotsFileNames ();
			if (null != m_aProcessList && 0 < m_aProcessList.Length)
				KillAllRobots ();
			m_aProcessList = new Process [m_saRobotsList.Length];
			m_asqRobotOutput = new Queue<string>[m_saRobotsList.Length];
            SpawnRobots((float)nRobotStartX, (float)nRobotStartY, m_fStep, m_fRobotSize);
            RaceReportHeader();
			
	    	if (bRestartTimer)
	    		ResetTimer ();
	    	
	    	if (null != m_RobStat && !m_RobStat.IsDisposed)
	    	{
	    		m_RobStat.Close (); // form is disposed when closed
                while (m_RobStat.Disposing)
                    Thread.Sleep (100);
                m_RobStat = null;
	    	}

            if (m_Config.IsStatusWindowVisible())
            {
                m_RobStat = new FormRobotStatus(this.m_aRobots.Length, m_Config.GetPlugInCfgPath ());

                int nBotNum = 0;
                foreach (Robot bot in m_aRobots)
                {
                    m_RobStat.SetPos(bot.GetPosX(), bot.GetPosY(), nBotNum);
                    m_RobStat.SetAngle(bot.GetAngle(), nBotNum);
                    m_RobStat.SetRadar(this.GetRadarNorthAt(bot.GetPosX(), bot.GetPosY(), bot.GetAngle()), nBotNum);
                    nBotNum++;
                }
                m_RobStat.SetStep(m_nStep);

                m_RobStat.Show();
            }
            if (null != m_RobStat && m_Config.IsStatusWindowVisible())
                m_RobStat.SetLightingEnablemendStatus(m_baLightSwitches[0],
                                                      m_baLightSwitches[1],
                                                      m_baLightSwitches[2]);
		}
		
		void RaceReportHeader()
		{
			DateTime dtnow = DateTime.Now;
			m_lsRobRaceResults.Add(global::RobEnvMK.Properties.Resources.Text_ReportDivider);
			m_lsRobRaceResults.Add(global::RobEnvMK.Properties.Resources.Text_RaceResultsReport);
            m_lsRobRaceResults.Add(global::RobEnvMK.Properties.Resources.Text_ReportDivider);
            m_lsRobRaceResults.Add(global::RobEnvMK.Properties.Resources.Text_Date01 + " " + dtnow);
            m_lsRobRaceResults.Add(global::RobEnvMK.Properties.Resources.Text_Rules01);
            m_lsRobRaceResults.Add(" -> " + global::RobEnvMK.Properties.Resources.SetupLbl_MaxIdleSteps + " " + m_Config.MaxIdleSteps);
            m_lsRobRaceResults.Add(" -> " + global::RobEnvMK.Properties.Resources.SetupLbl_MaxStuckSteps + " " + m_Config.MaxStuckSteps);
            m_lsRobRaceResults.Add(" -> " + global::RobEnvMK.Properties.Resources.Text_StuckBotRadius + " " + m_Config.StuckRadius * m_fStep);
            m_lsRobRaceResults.Add(" -> " + global::RobEnvMK.Properties.Resources.Text_HitTheWallOnceAndDie);
            m_lsRobRaceResults.Add(global::RobEnvMK.Properties.Resources.Text_RaceParticipants);
            int i = 0;
            foreach (Robot bot in m_aRobots) {
            	string sBotInfo = " -> " + global::RobEnvMK.Properties.Resources.RobotNumber + i + ", "
            						+ global::RobEnvMK.Properties.Resources.Text_Program01 + " " + bot.Name	+ ", "
            						+ global::RobEnvMK.Properties.Resources.Text_Start01 + " [" + bot.PosX + ", " + bot.PosY + "], "
            						+ global::RobEnvMK.Properties.Resources.Text_Angle01 + " " + bot.GetAngle() + " rad.";
				m_lsRobRaceResults.Add(sBotInfo);
            	i++;
            }
            m_lsRobRaceResults.Add(global::RobEnvMK.Properties.Resources.Text_ReportDivider);
            m_lsRobRaceResults.Add(global::RobEnvMK.Properties.Resources.Text_Results01);
		}
		
		public void RaceReportEvent(string sMsg)
		{
			DateTime dtnow = DateTime.Now;
            m_lsRobRaceResults.Add(dtnow + ": " + sMsg);
		}
		
		public void RaceReportSuccess(int num, string name)
		{
			DateTime dtnow = DateTime.Now;
            m_nPos++;
            m_lsRobRaceResults.Add(dtnow + ": " + global::RobEnvMK.Properties.Resources.RobotNumber + num
                                   + ", " + global::RobEnvMK.Properties.Resources.Text_Name01 + " " + name
                                   + " " + global::RobEnvMK.Properties.Resources.Text_FinishedRaceAtPos + " " + m_nPos + ".");
		}
		
		void RaceReportFooter()
		{
			if (m_nPos <= 0) {
				m_lsRobRaceResults.Add(global::RobEnvMK.Properties.Resources.Text_RaceFailed);
			}
			m_lsRobRaceResults.Add(global::RobEnvMK.Properties.Resources.Text_ReportDivider);
		}

        public void StatusLog(string s)
        {
            if (m_Config.IsStatusWindowVisible() && null != m_RobStat)
            {
                m_RobStat.AppendLog(s);
            }
        }
		
		void KillAllRobots ()
		{
			Trace.WriteLine ("KillAllRobots...");
			StatusLog (global::RobEnvMK.Properties.Resources.Text_KillingAllRobots);
			foreach (Process proc in m_aProcessList) {
				if (null != proc && !proc.HasExited) {
					LogMsg(global::RobEnvMK.Properties.Resources.Text_StoppingProcess + " " + proc.Id + "...");
					proc.EnableRaisingEvents = false;
					proc.Kill ();
					proc.WaitForExit();
					if (proc.HasExited)	{
						LogMsg (global::RobEnvMK.Properties.Resources.Text_Done01);
						proc.Dispose ();	// 'Dispose' calls 'Close'
					}
					else {
						LogMsg (global::RobEnvMK.Properties.Resources.Text_Failed01);
					}
				}
			}
			for (int i = 0; i < m_aProcessList.Length; i++) {
				m_aProcessList[i] = null;
			}
			RaceReportEvent(global::RobEnvMK.Properties.Resources.Text_RaceFailed01);
		}
		
		void SpawnRobots (float x, double y, double step, double size)
		{
			int i = 0;
			m_nNumOfRobots = m_saRobotsList.Length;
			m_aRobots = new Robot [m_nNumOfRobots];
			m_anRobotsIdleSteps = new int[m_nNumOfRobots];
			m_aRobotsStuckCounters = new RobotStuckCounters[m_nNumOfRobots];
			foreach (string s in m_saRobotsList)
			{
				m_aRobots[i] = new Robot (x, y, step, size, this, i, s);
				m_asqRobotOutput[i] = new Queue<string>();
				m_anRobotsIdleSteps[i] = 0;
				m_aRobotsStuckCounters[i].steps = 0;
				m_aRobotsStuckCounters[i].x = x;
				m_aRobotsStuckCounters[i].y = y;
                SpawnRobot(s, i++);
			}
		}
		
		void OnRobotOutputReceived(object sender, DataReceivedEventArgs e)
		{
			var p = (Process)sender;
			int n = -1, i = 0;
			
			try {
				if (!String.IsNullOrEmpty(e.Data)) {
					
					foreach(Process proc in m_aProcessList) {
						if (null != m_aRobots[i]
						    && !m_aRobots[i].isKilled
							&& null != proc
						    && !proc.HasExited
						    && p.Id == proc.Id) {
							
							n = i;
							break;
						}
						i++;
					}
					
					if (0 <= n
					    && null != m_aRobots[n]
					    && !m_aRobots[n].isKilled) {
						
							m_asqRobotOutput[n].Enqueue(e.Data);
					}
				}
			} catch (InvalidOperationException ex) {
				LogMsg(ex.Message);
				LogMsg(ex.StackTrace);
			}
		}
		
		int SpawnRobot (string sName, int i)
		{
            var sep = new string[1];
            sep[0] = ".";
            string[] ext = sName.Split(sep, StringSplitOptions.None);
            var StartInfo = new ProcessStartInfo();
            if (ext.Length > 1 && ext[ext.Length - 1].StartsWith("pl"))
            {
                StartInfo.FileName = "perl.exe";
                StartInfo.Arguments = sName;
                StartInfo.UseShellExecute = false;
                StartInfo.RedirectStandardInput = true;
                StartInfo.RedirectStandardOutput = true;
                StartInfo.CreateNoWindow = true;
            }
            else
            {
                StartInfo.FileName = sName;
                StartInfo.UseShellExecute = false;
                StartInfo.RedirectStandardInput = true;
                StartInfo.RedirectStandardOutput = true;
                StartInfo.CreateNoWindow = true;
            }
            Process p = Process.Start(StartInfo);
            p.EnableRaisingEvents = true;
            p.OutputDataReceived += OnRobotOutputReceived;
			p.StandardInput.AutoFlush = true;
			p.PriorityClass = m_aRobotsPriorities[i];
			
			p.BeginOutputReadLine();
            m_aProcessList[i] = p;

            return (1);
		}

        bool KillRobotRequested(int n)
        {
            if (m_Config.IsStatusWindowVisible() && null != m_RobStat)
            {
                return (m_RobStat.IsToBeKilled (n));
            }

            return (false);
        }

        void ConfirmRobotKill(int n)
        {
            if (m_Config.IsStatusWindowVisible() && null != m_RobStat)
            {
                m_RobStat.KillRequestCompleted(n);
            }
        }

        void LogMsg(String s)
        {
            Trace.WriteLine(s);
            StatusLog(s);
        }

        private void NoRespondingBot()
        {
            if (m_Config.CloseWhenAllBotsQuit())
            {
                LogMsg(global::RobEnvMK.Properties.Resources.Text_NoRespondingBot01);
				StopTimer();
				RaceReportFooter();
				if (null != m_RobStat) {
					m_RobStat.ShowTextLog();
				}
				foreach (string s in m_lsRobRaceResults) {
					LogMsg(s);
				}
                MessageBox.Show(global::RobEnvMK.Properties.Resources.ErrNoRespondingProcess);
                m_bExit = true;
				Close(); // There is no responding process present.
            }
            else
            {
                m_bRestart = true;
                StartValues();
                Trace.WriteLine("RESET.");	
            }
        }
		
		void CommLoop ()
		{
			int i = 0, j = 0;
			bool bShowAllBotsLog = m_Config.IsStatusWindowVisible() ? m_RobStat.ShowAllBotsLog : false;
			int nSelBot = m_Config.IsStatusWindowVisible() ? m_RobStat.SelectedBot : 0;
			
			m_bRender = false;
            if (0 >= m_aProcessList.Length)
            {
                StatusLog(global::RobEnvMK.Properties.Resources.Text_NoBots01);
                Trace.WriteLine("There are no robots...");
                return;
            }
			var naProc2Kill = new int [m_aProcessList.Length];
			int k = 0;
			for (i = 0; i < naProc2Kill.Length; i++)
				naProc2Kill [i] = -1;
            Thread.Sleep(0);
			i = 0;
			foreach (Process p in m_aProcessList)
			{
				if (null != p && !p.HasExited)
				{
					Robot robot = m_aRobots [i];
					if (KillRobotRequested (i))
					{
						naProc2Kill [k++] = i;
						ConfirmRobotKill (i);
						LogMsg (global::RobEnvMK.Properties.Resources.Text_KillReqConf01 + i);
					}
					else
					{
						// Text from bot's StandardOutput is received asynchronously into the array of FIFO string queues.
						// If there is something in the queue, it will be processed, otherwise it is considered an 'idle' step.
						string sInput = (null != m_asqRobotOutput[i] && m_asqRobotOutput[i].Count > 0)
							? m_asqRobotOutput[i].Dequeue() : string.Empty;
					    
					    // calculate the distance from stuck counter reference position
					    // (may need to re-calculate if robot changed position due to 'Move' command)
					    double fDist = GetDistance(m_aRobotsStuckCounters[i].x, m_aRobotsStuckCounters[i].y, robot.PosX, robot.PosY);
					    
					    if (sInput.Length > 0) {
					    	
					    	Trace.WriteLine(global::RobEnvMK.Properties.Resources.RobotNumber + i + ": received input: [" + sInput + "]");
					    	
					    	m_anRobotsIdleSteps[i] = 0;
					    	
						    if (!m_bRender
							    && (sInput.StartsWith("Move", StringComparison.Ordinal) 
					    	        || sInput.StartsWith("Turn", StringComparison.Ordinal)) ) {
					    		
							    m_bRender = true;	// bot moved, need to show it
					    	}
					
							// check if bot feeling suicidal					    	
						    if (sInput.StartsWith("Exit", StringComparison.Ordinal)
						        || sInput.StartsWith("Kill", StringComparison.Ordinal)) {
						    	
								if (!p.HasExited && p.Responding) {
						    		
									LogMsg(global::RobEnvMK.Properties.Resources.Text_RobotProcess01 
									       	+ " " + p.Id + " " + global::RobEnvMK.Properties.Resources.Text_DecidedToQuit01);
									
								} else
									Trace.WriteLine("WARNING: Process " + p + " already exited.");
						    }
						    
						    string sOutput = robot.ProcessCommand (sInput);
						    
						    if (!robot.isKilled) {
						    	
						    	if (m_bRender) {
						    		// need to re-calculate distance from stuck counter reference point only
						    		// in case the robot could have changed the position (m_bRender == true)
							    	fDist = GetDistance(m_aRobotsStuckCounters[i].x, m_aRobotsStuckCounters[i].y, 
						    		                    robot.PosX, robot.PosY);
						    	}
							    
							    if (bShowAllBotsLog || nSelBot == i) {
						    		
						    		LogMsg(global::RobEnvMK.Properties.Resources.RobotNumber + i + ":" + m_saRobotsList[i] + ": " + sInput
						    		       + "|S[" + m_aRobotsStuckCounters[i].steps + "], D[" + fDist + "]");
							    }
						    } else {
						    	
								LogMsg(global::RobEnvMK.Properties.Resources.RobotNumber + i
						    	       + " " + global::RobEnvMK.Properties.Resources.Text_HasBeenKilled01
						    	       + ", " + global::RobEnvMK.Properties.Resources.Text_Process01 + " " + p.Id
						    	       + " " + global::RobEnvMK.Properties.Resources.Text_WillBeTerminated01);
						    	
								naProc2Kill[k++] = i;						    	
						    }
  
						    if (null != sOutput && 0 < sOutput.Length) {

						    	// send the response from processed command back to robot process
								p.StandardInput.WriteLine(sOutput);						    	
						    }

					    } else {
					    	
					    	m_anRobotsIdleSteps[i]++;
					    	
					    	if (bShowAllBotsLog || nSelBot == i) {
					    		
					    		LogMsg(global::RobEnvMK.Properties.Resources.RobotNumber + i + ":" + m_saRobotsList[i]
					    		       + ": " + global::RobEnvMK.Properties.Resources.Text_EmptyOutputQueue01
					    		       + "|[" + m_anRobotsIdleSteps[i] + "]");
					    	}
					    	
					    	if (m_anRobotsIdleSteps[i] > m_Config.MaxIdleSteps) {
					    		
					    		LogMsg(global::RobEnvMK.Properties.Resources.RobotNumber + i + ":" + m_saRobotsList[i]
					    		       + ": " + global::RobEnvMK.Properties.Resources.Text_TooManyIdleSteps01
					    		       + " " + global::RobEnvMK.Properties.Resources.Text_RobotWillBeKilled01);
					    		RaceReportEvent(global::RobEnvMK.Properties.Resources.RobotNumber + i + ":" + m_saRobotsList[i]
					    		                + ": " + global::RobEnvMK.Properties.Resources.Text_Timeout01);
					    		naProc2Kill[k++] = i;
					    	}
					    }
					    
					    // If distance between current position and previously recorded position is
						// less than m_Config.StuckRadius * m_fStep, we increment the
						// robot stuck counter and if threshold of maximum allowed steps to be walked
						// within that radius is crossed, robot is presumed stuck / looping and is killed.
						
						if (!robot.isKilled && 0 == k) { // if not already killed or flagged to be killed, check stuck counter
							
							// is bot position within StuckRadius from reference position? (since counter was reset)
							if (fDist < m_Config.StuckRadius * m_fStep) {
						    	
						    	m_aRobotsStuckCounters[i].steps++;	// yes, increment counter
						    	
						    	// does counter exceed maximum allowed steps within the StuckRadius * m_fStep ?
						    	if (m_aRobotsStuckCounters[i].steps > m_Config.MaxStuckSteps) {
					    		
						    		// yes, stuck counter exceeded allowed maximum, robot will be terminated (stuck or looping)
						    		LogMsg(global::RobEnvMK.Properties.Resources.RobotNumber + i + ":" + m_saRobotsList[i]
						    		       + ": " + global::RobEnvMK.Properties.Resources.Text_RobotStuck01);
						    		RaceReportEvent(global::RobEnvMK.Properties.Resources.RobotNumber + i + ":" + m_saRobotsList[i]
						    		                + ": " + global::RobEnvMK.Properties.Resources.Text_Stuck01 + " ["
						    		                + m_aRobotsStuckCounters[i].x + " , " + m_aRobotsStuckCounters[i].y + "] - [" 
						    		                + robot.PosX + " , " + robot.PosY + "] : [" + fDist + "]");
						    		naProc2Kill[k++] = i;
					    		}
						    	
						    } else {
								
								// Current position is outside the StuckRadius * m_fStep, assumed not stuck nor looping.
								// (although it could still be, but outside predefined limits it is ignored)
								// We reset the stuck robot counter and record new reference position coordinates.
								// Counter is reset from this new reference position.
						    	
						    	m_aRobotsStuckCounters[i].steps = 0;
						    	m_aRobotsStuckCounters[i].x = robot.PosX;
						    	m_aRobotsStuckCounters[i].y = robot.PosY;
						    	if (bShowAllBotsLog || nSelBot == i) {
						    		
						    		LogMsg(global::RobEnvMK.Properties.Resources.RobotNumber + i + ":" + m_saRobotsList[i]
						    		       + ": " + global::RobEnvMK.Properties.Resources.Text_StuckCounter01
						    		       + " " + global::RobEnvMK.Properties.Resources.Text_Reset01);
						    	}
						    }
						}
					}
					j++;
				}
				else {
					if (bShowAllBotsLog || nSelBot == i) {
						
                    	LogMsg(global::RobEnvMK.Properties.Resources.RobotNumber + i
						       + " " + global::RobEnvMK.Properties.Resources.Text_IsNotResponding01);
					}
				}
				i++;
                Thread.Sleep(0);
			}
            if (0 == j)
                NoRespondingBot();
			if (0 < k) // there are some robots flagged for killing
			{
				foreach (int bot_num in naProc2Kill)
				{
					if (bot_num < 0) continue;
					Process p2k = m_aProcessList[bot_num];
					if (null != p2k)
					{
                        Robot robot = m_aRobots [bot_num];
                        string sResponse = (m_asqRobotOutput[bot_num].Count > 0) 
                        					? m_asqRobotOutput[bot_num].Dequeue() : string.Empty;
                        if (sResponse != "Reset")
                        {
                        	RaceReportEvent(global::RobEnvMK.Properties.Resources.RobotNumber + bot_num
                        	                + " " + global::RobEnvMK.Properties.Resources.Text_KilledAt01
                        	                + " " + global::RobEnvMK.Properties.Resources.Text_Step01 + " " + m_nStep);
                        	LogMsg(global::RobEnvMK.Properties.Resources.Text_Exiting01
                        	       + " " + global::RobEnvMK.Properties.Resources.RobotNumber + bot_num
                        	       + ", " + global::RobEnvMK.Properties.Resources.Text_Process01
                        	       + " " + global::RobEnvMK.Properties.Resources.Text_Id01 + " " + p2k.Id + "...");
							if (!p2k.HasExited) {
                        		
								p2k.Kill();
								p2k.WaitForExit();
							}
							if (p2k.HasExited) {
                        		
								p2k.Dispose();
								LogMsg(global::RobEnvMK.Properties.Resources.Text_Done01);
								
                        	} else {
                        		
								LogMsg(global::RobEnvMK.Properties.Resources.Text_Failed01);
                        	}
                        	m_aProcessList[bot_num] = null;
                        }
                        else
                            robot.ProcessCommand(global::RobEnvMK.Properties.Resources.Text_Reset02);
					}
                    Thread.Sleep(0);
				}
			}
			Trace.WriteLine ("CommLoop:END");
			return;
		}		
		
		string [] GetRobotsFileNames ()
		{
			string sCfgFile = m_Config.GetPlugInCfgPath ();
			var saRet = new string [255];
			var saPrior = new string [255];
			var saBodyTexture = new string [255];
			int i = 0;
			
			if (File.Exists (sCfgFile))
			{
	            using (var sr = new StreamReader(sCfgFile)) 
	            {
	                String line;
	            	string [] saSplit = null;
					const string sDelim = " ";
	            	char [] delim = sDelim.ToCharArray ();
	                while ((line = sr.ReadLine()) != null) 
	                {		
	                	saSplit = line.Split (delim, 3);
                        if (null != saSplit[0] && File.Exists(saSplit[0]))
                        {
                            saRet[i] = saSplit[0];
                            if (2 < saSplit.Length && null != saSplit[2] && File.Exists(saSplit[2]))
                                saBodyTexture[i] = saSplit[2];
                            else
                                saBodyTexture[i] = "./metal5a.bmp";
                            if (1 < saSplit.Length)
                                saPrior[i] = saSplit[1];
                            else
                                saPrior[i] = ProcessPriorityClass.Normal.ToString();
                            i++;
                        }
                        else
                        {
                            MessageBox.Show(global::RobEnvMK.Properties.Resources.WarnRobotImageNotExist + " - " + saSplit[0]);
                        }
	                }
	            }
			}
			else
			{
                MessageBox.Show(global::RobEnvMK.Properties.Resources.ErrConfigFileNotExist + " - " + sCfgFile);
				Application.Exit ();
			}
			
			m_saRobotsList = new string [i];
			m_aRobotsPriorities = new ProcessPriorityClass [i];
			m_saRobotBodyTextures = new string [i];
			for (int j = 0; j < i; j++)
			{
				m_saRobotsList [j] = saRet [j];
				m_aRobotsPriorities [j] = GetProcessPriorityFromName (saPrior [j]);
				m_saRobotBodyTextures [j] = saBodyTexture [j];
			}
			
			return (m_saRobotsList);
		}		
		
		ProcessPriorityClass GetProcessPriorityFromName (string sPriority)
		{
			var ret = (ProcessPriorityClass)Enum.Parse (typeof (ProcessPriorityClass), sPriority);
			
			return (ret);
		}
		
		bool StopTimer ()
		{
			const bool bRet = false; // timer stopped?
			
			return bRet;
		}
		
		void ResetTimer ()
		{
            //m_nOnTimerCount = 0;
            m_bTimerLock = false;
		}
		
		public bool InitializeGraphics()
		{
			try 
			{
				// Now let's setup the Direct3D stuff
				var presentParams = new PresentParameters();
				presentParams.Windowed   = true;
				presentParams.SwapEffect = SwapEffect.Discard;
        		presentParams.EnableAutoDepthStencil = true;
        		// 2. And the stencil format.
        		presentParams.AutoDepthStencilFormat = DepthFormat.D16;
        		Caps caps = Manager.GetDeviceCaps (0, DeviceType.Hardware);
				
				// Create the device
				if (m_Config.IsWorldVisible ())
				{
					device = new Device(0, DeviceType.Hardware, this, 
					                    ((m_Config.IsHWVertexProcessingOn () && caps.DeviceCaps.SupportsHardwareTransformAndLight) ? CreateFlags.HardwareVertexProcessing : CreateFlags.SoftwareVertexProcessing),
					                    presentParams);
					
					// Setup the event handlers for the device
					device.RenderState.ZBufferEnable = true;
					device.DeviceLost += InvalidateDeviceObjects;
					device.DeviceReset += RestoreDeviceObjects;
					device.Disposing += DeleteDeviceObjects;
					device.DeviceResizing += EnvironmentResizing;
					//device.DeviceCreated +=  new System.EventHandler(this.OnCreateDevice);
					SetCullingMode (device);
					
					this.OnCreateDevice(device, null);
                    this.CreateDevice(device);
					
					SetMaterials ();
					SetLights ();
				
					m_SysFont = new System.Drawing.Font ("Courier", this.ClientSize.Width/40 /*16*/);
					m_Font = new Microsoft.DirectX.Direct3D.Font (device, m_SysFont);

					int i = 0;
					m_aRobotSphereShape = new Sphere3D [m_aRobots.Length];
					m_avRobotsNosePos = new Vector3 [m_aRobots.Length];
					m_aRobotsNose = new Sphere3D [m_aRobots.Length];
                    double fHalfSize = m_Config.GetRobotSize () / 2.0f;
                    m_RobotGoalShape = new Sphere3D(device, new Vector3(m_RobotGoalLocation.X, m_RobotGoalLocation.Y, -m_fRobotGoalSize), m_fRobotGoalSize, 100, 100);
					foreach (Robot robot in m_aRobots)
					{
						m_aRobotSphereShape [i] = new Sphere3D (device, new Vector3 ((float) robot.GetPosX (), (float) robot.GetPosY (), (float) -fHalfSize), (float) fHalfSize, 100, 100);
						int nAngleIndex = (int) Math.Round (robot.GetAngle () * Constants.RAD2DEG);
						while (0 > nAngleIndex)
							nAngleIndex += Constants.RADAR_RESN;
						while (Constants.RADAR_RESN <= nAngleIndex)
							nAngleIndex -= Constants.RADAR_RESN;
						double x = fHalfSize * GetCosFromTbl (nAngleIndex);
						double y = fHalfSize * GetSinFromTbl (nAngleIndex);
						m_avRobotsNosePos [i] = new Vector3 ((float) (robot.GetPosX () + x), (float) (robot.GetPosY () + y), (float) -fHalfSize);
						m_aRobotsNose [i] = new Sphere3D (device, m_avRobotsNosePos [i], (float) (m_Config.GetRobotSize ()/3.0f), 25, 25);
						i++;
					}
				}
				Thread.Sleep (1000);
				ResetTimer ();
			
				return true;
			} 
			catch (DirectXException)
			{
				//MessageBox.Show (e.Message +  m_NewLine + e.ErrorString + m_NewLine + e.StackTrace, "Exception!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
		}
		
		void SetMaterials ()
		{
			m_MaterialScene = new Material ();
			m_MaterialScene.Diffuse = Color.DarkGray;//Color.White;
			m_MaterialScene.Ambient = Color.DimGray;
			//m_MaterialScene.Specular = Color.DarkGray;
			//m_MaterialScene.SpecularSharpness = 10.0f;
			device.Material = m_MaterialScene;
			
			m_MaterialRobotBody = new Material ();
			m_MaterialRobotBody.Diffuse = Color.Gray;//Color.LightPink;
			m_MaterialRobotBody.Ambient = Color.DarkRed;
			//m_MaterialRobotBody.Emissive = Color.DarkRed;
			m_MaterialRobotBody.Specular = Color.Gray;
			m_MaterialRobotBody.SpecularSharpness = 50.0f;
			
			m_MaterialRobotNose = new Material ();
			m_MaterialRobotNose.Diffuse = Color.DimGray;
			m_MaterialRobotNose.Ambient = Color.DimGray;
			m_MaterialRobotNose.Emissive = Color.DimGray;
			
			m_MaterialSightBall = new Material ();		
			m_MaterialSightBall.Diffuse = Color.Gray;
			m_MaterialSightBall.Ambient = Color.DarkRed;
			//m_MaterialSightBall.Emissive = Color.Red;
			m_MaterialSightBall.Specular = Color.White;
			m_MaterialSightBall.SpecularSharpness = 50.0f;			
		}
		
		void EnableLights ()
		{
			device.Lights[0].Enabled = this.m_baLightSwitches [0];
			device.Lights[1].Enabled = this.m_baLightSwitches [1];
			device.Lights[2].Enabled = this.m_baLightSwitches [2];
		}
		
		void SetLights ()
		{
            if (null != m_RobStat)
            {
                device.Lights[0].Enabled = m_RobStat.LightSwitchX;
                device.Lights[1].Enabled = m_RobStat.LightSwitchY;
                device.Lights[2].Enabled = m_RobStat.LightSwitchZ;
            }
            else
            {
                device.Lights[0].Enabled = this.m_baLightSwitches[0];
                device.Lights[1].Enabled = this.m_baLightSwitches[1];
                device.Lights[2].Enabled = this.m_baLightSwitches[2];
            }
			device.Lights[0].Type = LightType.Directional;
			device.Lights[0].Position = new Vector3 (1000f, 1000f, 1000f);
			device.Lights[0].Diffuse = System.Drawing.Color.DarkGray;
			device.Lights[0].Direction = new Vector3 ((float) -m_aRobots [0].GetPosX (), (float) -m_aRobots [0].GetPosY (), (float) m_LightZ);
			//device.Lights[0].Range = 2000.0f;
			//device.Lights[0].Ambient = Color.DarkGray;
			//device.Lights[0].Deferred = false;
			device.Lights[0].Specular = Color.DarkGray;
			device.Lights[0].InnerConeAngle = 3.14f;
			device.Lights[0].OuterConeAngle = 3.14f;
			//device.Lights[0].Commit ();		
            device.Lights[0].Update();
			
			device.Lights[1].Type = LightType.Directional;
			device.Lights[1].Position = m_Eye;
			device.Lights[1].Diffuse = System.Drawing.Color.DarkGray;
			device.Lights[1].Direction = new Vector3 ((float) -m_aRobots [0].GetPosX (), (float) -m_aRobots [0].GetPosY (), (float) m_LightZ);
			//device.Lights[1].Range = 1000.0f;
			//device.Lights[1].Ambient = Color.White;
			//device.Lights[1].Deferred = false;
			device.Lights[1].Specular = Color.DarkGray;
			device.Lights[1].InnerConeAngle = 3.14f;
			device.Lights[1].OuterConeAngle = 3.14f;			
			//device.Lights[1].Falloff = 1.0f;
			//device.Lights[1].Commit ();								
            device.Lights[1].Update ();
			
			device.Lights[2].Type = LightType.Directional;
			device.Lights[2].Position = new Vector3 (0f, 0f, 1000f);
			device.Lights[2].Diffuse = System.Drawing.Color.DarkGray;
			device.Lights[2].Direction =  new Vector3 ((float) m_LightX, (float) m_LightY, (float) m_LightZ);
			//device.Lights[2].Range = 2000.0f;
			//device.Lights[2].Ambient = Color.DarkGray;
			//device.Lights[2].Deferred = false;
			device.Lights[2].Specular = Color.DarkGray;
			device.Lights[2].InnerConeAngle = 3.14f;
			device.Lights[2].OuterConeAngle = 3.14f;			
			//device.Lights[2].Commit ();
            device.Lights[2].Update();
		}
		
		void SetCullingMode (Device dev)
		{
			dev.RenderState.CullMode = Cull.None;
		}
		
		int CountVerticesNumberFromBmp (Bitmap bmp)
		{
			int nRet = bmp.Height * bmp.Width * 6;
			
			Trace.WriteLine ("BMP resolution: " + bmp.Width.ToString () + " x " + bmp.Height.ToString ());
			Trace.WriteLine ("Number of vertices: " + nRet.ToString ());
			
			return (nRet);
		}
		
		/// <summary>
		/// ConvertMapToInt ()
		/// Convert bitmap file to internal world map representation.
		/// Cuurently internat format is represented by 2D integer array
		/// with 0/1 binary values.
		/// </summary>
		void ConvertMapToInt ()
		{
			int i = 0, j = 0;
			Trace.WriteLine ("Converting BMP to map array...");
			for (i = 0; i < m_WorldBmp.Width; i++)
			{
				for (j = 0; j < m_WorldBmp.Height; j++)
				{
					m_anWorldMap [i, j] = 1;	// default - wall
					if (m_nBmpTreshold <= (m_WorldBmp.GetPixel (i, j)).ToArgb ())
					{
						m_anWorldMap [i, j] = 0;
					}
				}
				Thread.Sleep (0);
			}
			Trace.WriteLine ("Done.");
			Trace.WriteLine ("Initializing Radar...");
			if (null == m_RadarInfo)
			{
				m_aRadar = new RadarPoint [m_WorldBmp.Width, m_WorldBmp.Height];
				#if (SHOWARRAYCALCINFO)
				Console.WriteLine ("Calculating radar array, size: " + m_WorldBmp.Width + " X " + m_WorldBmp.Height);
				#endif
				InitializeRadarRaysArrays ();
				InitializeRobBodyArrays ();
				var radarInit = new FormRadarInit (m_WorldBmp, m_anWorldMap, this);
				//radarInit.Show ();
				radarInit.ShowDialog ();
				m_aRadar = radarInit.RadarArray;
				SaveRadar ();
			}
			Trace.WriteLine ("Done.");
		}
		
		string GetRadarFilePath ()
		{
			string sRadarFile = RadarInfo.GetRadarFilePath (m_sMapImagePath);
			
			return (sRadarFile);
		}
		
		void SaveRadar ()
		{
			string sFileName = GetRadarFilePath ();
			
			try
			{
				m_RadarInfo = new RadarInfo (m_aRadar, m_sMapImagePath, Constants.RADAR_RESN, m_Config.GetSightDistance ());
				m_RadarInfo.SerializeRadar ();
			}
			catch(Exception e)
			{
				System.Console.WriteLine ("SaveRadar: Unable to save file " + sFileName + " : " + e.ToString ());
				File.Delete (sFileName);
			}			
		}
		
		int NormalizeRadarIndex (int index)
		{
			int nIndex = index;
			
            while (0 > nIndex)
                nIndex += Constants.RADAR_RESN;
            while (Constants.RADAR_RESN <= nIndex)
                nIndex -= Constants.RADAR_RESN;			
            
            return (nIndex);
		}

        public MkString m_sRadarDetailed = null;

        /*
         *  Return the text string with radar readings around the bot
         *  separated by spaces (360 values), starting at the point right behind
         *  the bot in 1 deg increments in the counterclockwise direction.
         *  The first string representation of type double is a radar reading
         *  behind the bot.
         *  The 90th reading is on the bot's exact right side. 
         *  The 180th is in directly in front of the bot.
         *  The 270th exactly to the left etc.
         */
		public string GetRadarAt (double x, double y, double angle)
		{
            int nx = (int) x;
            int ny = (int) y;
			int na = (int) Math.Round (angle * Constants.RAD2DEG);
            double fAngle = angle;
			var sRet = new MkString ();
            m_sRadarDetailed = new MkString();
			
			na = NormalizeRadarIndex (na);
			for (int i = 0, j = na; Constants.RADAR_RESN > i; i++, j++, fAngle += Constants.ANGLE_STEP)
			{
                if (Constants.RADAR_RESN <= j)
                {
                    j = 0;
                    fAngle = 0f;
                }
				sRet.Append (((m_aRadar [nx, ny]).GetReadAtStep (j)).ToString ());
                m_sRadarDetailed.Append ("[" + i + ":" + fAngle + "]" + ((m_aRadar[nx, ny]).GetReadAtStep(j)));
                if (Constants.RADAR_RESN - 1 > i)
                {
                    sRet.AddSpace();
                    m_sRadarDetailed.AddSpace();
                }
			}
			
			return (sRet.Buffer);
		}
		
		public string GetPartialRadarAt (double x, double y, double angle, int begin, int steps)
		{
            int nx = (int) x;
            int ny = (int) y;
			int na = (int) Math.Round (angle * Constants.RAD2DEG);
            double fAngle = angle;
			var sRet = new MkString ();
            m_sRadarDetailed = new MkString();
			
            if (steps >= Constants.RADAR_RESN) {
            	steps = Constants.RADAR_RESN;
            } else if (steps < 1) {
            	steps = 1;
            }
            if (begin + steps >= Constants.RADAR_RESN) {
            	begin = Constants.RADAR_RESN - steps;
            } else if (begin < 0) {
            	begin = 0;
            }
            na = (int) Math.Round (angle * Constants.RAD2DEG) + begin;
			na = NormalizeRadarIndex (na);
			fAngle += begin * Constants.ANGLE_STEP;
			for (int i = begin, j = na; 0 < steps; i++, j++, fAngle += Constants.ANGLE_STEP, steps--)
			{
                if (Constants.RADAR_RESN <= j)
                {
                    j = 0;
                    fAngle = 0f;
                }
				sRet.Append (((m_aRadar [nx, ny]).GetReadAtStep (j)).ToString ());
                m_sRadarDetailed.Append ("[" + i + ":" + fAngle + "]" + ((m_aRadar[nx, ny]).GetReadAtStep(j)));
                if (Constants.RADAR_RESN - 1 > i)
                {
                    sRet.AddSpace();
                    m_sRadarDetailed.AddSpace();
                }
			}
			
			return (sRet.Buffer);
		}		

        /*
         *  Get the reading of the radar at x,y coordinates in front of the
         *  bot that is oriented at provided in 3rd argument angle.
         */
        public double GetRadarNorthAt(double x, double y, double angle)
        {
            int nx = (int)x;
            int ny = (int)y;
            int na = (int)Math.Round(angle * Constants.RAD2DEG);
            double ret = 0.0;

            na = NormalizeRadarIndex(na+Constants.RADAR_RESN2-1);

            ret = m_aRadar[nx, ny].GetReadAtStep(na);

            return (ret);
        }

		public void OnCreateDevice(object sender, EventArgs e)
		{
			var dev = (Device)sender;
			// Load labirynth map
			if (File.Exists(m_sMapImagePath))
				m_Texture = TextureLoader.FromFile(dev, m_sMapImagePath);
			else
			{
				MessageBox.Show (global::RobEnvMK.Properties.Resources.ErrMapImageNotExist + " - " + m_sMapImagePath);
				var DefaultBmp = new Bitmap (128, 128);
				m_Texture = new Texture (dev, DefaultBmp, Usage.DepthStencil, Pool.Managed);	
			}
			if (null != m_saRobotBodyTextures)
			{
				m_aRobotBodyTextures = new Texture [m_saRobotBodyTextures.Length];
				int i = 0;
				foreach (string s in m_saRobotBodyTextures) {
					m_aRobotBodyTextures[i] = TextureLoader.FromFile(dev, s);
					i++;
				}
			}
			// Load default texture
			if (File.Exists ("./metal5a.bmp"))
				m_TextureRobotBody = TextureLoader.FromFile (dev, "./metal5a.bmp");
			else
			{
				var DefaultBmp = new Bitmap (128, 128);
				m_TextureRobotBody = new Texture (dev, DefaultBmp, Usage.DepthStencil, Pool.Managed);
			}

			m_VertexBuffer = new VertexBuffer(typeof(CustomVertex.PositionNormalTextured), 4, dev, Usage.WriteOnly, VertexFormats.Position | VertexFormats.Texture0 /*CustomVertex.PositionNormalTextured.Format*/, Pool.Managed);
			m_VertexBuffer.Created += OnCreateVertexBuffer2;
			OnCreateVertexBuffer2(m_VertexBuffer, null);
			
		}

        public void CreateDevice(Device device)
        {
            Device dev = device;
            // Load labirynth map
            if (File.Exists(m_sMapImagePath))
                m_Texture = TextureLoader.FromFile(dev, m_sMapImagePath);
            else
            {
                MessageBox.Show(global::RobEnvMK.Properties.Resources.ErrMapImageNotExist + " - " + m_sMapImagePath);
                var DefaultBmp = new Bitmap(128, 128);
                m_Texture = new Texture(dev, DefaultBmp, Usage.DepthStencil, Pool.Managed);
            }
            if (null != m_saRobotBodyTextures)
            {
				m_aRobotBodyTextures = new Texture[m_saRobotBodyTextures.Length];
                int i = 0;
				foreach (string s in m_saRobotBodyTextures) {
					m_aRobotBodyTextures[i] = TextureLoader.FromFile(dev, s);
					i++;
				}
            }
            // Load default texture
            if (File.Exists("./metal5a.bmp"))
                m_TextureRobotBody = TextureLoader.FromFile(dev, "./metal5a.bmp");
            else
            {
                var DefaultBmp = new Bitmap(128, 128);
                m_TextureRobotBody = new Texture(dev, DefaultBmp, Usage.DepthStencil, Pool.Managed);
            }

            m_VertexBuffer = new VertexBuffer(typeof(CustomVertex.PositionNormalTextured), 4, dev, Usage.WriteOnly, VertexFormats.Position | VertexFormats.Texture0 /*CustomVertex.PositionNormalTextured.Format*/, Pool.Managed);
			m_VertexBuffer.Created += OnCreateVertexBuffer2;
			OnCreateVertexBuffer2(m_VertexBuffer, null);

        }
		
		// This is textured rectangle implementation of labirynth visualisation
		public void OnCreateVertexBuffer2(object sender, EventArgs e)
		{
			var vb = (VertexBuffer)sender;
			var verts = (CustomVertex.PositionNormalTextured[]) vb.Lock (0, 0);
			
			// Define rectangle where world map (labirynth) will be displayed
            verts[0].Position = new Vector3(0f, 0f, 0f);
            verts[1].Position = new Vector3((float)m_WorldBmp.Height, 0f, 0f);
			verts [2].Position  = new Vector3 (0f, (float)m_WorldBmp.Width, 0f);
			verts [3].Position = new Vector3 ((float)m_WorldBmp.Height, (float)m_WorldBmp.Width, 0f);
			
			// Define normal vectors for lights
            verts[0].Normal = new Vector3(1f, 1f, -1f);
			verts [1].Normal = new Vector3 (1f, 1f, -1f);
			verts [2].Normal = new Vector3 (1f, 1f, -1f);
			verts [3].Normal = new Vector3 (1f, 1f, -1f);
			
			// Map world map BMP (texture) to the rectangle.
			verts [0].Tu = 0f;
			verts [0].Tv = 0f;
			
			verts [1].Tu = 1f;
			verts [1].Tv = 0f;
			
			verts [2].Tu = 0f;
			verts [2].Tv = 1f;
			
			verts [3].Tu = 1f;
			verts [3].Tv = 1f;
			
			vb.Unlock ();
		}
					
		// This is point by point triangle definition for labirynth representation
		public void OnCreateVertexBuffer(object sender, EventArgs e)
		{
			int i = 0;
			int width, height;	
			double x = 0.0f, y = 0.0f;

			Bitmap bmp = m_WorldBmp;
			var vb = (VertexBuffer)sender;
			var verts = (CustomVertex.PositionNormalColored[]) vb.Lock (0, 0);


			for (i = 0, width = 0, x = 0f; width < bmp.Width-1; width++, x += 1.0f)
			{
				for (height = 0, y = 0f; height < bmp.Height-1; height++, i+=6, y += 1.0f)
				{
					int nP0 = m_anWorldMap [width,   height];
					int nP1 = m_anWorldMap [width+1, height];
					int nP2 = m_anWorldMap [width,   height+1];
					int nP3 = m_anWorldMap [width+1, height+1];
					
					verts[i].X   = (float) x;
					verts[i + 1].X = (float) (x + 1.0f);
					verts[i + 2].X = (float) x;
					verts[i + 3].X = (float) x;
					verts[i + 4].X = (float) (x + 1.0f);
					verts[i + 5].X = (float) (x + 1.0f);
					
					const double upperPoint = -3.0f;
					const double lowerPoint = 0.0f;
					
					verts[i].Z = (float) ((1 == nP0) ? upperPoint : lowerPoint);
					verts[i].Color = (0 == nP0) ? m_LowPointColor : m_HighPointColor;
					verts[i+1].Z = (float) ((1 == nP1) ? upperPoint : lowerPoint);
					verts[i+1].Color = (0 == nP1) ? m_LowPointColor : m_HighPointColor;
					verts[i+2].Z = (float) ((1 == nP2) ? upperPoint : lowerPoint);
					verts[i+2].Color = (0 == nP2) ? m_LowPointColor : m_HighPointColor;
					verts[i+3].Z = (float) ((1 == nP2) ? upperPoint : lowerPoint);
					verts[i+3].Color = (0 == nP2) ? m_LowPointColor : m_HighPointColor;
					verts[i+4].Z = (float) ((1 == nP1) ? upperPoint : lowerPoint);
					verts[i+4].Color = (0 == nP1) ? m_LowPointColor : m_HighPointColor;
					verts[i+5].Z = (float) ((1 == nP3) ? upperPoint : lowerPoint);
					verts[i+5].Color = (0 == nP3) ? m_LowPointColor : m_HighPointColor;						
					
					verts[i].Y   = (float) y;
					verts[i + 1].Y = (float) y;
					verts[i + 2].Y = (float) (y + 1.0f);
					verts[i + 3].Y = (float) (y + 1.0f);
					verts[i + 4].Y = (float) y;
					verts[i + 5].Y = (float) (y + 1.0f);

					var normalVect = new Vector3 (1f, 1f, -1f);
					
					verts[i].Nx   = normalVect.X; verts[i].Ny   = normalVect.Y;	verts[i].Nz   = normalVect.Z;
					verts[i+1].Nx = normalVect.X; verts[i+1].Ny = normalVect.Y;	verts[i+1].Nz = normalVect.Z;
					verts[i+2].Nx = normalVect.X; verts[i+2].Ny = normalVect.Y;	verts[i+2].Nz = normalVect.Z;
					verts[i+3].Nx = normalVect.X; verts[i+3].Ny = normalVect.Y;	verts[i+3].Nz = normalVect.Z;
					verts[i+4].Nx = normalVect.X; verts[i+4].Ny = normalVect.Y;	verts[i+4].Nz = normalVect.Z;
					verts[i+5].Nx = normalVect.X; verts[i+5].Ny = normalVect.Y;	verts[i+5].Nz = normalVect.Z;
				}
			}

			vb.Unlock();
		}

		void UpdateNormalVector ()
		{
			Bitmap bmp = m_WorldBmp;
			VertexBuffer vb = m_VertexBuffer;
			var verts = (CustomVertex.PositionNormalTextured[]) vb.Lock (0, 0);

			float fXPos = (float) (m_aRobots [0].GetPosX ()) / bmp.Width;
			float fYPos = (float) (m_aRobots [0].GetPosY ()) / bmp.Height;
			
            verts[0].Normal = new Vector3(fXPos, fYPos, -1f);
            verts[1].Normal = new Vector3(fXPos, fYPos, -1f);
            verts[2].Normal = new Vector3(fXPos, fYPos, -1f);
            verts[3].Normal = new Vector3(fXPos, fYPos, -1f);
            verts[4].Normal = new Vector3(fXPos, fYPos, -1f);
            verts[5].Normal = new Vector3(fXPos, fYPos, -1f);

			vb.Unlock();
		}
		
		void RenderPrimitives ()
		{
			device.SetTexture(0,m_Texture); // set texture for labirynth visualisation
			device.TextureState[0].ColorOperation = TextureOperation.Modulate;
			device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
			device.TextureState[0].ColorArgument2 = TextureArgument.Diffuse;
			device.TextureState[0].AlphaOperation = TextureOperation.Disable;
			device.TextureState[0].TextureTransform = TextureTransform.Disable;
			
			device.SetStreamSource( 0, m_VertexBuffer, 0);
			device.VertexFormat = CustomVertex.PositionNormalTextured.Format;

			// Set up a material
			device.Material = m_MaterialScene;

			SetCullingMode (device);
			device.RenderState.Lighting = true;
			device.RenderState.Ambient = Color.Gray;
			device.RenderState.SpecularEnable = true;
			
			device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
			
			int i = 0;
			double fHalfSize = m_Config.GetRobotSize () / 2.0f;
			device.SetTexture(0, m_TextureRobotBody); // set default texture for robot's body
			foreach (Robot robot in m_aRobots)
			{
				var vRobotPos = new Vector3 ((float) robot.GetPosX (), (float) robot.GetPosY (), (float) -fHalfSize);
				int nAngleIndex = (int) Math.Round (robot.GetAngle () * Constants.RAD2DEG);
				nAngleIndex = NormalizeRadarIndex (nAngleIndex);

				double x = fHalfSize * GetCosFromTbl(nAngleIndex);
				double y = fHalfSize * GetSinFromTbl(nAngleIndex);
				m_avRobotsNosePos [i] = new Vector3 ((float) (robot.GetPosX () + x), (float) (robot.GetPosY () + y), (float) -fHalfSize);
				m_aRobotSphereShape [i].Position = vRobotPos;
				m_aRobotsNose [i].Position = m_avRobotsNosePos [i];
                				
				device.Material = m_MaterialRobotBody;
				if (null != m_aRobotBodyTextures)
					device.SetTexture (0, m_aRobotBodyTextures[i]); // set defined texture for robots body if exists
				m_aRobotSphereShape [i].DrawSubset (0);
				
				device.Material = m_MaterialRobotNose;						
				m_aRobotsNose [i].DrawSubset (0);
				i++;
			}
            m_RobotGoalShape.Position = new Vector3(m_RobotGoalLocation.X, m_RobotGoalLocation.Y, -m_fRobotGoalSize);
			
			device.Material = m_MaterialScene;
            m_RobotGoalShape.DrawSubset(0);
		}

		void SetupMatrices()
		{
    		// VIEW MATRIX: A view matrix can be defined given an eye point,
    		// a point to look at, and a direction for which way is up. Here, set the
    		// eye five units back along the z-axis and up three units, look at the
    		// origin, and define "up" to be in the y-direction.
    		if (device == null)
    			return;
		
    		m_Eye = new Vector3 ((float) m_EyeX, (float) m_EyeY, (float) m_EyeZ);
    		m_LookAt = new Vector3 ((float) (m_EyeX+m_fLookAtX), (float) (m_EyeY+m_fLookAtY), (float) (m_EyeZ+m_fLookAtZ));

    		device.Transform.View = Matrix.LookAtLH( 
                            			m_Eye, m_LookAt, m_Up
                            			);
			
    		// WORLD MATRIX
			
			var matrix = new Matrix ();
			
			matrix = device.Transform.World;

			matrix.RotateYawPitchRoll ((float) m_RotateY, (float) m_RotateX, (float) m_RotateZ);
			device.Transform.World = matrix;
    
    		// PROJECTION MATRIX: Set up a perspective transform (which
    		// transforms geometry from 3-D view space to 2-D viewport space), with
    		// a perspective divide making objects smaller in the distance. To build
    		// a perspective transform, use the field of view (1/4 pi is common),
    		// the aspect ratio, and the near and far clipping planes (which define
    		// at what distances geometry should be no longer be rendered).
    		device.Transform.Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4, 1.0f, 1.0f, 2000.0f );
		}
		
		
		protected virtual void InvalidateDeviceObjects(object sender, EventArgs e)
		{
            //Device dev = (Device)sender;

            //dev.Reset (dev.PresentationParameters);
		}
		
		protected virtual void RestoreDeviceObjects(object sender, EventArgs e)
		{
			var dev = (Device)sender;

			// Set up a material

			dev.Material = m_MaterialScene;
			
			// Set miscellaneous render states
			//device.RenderState.DitherEnable = false;
			//device.RenderState.SpecularEnable = false;

			// Turn off culling, so we see the front and back of the triangle
			SetCullingMode (dev);
			//dev.RenderState.CullMode = Cull.Clockwise;
			// Turn off D3D lighting, since we are providing our own vertex colors
			//dev.RenderState.Lighting = false;
			// Turn ON D3D lighting.
			dev.RenderState.Lighting = true;
			// Enable ambient lighting to a dim, light, so objects that
			// are not lit by the other lights are not completely black
			dev.RenderState.Ambient = Color.Brown;	
			int i = 0;
			foreach (Robot robot in m_aRobots)
			{
				m_aRobotSphereShape [i].Position = new Vector3 ((float) robot.GetPosX (), (float) robot.GetPosY (), (float) -(m_Config.GetRobotSize ()/2.0f));
				m_aRobotsNose [i].Position = m_avRobotsNosePos [i];
				i++;
			}
            m_RobotGoalShape.Position = new Vector3(m_RobotGoalLocation.X, m_RobotGoalLocation.Y, -m_fRobotGoalSize);
		}
		
		protected virtual void DeleteDeviceObjects(object sender, EventArgs e)
		{
		}
		
		protected virtual void EnvironmentResizing(object sender, CancelEventArgs e)
		{
			m_SysFont = new System.Drawing.Font ("Courier", ClientSize.Width/40 /*16*/);
			m_Font = new Microsoft.DirectX.Direct3D.Font (device, m_SysFont);
		}
		
		/// <summary>
		/// This method moves the scene
		/// </summary>
		protected virtual void FrameMove()
		{
			if (null != device) {
				if (Math.Abs(m_LightX) < Constants.EPSILON && Math.Abs(m_LightY) < Constants.EPSILON && Math.Abs(m_LightZ) < Constants.EPSILON)
					m_LightZ = -10;
				int i = 0;
				double fHalfSize = m_Config.GetRobotSize () / 2.0f;
				foreach (Robot robot in m_aRobots)
				{
					if (!robot.isKilled) {
						m_aRobotSphereShape[i].Dispose();
						m_aRobotsNose[i].Dispose();
						int nAngleIndex = (int) Math.Round (robot.GetAngle() * Constants.RAD2DEG);
						
						nAngleIndex = NormalizeRadarIndex (nAngleIndex);
		
						m_aRobotSphereShape[i] = new Sphere3D (device, new Vector3((float)robot.GetPosX(), (float)robot.GetPosY(), (float) -fHalfSize), (float)fHalfSize, 100, 100);
						m_avRobotsNosePos[i] = new Vector3((float) ((robot.GetPosX() + fHalfSize) * GetCosFromTbl(nAngleIndex)),
						                                   (float) ((robot.GetPosY() + fHalfSize) * GetSinFromTbl(nAngleIndex)),
						                                   (float) -fHalfSize
						                                   );				
						m_aRobotsNose[i] = new Sphere3D (device, m_avRobotsNosePos[i], (float)(m_Config.GetRobotSize() / 3.0f), 25, 25);
					}
				}
	            m_RobotGoalShape.Dispose();
	            m_RobotGoalShape = new Sphere3D(device, new Vector3(m_RobotGoalLocation.X, m_RobotGoalLocation.Y, m_fRobotGoalSize), m_fRobotGoalSize, 100, 100);
			}
		}
		
		/// <summary>
		/// This method renders the scene
		/// </summary>
		protected virtual void Render()
		{
			if (device != null) 
			{
				device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
				device.BeginScene();
				
				SetupMatrices ();
                SetLights();
				RenderPrimitives ();
				
				if (m_Config.ShowOnScreenInfo()) {
					double fRobotPosX = m_aRobots[m_nCurrRobotNum].GetPosX();
					double fRobotPosY = m_aRobots[m_nCurrRobotNum].GetPosY();
					int fontColor = Color.Red.ToArgb();

					m_Font.DrawText(null,
						global::RobEnvMK.Properties.Resources.RobotNumber + m_nCurrRobotNum + ", X=" + fRobotPosX + " Y=" + fRobotPosY,
						new Rectangle(10, 10, 100, 100),
						DrawTextFormat.NoClip,
						fontColor
					);					

					m_Font.DrawText(null,
						global::RobEnvMK.Properties.Resources.Angle + m_aRobots[m_nCurrRobotNum].GetAngle() + "  Step: " + m_nStep,
						new Rectangle(10, 10 + (this.ClientSize.Width / 32), 100, 100),
						DrawTextFormat.NoClip,
						Color.Red.ToArgb()
					);
					m_Font.DrawText(null,
						global::RobEnvMK.Properties.Resources.Cmd + m_aRobots[m_nCurrRobotNum].LastCommand,
						new Rectangle(10, 10 + (this.ClientSize.Width / 32) * 2, 100, 100),
						DrawTextFormat.NoClip,
						Color.Red.ToArgb()
					);
				}

				device.EndScene();
				device.Present();
			}
		}
		
		/// <summary>
		/// Our mainloop
		/// </summary>
		public void Run()
		{
			// While the form is still valid, render and process messages
			while (Created) {
                if (m_bRun)
                {
                    m_nStep++;
                    CommLoop();
                    if (m_Config.IsStatusWindowVisible())
                    {
                        int nBotCount = 0;
                        foreach (Robot bot in this.m_aRobots)
                        {
                            m_RobStat.SetPos(bot.GetPosX(), bot.GetPosY(), nBotCount);
                            m_RobStat.SetAngle(bot.GetAngle(), nBotCount);
                            m_RobStat.SetRadar(this.GetRadarNorthAt(bot.GetPosX(), bot.GetPosY(), bot.GetAngle()), nBotCount);
                            nBotCount++;
                        }
                        m_RobStat.SetStep(m_nStep);
                    }
                    if (m_bRender || 0 == (m_nStep % 200))	// even if nothing changes, refresh every 200 steps
                    {
                        if (m_Config.IsWorldVisible())
                        {
                            FrameMove();
                            Render();
                        }
                    }
                }
				Application.DoEvents();
			}
		}
		
		protected override void OnPaint(PaintEventArgs e)
		{
            try
            {
                if (m_Config.IsWorldVisible())
                {
                    device.Present();
					Render();
                }
            }
            catch (DeviceLostException)
            {
                device.Reset(device.PresentationParameters);
            }
		}
		
		void LookToTheLeft ()
		{
			m_fLookAtX -= (float) 1;
			Trace.WriteLine ("Look to the left.");			
		}
		
		void LookToTheRight ()
		{
			m_fLookAtX += (float) 1;
			Trace.WriteLine ("Look to the right.");							
		}
		
		void GoForward ()
		{
			m_EyeY += m_fStep;
			Trace.WriteLine ("Go forward.");											
		}
		
		void GoBackwards ()
		{
			m_EyeY -= m_fStep;
			Trace.WriteLine ("Go backwards.");															
		}
		
		void LookDown ()
		{
			m_fLookAtZ += (float) 1;
			Trace.WriteLine ("Look down.");
		}
		
		void LookUp ()
		{
			m_fLookAtZ -= (float) 1;
			Trace.WriteLine ("Look up.");							
		}
		
		void LookFarther ()
		{
			m_fLookAtY += (float) 1;
			Trace.WriteLine ("Look farther.");			
		}
		
		void LookCloser ()
		{
			m_fLookAtY -= (float) 1;
			Trace.WriteLine ("Look closer.");			
		}
		
		void SlideToTheRight ()
		{
			m_EyeX += m_fStep;
			Trace.WriteLine ("Slide to the right.");			
		}
		
		void SlideToTheLeft ()
		{
			m_EyeX -= m_fStep;
			Trace.WriteLine ("Slide to the left.");			
		}
		
		void FloatDown ()
		{
			m_EyeZ += (float) 1.0;
			Trace.WriteLine ("Float down.");				
		}
		
		void FloatUp ()
		{
			m_EyeZ -= (float) 1.0;
			Trace.WriteLine ("Float up.");				
		}
		
		void RotateXRight ()
		{
			m_RotateX += (float) 0.05;
			Trace.WriteLine ("Rotate X right.");				
		}
		
		void RotateXLeft ()
		{
			m_RotateX -= (float) 0.05;
			Trace.WriteLine ("Rotate X left.");				
		}
		
		void RotateYRight ()
		{
			m_RotateY += (float) 0.05;
			Trace.WriteLine ("Rotate Y right.");				
		}
		
		void RotateYLeft ()
		{
			m_RotateY -= (float) 0.05;
			Trace.WriteLine ("Rotate Y left.");				
		}
		
		void RotateZRight ()
		{
			m_RotateZ += (float) 0.05;
			Trace.WriteLine ("Rotate Z right.");				
		}
		
		void RotateZLeft ()
		{
			m_RotateZ -= (float) 0.05;
			Trace.WriteLine ("Rotate Z left.");				
		}
		
		void MainClassMouseLeave(object sender, System.EventArgs e)
		{
			StopTimer();
			Application.Exit ();
		}
		
		void ToggleLightSwitch (int num)
		{
			m_baLightSwitches[num] = !this.m_baLightSwitches [num];
		}

        void ToggleTimer()
        {
            m_bRun = !m_bRun;
        }
			
		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			base.OnKeyPress(e);		
			Trace.WriteLine ("Key pressed code: {0}" + e.KeyChar);
			if ((int)e.KeyChar == (int)System.Windows.Forms.Keys.Escape) 
			{
                m_bExit = true;
                m_bRender = false;
				StopTimer();
				KillAllRobots();
                if (m_Config.IsStatusWindowVisible())
                    m_RobStat.Close();
				Close();
			}
			else if ((char)e.KeyChar == (char) 'L')
			{
				LookToTheLeft ();
			}
			else if ((char)e.KeyChar == (char) 'R')
			{
				LookToTheRight ();
			}			
			else if ((int)e.KeyChar == (int)Keys.PageUp
			          || (char)e.KeyChar == (char) 'O'
			        )
			{
				LookDown ();
			}						
			else if ((int)e.KeyChar == (int)Keys.PageDown
			          || (char)e.KeyChar == (char) 'K'
			        )
			{
				LookUp ();
			}									
			else if ((char)e.KeyChar == (char) 'u')
			{
				GoForward ();
			}
			else if ((char)e.KeyChar == (char) 'd')
			{
				GoBackwards ();
			}			
			else if ((char)e.KeyChar == (char) 'f')
			{
				LookFarther ();
			}						
			else if ((char)e.KeyChar == (char) 'F')
			{
				LookCloser ();
			}												
			else if ((char)e.KeyChar == (char) 'r')
			{
				SlideToTheRight ();
			}
			else if ((char) e.KeyChar  == (char) 'l')
			{
				SlideToTheLeft ();
			}			
			else if ((char) e.KeyChar == (char) 'o')
			{
				FloatDown ();
			}			
			else if ((char) e.KeyChar == (char) 'k')
			{
				FloatUp ();
			}						
			else if ((char) e.KeyChar == (char) 'x')
			{
				RotateXRight ();
			}						
			else if ((char) e.KeyChar == (char) 'X')
			{
				RotateXLeft ();
			}												
			else if ((char) e.KeyChar == (char) 'y')
			{
				RotateYRight ();
			}						
			else if ((char) e.KeyChar == (char) 'Y')
			{
				RotateYLeft ();
			}									
			else if ((char) e.KeyChar == (char) 'z')
			{
				RotateZRight ();
			}						
			else if ((char) e.KeyChar == (char) 'Z')
			{
				RotateZLeft ();
			}												
			else if ((char) e.KeyChar == (char) '=')
			{
				m_LightX += 1.0f;
				Trace.WriteLine ("Amplify light in X direction.");	
			}						
			else if ((char) e.KeyChar == (char) '-')
			{
				m_LightX -= 1.0f;
				Trace.WriteLine ("Reduce light in X direction.");	
			}
			else if ((char) e.KeyChar == (char) ']')
			{
				m_LightY += 1.0f;
				Trace.WriteLine ("Amplify light in Y direction.");	
			}						
			else if ((char) e.KeyChar == (char) '[')
			{
				m_LightY -= 1.0f;
				Trace.WriteLine ("Reduce light in Y direction.");	
			}
			else if ((char) e.KeyChar == (char) '.')
			{
				m_LightZ += 1.0f;
				Trace.WriteLine ("Amplify light in Z direction.");	
			}						
			else if ((char) e.KeyChar == (char) ',')
			{
				m_LightZ -= 1.0f;
				Trace.WriteLine ("Reduce light in Z direction.");	
			}			
			/*else if ((char) e.KeyChar == (char) 'q')
			{
				Trace.WriteLine ("Increase updating interval.");	
				m_fUpdateEvery += 0.1f;
				ResetTimer ();
			}									
			else if ((char) e.KeyChar == (char) 'a')
			{
				Trace.WriteLine ("Reduce updating interval.");	
				if (10.0f < m_fUpdateEvery)
				{
					m_fUpdateEvery -= 0.1f;
					ResetTimer ();
				}
				else
					Trace.WriteLine ("Update interval is set to the minimum allowed.");	
			}*/												
			else if ((char) e.KeyChar == (char) 'c')
			{
				StartValues ();
				Trace.WriteLine ("RESET.");	
			}
			else if ((char) e.KeyChar == (char) 'C')
			{
				if (StopTimer ())
					Trace.WriteLine ("Timer Stop.");	
				else
				{
					ResetTimer ();
					Trace.WriteLine ("Timer Start.");	
				}
			}			
			else if ((char)e.KeyChar == (char)'S' && !m_bScreenSaver)
			{
                StopTimer();
                m_nStep = 0;
				Setup ();
				if (!InitializeGraphics()) 
				{
                    MessageBox.Show(global::RobEnvMK.Properties.Resources.ErrInitDirect3d);
					throw (new Exception ());
				}
				Trace.WriteLine ("Configuration Setup.");	
			}
			else if ((char) e.KeyChar == (char) 'n')
			{
				m_nCurrRobotNum++;
				if (m_saRobotsList.Length <= m_nCurrRobotNum)
					m_nCurrRobotNum = 0;
			}
			else if ((char) e.KeyChar == (char) '1')
			{
				ToggleLightSwitch(0);
			}
			else if ((char) e.KeyChar == (char) '2')
			{
				ToggleLightSwitch(1);
			}			
			else if ((char) e.KeyChar == (char) '3')
			{
				ToggleLightSwitch(2);
			}
            else if ((char)e.KeyChar == (char)'p' || (char)e.KeyChar == (char)' ')
            {
                // PAUSE
                ToggleTimer();
            }
            else if (m_bScreenSaver) {
				StopTimer();
				Application.Exit();
			}
			Trace.WriteLine ("Eye: " + m_Eye.X.ToString () + ", " + m_Eye.Y.ToString () + ", " + m_Eye.Z.ToString ());
			Trace.WriteLine ("Rotation: " + m_RotateX.ToString () + ", " + m_RotateY.ToString () + ", " + m_RotateZ.ToString ());
			Trace.WriteLine ("Look At: " + m_fLookAtX.ToString () + ", " + m_fLookAtY.ToString () + ", " + m_fLookAtZ.ToString ());
			Trace.WriteLine ("Light: " + m_LightX.ToString () + ", " +  m_LightY.ToString () + ", " + m_LightZ.ToString ());
            if (Created && m_bRun)
            {
                if (m_Config.IsStatusWindowVisible())
                    m_RobStat.SetLightingEnablemendStatus(m_baLightSwitches[0],
                                                          m_baLightSwitches[1],
                                                          m_baLightSwitches[2]);
                if (m_Config.IsWorldVisible())
					Render();
            }
		}
		
		/// <summary>
		/// The main entry point for the application
		/// </summary>
		static void Main(string [] args)
		{
            bool bScrSaver = false;
            bool bRunConfig = false;
			const string sTraceName = @".\RobEnvMK.trc";
            Trace.Listeners.Add(new TextWriterTraceListener(sTraceName));
            Trace.AutoFlush = true;
            Trace.WriteLine("RobEnvMK STARTED.");

            if (0 < args.Length)
            {
            	if ("/s" == args[0]) {
               		bScrSaver = true;
            	} else if (args[0].StartsWith("/c", StringComparison.Ordinal)) {
					bRunConfig = true;
            	}
            }

            if (bRunConfig)
            {
                var configSetup = new ConfigurationForm();
                configSetup.ShowDialog();
                Application.ExitThread();
            }
            var mainClass = new MainClass(bScrSaver);
            if (!mainClass.InitializeGraphics())
            {
                MessageBox.Show(global::RobEnvMK.Properties.Resources.ErrInitDirect3d);
                //Application.ExitThread();
                Application.Exit();
            }
            mainClass.Show();

			while (!mainClass.ExitProgram) {
				try {
					mainClass.Run();
					Application.Exit();
				} catch (DeviceLostException) {
					if (!mainClass.InitializeGraphics()) {
						Trace.WriteLine("Error while initializing Direct3D");
					}
				} catch (FileNotFoundException e) {
					Console.WriteLine("System.IO.FileNotFoundException caught: " + e.Message);
					Console.WriteLine("Stack: " + e.StackTrace);
					Trace.WriteLine("System.IO.FileNotFoundException caught: " + e.Message);
					Trace.WriteLine("Stack: " + e.StackTrace);
					mainClass.KillAllRobots();
					MessageBox.Show(global::RobEnvMK.Properties.Resources.ExcFileNotFound + e.Message + "\n" + global::RobEnvMK.Properties.Resources.Stack + e.StackTrace);
					//Application.ExitThread();
					Application.Exit();
				} catch (IOException e) {
					Console.WriteLine("IOException caught: " + e.Message);
					Console.WriteLine("Stack: " + e.StackTrace);
					Trace.WriteLine("IOException caught: " + e.Message);
					Trace.WriteLine("Stack: " + e.StackTrace);					
					mainClass.KillAllRobots();
					MessageBox.Show(global::RobEnvMK.Properties.Resources.ExcIO + e.Message + "\n" + global::RobEnvMK.Properties.Resources.Stack + e.StackTrace);
					//Application.ExitThread();
					Application.Exit();
				} catch (ApplicationException e) {
					Console.WriteLine("System.ApplicationException caught: " + e.Message);
					Console.WriteLine("Stack: " + e.StackTrace);
					Trace.WriteLine("System.ApplicationException caught: " + e.Message);
					Trace.WriteLine("Stack: " + e.StackTrace);					
					mainClass.KillAllRobots();
					MessageBox.Show(global::RobEnvMK.Properties.Resources.ExcApplication + e.Message + "\n" + global::RobEnvMK.Properties.Resources.Stack + e.StackTrace);
					//Application.ExitThread();
					Application.Exit();
            	} catch (NullReferenceException e) {
					Console.WriteLine("Exception caught: " + e.Message);
					Console.WriteLine("Stack: " + e.StackTrace);
					Trace.WriteLine("Exception caught: " + e.Message);
					Trace.WriteLine("Stack: " + e.StackTrace);					
					mainClass.KillAllRobots();
					MessageBox.Show(global::RobEnvMK.Properties.Resources.ExcCaught + e.Message + "\n" + global::RobEnvMK.Properties.Resources.Stack + e.StackTrace);
					//Application.ExitThread();            		
					Application.Exit();
				} catch (Exception e) {
					Console.WriteLine("Exception caught: " + e.Message);
					Console.WriteLine("Stack: " + e.StackTrace);
					Trace.WriteLine("Exception caught: " + e.Message);
					Trace.WriteLine("Stack: " + e.StackTrace);					
					mainClass.KillAllRobots();
					MessageBox.Show(global::RobEnvMK.Properties.Resources.ExcCaught + e.Message + "\n" + global::RobEnvMK.Properties.Resources.Stack + e.StackTrace);
					//Application.ExitThread();
					Application.Exit();
				}
			}
            //Application.ExitThread();
            Application.Exit();
		}

        void InitializeComponent()
        {
            var resources = new ComponentResourceManager(typeof(MainClass));
			SuspendLayout();
            // 
            // MainClass
            // 
			ClientSize = new Size(292, 273);
			Icon = ((Icon)(resources.GetObject("$this.Icon")));
			Name = "MainClass";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "RobEnvMK";
			FormClosed += MainClass_FormClosed;
			FormClosing += MainClass_FormClosing;
			Resize += MainClass_Resize;
			ResumeLayout(false);

        }

        void MainClass_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.ExitThread();
            Application.Exit();
        }

        private void MainClass_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_bRender = false;
			m_RobStat.Close();
            m_bExit = true;
			StopTimer();
            KillAllRobots();
        }

        private void MainClass_Resize(object sender, EventArgs e)
        {
            if (m_Config.IsWorldVisible())
				Render();
        }
	}

    /// <summary>
    /// This class represents radar vector of one point on the map.
    /// </summary>
    [Serializable]
    public class RadarPoint
    {
        private double[] m_faRadar;

        /// <summary>
        /// Calculate radar vector.
        /// </summary>
        public RadarPoint(int[,] naMap, MainClass world, int nX, int nY)
        {
            if (0 == world.m_anWorldMap[nX, nY])
            {
                m_faRadar = new double[Constants.RADAR_RESN];
                int i = 0, j = 0;
                double fSightDist = world.m_Config.GetSightDistance();
                int nWorldSizeX = world.m_anWorldMap.GetLength(0);
                int nWorldSizeY = world.m_anWorldMap.GetLength(1);
                
                /*
                 * Radar scan step: radar ray is sent out from the current
				 * coordinates. At each step the end of ray coordinates are
				 * converted to integers of the world map coordinates and
				 * if the cell is an obstacle, the ray end-point distance
				 * from current coordinates is the radar's value.
				 */
				const double fScanStep = Constants.RADAR_SCAN_STEP;
				
                int nMaxRayIndex = (int)(fSightDist / fScanStep);
                double x = (float) nX;
                double y = (float) nY;
                double fXPos = x;
                double fYPos = y;
                double step = 0f;
                int nAngleIndex = Constants.RADAR_RESN2;
                int nx = 0, ny = 0;
                for (j = 0, nAngleIndex = Constants.RADAR_RESN2;
                     Constants.RADAR_RESN > j;
                     j++, nAngleIndex++)
                {
                    step = 0.0f;
                    nx = 0; ny = 0;
                    if (Constants.RADAR_RESN <= nAngleIndex)
                        nAngleIndex = 0;
                    for (i = 0; nMaxRayIndex > i; i++, step += fScanStep)
                    {
                        x = fXPos + world.GetRadarRayValueX(nAngleIndex, i);
                        y = fYPos + world.GetRadarRayValueY(nAngleIndex, i);
                        nx = (int)x;
                        ny = (int)y;
                        if (0 > nx || 0 > ny || nWorldSizeX <= nx || nWorldSizeY <= ny)
                            break;
                        if (0 != world.m_anWorldMap[nx, ny])
                            break;
                    }
                    m_faRadar[j] = step;
                }
            }
            else
                m_faRadar = null;
        }

		int NormalizeRadarIndex (int index)
		{
			int nIndex = index;
			
            while (0 > nIndex)
                nIndex += Constants.RADAR_RESN;
            while (Constants.RADAR_RESN <= nIndex)
                nIndex -= Constants.RADAR_RESN;			
            
            return (nIndex);
		}		
		
        /// <summary>
        /// Get radar reading at given angle.
        /// </summary>
        public double GetReadAtAngle(float angle)
        {
            int n = (int) Math.Round (angle * Constants.RAD2DEG);
            
            n = NormalizeRadarIndex (n);

            return (m_faRadar[n]);
        }

        /// <summary>
        /// Get radar reading at given index.
        /// </summary>
        public double GetReadAtStep(int step)
        {
        	int nStep = step;
        	
        	nStep = NormalizeRadarIndex (nStep);

            return (m_faRadar[nStep]);
        }
    }

    /// <summary>
    /// This class holds the radar information used to detect if new radar
    /// file should be generated.
    /// </summary>
    [Serializable]
    public sealed class RadarInfo
    {
        RadarPoint[,] m_aRadar;
        string m_sBitmapPath;
        int m_nRadarSize;
        double m_fSightDistance;

        public RadarPoint[,] RadarArray
        {
            get { return (m_aRadar); }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public RadarInfo(RadarPoint[,] radar, string bitmappath, int radarsize, double sightdistance)
        {
            m_aRadar = radar;
            m_sBitmapPath = bitmappath;
            m_nRadarSize = radarsize;
            m_fSightDistance = sightdistance;
        }

        /// <summary>
        /// Check if existing radar file can be used.
        /// </summary>
        public static bool IsRadarFileValid(string bitmappath, int radarsize, double sightdistance)
        {
            bool bRet = false;
            RadarInfo RadarInfoObj;
            if (null != (RadarInfoObj = DeserializeRadar(bitmappath)))
            {
                if (RadarInfoObj.m_nRadarSize == radarsize
				    && Math.Abs(RadarInfoObj.m_fSightDistance - sightdistance) < Constants.EPSILON
				    && RadarInfoObj.MapFileOlderThanRadarFile(bitmappath)) {
            		
                	bRet = true;
            	}
            }

            return (bRet);
        }

        public static RadarInfo DeserializeRadar(string bitmappath)
        {
            string sFileName = GetRadarFilePath(bitmappath);
            RadarInfo result = null;
            try
            {
                if (File.Exists(sFileName))
                {
                    IFormatter binFmt = new BinaryFormatter();
                    Stream s = File.Open(sFileName, FileMode.Open);
                    result = (RadarInfo)binFmt.Deserialize(s);
                    s.Close();
                }
                else
					Console.WriteLine("DeserializeRadar: Radar file does not exist: " + sFileName);
            }
            catch (Exception e)
            {
				Console.WriteLine("DeserializeRadar: Unable to load file " + sFileName + " : " + e);
            }

            return (result);
        }

        public void SerializeRadar()
        {
            string sFileName = GetRadarFilePath(m_sBitmapPath);

            try
            {
                IFormatter binFmt = new BinaryFormatter();
                Stream s = File.Open(sFileName, FileMode.Create);
                binFmt.Serialize(s, this);
                s.Close();
            }
            catch (Exception e)
            {
				Console.WriteLine("SerializeRadar " + global::RobEnvMK.Properties.Resources.ErrUnableToSaveFile + " - " + sFileName + " : " + e);
                File.Delete(sFileName);
            }
        }

        public static string GetRadarFilePath(string bitmappath)
        {
            string sRet = bitmappath + ".radarinfo";

            return (sRet);
        }

        bool MapFileOlderThanRadarFile(string bitmappath)
        {
            bool bRet = false;

            if (File.GetLastWriteTime(GetRadarFilePath(bitmappath)) > File.GetLastWriteTime(bitmappath))
                bRet = true;

            return (bRet);
        }
    }
}
