using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestBotEnterprise.HelperServices
{
    public class Translator
    {

        public Translator()
        {

        }


        ///
        public static async Task<string> TranslateText(string inputText, string language, string accessToken)
        {
            //if (language == "es") return inputText;

            string url = "http://api.microsofttranslator.com/v2/Http.svc/Translate";
            //string url = "http://api.microsofttranslator.com/v2/Http.svc/Detect";
            string query = $"?text={System.Net.WebUtility.UrlEncode(inputText)}&to={language}&contentType=text/plain";

            using (var client = new System.Net.Http.HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.GetAsync(url + query);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return "Hata: " + result;

                var translatedText = System.Xml.Linq.XElement.Parse(result).Value;
                return translatedText;
            }
        }

        public static async Task<string> DetectText(string inputText, string accessToken)
        {
            string url = "http://api.microsofttranslator.com/v2/Http.svc/Detect";
            string query = $"?text={System.Net.WebUtility.UrlEncode(inputText)}";

            using (var client = new System.Net.Http.HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.GetAsync(url + query);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return "Hata: " + result;

                var translatedText = System.Xml.Linq.XElement.Parse(result).Value;
                return translatedText;
            }
        }

        public static async Task<string> GetAuthenticationToken(string key)
        {
            string endpoint = "https://api.cognitive.microsoft.com/sts/v1.0/issueToken";

            using (var client = new System.Net.Http.HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
                var response = await client.PostAsync(endpoint, null);
                var token = await response.Content.ReadAsStringAsync();
                return token;
            }
        }
        /////

    }


}
