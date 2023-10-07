using handlang.service.Interface;
using handlang.service.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace handlang_restApi.Controllers
{
    public class SysCodeController : ApiController
    {
        private ICode _codeService;
        public SysCodeController(ICode codeService)
        {
            _codeService = codeService;
        }

        [HttpGet]
        [Route("api/codeTable")]
        public async Task<IHttpActionResult> GetCodeTable()
        {
            try
            {
                var result = await _codeService.GetCodeAsDictionary();
                return Ok(new APIResponseWithData<Dictionary<string, object>>() { Data = result });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("api/updateApp")]
        public async Task<IHttpActionResult> UpdateApp(UpdateAppRequest param)
        {
            try
            {
                List<CodeDTOV1> appList = await _codeService.GetByType("APP");
                CodeDTOV1 currentApp = appList.Where(x => x.CodeId == param.Type).FirstOrDefault();
                List<CodeDTOV1> verList = await _codeService.GetByType("VER");
                CodeDTOV1 currentVersion = verList.Where(x => x.CodeId == param.Type).FirstOrDefault();

                if (param.Type == "ios")
                {
                    param.Version = param.Version.Replace(" ", "-");
                    string path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/npa.plist");
                    string plistPath = param.IsProd ? "itms-services://?action=download-manifest&url=https://sdm.cib.gov.tw:8831/Install/ios/{0}.plist"
                        : "itms-services://?action=download-manifest&url=https://l2.ibb.tw/motorent168/ipa/npa/{0}.plist";
                    plistPath = string.Format(plistPath, param.Version);

                    await _codeService.Update(new CodeDTOV1
                    {
                        CodeType = currentApp.CodeType,
                        CodeId = currentApp.CodeId,
                        DtEffect = currentApp.DtEffect,
                        CodeName = currentApp.CodeName,
                        Seq = currentApp.Seq,
                        IntVal = currentApp.IntVal,
                        IsValid = currentApp.IsValid,
                        StrVal = plistPath
                    });

                    string content = File.ReadAllText(path);

                    content = string.Format(content,
                    ConfigurationManager.AppSettings["WebHost"].ToString(), param.Version,
                        string.Format("npa-{0}{1}.ipa", param.Version, param.IsProd ? "-production" : ""));

                    var virtualPath = System.Web.Hosting.HostingEnvironment.MapPath("~/install");
                    var contentBytes = Encoding.UTF8.GetBytes(content);
                    using (FileStream _file = new FileStream(string.Format("{0}/ios/{1}.plist", virtualPath, param.Version), FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                    {
                        _file.Seek(0, SeekOrigin.End);
                        _file.Write(contentBytes, 0, contentBytes.Length);
                    }
                }

                await _codeService.Update(new CodeDTOV1
                {
                    CodeType = currentVersion.CodeType,
                    CodeId = currentVersion.CodeId,
                    DtEffect = currentVersion.DtEffect,
                    CodeName = currentVersion.CodeName,
                    Seq = currentVersion.Seq,
                    IntVal = currentVersion.IntVal,
                    IsValid = currentVersion.IsValid,
                    StrVal = param.Version
                });


                return Ok(new APIResponse());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public class UpdateAppRequest
        {
            /// <summary>
            /// 更新類型：ios, android
            /// </summary>
            public string Type { get; set; }
            /// <summary>
            /// 更新版號
            /// </summary>
            public string Version { get; set; }
            /// <summary>
            /// 是否為Production
            /// </summary>
            public bool IsProd { get; set; }
        }

    }
}