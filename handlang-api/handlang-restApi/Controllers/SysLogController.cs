using handlang.service.Interface;
using handlang.service.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace handlang_restApi.Controllers
{
    public class SysLogController : ApiController
    {
        private ISysLog _sysLogService;



        public SysLogController(ISysLog sysLogService)
        {
            _sysLogService = sysLogService;
        }

        [HttpPost]
        [Route("api/sysLog/{target}")]
        public async Task<IHttpActionResult> Post([FromUri] string target, [FromBody] LogData data)
        {
            var targets = new string[] { "RF" };
            string msg = "";
            // 設定 db target 的資料庫連線字串
            if (!targets.Contains(target))
            {
                msg = $"參數錯誤:無此類型:{target}";
                await _sysLogService.LogTrace(msg, "");

                return BadRequest(msg);
            }

            if (target == "RF")
            {
                var rawDataFR = JsonConvert.DeserializeObject<RfidLogRawDTO>(data.RawData);
                msg = $"[{rawDataFR.epcid}]{rawDataFR.rdno}-{rawDataFR.regdate}";
                await _sysLogService.Log(data);
            }

            return Ok(new APIResponseWithData<string> { Data = msg });
        }

        public class RfidLogRawDTO
        {
            public string empid { get; set; }
            public string rdno { get; set; }
            public string regdate { get; set; }
            public string epcid { get; set; }
        }




    }
}