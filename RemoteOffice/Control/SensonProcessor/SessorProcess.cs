using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.UI.Core;

namespace RemoteOffice.Control.SensonProcessor
{
    class SessorProcess
    {
        private ISensorAction callback;
        private Accelerometer acc;
        private uint time = 200;//ms
        private CoreDispatcher Dispatcher;


        public SessorProcess(CoreDispatcher Dispatcher,ISensorAction action) { 
            this.Dispatcher = Dispatcher;
            this.callback = action;

        }


        public void init() {
            acc = Accelerometer.GetDefault();
            acc.ReportInterval = time;
            acc.ReadingChanged += acc_ReadingChanged;
        }

        public void dispose() {
            acc.ReadingChanged -=acc_ReadingChanged;
        }


        async void acc_ReadingChanged ( Accelerometer sender, AccelerometerReadingChangedEventArgs args )
        {
            // 将读数乘以100，以扩大其值，便于比较
            double x = args.Reading.AccelerationX * 100d;
            double y = args.Reading.AccelerationY * 100d;
            double z = args.Reading.AccelerationZ * 100d;
            System.Diagnostics.Debug.WriteLine("X= {0:N0}, Y= {1:N0}, Z= {2:N0}", x, y, z);
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                async () =>
                {
                    // 具体取什么数值，可以经过试验获得
                    if (Math.Abs(x) > 145d || Math.Abs(y) > 140d || Math.Abs(z) > 155d)
                    {
                        if (Math.Abs(x) > 145d) {
                            callback.onRight();
                        } else if (Math.Abs(x) < -145d) {
                            callback.onLeft();
                        }
                        callback.onShake();
                    }
                });
        }

    }
}
