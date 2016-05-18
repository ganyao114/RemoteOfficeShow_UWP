using RemoteOfficeShow.Entity;
using System.Threading.Tasks;
using Windows.Data.Json;
using System.Net;
using Windows.Web;
using Windows.Storage;
using System.Collections.Generic;
using System.Collections;
using RemoteOffice.Model.Net;

namespace RemoteOfficeShow.Model.Net
{
    class MyService
    {
        private BaseHttpService httpbase;
        private string url = "http://www.jjust.top/ShowPPT/servlet/GetRoomIDServlet";
        private string uploadurl = "http://www.jjust.top/ShowPPT/servlet/GetRecServlet";
        private string controlurl = "http://www.jjust.top/ShowPPT/servlet/ChangePPTServlet";
        private List<KeyValuePair<string, string>> nulllist = new List<KeyValuePair<string, string>>();
        public MyService() {
            httpbase = new BaseHttpService();
            //HttpClientFactory.addHeader("Referer", "ShowPPT/room.jsp");
        }
        public async Task<SessionBean> getSession() {
            string res = await httpbase.SendPostRequest(url, nulllist, nulllist);
            SessionBean session = new SessionBean();
            JsonObject obj = JsonObject.Parse(res);
            session.Id = obj.GetNamedString("roomID");
            session.Pass = obj.GetNamedString("pd");
            return session;
        }

        public async Task<string> upload(StorageFile file,SessionBean bean,IUploadProgress progress) {
            return await httpbase.upload(uploadurl, file, bean, progress);
        }

        public async Task<string> control(ControlFlag flag,SessionBean bean) {
            List<KeyValuePair<string, string>> controlpar = new List<KeyValuePair<string, string>>();
            List<KeyValuePair<string, string>> controlheaders = new List<KeyValuePair<string, string>>();
            controlheaders.Add(new KeyValuePair<string, string>("Cookie",bean.Id));
            if (flag == ControlFlag.Pre)
                controlpar.Add(new KeyValuePair<string, string>("mdo", "pre"));
            else if (flag == ControlFlag.Next)
                controlpar.Add(new KeyValuePair<string, string>("mdo", "next"));
            string res = await httpbase.SendPostRequest(controlurl, controlpar, nulllist);
            return res;
        }
       
    }
}
