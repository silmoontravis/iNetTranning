using handlang.service.Domain;
using handlang.service.Interface;
using iNet.Service;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace handlang.service.Service
{
    public class CodeService : CommonService<SysCodeTable, Guid>, ICode
    {
        public async Task<Dictionary<string, object>> GetCodeAsDictionary()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            try
            {
                List<string> verType = new List<string>() { "VER", "APP" };

                var verList = _db.SysCodeTable.AsNoTracking()
                    .Where(x => verType.Contains(x.CodeType))
                    .ToList();
                Dictionary<string, object> tempList = new Dictionary<string, object>();
                foreach (var version in verList)
                {
                    result.Add(version.CodeName, version.StrVal);
                }

                return result;
            }
            catch
            {
                throw;
            }
        }

        private async Task<String> GetSerialNumber(SerialNumberType request)
        {
            var result = await GetSerialNumbers(request, new List<object>());
            return result.First();
        }

        private async Task<String> GetSerialNumber(SerialNumberType request, List<Object> para)
        {
            var result = await GetSerialNumbers(request, para);
            return result.First();
        }

        private async Task<List<String>> GetSerialNumbers(SerialNumberType request, List<Object> para, int batchNumber = 1)
        {
            BeginTrans();
            List<String> serialNumbers = new List<string>();
            var snList = _db.SysCodeTable.Where(x =>
                    x.CodeType == request.codeType && x.CodeId == request.codeId && x.DtEffect == request.dtEffect && x.IsValid == true
                ).OrderBy(x => x.Seq).ToList();

            if (snList.Count == 1)
            {
                for (var i = 1; i <= batchNumber; i++)
                {
                    snList[0].IntVal += 1;
                    List<Object> ary = new List<object>() { snList[0].IntVal };
                    ary.AddRange(para);
                    serialNumbers.Add(string.Format(snList[0].StrVal, ary.ToArray()));
                }
            }
            else
            {
                var sn = new SysCodeTable()
                {
                    CodeType = request.codeType,
                    CodeId = request.codeId,
                    DtEffect = request.dtEffect,
                    StrVal = request.pattern,
                    Seq = 0,
                    IntVal = 1,
                    IsValid = true,
                    CodeDesc = "",
                    CodeName = request.codeName
                };
                for (var i = sn.IntVal; i <= batchNumber; i++)
                {
                    List<Object> ary = new List<object>() { i };
                    ary.AddRange(para);
                    serialNumbers.Add(string.Format(sn.StrVal, ary.ToArray()));
                    sn.IntVal = i;
                }
                _db.SysCodeTable.Add(sn);
            }
            await _db.SaveChangesAsync();
            CommitTrans();
            return serialNumbers;
        }

        public async Task<String> GetNormalCaseNo()
        {
            try
            {
                var today = DateTime.Now;
                //string snNumber = string.Format("{0:000}{1:MM}{1:dd}{2}", today.Year - 1911, today, organCode);
                string snNumber = string.Format("{0:000}{1:MM}{1:dd}", today.Year - 1911, today);
                string snPattern = snNumber + "0{0:0000}";
                SerialNumberType snType = new SerialNumberType()
                {
                    codeType = "SN",
                    codeId = "NORM",
                    codeName = "正式案號",
                    dtEffect = snNumber,
                    pattern = snPattern
                };

                string serialNumber = await GetSerialNumber(snType);
                return serialNumber;
            }
            catch
            {
                throw;
            }
        }

        public async Task<String> GetTempCaseNo()
        {
            try
            {
                var today = DateTime.Now;
                //string snNumber = string.Format("{0:000}{1:MM}{1:dd}{2}", today.Year - 1911, today, organCode);
                string snNumber = string.Format("{0:000}{1:MM}{1:dd}", today.Year - 1911, today);
                string snPattern = snNumber + "9{0:0000}";
                SerialNumberType snType = new SerialNumberType()
                {
                    codeType = "SN",
                    codeId = "TEMP",
                    codeName = "臨時案號",
                    dtEffect = snNumber,
                    pattern = snPattern
                };

                string serialNumber = await GetSerialNumber(snType);
                return serialNumber;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<CodeDTOV1>> GetByType(string CodeType)
        {
            try
            {
                var query = await _db.SysCodeTable.AsNoTracking().Where(x => x.CodeType == CodeType && x.IsValid == true).OrderBy(x => x.Seq).ToListAsync();
                return query.Select(x => new CodeDTOV1
                {
                    CodeType = x.CodeType,
                    CodeId = x.CodeId,
                    DtEffect = x.DtEffect,
                    CodeName = x.CodeName,
                    CodeDesc = x.CodeDesc,
                    Seq = x.Seq,
                    IntVal = x.IntVal,
                    StrVal = x.StrVal,
                    IsValid = x.IsValid
                }).ToList(); ;
            }
            catch
            {
                throw;
            }
        }


        public async Task Update(CodeDTOV1 code)
        {
            try
            {
                var item = await _db.SysCodeTable.Where(x => x.CodeType == code.CodeType && x.CodeId == code.CodeId && x.DtEffect == code.DtEffect).FirstOrDefaultAsync();
                if (item == null) throw new Exception(string.Format("Code不存在！Type={0},Id={1},DtEffect={2}", code.CodeType, code.CodeId, code.DtEffect));
                item.CodeName = code.CodeName;
                item.CodeDesc = code.CodeDesc;
                item.IntVal = code.IntVal;
                item.StrVal = code.StrVal;
                item.Seq = code.Seq;
                item.IsValid = code.IsValid;

                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
