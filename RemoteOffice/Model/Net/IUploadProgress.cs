using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteOffice.Model.Net
{
    interface IUploadProgress
    {
        void onProgress(int progress);
    }
}
