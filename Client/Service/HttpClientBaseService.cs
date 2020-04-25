using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ClientChat.Service
{
    public abstract class HttpClientBaseService
    {
        protected HttpClient clientHttp = new HttpClient();

        public HttpClientBaseService ()
        {
            clientHttp.BaseAddress = new Uri("http://localhost:3000/");
            clientHttp.DefaultRequestHeaders.Accept.Clear();
            clientHttp.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        protected async Task<HttpResponseMessage> GetAsync(string path)
        {
            return await clientHttp.GetAsync(path);
            
        }

        protected HttpResponseMessage Get(string path)
        {
            return clientHttp.GetAsync(path).Result;

        }
    }
}
