 using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using AutoBackupServices.Model;
using AutoBackupServices.Common;

namespace AutoBackupServices
{
    public partial class AutoBackup : ServiceBase
    {
        public AutoBackup()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }

        /// <summary>
        /// 计时器事件入口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            int hour = System.DateTime.Now.Hour;
            int sethour = Utils.GetConfigInfo().hour;
            if (hour != sethour)
            {
                this.timer1.Enabled = false;
                //Utils.Log("$$服务正常扫描一次，但没有下载文件...");
                this.timer1.Enabled = true;
            }
            else
            {

                this.timer1.Enabled = false;
                List<BakInfo> oldlist = new List<BakInfo>();
                List<ServerInfo> list = Utils.GetServerList();
                if (list != null && list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        ServerInfo info = list[i];
                        List<BakInfo> files = FTPTools.GetNewFileList(info.ip, info.port, info.uid, info.password, ref oldlist);

                        ///
                        ///下载最新的文件
                        ///
                        if (files != null && files.Count > 0)
                        {
                            for (int j = 0; j < files.Count; j++)
                            {

                                Console.WriteLine("开始下载：" + files[j].path + "/" + files[j].filename);
                                Utils.WriteSuccess("==========================================================");
                                Utils.WriteSuccess("开始下载：" + files[j].path + "\\" + files[j].filename);
                                DateTime begintime = System.DateTime.Now;
                                int status = FTPTools.DownLoadFile(info.ip, info.port, info.uid, info.password, files[j]);
                                if (status == 9)
                                {
                                    DateTime endtime = System.DateTime.Now;

                                    TimeSpan ts = endtime - begintime;
                                    int min = (int)ts.TotalMinutes;
                                    int se = (int)ts.TotalSeconds;

                                    Console.WriteLine("@@下载成功@@ " + files[j].path + "/" + files[j].filename);
                                    Utils.WriteSuccess("@@下载成功 耗时约 " + min + " 分钟/ " + se + " 秒 " + files[j].path + "\\" + files[j].filename);
                                }
                                else if (status == 8)
                                {
                                    Console.WriteLine("本地文件已存在...");
                                    Utils.WriteSuccess("本地文件已存在...");
                                }
                                else if (status == 0)
                                {
                                    Console.WriteLine("!!下载失败!! " + files[j].path + "/" + files[j].filename);
                                    Utils.WriteSuccess("!!下载失败!! " + files[j].path + "\\" + files[j].filename);
                                }
                                else if (status == -1)
                                {
                                    Console.WriteLine("本地目录配置错误...");
                                    Utils.WriteSuccess("本地目录配置错误...");
                                }

                                Utils.WriteSuccess("==========================================================");
                                Utils.WriteSuccess("");
                                Utils.WriteSuccess("");
                            }
                        }

                        ///
                        ///删除旧文件.
                        ///
                        if (oldlist != null && oldlist.Count > 0)
                        {
                            for (int j = 0; j < oldlist.Count; j++)
                            {
                                FTPTools.DeleteFile(info.ip, info.port, info.uid, info.password, oldlist[j]);
                            }
                        }

                    }
                }

                this.timer1.Enabled = true;
            }
        }


    }
}
