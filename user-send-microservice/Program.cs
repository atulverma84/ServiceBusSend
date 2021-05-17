using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace user_send_microservice
{
    class Program
    {
        static ITopicClient topicClient;
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            string ServiceBusConnectionString = "Endpoint=sb://pharma-nexus-dev.servicebus.windows.net/;SharedAccessKeyName=CentricManageTopic;SharedAccessKey=Lm75zQ41mfPUjMbgicKQaBuIMKEFtw11EJjjgYWjDpE=";
                //"Endpoint=sb://pharma-nexus-dev.servicebus.windows.net/;SharedAccessKeyName=SendAccessKey;SharedAccessKey=/MRlyUrhV5PC5P+s/Ez+V3D9ZHP73SSVxO023pXowmo=";
            //Endpoint = sb://testav-servicebus.servicebus.windows.net/;SharedAccessKeyName=send-serviceBus;SharedAccessKey=7ZxMBV3namc+oxCvimbChvUVdze+FJ5UqSMPSgbpdYM=
            string TopicName = "centrictesttopic";

            topicClient = new TopicClient(ServiceBusConnectionString, TopicName);

            // Send messages.
            await SendUserMessage();

            Console.ReadKey();

            await topicClient.CloseAsync();
        }


        static async Task SendUserMessage()
        {
            List<User> users = GetDummyDataForUser();

            var serializeUser = JsonConvert.SerializeObject(users);

            string messageType = "userData";

            string messageId = Guid.NewGuid().ToString();

            var message = new ServiceBusMessage
            {
                Id = messageId,
                Type = messageType,
                Content = serializeUser
            };

            var serializeBody = JsonConvert.SerializeObject(message);

            // send data to bus

            var busMessage = new Message(Encoding.UTF8.GetBytes(serializeBody));
            busMessage.UserProperties.Add("Type", messageType);
            busMessage.MessageId = messageId;

            await topicClient.SendAsync(busMessage);

            Console.WriteLine("message has been sent");

        }

       public class User
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }

        public class ServiceBusMessage
        {
            public string Id { get; set; }
            public string Type { get; set; }
            public string Content { get; set; }
        }

        private static List<User> GetDummyDataForUser()
        {
            User user = new User();
            List<User> lstUsers = new List<User>();
            for (int i = 1; i < 3; i++)
            {
                user = new User();
                user.Id = i;
                user.Name = "AtulVerma" + i;

                lstUsers.Add(user);
            }

            return lstUsers;
        }
    }
}
