using System;

namespace AutoBackupServices.Model
{
    /// <summary>
    /// 备份文件信息
    /// </summary>
    public class BakInfo
    {
        /// <summary>
        /// 目录
        /// </summary>
        public string path { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string filename { get; set; }
    }
}
