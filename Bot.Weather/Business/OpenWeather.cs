using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Bot.Weather.Models;

namespace Bot.Weather.Business
{
    public static class OpenWeather
    {
        public static async Task<string> GetWeather(string city)
        {
            OpenWeatherResponse _response = await GetWeatherAsync(city);
            StringBuilder _message = new StringBuilder();


            if (_response != null && _response.cod == 200)
            {
                _message.Append($"Weather in {city}, {_response.sys.country} : \n\n");
                _message.AppendLine($"* { _response.weather[0].main} - { _response.weather[0].description} \n\n");
                _message.AppendLine($"* Temperature : {_response.main.temp} °C \n\n");
                _message.AppendLine($"* Humidity : {_response.main.humidity} % \n\n");
                _message.AppendLine($"* Wind speed : {_response.wind.speed} mps \n\n");
            }
            else
            {
                _message.Append($"Sorry, i'm having some problems now getting weather's info.");
            }

            return _message.ToString();
        }

        private static async Task<OpenWeatherResponse> GetWeatherAsync(string city)
        {
            try
            {
                string ServiceURL = $"http://api.openweathermap.org/data/2.5/weather?q={city}&units=metric&appid={Properties.Resources.OpenWeatherAppID}";
                string ResultIN;

                using (WebClient client = new WebClient())
                {
                    ResultIN = await client.DownloadStringTaskAsync(ServiceURL).ConfigureAwait(false);
                }

                if (!String.IsNullOrEmpty(ResultIN))
                {
                    return JsonConvert.DeserializeObject<OpenWeatherResponse>(ResultIN);
                }
                return null;
            }
            catch (WebException)
            {
                return null;
            }
        }
    }
}