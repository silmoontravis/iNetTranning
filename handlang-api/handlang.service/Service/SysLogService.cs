using handlang.service.Domain;
using handlang.service.Interface;
using iNet.Helper;
using Newtonsoft.Json;
using NLog;
using NLog.Common;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace handlang.service.Service
{
    public class SysLogService : ISysLog
    {
        public async Task LogTrace(string message, object rawData)
        {
            await Log(new LogData()
            {
                Id = GuidHelper.GetSeqGuid(),
                Level = Enum.LogLevel.Trace.ToNumberValue(),
                Sender = "iNet",
                Code = "",
                Message = message,
                RawData = JsonConvert.SerializeObject(rawData),
                LogTime = DateTime.Now
            });
        }
        public async Task Log(LogData data)
        {
            try
            {
                LogManager.Configuration.RemoveTarget("SysLog");
                //動態建立iNet的target
                DatabaseTarget target = new DatabaseTarget();
                DatabaseParameterInfo param;
                var conn = ConfigurationManager.ConnectionStrings["DB_Dmz"].ConnectionString;
                var realConnString = EncryptHelper.dec(conn);
                target.Name = "SysLog";

                target.ConnectionString = realConnString;
                target.CommandText = @"INSERT INTO SysLog
                                    ([Id]
                                    ,[Sender]
                                    ,[ReferenceId]
                                    ,[Code]
                                    ,[Message]
                                    ,[Level]
                                    ,[RawData]
                                    ,[LogTime])
                                    VALUES
                                    (CASE WHEN @Id = '' THEN NEWID() ELSE convert(uniqueidentifier, @Id) END
                                    ,@Sender
                                    ,CASE WHEN @ReferenceId = '' THEN NULL ELSE convert(uniqueidentifier, @ReferenceId) END
                                    ,@Code
                                    ,@Message
                                    ,@Level
                                    ,@RawData
                                    ,@LogTime)";
                param = new DatabaseParameterInfo();
                param.Name = "@Id";
                param.Layout = "${event-properties:item=Id}";
                target.Parameters.Add(param);
                param = new DatabaseParameterInfo();
                param.Name = "@Sender";
                param.Layout = "${event-properties:item=Sender}";
                target.Parameters.Add(param);
                param = new DatabaseParameterInfo();
                param.Name = "@ReferenceId";
                param.Layout = "${event-properties:item=ReferenceId}";
                target.Parameters.Add(param);
                param = new DatabaseParameterInfo();
                param.Name = "@Code";
                param.Layout = "${event-properties:item=Code}";
                target.Parameters.Add(param);
                param = new DatabaseParameterInfo();
                param.Name = "@Message";
                param.Layout = "${event-properties:item=Message}";
                target.Parameters.Add(param);
                param = new DatabaseParameterInfo();
                param.Name = "@Level";
                param.Layout = "${event-properties:item=Level}";
                target.Parameters.Add(param);
                param = new DatabaseParameterInfo();
                param.Name = "@RawData";
                param.Layout = "${event-properties:item=RawData}";
                target.Parameters.Add(param);
                param = new DatabaseParameterInfo();
                param.Name = "@LogTime";
                param.Layout = "${event-properties:item=LogTime}";
                target.Parameters.Add(param);
                LogManager.Configuration.AddRuleForAllLevels(target);
                LogManager.ReconfigExistingLoggers();
                //建立log存入物件
                var info = new LogEventInfo(LogLevel.Info, "SysLog", "");
                info.Properties["Id"] = Guid.NewGuid();
                info.Properties["Sender"] = data.Sender;
                info.Properties["ReferenceId"] = data.ReferenceId;
                info.Properties["Code"] = data.Code;
                info.Properties["Message"] = data.Message;
                info.Properties["Level"] = data.Level;
                info.Properties["RawData"] = data.RawData;
                info.Properties["LogTime"] = data.LogTime;
                LogManager.ThrowExceptions = true;
                LogManager.GetLogger("SysLog").Log(info);
            }
            catch (Exception e)
            {
                var logger = LogManager.GetLogger("File");
                logger.Error(e);
            }
        }
        public async Task Log(List<LogData> datas)
        {
            try
            {
                //設定NLog的連線字串
                var dbTarget = LogManager.Configuration.FindTargetByName("SysLog") as DatabaseTarget;
                var conn = ConfigurationManager.ConnectionStrings["DB_iNet"].ConnectionString;
                var realConnString = EncryptHelper.dec(conn);
                if (dbTarget != null) dbTarget.ConnectionString = realConnString;

                LogManager.ReconfigExistingLoggers();
                LogManager.ThrowExceptions = true;
                InternalLogger.LogLevel = LogLevel.Error;
                foreach (var data in datas)
                {
                    //建立log存入物件
                    var info = new LogEventInfo(LogLevel.Info, "SysLog", "");
                    info.Properties["Id"] = GuidHelper.GetSeqGuid();
                    info.Properties["Sender"] = data.Sender;
                    info.Properties["ReferenceId"] = data.ReferenceId;
                    info.Properties["Code"] = data.Code;
                    info.Properties["Message"] = data.Message;
                    info.Properties["Level"] = EnumHelper.ToNumberValue(Enum.LogLevel.Info);
                    info.Properties["RawData"] = data.RawData;
                    info.Properties["LogTime"] = data.LogTime;
                    LogManager.GetLogger("SysLog").Log(info);
                }
            }
            catch (Exception e)
            {
                //_Logger = LogManager.GetLogger("File");
                //_Logger.Error(e);
            }
        }
    }
}
