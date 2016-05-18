using RemoteOfficeShow.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteOffice.Entity
{
    class ControlPageArg
    {
        private SessionBean bean;
        private string str;

        public string Str
        {
            get
            {
                return str;
            }

            set
            {
                str = value;
            }
        }

        internal SessionBean Bean
        {
            get
            {
                return bean;
            }

            set
            {
                bean = value;
            }
        }
    }
}
