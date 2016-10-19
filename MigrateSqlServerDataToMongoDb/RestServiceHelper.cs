using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MigrateSqlServerDataToMongoDb
{
    public class RestServiceHelper
    {
        private ILoggingService _loggingService;

        public RestServiceHelper()
        {
            _loggingService = new LoggingService();
        }

        public void Call(
            string url,
            bool isPost,
            //int timeOut,
            //string userAgent,
            string data = "",
            Action<string> callBack = null
            )
        {
            Task.Factory.StartNew(() =>
            {
                WriteDataToRemoteService(url, isPost, data, callBack);
            });
        }


        void WriteDataToRemoteService(string url,
            bool isPost,
            string data = "",
            Action<string> callBack = null)
        {
            try
            {
                // _loggingService.Write("Data to Send to Service, data=" + data);

                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = isPost ? "POST" : "GET";
                request.Timeout = -1;
                //request.UserAgent = userAgent;
                request.ContentType = "application/json; charset=utf-8";

                if (isPost)
                {
                    byte[] bytes = new UTF8Encoding().GetBytes(data);
                    request.ContentLength = bytes.Length;
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(bytes, 0, bytes.Length);
                    }
                }

                // synchronous response
                //var result = request.GetResponse() as HttpWebResponse;
                //StreamReader reader = new StreamReader(result.GetResponseStream());
                //string responseString = reader.ReadToEnd();
                //_loggingService.Write(responseString, LogLevel.Info,
                //    typeof(RestServiceHelper));

                request.BeginGetResponse((x) =>
                {
                    using (HttpWebResponse httpWebResponse = (HttpWebResponse)request.EndGetResponse(x))
                    {
                        //get the stream containing content returned by the server
                        Stream responseStream = httpWebResponse.GetResponseStream();
                        String logToWrite = "ResponseStatus: StatusCode=" + httpWebResponse.StatusCode
                                            + ",StatusDescription=" + httpWebResponse.StatusDescription;
                        _loggingService.Write(logToWrite, LogLevel.Info, typeof(RestServiceHelper));
                        
                        //open the stream using a streamReader for easy access
                        using (StreamReader reader = new StreamReader(responseStream))
                        {
                            var response = reader.ReadToEnd();
                            _loggingService.Write("ResponseReceived: " + response);

                            callBack?.Invoke(response);
                        }
                    }
                }, null);

            }
            catch (Exception ex)
            {
                _loggingService.Write(ex, LogLevel.Error,
                    typeof(RestServiceHelper), true);
                throw;
            }
        }


    }
}
