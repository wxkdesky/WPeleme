﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Data.Json;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using waimai.BingdingClass;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media.Imaging;
using System.Text.RegularExpressions;


// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace waimai
{
    /// <summary>
    /// 可独立使用或用于导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private HttpResponseMessage msg;
        private string latitude = null;
        private string longitude = null;
        public string responseText;
        HttpClient a = new HttpClient();
        string time = "";
        ordinaryRest myNRest; 
        ObservableCollection<ordinaryRest> nRest = new ObservableCollection<ordinaryRest>();
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
            //HttpClient a = new HttpClient();
            var httpHeader = a.DefaultRequestHeaders;
            httpHeader.UserAgent.ParseAdd("ie");
            httpHeader.UserAgent.ParseAdd("Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
            TimeSpan now = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            time = Convert.ToInt64(now.TotalMilliseconds).ToString();
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: 准备此处显示的页面。

            // TODO: 如果您的应用程序包含多个页面，请确保
            // 通过注册以下事件来处理硬件“后退”按钮:
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed 事件。
            // 如果使用由某些模板提供的 NavigationHelper，
            // 则系统会为您处理该事件。
            Geolocator mylocator = new Geolocator();
            mylocator.DesiredAccuracyInMeters = 50;
            try
            {
                Geoposition geopostion = await mylocator.GetGeopositionAsync(maximumAge: TimeSpan.FromMinutes(5), timeout: TimeSpan.FromSeconds(10));
                latitude = geopostion.Coordinate.Point.Position.Latitude.ToString();
                longitude = geopostion.Coordinate.Point.Position.Longitude.ToString();
            }
            catch (UnauthorizedAccessException)
            {
                //tb1.Text = "Get position infor failed!";
            }
        }
        JsonObject jsonObject;
        JsonArray myArray;
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            connect.Background = null;
            // HttpResponseMessage x = await a.GetAsync(new Uri("http://m.ele.me/restaurants"));
            //  tb1.Text = x.Content.ToString();

            msg = new HttpResponseMessage();
            string address = "http://api.ele.me/1/home?banner_width=640&consumer_key=7284397383&geohash=wtw37tkct0fw&session_id=066b2f78e5b6a28eba862842e911d19c&sig=b13cf07a2fcce597ef70c6d15c46a50e&timestamp="+time+"&track_id=1431963253%7C_561daf6c-fd73-11e4-bf65-549f3515da4c%7Cdffd7577b1ef7a401665a87e8bdda416"; //"http://v2.openapi.ele.me/restaurants?geo=" + longitude+","+latitude;
            try
            {
                msg = await a.GetAsync(new Uri(address));
                msg.EnsureSuccessStatusCode();
                responseText = await msg.Content.ReadAsStringAsync();
                Debug.WriteLine(responseText);
                //jsonObject = JsonObject.Parse(responseText);
                // Type x = jsonObject["home"].GetType();
               // string x = jsonObject.GetNamedValue("");
               // myArray = jsonObject.GetArray();
               // JsonObject item = myArray.GetObject();
                //tb1.Text = "finish";
             }
            catch(Exception ex)
            {
                //tb1.Text = "Network request failed" + ex.Message;
            }
            connect.Background = new SolidColorBrush(Windows.UI.Colors.Orange);

        }

        private void bt1_Click(object sender, RoutedEventArgs e)
        {
            jsonObject = JsonObject.Parse(responseText);
           // myArray = jsonObject["promotions"].GetArray();
            JsonObject temp = jsonObject["home"].GetObject();
            myArray=temp["restaurants"].GetArray();
            foreach (var item in myArray)
            {
                myNRest = new ordinaryRest();
                //next is to resolve the data
                temp = item.GetObject();
                JsonArray icon = temp["icons"].GetArray();
                int count = 0;
                for(int i=0;i<icon.Count;i++)
                {
                    JsonObject otmp = icon[i].GetObject();
                    // Oh,i will keep on going when i come back from home 
                    string nameee = otmp["id"].GetString();
                    if (nameee=="减")
                    {
                        myNRest.iconMinus = otmp["id"].GetString() + "·";
                        myNRest.iconMinusText = otmp["name"].GetString();
                        count++;
                    }
                    else if (nameee == "付")
                    {
                        myNRest.iconPay = otmp["id"].GetString() + "·";
                        myNRest.iconPayText = otmp["name"].GetString();
                    }
                    else if (nameee == "票")
                    {
                        myNRest.iconCheck = otmp["id"].GetString()+"·";
                        myNRest.iconCheckText = otmp["name"].GetString();
                    }
                    else if (nameee == "首")
                    {
                        myNRest.iconFirst = otmp["id"].GetString() + "·";
                        myNRest.iconFirstText = otmp["name"].GetString();
                        count++;
                    }
                    else if (nameee == "配")
                    {
                        myNRest.iconDeliver = otmp["id"].GetString() + "·";
                        myNRest.iconDeliverText = otmp["name"].GetString();
                    }
                    else
                    {
                        myNRest.iconFirst = otmp["id"].GetString() + "·";
                        myNRest.iconFirstText = otmp["name"].GetString();
                        count++;
                    }
   
                    //    temp = item.GetObject();
                    //    tb1.Text += temp["subtitle"].GetString()+"\n"+temp["title"].GetString()+"\n"+temp["url"].GetString()+"\n"+temp["image_url"].GetString();
                }
                myNRest.restName = temp["name"].GetString();
                myNRest.Distance = temp["distance"].GetString();
                myNRest.deliverSpent =Convert.ToString( temp["deliver_spent"].GetNumber())+"分钟";
                myNRest.imageSource = new BitmapImage(new Uri(temp["image_url"].GetString()));
                Regex mod = new Regex(@"(月售\d+份)[\s]+(\d+)元起送*");
                Match x = mod.Match(temp["tips"].GetString());
                myNRest.leastMoneyTips = x.Groups[2].Value.ToString();
                myNRest.monthSellTips = x.Groups[1].Value.ToString();
                nRest.Add(myNRest);
            }
                NRestList.ItemsSource = nRest;
          
           // temp = myArray[0].GetObject();
           // tb1.Text = temp["subtitle"].GetString();
        }
    }
}
