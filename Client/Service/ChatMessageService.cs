using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClientChat.Service
{
    public class ChatMessageService : HttpClientBaseService
    {
        private static ChatMessageService instance;
        public static ChatMessageService Instance
        {
            get { return instance ?? (instance = new ChatMessageService()); }
        }
        private ChatMessageService() { }

        public async Task<List<TopicArea>> GetDados(string path)
        {
            List<TopicArea> topicAreas = null;
            HttpResponseMessage response = await base.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                topicAreas = JsonConvert.DeserializeObject<List<TopicArea>>(jsonString);
            }
            return topicAreas;
        }

        public async Task Disconnect(string nickName)
        {
            await base.GetAsync("connection/stop?nickname=" + nickName);
        }

        public async Task<List<string>> ListAllUsers()
        {
            List<string> users = null;
            HttpResponseMessage response = await base.GetAsync("connection/users");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                users = JsonConvert.DeserializeObject<List<string>>(jsonString);
            }
            return users;
        }
    }
}
