using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace noobsmuc.AzureCognitivesServices.AzureOcr
{
    public class AzureOcr
    {
        const string UriBaseOcr = "https://westeurope.api.cognitive.microsoft.com/vision/v2.0/ocr";

        private readonly string _SubscriptionKey;

        public AzureOcr(string subscriptionKey)
        {
            _SubscriptionKey = subscriptionKey;
        }

        public async Task<string> MakeOcrRequest(byte[] byteData)
        {
            HttpClient client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _SubscriptionKey);

            // Request parameters.
            string requestParameters = "language=unk&detectOrientation=true";

            // Assemble the URI for the REST API Call.
            string uri = UriBaseOcr + "?" + requestParameters;

            HttpResponseMessage response;
            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json"
                // and "multipart/form-data".
                content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");

                // Make the REST API call.
                response = await client.PostAsync(uri, content);
            }

            // Get the JSON response.
            string contentString = await response.Content.ReadAsStringAsync();

            return contentString;
        }
    }
}