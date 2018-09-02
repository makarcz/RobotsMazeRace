/*
 * Created by SharpDevelop.
 * User: mkarcz
 * Date: 2007*04*20
 * Time: 5:09 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace RobEnvMK
{
	/// <summary>
	/// Description of FormRadarInit.
	/// </summary>
	public class FormRadarInit : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button buttonOK;
		private RadarPoint [,] m_aRadar = null;
		private Bitmap m_WorldBmp = null;
		private int [,] m_anWorldMap = null;
		private MainClass m_MainForm = null;
		private bool m_bIsRadarInitDone = false;
		
		public bool RadarInitialized
		{
			get 
			{
				return (m_bIsRadarInitDone);
			}
		}
		
		public RadarPoint [,] RadarArray
		{
			get
			{
				return (m_aRadar);
			}
		}
		
		public FormRadarInit()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		public FormRadarInit(Bitmap worldBmp, int [,] worldMap, MainClass mainForm)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			m_aRadar = new RadarPoint [worldBmp.Width, worldBmp.Height];
			m_WorldBmp = worldBmp;
			m_anWorldMap = worldMap;
			m_MainForm = mainForm;
			StartRadarInit ();
		}
		
		public void StartRadarInit ()
		{
			var t = new Thread (new ThreadStart (InitializeRadar));
			t.Start ();
		}
		
		public void InitializeRadar ()
		{
			int i = 0, j = 0;
			for (i = 0; i < m_WorldBmp.Width; i++)
			{
				for (j = 0; j < m_WorldBmp.Height; j++)
				{
					if (0 == m_anWorldMap [i, j])
					{
						m_aRadar [i, j] = new RadarPoint (m_anWorldMap, m_MainForm, i, j);
						SetText ("(" + m_WorldBmp.Width + "," + m_WorldBmp.Height + "): " + i + ", " + j);
					}
					else
						m_aRadar [i, j] = null;
					Thread.Sleep (0);
				}
				Thread.Sleep (0);
			}						
			m_bIsRadarInitDone = true;
			Close();	
		}
		
		delegate void SetTextCallback(string text);		
		
		public void SetText (string sText)
		{
			  // InvokeRequired required compares the thread ID of the
			  // calling thread to the thread ID of the creating thread.
			  // If these threads are different, it returns true.
			  if (this.textBox1.InvokeRequired)
			  { 
			    var d = new SetTextCallback(SetText);
			    Invoke(d, new object[] { sText });
			  }
			  else
			  {
			    textBox1.Text = sText;
			    textBox1.Update ();
				textBox1.Refresh ();
			  }

		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.buttonOK = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(41, 59);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 1;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.ButtonOKClick);
			// 
			// textBox1
			// 
			this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox1.Location = new System.Drawing.Point(20, 18);
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new System.Drawing.Size(128, 13);
			this.textBox1.TabIndex = 0;
			this.textBox1.Text = "";
			// 
			// FormRadarInit
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CausesValidation = false;
			this.ClientSize = new System.Drawing.Size(170, 90);
			this.ControlBox = false;
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.textBox1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRadarInit";
			this.Text = "Initializing radar rays...";
			this.TopMost = true;
			this.Load += new System.EventHandler(this.FormRadarInitLoad);
			this.ResumeLayout(false);
		}
		#endregion
		
		void FormRadarInitLoad(object sender, System.EventArgs e)
		{

		}
		
		void ButtonOKClick(object sender, System.EventArgs e)
		{
			if (m_bIsRadarInitDone)
				Close();
			else
				MessageBox.Show("Radar initialization is in progress: " + textBox1.Text);
		}
		
	}
}
