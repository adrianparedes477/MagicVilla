using MagicVilla_Utilidad;
using MagicVilla_Web.Models;
using MagicVilla_Web.Services.IServices;
using Newtonsoft.Json;
using System.Text;
using System.Net;

namespace MagicVilla_Web.Services
{
    public class BaseService : IBaseService
    {
        public APIResponse resposeModel { get; set; }
        public IHttpClientFactory _httpClient { get; set; }
        public BaseService(IHttpClientFactory httpClient)
        {
            this.resposeModel = new();
            _httpClient = httpClient;
        }

        public async Task<T> SeedAsync<T>(APIRequest apiRequest)
        {
            try
            {
                var client = _httpClient.CreateClient("MagicAPI");
                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(apiRequest.Url);

                if(apiRequest.Datos != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Datos),
                        Encoding.UTF8, "application/json");
                }

                switch (apiRequest.APITipo)
                {
                    case DS.APITipo.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case DS.APITipo.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case DS.APITipo.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                HttpResponseMessage apiRespose = null;
                apiRespose = await client.SendAsync(message);
                var apiContent = await apiRespose.Content.ReadAsStringAsync();
                

                try
                {
                    APIResponse response = JsonConvert.DeserializeObject<APIResponse>(apiContent);
                    if(apiRespose.StatusCode == HttpStatusCode.BadRequest || apiRespose.StatusCode == HttpStatusCode.NotFound)
                    {
                        response.statusCode = HttpStatusCode.BadRequest;
                        response.IsExitoso = false;
                        var res = JsonConvert.SerializeObject(response);
                        var obj = JsonConvert.DeserializeObject<T>(res);
                        return obj;
                    }
                }
                catch (Exception ex)
                {
                    var errorResponse = JsonConvert.DeserializeObject<T>(apiContent);
                    return errorResponse;
                }

                var APIresponse = JsonConvert.DeserializeObject<T>(apiContent);
                return APIresponse;

            }
            catch (Exception ex)
            {

                var dto = new APIResponse
                {
                    ErrorMessages = new List<string> { Convert.ToString(ex.Message) },
                    IsExitoso = false
                };
                var res = JsonConvert.SerializeObject(dto);
                var responseEx = JsonConvert.DeserializeObject<T>(res);
                return responseEx;
            }
        }
    }
}
