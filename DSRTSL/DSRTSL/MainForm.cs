using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
namespace DSRTSL
{
    public partial class MainForm : Form
    {
        globalKeyboardHook gkh = new globalKeyboardHook();
        config conf;
        string configfilepath;
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            gkh.HookedKeys.Add(Keys.F5);
            gkh.HookedKeys.Add(Keys.F8);
            gkh.KeyUp += Gkh_KeyUp;
            string apppath =new Uri( System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).LocalPath;
            configfilepath = apppath + "\\config.bin";
            if(File.Exists(configfilepath))
            {
                try
                {
                    IFormatter formatter = new BinaryFormatter();
                    Stream stream = new FileStream(configfilepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    conf = (config)formatter.Deserialize(stream);
                    stream.Close();
                }
                catch (Exception)
                {
                    conf = null;
                }
                
            }
            else
            {
                conf = null;
            }
        }

        private void Gkh_KeyUp(object sender, KeyEventArgs e)
        {
            //save file || dir not set
            if (null==conf)
                return;
            if ("" == conf.backupdirpath || "" == conf.savefilepath)
                return;
            //save
            if(e.KeyCode==Keys.F5)
            {
                string latest= Directory.CreateDirectory(conf.backupdirpath + "\\latest").FullName;
                string newpath=Directory.CreateDirectory(conf.backupdirpath + "\\" + DateTime.Now.ToString("yyyyMMdd_HHmmss")).FullName;
                File.Copy(conf.savefilepath, newpath + "\\" + Path.GetFileName(conf.savefilepath));
                File.Copy(conf.savefilepath, latest + "\\" + Path.GetFileName(conf.savefilepath),true);
            }
            //load
            if(e.KeyCode==Keys.F8)
            {
                string latest = conf.backupdirpath + "\\latest";
                File.Copy(latest + "\\" + Path.GetFileName(conf.savefilepath),conf.savefilepath,true);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(null==conf)
            {
                conf = new config();
            }
            OpenFileDialog ofd = new OpenFileDialog();
            if(DialogResult.OK==ofd.ShowDialog())
            {
                conf.savefilepath = ofd.FileName;
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(configfilepath, FileMode.Create);
                formatter.Serialize(stream, conf);
                stream.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (null == conf)
            {
                conf = new config();
            }
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (DialogResult.OK == fbd.ShowDialog())
            {
                conf.backupdirpath = fbd.SelectedPath;
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(configfilepath, FileMode.Create);
                formatter.Serialize(stream, conf);
                stream.Close();
            }
        }
    }
    [Serializable]
    class config
    {
        public string savefilepath="";
        public string backupdirpath="";
    }
}
