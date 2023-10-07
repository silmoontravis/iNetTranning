using handlang.service.Domain;
using iNet.Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace handlang.service.Interface
{
    public interface ICode : ICommonService<SysCodeTable, Guid>
    {
        Task<Dictionary<string, object>> GetCodeAsDictionary();
        Task<String> GetNormalCaseNo();
        Task<String> GetTempCaseNo();
        Task<List<CodeDTOV1>> GetByType(string CodeType);
        Task Update(CodeDTOV1 code);
    }

    public class CodeDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, Object> Categories { get; set; }
    }

    public class SerialNumberType
    {
        public string codeId { get; set; }
        public string codeType { get; set; }
        public string codeName { get; set; }

        public string dtEffect { get; set; }
        public string pattern { get; set; }
    }

    public class CodeDTOV1
    {
        public string CodeType { get; set; }
        public string CodeId { get; set; }
        public string CodeName { get; set; }
        public string CodeDesc { get; set; }
        public int? Seq { get; set; }
        public int? IntVal { get; set; }
        public string StrVal { get; set; }
        public bool IsValid { get; set; }
        public string DtEffect { get; set; }
        public List<CodeDTO> Children { get; set; }
    }
}
