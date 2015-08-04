using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Translator
{
    /// <summary>
    /// κλάση διαχείρισης της μετάφρασης
    /// </summary>
    class Translate
    {
        static RegUserPreferences data;
        static AdmAccessToken admToken;
        static string headerValue;
        static AdmAuthentication admAuth;
        /// <summary>
        /// συνάρτηση εκκίνησης της διαδικασίας μετάφρασης
        /// </summary>
        public static bool init()
        {
            data = new RegUserPreferences();
            admAuth = new AdmAuthentication("teikavtranslatorZaxVag", "c8NjSbyiJGdZRkY9Iz8vS2K8dpIgxxy94CTpe/ii1Zg=");
            try
            {
                admToken = admAuth.GetAccessToken();
                headerValue = "Bearer " + admToken.access_token;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            //GetLanguagesForSpeakMethod();
            return true;
        }
        /// <summary>
        /// συνάρτηση μετάφρασης
        /// </summary>
        /// <param name="sourceText"> κείμενο προς μετάφραση</param>
        /// <returns>μεταφρασμένο κείμενο</returns>
        public static string translate(string sourceText)
        {
            data = new RegUserPreferences();
            string fromLanguage = data.FromLanguageShort;
            if (data.FromLanguageShort.Equals("auto"))
            {
                fromLanguage = detect(sourceText);
                if (fromLanguage.Equals("TranslatorException"))
                {
                    init();
                    fromLanguage = detect(sourceText);
                }
            }

            try
            {
                TranslatorService.LanguageServiceClient client = new TranslatorService.LanguageServiceClient();

                HttpRequestMessageProperty httpRequestProperty = new HttpRequestMessageProperty();
                httpRequestProperty.Headers.Add("Authorization", headerValue);
                using (OperationContextScope scope = new OperationContextScope(client.InnerChannel))
                {
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;
                    string translationResult;

                    translationResult = client.Translate("", sourceText, fromLanguage, data.ToLanguageShort, "text/plain", "");
                    
                    return translationResult;
                }
            }
            catch (Exception )
            {
                //return (ex.Message);
                return "TranslatorException";
            }
        }
        /// <summary>
        /// συνάρτηση που ελέγχει σε τι γλώσσα είναι το κείμενο που δίνεται σαν παράμετρος,
        /// χρησιμοποιείται για την αυτόματη αναγνώριση
        /// </summary>
        /// <param name="sourceText">κείμενο που γίνεται ο έλεγχος της γλώσσας προέλευσης</param>
        /// <returns>κωδικός γλώσσας προέλευσης</returns>
        public static string detect(string sourceText)
        {
            try
            {
                TranslatorService.LanguageServiceClient client = new TranslatorService.LanguageServiceClient();

                HttpRequestMessageProperty httpRequestProperty = new HttpRequestMessageProperty();
                httpRequestProperty.Headers.Add("Authorization", headerValue);
                using (OperationContextScope scope = new OperationContextScope(client.InnerChannel))
                {
                    OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;
                    string translationResult;
                    translationResult = client.Detect("", sourceText);
                    
                    return translationResult;
                }
            }
            catch (Exception)
            {
                //return (ex.Message);
                return "TranslatorException";
            }
        }

        /// <summary>
        /// συνάρτηση για λίστα με τις γλώσσες που υποστηρίζονται για ήχο
        /// </summary>
        public static void GetLanguagesForSpeakMethod()
        {
            string uri = "http://api.microsofttranslator.com/V2/Http.svc/GetLanguagesForSpeak";

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.Headers.Add("Authorization", headerValue);
            WebResponse response = null;
            try
            {
                response = httpWebRequest.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    System.Runtime.Serialization.DataContractSerializer dcs = new System.Runtime.Serialization.DataContractSerializer(typeof(List<string>));
                    List<string> languagesForSpeak = (List<string>)dcs.ReadObject(stream);
                    Console.WriteLine("The languages available for speak are: ");
                    languagesForSpeak.ForEach(a => Console.WriteLine(a));
                }
            }
            catch
            {
                throw; //400?
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
            }
        }

        /// <summary>
        /// συνάρτηση δημιουργίας ομιλίας σύμφωνα με επιλεγμένο κείμενο
        /// </summary>
        /// <param name="message">κείμενο να "ακουστεί"</param>
        public static string SpeakMethod(string message)
        {
            
            data = new RegUserPreferences();
            string uri = "http://api.microsofttranslator.com/v2/Http.svc/Speak?text="
                            + message + "&language=" + data.FromLanguageShort + "&format=audio/wav"
                            + "&options=MaxQuality";

            WebRequest webRequest = WebRequest.Create(uri);
            webRequest.Headers.Add("Authorization", headerValue);
            WebResponse response = null;
            try
            {
                response = webRequest.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    using (SoundPlayer player = new SoundPlayer(stream))
                    {
                        player.PlaySync();
                    }
                }
            }
            catch
            {
                return "error";
                throw;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
            }
            return "";
        }
    }

    [DataContract]
    public class AdmAccessToken
    {
        [DataMember]
        public string access_token { get; set; }
        [DataMember]
        public string token_type { get; set; }
        [DataMember]
        public string expires_in { get; set; }
        [DataMember]
        public string scope { get; set; }
    }

    /// <summary>
    /// κλάση για την αυθεντικοποίηση με τον server
    /// </summary>
    public class AdmAuthentication
    {
        public static readonly string DatamarketAccessUri = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
        private string clientId;
        private string cientSecret;
        private string request;

        public AdmAuthentication(string clientId, string clientSecret)
        {
            this.clientId = clientId;
            this.cientSecret = clientSecret;
            //If clientid or client secret has special characters, encode before sending request
            this.request = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com",
                System.Uri.EscapeDataString(clientId), System.Uri.EscapeDataString(clientSecret));
        }

        public AdmAccessToken GetAccessToken()
        {
            return HttpPost(DatamarketAccessUri, this.request);
        }

        private AdmAccessToken HttpPost(string DatamarketAccessUri, string requestDetails)
        {
            //Prepare OAuth request 
            WebRequest webRequest = WebRequest.Create(DatamarketAccessUri);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";
            byte[] bytes = Encoding.ASCII.GetBytes(requestDetails);
            webRequest.ContentLength = bytes.Length;
            using (Stream outputStream = webRequest.GetRequestStream())
            {
                outputStream.Write(bytes, 0, bytes.Length);
            }
            using (WebResponse webResponse = webRequest.GetResponse())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AdmAccessToken));
                //Get deserialized object from JSON stream
                AdmAccessToken token = (AdmAccessToken)serializer.ReadObject(webResponse.GetResponseStream());
                return token;
            }
        }
    }
}
