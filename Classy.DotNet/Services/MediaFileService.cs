using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Classy.DotNet.Responses;
using Classy.DotNet.Security;
using Newtonsoft.Json.Linq;
using ServiceStack.Text;

namespace Classy.DotNet.Services
{
    public class MediaFileService : ServiceBase
    {
        private readonly string TEMP_MEDIA_FILE_URL = ENDPOINT_BASE_URL + "/media";

        public string AddTempFile(System.Web.HttpPostedFileBase file)
        {
            try
            {
                var request = ClassyAuth.GetAuthenticatedWebRequest(TEMP_MEDIA_FILE_URL);
                byte[] content = new byte[file.InputStream.Length];
                file.InputStream.Read(content, 0, content.Length);
                WebResponse response = HttpUploadFile(request, content, file.ContentType);

                Stream stream = response.GetResponseStream();
                string responseJson = null;
                using (StreamReader rd = new StreamReader(stream))
                {
                    responseJson = rd.ReadToEnd();
                }
                stream.Close();

                JToken token = (JToken)Newtonsoft.Json.JsonConvert.DeserializeObject(responseJson);
                return token.Value<string>("FileId");
            }
            catch (WebException wex)
            {
                throw wex.ToClassyException();
            }   
        }

        public void DeleteTempFile(string fileId)
        {
            var webClient = ClassyAuth.GetAuthenticatedWebClient();
            webClient.UploadString(TEMP_MEDIA_FILE_URL, "DELETE", new { FileId = fileId }.ToJson());
            webClient.Dispose();
        }

        public void RemoveImageFromListing(string listingId, string fileId)
        {
            var webClient = ClassyAuth.GetAuthenticatedWebClient();
            webClient.UploadString(TEMP_MEDIA_FILE_URL, "DELETE", new { FileId = fileId, ListingId = listingId }.ToJson());
            webClient.Dispose();
        }

        private static WebResponse HttpUploadFile(HttpWebRequest wr, byte[] fileContent, string contentType)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;

            var rs = wr.GetRequestStream();
            //header
            rs.Write(boundarybytes, 0, boundarybytes.Length);
            string headerTemplate = "Content-Disposition: form-data; name=\"file\"; filename=\"file\"\r\nContent-Type: {0}\r\n\r\n";
            string header = string.Format(headerTemplate, contentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);
            //file
            rs.Write(fileContent, 0, fileContent.Length);
            //footer
            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            return wr.GetResponse();
        }
    }
}
