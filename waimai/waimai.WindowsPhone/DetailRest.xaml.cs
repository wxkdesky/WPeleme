using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace waimai
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DetailRest : Page
    {
        private HttpResponseMessage msg;
        public string responseText;
        HttpClient a = new HttpClient();
        JsonObject jsonObject;
        JsonArray myArray;
        public DetailRest()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            var httpHeader = a.DefaultRequestHeaders;
            httpHeader.UserAgent.ParseAdd("ie");
            httpHeader.UserAgent.ParseAdd("Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            string name_for_url = (string)e.Parameter;
            msg = new HttpResponseMessage();
            string add = "http://restapi.ele.me/v1/restaurants/"+name_for_url+"/menu?full_image_path=1";
            try {
                msg = await a.GetAsync(new Uri(add));
                msg.EnsureSuccessStatusCode();
                responseText =await msg.Content.ReadAsStringAsync();
                Debug.WriteLine(responseText);
            }
            catch(Exception ex)
            {
                // Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ShowAsync();
                XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
                XmlNodeList element = toastXml.GetElementsByTagName("text");
                element[0].AppendChild(toastXml.CreateTextNode("获取远程数据时发生意外，请检查您的网络"));
                ToastNotification toast = new ToastNotification(toastXml);
                ToastNotificationManager.CreateToastNotifier().Show(toast);
            }
            jsonObject = JsonObject.Parse(responseText);
            myArray = jsonObject.GetArray();
            foreach (var item in myArray)
            {
                JsonObject temp = item.GetObject();
                tb1.Text += temp["description"].GetString();
            }
        }
    }
}
