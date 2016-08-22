using System;

namespace AutoBackupServices.Model
{
    /// <summary>
    /// 服务器配置信息
    /// </summary>
    public class ServerInfo
    {
        /// <summary>
        /// 服务器ip
        /// </summary>
        public string ip { get; set; }

        /// <summary>
        /// ftp端口号
        /// </summary>
        public int port { get; set; }

        /// <summary>
        /// ftp帐号
        /// </summary>
        public string uid { get; set; }

        /// <summary>
        /// ftp密码
        /// </summary>
        public string password { get; set; }



    }
}
