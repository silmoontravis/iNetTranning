using System.Net.Http;


namespace mjib_minipc
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageHandler : DelegatingHandler
    {
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public void SaveLog(HttpRequestMessage request, HttpResponseMessage response)
        {
            var caller = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string spTxt1 = "----------------------------------------------------------------------------------------------";
            string spTxt2 = "==============================================================================================";

            string requestContent = request.Content?.ReadAsStringAsync().Result.ToString() ?? "";
            string responseContent = response.Content?.ReadAsStringAsync().Result.ToString() ?? "";

            string resultMsg = "\r\nRequest:\r\n" + request.ToString() + "\r\nRequest Content:\r\n" + requestContent + "\r\n" + spTxt1 + "\r\nResponse:\r\n" + response.ToString() + "\r\nResponse Content :\r\n " + responseContent + "\r\n" + spTxt2 + "\r\n";

            if (response.IsSuccessStatusCode)
            {
                logger.Info(resultMsg);
            }
            else
            {
                if (response.StatusCode != System.Net.HttpStatusCode.NotFound &&
                    response.StatusCode != System.Net.HttpStatusCode.Forbidden &&
                    response.StatusCode != System.Net.HttpStatusCode.Unauthorized &&
                    response.StatusCode != System.Net.HttpStatusCode.BadRequest)
                {
                    logger.Error(resultMsg);
                }
            }
        }

        public void WriteLog(string message)
        {
            var caller = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string spTxt1 = "----------------------------------------------------------------------------------------------";
            string spTxt2 = "==============================================================================================";

            string resultMsg = "\r\nCaller:\r\n" + caller.ToString() + "\r\n" + spTxt1 + "\r\nMessage:\r\n" + message + "\r\n" + spTxt2 + "\r\n";

            logger.Error(resultMsg);
        }
    }
}