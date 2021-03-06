﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quickblox.Sdk;
using Quickblox.Sdk.Modules.UsersModule.Requests;
using Quickblox.Sdk.Modules.ChatModule.Models;
using Quickblox.Sdk.GeneralDataModel.Response;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Quickblox.Sdk.Modules.ChatXmppModule;
using Windows.ApplicationModel.Core;

namespace iotX_Backend_Test
{
    public class MainInstance
    { 
        public static UserRequest user { get; set; }
        public static string dId { get; set; }
        public static PrivateChatManager privateChatManagerX { get; set; }
        public static QuickbloxClient quickbloxClient = new QuickbloxClient(Keys.appId, Keys.authKey, Keys.authSecret, Keys.apiEndpoint, Keys.chatEndpoint);
        public static void Init()
        {
            Quickblox.Sdk.Platform.QuickbloxPlatform.Init();
        }
        public static async Task SignUp(string Identifer, string Email, string Password)
        {
            dId = null;
            var sessionResponse = await quickbloxClient.AuthenticationClient.CreateSessionBaseAsync();
            if (sessionResponse.StatusCode == System.Net.HttpStatusCode.Created)
            {
                user = new UserRequest();
                user.Email = Email;
                user.Password = Password;
                user.FullName = Identifer;
                var loginResponse = await quickbloxClient.AuthenticationClient.ByEmailAsync(user.Email, user.Password);
                await quickbloxClient.ChatXmppClient.Connect(loginResponse.Result.User.Id, user.Password);
            }
            
        }
        public static async Task<string> createLinktoHub()
        {
            //testing a commit
            var dialogsResponse = await getDialogs();
            var dResult = dialogsResponse.RawData;
            var json = (JObject)JsonConvert.DeserializeObject(dResult);
            int dCount = (int)json["total_entries"];
            if (dCount > 0)
            {
                JArray dItems = (JArray)json["items"];
                foreach (var item in dItems)
                {
                    JArray ids = (JArray)item["occupants_ids"];
                    foreach (var id in ids)
                    {
                        if ((int)id == 21010378)
                        {
                            dId = item["_id"].ToString();
                        }
                    }
                    if (dId == null)
                        dId = await connectToHub();
                }                
            }
            return dId;
        }

        public static async void startTranmission()
        {
            if (dId == null)
                dId = await createLinktoHub();
            privateChatManagerX = quickbloxClient.ChatXmppClient.GetPrivateChatManager(21010378, dId);
            privateChatManagerX.MessageReceived += ChatManager_MessageReceived;
            dynamic toSend = new JObject();
            toSend.FriendlyName = user.FullName;
            toSend.Date = DateTime.Now;
            toSend.Type = "Init";
            privateChatManagerX.SendMessage(JsonConvert.SerializeObject(toSend));
            quickbloxClient.ChatXmppClient.MessageReceived += (object sender, MessageEventArgs messageEventArgs) =>
            {
                var message = ((System.Xml.Linq.XElement)messageEventArgs.Message.ExtraParameters.NextNode).Value;
                var friendlyName = messageEventArgs.Message.SenderId;
                var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
                dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {

                }
               );
            };
        }

        public static async void setGPIOstatus(int GPIOPin, bool bit)
        {
            if (dId == null)
                dId = await createLinktoHub();
            privateChatManagerX = quickbloxClient.ChatXmppClient.GetPrivateChatManager(21010378, dId);
            dynamic toSend = new JObject();
            toSend.FriendlyName = user.FullName;
            toSend.Date = DateTime.Now;
            toSend.Type = "setGPIOstatus";
            toSend.GPIOPin = GPIOPin;
            toSend.bit = bit;        
            privateChatManagerX.SendMessage(JsonConvert.SerializeObject(toSend));
        }

        public static async void sendMessage()
        {
            if (dId == null)
                dId = await createLinktoHub();
            dynamic toSend = new JObject();
            toSend.FriendlyName = user.FullName;
            toSend.Date = DateTime.Now;
            toSend.Type = "setGPIOstatus";
          //  toSend.Args = "Hello";
            privateChatManagerX.SendMessage(JsonConvert.SerializeObject(toSend));
        }

        private static void ChatManager_MessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
           
        }
        public static async Task<HttpResponse> getDialogs()
        {
            HttpResponse response = await quickbloxClient.ChatClient.GetDialogsAsync();
            return response;
        }
        public static async Task<string> connectToHub()
        {           
            DialogType dialogType = DialogType.Private;
            var userIds = new List<int>() { 21010378 };
            var dialogResponse = await quickbloxClient.ChatClient.CreateDialogAsync("", dialogType, "21010378");
            return dialogResponse.Result.Id;
        }
    }


}
