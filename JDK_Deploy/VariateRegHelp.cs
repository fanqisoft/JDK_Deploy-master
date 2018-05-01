using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDK_Deploy
{
    public enum Variate
    {
        Sys,
        User,
    }
    public class VariateRegHelp
    {
        /// <summary>
        /// 根据名称获取指定的环境变量
        /// </summary>
        /// <param name="type">枚举环境变量</param>
        /// <param name="name">变量名称</param>
        /// <returns></returns>
        public static string GetEnvironmentByName(Variate type, string name)
        {
            string result = string.Empty;
            try
            {
                result = OpenEnvironmentReg(type).GetValue(name).ToString();//读取
            }
            catch (Exception)
            {
                return string.Empty;
            }
            return result;
        }

        /// <summary>
        /// 打开指定的环境变量注册表
        /// </summary>
        /// <param name="type">枚举环境变量</param>
        /// <returns></returns>
        private static RegistryKey OpenEnvironmentReg(Variate type)
        {
            RegistryKey regEnvironment = null;
            if (type == Variate.Sys)
            {
                RegistryKey regLocalMachine = Registry.LocalMachine;
                RegistryKey regSYSTEM = regLocalMachine.OpenSubKey("SYSTEM", true);//打开HKEY_LOCAL_MACHINE下的SYSTEM 
                RegistryKey regControlSet001 = regSYSTEM.OpenSubKey("ControlSet001", true);//打开ControlSet001 
                RegistryKey regControl = regControlSet001.OpenSubKey("Control", true);//打开Control 
                RegistryKey regManager = regControl.OpenSubKey("Session Manager", true);//打开Control 
                regEnvironment = regManager.OpenSubKey("Environment", true);
            }
            else if (type == Variate.User)
            {
                RegistryKey regCurrentUser = Registry.CurrentUser;
                regEnvironment = regCurrentUser.OpenSubKey("Environment", true);
            }
            return regEnvironment;
        }

        /// <summary>
        /// 设置指定的环境变量
        /// </summary>
        /// <param name="type">枚举环境变量</param>
        /// <param name="name">变量名称</param>
        /// <param name="strValue">变量值</param>
        public static void SetEnvironment(Variate type, string name, string strValue)
        {
            OpenEnvironmentReg(type).SetValue(name, strValue);

        }
        /// <summary>
        /// 移除指定的环境变量
        /// </summary>
        /// <param name="type">枚举环境变量</param>
        /// <param name="name">变量名称</param>
        public static void RemoveEnvironment(Variate type, string name)
        {
            try
            {
                OpenEnvironmentReg(type).DeleteValue(name);
            }
            catch (Exception)
            {

                return;
            }
            
        }

        /// <summary>
        /// 检测指定的环境变量是否存在指定变量
        /// </summary>
        /// <param name="type">枚举环境变量</param>
        /// <param name="name">变量名称</param>
        /// <returns>存在则返回true，否则返回false</returns>
        public static bool CheckEnvironmentExist(Variate type, string name)
        {
            if (!string.IsNullOrEmpty(GetEnvironmentByName(type, name)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 添加到JAVA_HOME环境变量（检测值是否存在，存在则不添加）
        /// </summary>
        /// <param name="type"></param>
        /// <param name="jdkpath"></param>
        public static void AddJavaHome(Variate type, string jdkpath)
        {
            bool exist = CheckEnvironmentExist(type,"JAVA_HOME");
            if (exist)  //如果存在
            {
                string javaHome = GetEnvironmentByName(type,"JAVA_HOME");
                if (javaHome.Substring(javaHome.Length - 1, 1) != ";")
                {
                    SetEnvironment(type,"JAVA_HOME", javaHome + ";");
                    javaHome = GetEnvironmentByName(type,"JAVA_HOME");
                }
                string[] list = javaHome.Split(';');
                foreach (var item in list)
                {
                    if (item == jdkpath)
                    {
                        return;
                    }
                }
                //SetEnvironment(type,"JAVA_HOME", javaHome + jdkpath + ";");
                SetEnvironment(type, "JAVA_HOME", jdkpath + ";");

            }
            else //如果不存在
            {
                SetEnvironment(type,"JAVA_HOME", jdkpath + ";");
            }
        }


        /// <summary>
        /// 添加到PATH环境变量（检测值是否存在，存在则不添加）
        /// </summary>
        /// <param name="strHome"></param>
        public static void AddPathAppend(Variate type, string strHome)
        {
            string pathlist;
            pathlist = GetEnvironmentByName(type, "PATH");
            //检测是否以;结尾
            if (pathlist.Substring(pathlist.Length - 1, 1) != ";")
            {
                SetEnvironment(type, "PATH", pathlist + ";");
                pathlist = GetEnvironmentByName(type, "PATH");
            }
            string[] list = pathlist.Split(';');
            bool isPathExist = false;    //判断附加的值是否已存在

            foreach (string item in list)
            {
                if (item == strHome)
                    isPathExist = true;
            }
            if (!isPathExist)
            {
                SetEnvironment(type, "PATH", pathlist + strHome + ";");
            }
        }

        public static void AddClassPath(Variate type, string path)
        {
            bool exist = CheckEnvironmentExist(type, "CLASSPATH");
            if (exist)  //如果存在
            {
                string classPath = GetEnvironmentByName(type, "CLASSPATH");
                if (classPath.Substring(classPath.Length - 1, 1) != ";")
                {
                    SetEnvironment(type, "CLASSPATH", classPath + ";");
                    classPath = GetEnvironmentByName(type, "CLASSPATH");
                }
                string[] list = classPath.Split(';');
                foreach (var item in list)
                {
                    if (item == path)
                    {
                        return;
                    }
                }
                SetEnvironment(type, "CLASSPATH", classPath + path + ";");

            }
            else //如果不存在
            {
                SetEnvironment(type, "CLASSPATH", path + ";");
            }
        }

        /// <summary>
        /// 移除指定环境变量JavaHome && ClassPath
        /// </summary>
        /// <param name="type"></param>
        public static void RemoveJavaHomeAndClassPath(Variate type)
        {
            RemoveEnvironment(type, "JAVA_HOME");
            RemoveEnvironment(type, "CLASSPATH");
        }
        public static void RemovePathAppend(Variate type)
        {
            string pathlist;
            pathlist = GetEnvironmentByName(type, "PATH");
            //检测是否以;结尾
            if (pathlist.Substring(pathlist.Length - 1, 1) != ";")
            {
                SetEnvironment(type, "PATH", pathlist + ";");
                pathlist = GetEnvironmentByName(type, "PATH");
            }
            List<string> list = pathlist.Split(';').ToList();
            for (int i = list.Count - 1; i > 0; i--)
            {
                if (list[i].ToUpper() == @"%JAVA_HOME%\bin".ToUpper())
                {
                    list.Remove(list[i]);
                    continue;
                }
                if (list[i].ToUpper() == @"%JAVA_HOME%\jre\bin".ToUpper())
                {
                    list.Remove(list[i]);
                    continue;
                }
            }
            pathlist = "";
            foreach (var item in list)
            {
                if(list.IndexOf(item) == list.Count() - 1)
                {
                    pathlist += item;
                }
                else
                {
                    pathlist += item + ";";
                }
            }
            SetEnvironment(type, "PATH", pathlist);
        }
    }
}
