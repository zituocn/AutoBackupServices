using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Windows.Forms;
using AutoBackupServices.Model;

namespace AutoBackupServices.Common
{
    /// <summary>
    /// ftp操作工具类
    /// </summary>
    public class FTPTools
    {
        static FtpWebRequest ftp;


        /// <summary>
        /// 使用服务器信息连接ftp指定的目录
        /// </summary>
        /// <param name="ip">服务器ip</param>
        /// <param name="port">端口</param>
        /// <param name="uid">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="path">远程目录</param>
        private static void Connect(string ip, int port, string uid, string password, string path, string filename = null)
        {
            string server = "ftp://" + ip + ":" + port + "/";
            if (!string.IsNullOrEmpty(path))
                server = server + path + "/";
            if (!string.IsNullOrEmpty(filename))
                server = server + "/" + filename;
            ftp = (FtpWebRequest)FtpWebRequest.Create(new Uri(server));
            ftp.UseBinary = true;
            //ftp.UsePassive = true;
            ftp.KeepAlive = true;
            ftp.Credentials = new NetworkCredential(uid, password);
        }


        /// <summary>
        /// 取服务器ftp根目录下的最新文件列表
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="uid"></param>
        /// <param name="password"></param>
        /// <param name="oldlist">待删除的文件名称</param>
        /// <returns></returns>
        public static List<BakInfo> GetNewFileList(string ip, int port, string uid, string password, ref List<BakInfo> oldlist)
        {
            List<BakInfo> list = new List<BakInfo>();
            List<string> files = GetFileList(ip, port, uid, password, "", WebRequestMethods.Ftp.ListDirectoryDetails);
            oldlist = new List<BakInfo>();
            List<string> dir = new List<string>();

            ///列出根目录下的所有目录
            if (files != null)
            {
                foreach (string s in files)
                {
                    if (s.Substring(0, 2).Equals("dr"))
                    {
                        string temp = s.Substring(s.LastIndexOf(' ') + 1);
                        dir.Add(temp);
                    }
                }
            }

            ///
            ///分别取出目录下的最新文件和待删除的文件
            ///
            if (dir.Count > 0)
            {
                foreach (string s in dir)
                {
                    List<string> tempfiles = GetFileList(ip, port, uid, password, s);
                    if (tempfiles != null && tempfiles.Count > 0)
                    {
                        for (int i = 0; i < tempfiles.Count; i++)
                        {
                            BakInfo info = new BakInfo();
                            info.path = s;
                            info.filename = tempfiles[i];

                            if (i == tempfiles.Count - 1)
                                list.Add(info);
                            else
                                oldlist.Add(info);
                        }
                    }
                }
            }

            return list;

        }

        #region 基本的目录列表方法
        /// <summary>
        /// 取某服务器 某目录下的所有文件
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="uid"></param>
        /// <param name="password"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private static List<string> GetFileList(string ip, int port, string uid, string password, string path)
        {
            return GetFileList(ip, port, uid, password, path, WebRequestMethods.Ftp.ListDirectory);
        }

        /// <summary>
        /// 获取某个服务器上某个目录下的文件列表..
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="uid"></param>
        /// <param name="password"></param>
        /// <param name="path"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private static List<string> GetFileList(string ip, int port, string uid, string password, string path, string method)
        {
            List<string> list = new List<string>();
            ArrayList files = new ArrayList();
            StringBuilder result = new StringBuilder();
            StreamReader reader = null;
            WebResponse response = null;
            try
            {
                Connect(ip, port, uid, password, path);
                ftp.Method = method;

                response = ftp.GetResponse();
                reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8);
                string line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line + "\n");
                    line = reader.ReadLine();
                }
                result.Remove(result.ToString().Length - 1, 1);
                list = new List<string>(result.ToString().Split('\n'));

            }
            catch (Exception ex)
            {
                Utils.Log(ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (response != null)
                    response.Close();
            }


            return list;
        }
        #endregion

        #region 获取服务器上文件的大小
        /// <summary>
        /// 获取ftp server上指定文件的大小..
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="uid"></param>
        /// <param name="password"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static long GetFileSize(string ip, int port, string uid, string password, BakInfo info)
        {
            FtpWebResponse response = null;
            long size = 0;
            try
            {
                Connect(ip, port, uid, password, info.path, info.filename);
                ftp.Method = WebRequestMethods.Ftp.GetFileSize;
                response = (FtpWebResponse)ftp.GetResponse();
                size = response.ContentLength;
            }
            catch (Exception ex)
            {
                Utils.Log(ex.Message);
            }
            finally
            {
                if (response != null)
                    response.Close();
            }
            return size;
        }
        #endregion

        #region 删除服务器上的指定文件
        /// <summary>
        /// 删除服务器上的指定文件
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="uid"></param>
        /// <param name="password"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static void DeleteFile(string ip, int port, string uid, string password, BakInfo info)
        {
            FtpWebResponse response = null;
            try
            {
                Connect(ip, port, uid, password, info.path, info.filename);
                ftp.Method = WebRequestMethods.Ftp.DeleteFile;
                response = (FtpWebResponse)ftp.GetResponse();
            }
            catch (Exception ex)
            {
                Utils.Log(ex.Message);
            }
            finally
            {
                if (response != null)
                    response.Close();
            }
        }
        #endregion

        #region 下载文件到本地
        /// <summary>
        /// 下载文件保存到本地目录..
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="uid"></param>
        /// <param name="password"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static int DownLoadFile(string ip, int port, string uid, string password, BakInfo info)
        {
            FtpWebResponse ftpWebResponse = null;
            Stream ftpResponseStream = null;
            FileStream outputStream = null;
            int status = 0; //失败的状态
            try
            {
                string localroot = Utils.GetConfigInfo().localroot;
                if (string.IsNullOrEmpty(localroot))
                    return -1;

                string localpath = localroot + info.path;

                if (!Directory.Exists(localpath))
                    Directory.CreateDirectory(localpath);

                ///如果本地文件已经存在，删除本地文件，然后重新下载..
                if (File.Exists(localpath + "\\" + info.filename))
                {
                    FileInfo file = new FileInfo(localpath + "\\" + info.filename);
                    long remotesize = GetFileSize(ip, port, uid, password, info);
                    long localsize = file.Length;
                    ///下载不完整的情况...
                    if (remotesize != localsize)
                    {
                        File.Delete(localpath + "\\" + info.filename);
                    }
                    else
                    {
                        ///
                        ///本地已经有最新文件，并且和服务器上大小一样
                        ///返回
                        ///
                        status = 8;
                        return status;
                    }
                }


                outputStream = new FileStream(localpath + "\\" + info.filename, FileMode.Create);
                Connect(ip, port, uid, password, info.path, info.filename);
                ftp.Method = WebRequestMethods.Ftp.DownloadFile;
                ftpWebResponse = (FtpWebResponse)ftp.GetResponse();
                ftpResponseStream = ftpWebResponse.GetResponseStream();
                long contentLength = ftpWebResponse.ContentLength;
                int bufferSize = 2048;
                byte[] buffer = new byte[bufferSize];
                int readCount;
                readCount = ftpResponseStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpResponseStream.Read(buffer, 0, bufferSize);
                }
                status = 9; //下载成功
            }
            catch (Exception ex)
            {
                Utils.Log(ex.Message);
            }
            finally
            {
                if (outputStream != null)
                    outputStream.Close();
                if (ftpResponseStream != null)
                    ftpResponseStream.Close();
                if (ftpWebResponse != null)
                    ftpWebResponse.Close();
            }
            return status;
        }
        #endregion

    }
}
