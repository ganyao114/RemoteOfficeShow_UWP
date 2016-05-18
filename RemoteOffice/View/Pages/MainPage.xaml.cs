using file;
using RemoteOffice.Control.SensonProcessor;
using RemoteOffice.Entity;
using RemoteOffice.Model.Entity;
using RemoteOffice.View;
using RemoteOffice.View.Dialog;
using RemoteOffice.View.Notify;
using RemoteOfficeShow.Entity;
using RemoteOfficeShow.Model.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace RemoteOffice
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page,ISensorAction
    {
        private model m;
        private static SessionBean session;
        private MyService httpservice;
        private SessorProcess sensor;
        public MainPage()
        {
            this.InitializeComponent();
            m = new model();
            httpservice = new MyService();
            if (session == null)
                getSession();
            else {
                 sessionid.Text = session.Id;
                 sessionpass.Text = session.Pass;
            }
        }

        private async void Bind_Click(object sender, RoutedEventArgs e)
        {
            StorageFile file = await m.file_open();
            if (file == null)
                return;
            ProgressDialog dialog = new ProgressDialog();
            dialog.ShowAsync();
            string res = await httpservice.upload(file, session, dialog);
            dialog.Hide();
            if (res==null)
                return;
            jumpToControl(res);
        }

        private async void getSession()
        {
            session = await httpservice.getSession();
            sessionid.Text = session.Id;
            sessionpass.Text = session.Pass;
        }
        private void jumpToControl(string str) {
            Frame root = Window.Current.Content as Frame;
            ControlPageArg arg = new ControlPageArg();
            arg.Bean = session;
            arg.Str = str;
            root.Navigate(typeof(ControlPage), arg);
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            NotifyUtils.ShowToastNotification("remote2.png", "正在刷新房间", NotificationAudioNames.Default);
            getSession();
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);                                 
            sensor = new SessorProcess(Dispatcher, this);
            sensor.init();

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            sensor.dispose();
        }

        public void onShake()
        {
            NotifyUtils.ShowToastNotification("remote2.png","正在刷新房间", NotificationAudioNames.Default);
            getSession();
        }

        public void onRight()
        {
           
        }

        public void onLeft()
        {
            
        }
    }
}
