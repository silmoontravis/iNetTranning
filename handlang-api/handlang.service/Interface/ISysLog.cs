using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace handlang.service.Interface
{
    public interface ISysLog
    {
        Task LogTrace(string message, object rawData);
        Task Log(LogData data);
        Task Log(List<LogData> data);
    }

    public class LogData
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Log等級
        /// Info = 0, Trace = 1, Debug = 2, Warn = 4, Error = 8, Fatal = 16
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 識別碼
        /// </summary>
        public string Sender { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? ReferenceId { get; set; }
        /// <summary>
        /// Log代碼
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Log訊息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 額外資訊
        /// </summary>
        public string RawData { get; set; }
        /// <summary>
        /// Log時間
        /// </summary>
        public DateTime? LogTime { get; set; }
    }

    public class WinformRawData
    {
        public string usercode { get; set; }

        public string operation { get; set; }

        public string status { get; set; }

    }

    public class AlarmRawData
    {
        public string operation { get; set; }

        public string status { get; set; }

    }

    public class EnumDTO
    {
        public string Key { get; set; }
        public int Value { get; set; }
        public string Descrription { get; set; }
    }
}
