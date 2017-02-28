using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Connector;
using Bot.Weather.Models;
using System.Text;
using Microsoft.Bot.Builder.Dialogs;
using Bot.Weather.Business;

namespace Bot.Weather
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            Activity reply = null;

            if (activity.Type == ActivityTypes.Message)
            {
                //Here, i'm using LuisDialog
                await Conversation.SendAsync(activity, () => new BotDialog());

                //Here, a very classic way
                string Response = String.Empty;
                try
                {
                    LuisResponse _luisResponse = await LUIS.GetLuisResponse(activity.Text);

                    if (_luisResponse.intents.Count() > 0)
                    {
                        switch (_luisResponse.intents[0].intent)
                        {
                            case "Greetings":
                                Response = "Hi there !";
                                break;
                            case "Weather":
                                var entities = _luisResponse.entities?.Count() ?? 0;
                                Response = entities > 0 ? await OpenWeather.GetWeather(_luisResponse?.entities?[0]?.entity) : "If you're asking about weather, please enter a valid city name";
                                break;
                            default:
                                Response = "Sorry, I am not getting you...";
                                break;
                        }
                    }
                    else
                    {
                        Response = "Sorry, I am not getting you...";
                    }
                }
                catch (Exception ex)
                {
                    Response = ex.Message;
                }
                // return our reply to the user
                reply = activity.CreateReply(Response);
            }
            else
            {
                reply = HandleSystemMessage(activity);                
            }

            if (reply != null)
            {
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }        

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
                StringBuilder replyMessage = new StringBuilder();
                replyMessage.Append($"Hi there, i'm your weather bot.\n\n");
                replyMessage.Append($"feel free to ask me if it's going to rain, if you need sunglasses or a jacket, or just say hello from time to time\n\n");
                replyMessage.Append($"Currently you can ask me questions like 'How is the weather in Paris ?'\n\n");
                replyMessage.Append($"I will get more intelligent in future.");
                return message.CreateReply(replyMessage.ToString());
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}