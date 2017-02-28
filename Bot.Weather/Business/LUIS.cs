using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Bot.Weather.Models;

namespace Bot.Weather.Business
{
    public static class LUIS
    {
        public static async Task<LuisResponse> GetLuisResponse(string Query)
        {
            Query = Uri.EscapeDataString(Query);
            LuisResponse Data = new LuisResponse();

            using (HttpClient client = new HttpClient())
            {
                string RequestURI = $"https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/{Properties.Resources.modelid}?subscription-key={Properties.Resources.subkey}&q={Query}&timezoneOffset=0.0&verbose=true";

                HttpResponseMessage msg = await client.GetAsync(RequestURI);

                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                    Data = JsonConvert.DeserializeObject<LuisResponse>(JsonDataResponse);
                }
            }
            return Data;
        }
    }
}