using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Timers;
using System.Threading;
using System.Diagnostics;

namespace RobEnvMK
{
    public partial class FormRobotProperties : Form
    {

        private RobotProperties m_RobotProperties = new RobotProperties ();
        public CustomEventPublisher m_EventPublisher;

        public FormRobotProperties()
        {
            InitializeComponent();
        }

        public FormRobotProperties(RobotProperties robprop, CustomEventPublisher publisher)
        {
            InitializeComponent();
            m_RobotProperties = robprop;
            m_EventPublisher = publisher;
            InitScriptsList();
            InitPriorityList();
            InitTextureList();
        }

        private void InitTextureList()
        {
            string[] asFileList = Directory.GetFiles(Environment.CurrentDirectory);

            foreach (string sFile in asFileList)
            {
                if (IsTextureClass(sFile))
                {
                    comboBoxTexture.Items.Add(Path.GetFileName(sFile));
                }
            }

            if (comboBoxTexture.Items.Contains(m_RobotProperties.Texture))
            {
                comboBoxTexture.Text = m_RobotProperties.Texture;
            }
        }

        private void InitPriorityList()
        {
            foreach (string s in Enum.GetNames(typeof(ProcessPriorityClass)))
            {
                comboBoxPriority.Items.Add(s);
            }

            if (comboBoxPriority.Items.Contains(m_RobotProperties.Priority.ToString ()))
            {
                comboBoxPriority.Text = m_RobotProperties.Priority.ToString ();
            }
        }

        private void InitScriptsList()
        {
            string[] asFileList = Directory.GetFiles(Environment.CurrentDirectory);

            foreach (string sFile in asFileList)
            {
                if (IsScriptClass(sFile))
                {
                    comboBoxScript.Items.Add(".\\\\" + Path.GetFileName(sFile));
                }
            }

            if (comboBoxScript.Items.Contains(m_RobotProperties.Script))
            {
                comboBoxScript.Text = m_RobotProperties.Script;
            }
        }

        private bool IsScriptClass(string sFilePath)
        {
            bool bRet = false;

            string sFileName = Path.GetFileName(sFilePath);
            string sExtension = string.Empty;
            ArrayList aExtList = new ArrayList(3);

            aExtList.Add(".bat");
            aExtList.Add(".exe");
            aExtList.Add(".pl");

            if (null != sFileName && 0 < sFileName.Length)
            {
                sExtension = Path.GetExtension(sFileName);
            }

            if (0 < sExtension.Length)
            {
                foreach (string sExt in aExtList)
                {
                    if (0 == sExtension.ToLower().CompareTo(sExt.ToLower()))
                    {
                        bRet = true;
                        break;
                    }
                }
            }

            return bRet;
        }

        private bool IsTextureClass(string sFilePath)
        {
            bool bRet = false;

            string sFileName = Path.GetFileName(sFilePath);
            string sExtension = string.Empty;
            ArrayList aExtList = new ArrayList(3);

            aExtList.Add(".bmp");
            aExtList.Add(".jpg");
            aExtList.Add(".gif");

            if (null != sFileName && 0 < sFileName.Length)
            {
                sExtension = Path.GetExtension(sFileName);
            }

            if (0 < sExtension.Length)
            {
                foreach (string sExt in aExtList)
                {
                    if (0 == sExtension.ToLower().CompareTo(sExt.ToLower()))
                    {
                        bRet = true;
                        break;
                    }
                }
            }

            return bRet;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            SaveRobotProperties();
            this.Close();
        }

        private void SaveRobotProperties()
        {
            CustomEventArgs args = new CustomEventArgs (m_RobotProperties);
            m_EventPublisher.OnRaiseCustomEvent(args);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void comboBoxScript_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox list = ((ComboBox)sender);
            m_RobotProperties.Script = (string)list.Items[list.SelectedIndex];
        }

        private void comboBoxPriority_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox list = ((ComboBox)sender);
            m_RobotProperties.Priority = RobotsConfigTools.GetProcessPriorityFromName((string)list.Items[list.SelectedIndex]);
        }

        private void comboBoxTexture_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox list = ((ComboBox)sender);
            m_RobotProperties.Texture = (string)list.Items[list.SelectedIndex];
        }
    }
}
