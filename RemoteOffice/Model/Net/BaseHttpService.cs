using RemoteOffice.Model.Net;
using RemoteOfficeShow.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Http;

namespace RemoteOfficeShow.Model.Net
{
    class BaseHttpService
    {
        private static BaseHttpService service;
        private static String mylock = "";
        private IUploadProgress progresscall;

        /// <summary>
        /// 访问服务器时的cookies
        /// </summary>
        public CookieContainer CookiesContainer;
        public static BaseHttpService getInstance() {
            lock (mylock) {
                if (service == null)
                    service = new BaseHttpService();
            }
            return service;
        }
        /// <summary>
        /// 向服务器发送get请求  返回服务器回复数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<string> SendGetRequest(string url)
        {
            try
            {

                HttpClient client = HttpClientFactory.getClient();
                Uri uri = new Uri(url);

                HttpResponseMessage response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch
            {
                return null;
            }

        }
        /// <summary>
        /// 向服务器发送post请求 返回服务器回复数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<string> SendPostRequest(string url, List<KeyValuePair<string,string>> pars, List<KeyValuePair<string, string>> headers)
        {
            try
            {
                // 创建一个 HttpRequestMessage 对象
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri(url));

                // 需要 post 的数据
                HttpFormUrlEncodedContent postData = new HttpFormUrlEncodedContent(pars);
                // http 请求的数据
                request.Content = postData;

                request.Headers.Referer = new Uri(url);
                if(headers!=null)
                foreach (KeyValuePair<string, string> header in headers) {
                    request.Headers.Add(header.Key,header.Value);
                }

                // 请求 HttpRequestMessage 对象，并返回 HttpResponseMessage 数据
                HttpResponseMessage response = await HttpClientFactory.getClient().SendRequestAsync(request); 

                return await response.Content.ReadAsStringAsync();
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> upload(string url,StorageFile file,SessionBean session,IUploadProgress call)
        {
            progresscall = call;
            HttpClient _httpClient = HttpClientFactory.getClient();
            CancellationTokenSource _cts = new CancellationTokenSource();

            try
            {
                Uri resourceAddress = new Uri(url);
                // 构造需要上传的文件数据
                IRandomAccessStreamWithContentType stream = await file.OpenReadAsync();
                HttpStreamContent streamContent = new HttpStreamContent(stream);
                ulong streamLength = stream.Size;
                IProgress<HttpProgress> progress = new Progress<HttpProgress>(ProgressHandler);


                // 通过 HttpMultipartFormDataContent 来指定需要“multipart/form-data”上传的文件
                HttpMultipartFormDataContent fileContent = new HttpMultipartFormDataContent();
                // 第 1 个参数：需要上传的文件数据
                // 第 2 个参数：对应 asp.net 服务的 Request.Files 中的 key（参见：WebServer 项目中的 HttpDemo.aspx.cs）
                // 第 3 个参数：对应 asp.net 服务的 Request.Files 中的 fileName（参见：WebServer 项目中的 HttpDemo.aspx.cs）
                HttpStringContent idct = new HttpStringContent(session.Id);
                HttpStringContent pdct = new HttpStringContent(session.Pass);
                fileContent.Add(idct, "room_id");
                fileContent.Add(pdct, "room_pd");
                fileContent.Add(streamContent, "file1", "Document"+file.FileType);
                HttpResponseMessage response = await _httpClient.PostAsync(resourceAddress, fileContent).AsTask(_cts.Token,progress);
                return await response.Content.ReadAsStringAsync();
            }
            catch (TaskCanceledException)
            {
                //lblMsg.Text += "取消了";
                //lblMsg.Text += Environment.NewLine;
            }
            catch (Exception ex)
            {
                //lblMsg.Text += ex.ToString();
                //lblMsg.Text += Environment.NewLine;
            }
            return null;
        }

        private void ProgressHandler(HttpProgress progress)
        {
            /*
             * HttpProgress - http 通信的进度
             *     BytesReceived - 已收到的字节数
             *     BytesSent - 已发送的字节数
             *     TotalBytesToReceive - 总共需要收到的字节数
             *     TotalBytesToSend - 总共需要发送的字节数
             *     Retries - 重试次数
             *     Stage - 当前通信的阶段（HttpProgressStage 枚举）
             */

            //string result = "BytesReceived: {0}\nBytesSent: {1}\nRetries: {2}\nStage: {3}\nTotalBytesToReceive: {4}\nTotalBytesToSend: {5}\n";
            // result = string.Format(result, progress.BytesReceived, progress.BytesSent, progress.Retries, progress.Stage, progress.TotalBytesToReceive, progress.TotalBytesToSend);
            int progressvalue = (int)(progress.BytesSent / progress.TotalBytesToSend) * 100;
            progresscall.onProgress(progressvalue);
            //lblMsg.Text = result;
        }
    }
}
