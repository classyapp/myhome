using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Classy.DotNet.Services
{
    public class TranslationApiResponse
    {
        public IList<TranslatedResponse> Translations { get; set; }
    }

    public class TranslatedResponse
    {
        public string TranslatedText { get; set; }
    }

    public class GoogleTranslationApiResponse
    {
        public TranslationApiResponse Data { get; set; }
    }

    public class GoogleTranslationService
    {
        private readonly string _apiKey = "AIzaSyAlsrWb_5EcXL7AkSt5aNjk8vsPxps8yLE";

        public static List<string> SupportedLanguages = new List<string> { "en", "iw", "fr" };

        public string Translate(string q, string sourceLanguage, string targetLanguage)
        {
            GoogleTranslationApiResponse responseObject;
            var apiUrl = "https://www.googleapis.com/language/translate/v2?key={0}&q={1}&source={2}&target={3}";
            using (var client = new WebClient())
            {
                var apiJsonResult = client.DownloadData(string.Format(apiUrl, _apiKey, q, sourceLanguage, targetLanguage));
                var encodedResponse = Encoding.UTF8.GetString(apiJsonResult);
                responseObject = JsonConvert.DeserializeObject<GoogleTranslationApiResponse>(encodedResponse);
            }

            if (responseObject.Data == null || responseObject.Data.Translations == null || responseObject.Data.Translations.Count == 0)
                return null;

            return responseObject.Data.Translations[0].TranslatedText;
        }
    }
}
