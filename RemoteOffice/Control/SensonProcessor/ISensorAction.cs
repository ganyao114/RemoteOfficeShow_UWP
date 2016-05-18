using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteOffice.Control.SensonProcessor
{
    interface ISensorAction
    {
        void onShake();
        void onRight();
        void onLeft();
    }
}
