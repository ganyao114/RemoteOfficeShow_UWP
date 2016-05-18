﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace RemoteOffice.View.Notify
{
    class NotifyUtils
    {
        public static void ShowToastNotification(string assetsImageFileName, string text, NotificationAudioNames audioName)
        {
            // 1. create element
            ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText01;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            // 2. provide text
            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(text));

            // 3. provide image
            XmlNodeList toastImageAttributes = toastXml.GetElementsByTagName("image");
            ((XmlElement)toastImageAttributes[0]).SetAttribute("src", $"ms-appx:///assets/{assetsImageFileName}");
            ((XmlElement)toastImageAttributes[0]).SetAttribute("alt", "logo");

            // 4. duration
            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            ((XmlElement)toastNode).SetAttribute("duration", "short");

            // 5. audio
            XmlElement audio = toastXml.CreateElement("audio");
            audio.SetAttribute("src", $"ms-winsoundevent:Notification.{audioName.ToString().Replace("_", ".")}");
            toastNode.AppendChild(audio);

            // 6. app launch parameter
            //((XmlElement)toastNode).SetAttribute("launch", "{\"type\":\"toast\",\"param1\":\"12345\",\"param2\":\"67890\"}");

            // 7. send toast
            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }

    public enum NotificationAudioNames
    {
        Default,
        IM,
        Mail,
        Reminder,
        SMS,
        Looping_Alarm,
        Looping_Alarm2,
        Looping_Alarm3,
        Looping_Alarm4,
        Looping_Alarm5,
        Looping_Alarm6,
        Looping_Alarm7,
        Looping_Alarm8,
        Looping_Alarm9,
        Looping_Alarm10,
        Looping_Call,
        Looping_Call2,
        Looping_Call3,
        Looping_Call4,
        Looping_Call5,
        Looping_Call6,
        Looping_Call7,
        Looping_Call8,
        Looping_Call9,
        Looping_Call10,
    }
}
