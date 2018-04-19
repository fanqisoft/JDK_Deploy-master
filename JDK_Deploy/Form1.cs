using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace JDK_Deploy
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.Description = "请选择JDK安装路径";
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }
        /// <summary>
        /// 写入环境变量点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == string.Empty)
            {
                MessageBox.Show("请选择JDK的安装路径！", "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (radioButton1.Checked == false && radioButton2.Checked == false)
            {
                MessageBox.Show("请选择写入变量的位置！", "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string jdkpath = textBox1.Text;
            if(radioButton1.Checked) //系统变量
            {
                VariateRegHelp.AddJavaHome(Variate.Sys, jdkpath);
                VariateRegHelp.AddPathAppend(Variate.Sys, @"%JAVA_HOME%\bin");
                VariateRegHelp.AddPathAppend(Variate.Sys, @"%JAVA_HOME%\jre\bin");
                VariateRegHelp.AddClassPath(Variate.Sys, @".");
                VariateRegHelp.AddClassPath(Variate.Sys, @"%JAVA_HOME%\lib");
                VariateRegHelp.AddClassPath(Variate.Sys, @"%JAVA_HOME%\lib\tools.jar");
            }
            if(radioButton2.Checked) //用户变量
            {
                VariateRegHelp.AddJavaHome(Variate.User, jdkpath);
                VariateRegHelp.AddPathAppend(Variate.User, @"%JAVA_HOME%\bin");
                VariateRegHelp.AddPathAppend(Variate.User, @"%JAVA_HOME%\jre\bin");
                VariateRegHelp.AddClassPath(Variate.User, @".");
                VariateRegHelp.AddClassPath(Variate.User, @"%JAVA_HOME%\lib");
                VariateRegHelp.AddClassPath(Variate.User, @"%JAVA_HOME%\lib\tools.jar");
            }
            MessageBox.Show("JDK变量已成功写入！", "写入成功!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Process.Start("cmd.exe", "/k java -version");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration dec =  doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(dec);
            XmlElement variate =  doc.CreateElement("Variate");
            doc.AppendChild(variate);

            //用户Start
            XmlElement user = doc.CreateElement("User");
            variate.AppendChild(user);
            XmlElement userPath = doc.CreateElement("path");
            userPath.InnerText = VariateRegHelp.GetEnvironmentByName(Variate.User, "PATH");
            user.AppendChild(userPath);

            //系统Start
            XmlElement sys = doc.CreateElement("Sys");
            variate.AppendChild(sys);
            XmlElement sysPath = doc.CreateElement("path");
            sysPath.InnerText = VariateRegHelp.GetEnvironmentByName(Variate.Sys, "PATH");
            sys.AppendChild(sysPath);

            string fileName = DateTime.Now.ToString("yyyy-MM-dd HHmmss") + ".xml";
            doc.Save(fileName);
            MessageBox.Show("环境变量已成功备份！", "备份成功!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string fileName = string.Empty;
            openFileDialog1.InitialDirectory = System.Environment.CurrentDirectory;
            openFileDialog1.Filter = "XML文件|*.xml";
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.FilterIndex = 1;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog1.FileName;
            }
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);
            XmlNode xn = doc.SelectSingleNode("/Variate/User/path");
            string msg = xn.InnerText;
            VariateRegHelp.SetEnvironment(Variate.User, "PATH", msg);
            xn = doc.SelectSingleNode("/Variate/Sys/path");
            msg = xn.InnerText;
            VariateRegHelp.SetEnvironment(Variate.Sys, "PATH", msg);
            MessageBox.Show("环境变量已成功还原！", "还原成功!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }
    }
}
