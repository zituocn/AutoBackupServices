using System;

namespace AutoBackupServices.Model
{
    /// <summary>
    /// 本地配置类..
    /// </summary>
    public class ConfigInfo
    {
        /// <summary>
        /// 本地保存文件的地址
        /// e:\xxx\
        /// </summary>
        public string localroot { get; set; }

        /// <summary>
        /// 选择备份的时间
        /// 小时值
        /// 如晚上1点，设置成1
        /// 下午15 设置成15
        /// </summary>
        public int hour { get; set; }
    }
}
