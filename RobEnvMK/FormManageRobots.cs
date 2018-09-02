using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace RobEnvMK
{
    public partial class FormManageRobots : Form
    {
        ArrayList m_RobotsPropertiesList = new ArrayList();
        RobotsConfigTools m_ConfigTools = new RobotsConfigTools();
        CustomEventPublisher m_Publisher = new CustomEventPublisher();
        RobotProperties m_RobotProperties;

        public FormManageRobots()
        {
            InitializeComponent();
        }

        public FormManageRobots(string cfgpath)
        {
            InitializeComponent();
            m_ConfigTools = new RobotsConfigTools(cfgpath);
            m_RobotsPropertiesList = m_ConfigTools.GetRobotsFromConfigFile();
            
            foreach (RobotProperties p in m_RobotsPropertiesList)
            {
                var item = new ListViewItem (p.Script, 0);
                item.SubItems.Add (p.Priority.ToString ());
                item.SubItems.Add (p.Texture);
                listView1.Items.Add (item);
            }

            m_Publisher.RaiseCustomEvent += OnRobotPropertiesChanged;
        }

        void OnRobotPropertiesChanged(object sender, CustomEventArgs e)
        {
            m_RobotProperties = e.RobotProperties;
        }

        void SaveRobotsCfg()
        {
            var robtbl = new RobotProperties [m_RobotsPropertiesList.Count];
            int n = 0;
            foreach (RobotProperties rp in m_RobotsPropertiesList)
            {
                robtbl[n] = rp;
                n++;
            }
            m_ConfigTools.SaveRobotsConfigFile(robtbl);
        }

        void buttonOK_Click(object sender, EventArgs e)
        {
            SaveRobotsCfg();
			Close();
        }

        void buttonCancel_Click(object sender, EventArgs e)
        {
			Close();
        }

        void EditRobot(RobotProperties rp)
        {
            var RobProp = new FormRobotProperties(rp, m_Publisher);
            RobProp.ShowDialog();
        }

        void EditBots()
        {
            if (listView1.CheckedItems.Count > 0)
            {
                foreach (ListViewItem item in listView1.CheckedItems)
                {
                    var rp = new RobotProperties();
                    rp.Script = item.SubItems[0].Text;
                    rp.Priority = RobotsConfigTools.GetProcessPriorityFromName(item.SubItems[1].Text);
                    rp.Texture = item.SubItems[2].Text;
                    m_RobotProperties = null;
                    EditRobot(rp);
                    if (null != m_RobotProperties)
                    {
                        listView1.Items[listView1.Items.IndexOf(item)].SubItems[0].Text = m_RobotProperties.Script;
                        listView1.Items[listView1.Items.IndexOf(item)].SubItems[1].Text = m_RobotProperties.Priority.ToString();
                        listView1.Items[listView1.Items.IndexOf(item)].SubItems[2].Text = m_RobotProperties.Texture;
                    }
                }

                m_RobotsPropertiesList.Clear();
                foreach (ListViewItem item in listView1.Items)
                {
                    var rp = new RobotProperties(item.SubItems[0].Text, item.SubItems[2].Text, RobotsConfigTools.GetProcessPriorityFromName(item.SubItems[1].Text));

                    m_RobotsPropertiesList.Add(rp);
                }
            }
        }

        void editToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            EditBots();
        }

        void AddNewBot()
        {
            var rp = new RobotProperties();

            m_RobotProperties = null;
            EditRobot(rp);
            if (null != m_RobotProperties)
            {
                m_RobotsPropertiesList.Add(rp);
                var item = new ListViewItem(rp.Script, 0);
                item.SubItems.Add(rp.Priority.ToString());
                item.SubItems.Add(rp.Texture);
                listView1.Items.Add(item);
            }
        }

        void addNewToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddNewBot();
        }

        void DeleteBots()
        {
            if (listView1.CheckedItems.Count > 0)
            {
                foreach (ListViewItem item in listView1.CheckedItems)
                {
                    listView1.Items.Remove(item);
                }

                m_RobotsPropertiesList.Clear();
                foreach (ListViewItem item in listView1.Items)
                {
                    var rp = new RobotProperties(item.SubItems[0].Text, item.SubItems[2].Text, RobotsConfigTools.GetProcessPriorityFromName(item.SubItems[1].Text));

                    m_RobotsPropertiesList.Add(rp);
                }
            }
        }

        void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DeleteBots();
        }

        void addNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewBot();
        }

        void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditBots();
        }

        void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteBots();
        }
    }

    public class RobotProperties
    {
        string m_sScriptFileName = "./perlbot3.pl";
        string m_sTextureBmpFileName = "./metal5a.bmp";
        ProcessPriorityClass m_ProcessPriority = ProcessPriorityClass.Normal;

        public string Script
        {
        	get { return (m_sScriptFileName); }
        	set { m_sScriptFileName = value; }
        }

        public string Texture
        {
            get { return m_sTextureBmpFileName; }
            set { m_sTextureBmpFileName = value; }
        }

        public ProcessPriorityClass Priority
        {
            get { return m_ProcessPriority; }
            set { m_ProcessPriority = value; }
        }

        public RobotProperties()
        {
        }

        public RobotProperties(string script, string texture, ProcessPriorityClass priority)
        {
            m_sScriptFileName = script;
            m_sTextureBmpFileName = texture;
            m_ProcessPriority = priority;
        }
    }

    public class RobotsConfigTools
    {
        string m_sConfigFilePath = "./robots.cfg";
        RobotProperties [] m_aRobotProperties = null;

        public RobotsConfigTools()
        {
            m_sConfigFilePath = "./robots.cfg";
        }

        public RobotsConfigTools(string cfgpath)
        {
            m_sConfigFilePath = cfgpath;
        }

        public static ProcessPriorityClass GetProcessPriorityFromName(string sPriority)
        {
            var ret = (ProcessPriorityClass)Enum.Parse(typeof(ProcessPriorityClass), sPriority);

            return (ret);
        }

        public ArrayList GetRobotsFromConfigFile()
        {
            return GetRobotsFromConfigFile(m_sConfigFilePath);
        }

        public static ArrayList GetRobotsFromConfigFile(string sCfgFile)
        {
            var retList = new ArrayList();
            RobotProperties robotProperties;
            var saRet = new string[255];
            var saPrior = new string[255];
            var saBodyTexture = new string[255];

            if (File.Exists(sCfgFile))
            {
                using (var sr = new StreamReader(sCfgFile))
                {
                    String line;
                    string[] saSplit = null;
					const string sDelim = " ";
                    char[] delim = sDelim.ToCharArray();
                    while ((line = sr.ReadLine()) != null)
                    {
                        robotProperties = new RobotProperties();
                        saSplit = line.Split(delim, 3);
                        robotProperties.Script = saSplit[0];
                        if (2 < saSplit.Length)
                            robotProperties.Texture = saSplit[2];
                        if (1 < saSplit.Length)
                            robotProperties.Priority = GetProcessPriorityFromName (saSplit[1]);
                        retList.Add(robotProperties);
                    }
                }
            }
            else
            {
            	MessageBox.Show(global::RobEnvMK.Properties.Resources.Text_FileDoesNotExist + " " + sCfgFile);
                Application.Exit();
            }

            return (retList);
        }

        public void SaveRobotsConfigFile()
        {
            SaveRobotsConfigFile(m_sConfigFilePath);
        }

        public void SaveRobotsConfigFile(RobotProperties[] arp)
        {
            m_aRobotProperties = arp;
            SaveRobotsConfigFile(m_sConfigFilePath);
        }

        public void SaveRobotsConfigFile(string sCfgFile)
        {
            if (null != m_aRobotProperties && 0 < m_aRobotProperties.Length)
            {
                if (File.Exists(sCfgFile))
                {
                    DateTime tiepier = DateTime.Now;
                    string sDateTimeNow = string.Format("{0}{1}", tiepier.Year + tiepier.Month + tiepier.Day + tiepier.Hour + tiepier.Minute, tiepier.Second);
                    File.Copy(sCfgFile, sCfgFile + "_BAK_" + sDateTimeNow);
                }

                using (var sw = new StreamWriter(sCfgFile))
                {
                    foreach (RobotProperties rp in m_aRobotProperties)
                    {
                        string line = string.Empty;
                        line += rp.Script + " ";
                        line += rp.Priority + " ";
                        line += rp.Texture;
                        sw.WriteLine(line);
                    }
                }

                MessageBox.Show(global::RobEnvMK.Properties.Resources.Text_RobotsConfigSaved);
            }
        }
    }
}
