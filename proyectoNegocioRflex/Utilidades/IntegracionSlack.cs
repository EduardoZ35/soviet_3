using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace proyectoNegocioRflex.Utilidades
{
    public class IntegracionSlack
    {
        private readonly Uri _webhookUrl;
        private readonly HttpClient _httpClient = new HttpClient();

        public IntegracionSlack(string urlHook)
        {
            Uri myUri = new Uri(urlHook, UriKind.Absolute);
            _webhookUrl = myUri;
        }

        public async Task<HttpResponseMessage> enviarMensajeSlack(string message,
            string channel = null, string username = null)
        {
            var payload = new
            {
                text = message,
                channel,
                username
            };
            var serializedPayload = JsonConvert.SerializeObject(payload);
            var response = await _httpClient.PostAsync(_webhookUrl,
                new StringContent(serializedPayload, Encoding.UTF8, "application/json"));
            return response;
        }
    }
}