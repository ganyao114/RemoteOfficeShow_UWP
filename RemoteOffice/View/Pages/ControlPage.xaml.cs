using RemoteOffice.Control.SensonProcessor;
using RemoteOffice.Entity;
using RemoteOfficeShow.Entity;
using RemoteOfficeShow.Model.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace RemoteOffice.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ControlPage : Page,ISensorAction
    {
        private MyService service;
        private SessionBean session;
        private string uploadRes;
        private SessorProcess sensor;
        public ControlPage()
        {
            this.InitializeComponent();
            service = new MyService();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            DataTransferManager.GetForCurrentView().DataRequested += shareProxy;
            ControlPageArg arg = (ControlPageArg)e.Parameter;
            session = arg.Bean;
            uploadRes = arg.Str;
            sensor = new SessorProcess(Dispatcher, this);
            sensor.init();
            showWebViewFirst();
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            DataTransferManager.GetForCurrentView().DataRequested -= shareProxy;
            sensor.dispose();
        }

        private async void pre_Click(object sender, RoutedEventArgs e)
        {
            showWebView(await service.control(ControlFlag.Pre,session));
        }

        private async void next_Click(object sender, RoutedEventArgs e)
        {
            showWebView(await service.control(ControlFlag.Next,session));
        }

        private void showWebView(string str) {
            string url = "http://www.jjust.top/ShowPPT/Rooms/" + session.Id + "/" + str + ".png";
            webview.Navigate(new Uri(url, UriKind.Absolute));
        }

        private void showWebViewFirst() {
            JsonObject obj = JsonObject.Parse(uploadRes);
            string url = "http://www.jjust.top/ShowPPT/" + obj.GetNamedString("url");
            string type = obj.GetNamedString("type");
            if (!(type.Equals("ppt") || type.Equals("pptx")))
            {
                pre.Visibility = Visibility.Collapsed;
                next.Visibility = Visibility.Collapsed;
            }
            webview.Navigate(new Uri(url, UriKind.Absolute));
        }

        private void share_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        private void onBack(object sender, RoutedEventArgs e)
        {
            Frame root = Window.Current.Content as Frame;
            root.Navigate(typeof(MainPage), session);
        }

        private async void onTip(object sender, RoutedEventArgs e)
        {
            await(new MessageDialog("ID:"+session.Id+"  密码:"+session.Pass)).ShowAsync();
        }

        void shareProxy(DataTransferManager sender,DataRequestedEventArgs arg) {
            string content = "http://www.jjust.top/ShowPPT/visitorpage.jsp?ID=" + session.Id + "&PD=" + session.Pass;
            var def = arg.Request.GetDeferral();
            arg.Request.Data.Properties.Title = "分享链接";
            arg.Request.Data.Properties.Description = "分享控制网页链接";
            arg.Request.Data.SetText(content);
            def.Complete();
        }

        public async void onShake()
        {
          
        }

        public async void onRight()
        {
          showWebView(await service.control(ControlFlag.Next, session));
        }

        public async void onLeft()
        {
          showWebView(await service.control(ControlFlag.Pre, session));
        }
    }
}
