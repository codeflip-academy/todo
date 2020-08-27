using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using CodeJar.Response;

namespace CodeJar.Gateway
{
    public class CodeJarGateway
    {
        private HttpClient Client = new HttpClient();
        private JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        public async Task<RedeemCodeResponse> RedeemCodeAsync(string codeStringValue)
        {
            var content = JsonSerializer.Serialize(codeStringValue, _jsonOptions);

            var response = await Client.PostAsync(
                requestUri: "http://localhost:5000/redeem-code",
                content: new StringContent(
                    content: content,
                    encoding: Encoding.UTF8,
                    mediaType: "application/json"
                )
            );

            var responseContent = await response.Content.ReadAsStringAsync();

            var redeemCodeResponse = new RedeemCodeResponse();
            redeemCodeResponse.SetSuccessStatus(response.IsSuccessStatusCode);
            redeemCodeResponse.SetPromotionType(responseContent);

            return redeemCodeResponse;
        }
    }
}