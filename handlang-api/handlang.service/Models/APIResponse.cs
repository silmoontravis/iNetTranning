namespace handlang.service.Models
{
    public class APIResponseWithData<T>
    {
        public APIResponseWithData()
        {

        }
        public APIResponseWithData(T data)
        {
            Data = data;
        }
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public T Data { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class APIResponse //只回傳是否成功，無Data
    {
        public APIResponse()
        {

        }
        public APIResponse(string message)
        {
            Message = message;
        }
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } = string.Empty;
    }
}
