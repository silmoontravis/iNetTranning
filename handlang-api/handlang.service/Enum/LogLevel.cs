using System.ComponentModel;

namespace handlang.service.Enum
{
    public enum LogLevel
    {
        [Description("一般訊息")]
        Info = 0,
        [Description("追蹤訊息")]
        Trace = 1,
        [Description("偵錯訊息")]
        Debug = 2,
        [Description("警告訊息")]
        Warn = 4,
        [Description("錯誤訊息")]
        Error = 8,
        [Description("嚴重錯誤訊息")]
        Fatal = 16
    }
}
