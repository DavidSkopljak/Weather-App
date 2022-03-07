using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using RestSharp;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace Weather_App
{


    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
            Globals.place = "samobor,hr";
        }


        //Dohvacanje podataka sa API-eva u obliku JSON string-ova
        public void fetchForecast(object sender, RoutedEventArgs e)
        {
            string[] s = Globals.place.Split(',');
            var client1 = new RestClient("https://community-open-weather-map.p.rapidapi.com/forecast?q="+s[0]+"%2C"+s[1]);
            var client2 = new RestClient("https://community-open-weather-map.p.rapidapi.com/forecast/daily?q=" + s[0] + "%2C" + s[1] +"&units=metric");
            var request = new RestRequest();
            request.AddHeader("x-rapidapi-host", "community-open-weather-map.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "5d7b36c582msh962bb3690e11597p15f625jsn2e24b5e6e7da");
            var response1 = client1.ExecuteAsync(request).GetAwaiter().GetResult().Content;
            var response2 = client2.ExecuteAsync(request).GetAwaiter().GetResult().Content;
            var details1 = JObject.Parse(response1);
            var details2 = JObject.Parse(response2);
            FetchForecast(sender, e, details1, details2);


        }

        //filtriranje relevantnih podataka iz kreiranog JSON objekta
        public string LoopThroughData(JObject details, ref int j,ref int currentClimate)
        {

            int i = j;
            string Base = Convert.ToString(details["list"][i]["dt_txt"]).Substring(0, 10);
            for (j = j; true; j++)
            {
                string next = Convert.ToString(details["list"][j]["dt_txt"]).Substring(0, 10);
                if (Base == next)
                {
                    string time = Convert.ToString(details["list"][j]["dt_txt"]).Substring(11, 2);
                    if (time == "12")
                    {
                        currentClimate = j;
                        return Convert.ToString(details["list"][j]["weather"][0]["main"]);
                    }
                }
                else
                {
                    break;
                }
            }
            currentClimate = i;
            return Convert.ToString(details["list"][i]["weather"][0]["main"]);
        }





        //Postavljanje vrijednosti elemenata GUI-a
        private void FetchForecast(object sender, RoutedEventArgs e,JObject details1, JObject details2)
        {
            int currentClimate=0;
            int j = 0;
            string prev = Convert.ToString(details1["list"][0]["dt_txt"]).Substring(0, 10);
            weather_1.Text = LoopThroughData(details1, ref j, ref currentClimate);
            while (Convert.ToString(details1["list"][j]["dt_txt"]).Substring(0, 10) == prev)
            {
                j++;
            }
            prev = Convert.ToString(details1["list"][j]["dt_txt"]).Substring(0, 10);
            weather_2.Text = LoopThroughData(details1, ref j, ref currentClimate);
            while (Convert.ToString(details1["list"][j]["dt_txt"]).Substring(0, 10) == prev)
            {
                j++;
            }
            prev = Convert.ToString(details1["list"][j]["dt_txt"]).Substring(0, 10);
            weather_3.Text = LoopThroughData(details1, ref j, ref currentClimate);
            while (Convert.ToString(details1["list"][j]["dt_txt"]).Substring(0, 10) == prev)
            {
                j++;
            }
            prev = Convert.ToString(details1["list"][j]["dt_txt"]).Substring(0, 10);

            weather_4.Text = LoopThroughData(details1, ref j, ref currentClimate);
            while (Convert.ToString(details1["list"][j]["dt_txt"]).Substring(0, 10) == prev)
            {
                j++;
            }
            weather_5.Text = LoopThroughData(details1, ref j, ref currentClimate);

            
            humidity.Text = Convert.ToString(details2["list"][0]["humidity"]);
            pressure.Text = Convert.ToString(details2["list"][0]["pressure"]);
            windspeed.Text = Convert.ToString(details2["list"][0]["speed"]) + "km/h";
            sunrise.Text = Convert.ToString(UnixTimeStampToDateTime(Convert.ToDouble(details2["list"][0]["sunrise"]))).Substring(9,10);
            sunset.Text = Convert.ToString(UnixTimeStampToDateTime(Convert.ToDouble(details2["list"][0]["sunset"]))).Substring(9, 10);
            currentTemp.Text = Convert.ToString(details1["list"][0]["main"]["temp"])+"K";    
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        public void baseMethod(object sender, RoutedEventArgs e)
        {
            fetchForecast(sender, e);
        }

        public void setCity(object sender, RoutedEventArgs e)
        {
            Globals.place = input.Text;
            baseMethod(sender,e);
        }

        public static class Globals
        {
            public static string place;
            
        }
    }
}