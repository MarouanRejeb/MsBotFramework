using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Bot.Weather.Business;

namespace Bot.Weather.Models
{
    [LuisModel("modelid", "subkey")]
    [Serializable]
    public class BotDialog : LuisDialog<object>
    {
        public BotDialog(params ILuisService[] services) : base(services)
        {
        }

        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I'm sorry. I didn't understand you.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Weather")]
        public async Task Weather(IDialogContext context, LuisResult result)
        {
            if (result.Entities != null && result.Entities.Count > 0)
            {
                await context.PostAsync(await OpenWeather.GetWeather(result.Entities[0].Entity));
            }
            else
            {
                await context.PostAsync("This is not an valid request, please type a valid city name.");
            }
            
            context.Wait(MessageReceived);
        }

        [LuisIntent("Greetings")]
        public async Task Greetings(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hi there !");
            context.Wait(MessageReceived);
        }
    }
}