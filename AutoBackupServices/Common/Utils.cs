using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Windows.Forms;
using AutoBackupServices.Model;

namespace AutoBackupServices.Common
{
    public class Utils
    {

        /// <summary>
        /// 读取本地的配置文件信息
        /// </summary>
        /// <returns></returns>
        public static ConfigInfo GetConfigInfo()
        {
            string path = Application.StartupPath.ToString() + "\\config.json";
            ConfigInfo info = new ConfigInfo();
            string json = string.Empty;
            StreamReader sr = File.OpenText(path);
            try
            {
                json = sr.ReadToEnd();
                if (!string.IsNullOrEmpty(json))
                    info = JSONUtil.ParseFormByJson<ConfigInfo>(json);
            }
            catch (Exception ex)
            {
                Log("读取配置文件config.json出错 " + ex.Message);
            }
            finally
            {
                if (sr != null)
                    sr.Close();
            }


            return info;
        }

        /// <summary>
        /// 读取配置文件中的FTP服务信息列表
        /// </summary>
        /// <returns></returns>
        public static List<ServerInfo> GetServerList()
        {
            List<ServerInfo> list = new List<ServerInfo>();
            string path = Application.StartupPath.ToString() + "\\server.json";
            string json = string.Empty;
            StreamReader sr = File.OpenText(path);
            try
            {
                json = sr.ReadToEnd();
                if (!string.IsNullOrEmpty(json))
                    list = JSONUtil.ParseFormByJson<List<ServerInfo>>(json);
            }
            catch (Exception ex)
            {
                Log("读取服务器配置文件server.json出错 " + ex.Message);
            }
            finally
            {
                if (sr != null)
                    sr.Close();
            }
            return list;
        }

        /// <summary>
        /// 记录日志 
        /// </summary>
        /// <param name="msg"></param>
        public static void Log(string msg)
        {
            msg = msg.Replace("\r\n", " ");
            string path = Application.StartupPath.ToString() + "\\log.txt";
            StreamWriter sw = new StreamWriter(path, true);
            sw.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd:HH:mm:ss") + "：" + msg);
            sw.Close();
        }

        /// <summary>
        /// 写成功的日志...
        /// </summary>
        /// <param name="msg"></param>
        public static void WriteSuccess(string msg)
        {
            string path = GetConfigInfo().localroot + "log.txt";
            StreamWriter sw = new StreamWriter(path, true);
            if (!string.IsNullOrEmpty(msg))
                sw.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd:HH:mm:ss") + "：" + msg);
            else
                sw.WriteLine("");
            sw.Close();
        }
    }
}
