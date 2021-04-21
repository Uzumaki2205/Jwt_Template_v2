using Jwt_Template.Filters;
using Jwt_Template.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Jwt_Template.Controllers.API
{
    public class FillDocxController : ApiController
    {
        private string CurrentDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}";

        [HttpGet]
        public HttpResponseMessage IsExistFile(string fileName)
        {
            var filepath = $"{CurrentDirectory}Renders/{fileName}";

            if (File.Exists(filepath))
                return Request.CreateResponse(HttpStatusCode.OK);
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        [JwtAuthentication]
        [HttpPost]
        public string Fill([FromBody] Doc doc)
        {
            try
            {
                InfoVuln.GetInstance().ProcessDocx(doc.NameTemplate, doc.Jsonpath);
                if (IsExistFile($"{InfoVuln.GetInstance().TimeStamp}.Report.docx").StatusCode == HttpStatusCode.OK)
                    return $"{InfoVuln.GetInstance().TimeStamp}.Report.docx";
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }

        [JwtAuthentication]
        [HttpPost]
        public HttpResponseMessage Download([FromBody] Download file)
        {
            var filePath = $"{CurrentDirectory}Renders/{file.FileName}";
            if (!File.Exists(filePath))
                return Request.CreateResponse(HttpStatusCode.NotFound);

            HttpResponseMessage response = new HttpResponseMessage();
            byte[] bytes = File.ReadAllBytes(filePath);
            response.Content = new ByteArrayContent(bytes);
            response.Content.Headers.ContentLength = bytes.LongLength;

            //Set the Content Disposition Header Value and FileName.
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = file.FileName;

            //Set the File Content Type.
            response.Content.Headers.ContentType =
                new MediaTypeHeaderValue(System.Web.MimeMapping.GetMimeMapping(file.FileName));
            return response;
        }
    }

    public class Doc
    {
        [Required]
        public string NameTemplate { get; set; }
        [Required]
        public string Jsonpath { get; set; }
    }
    public class Download
    {
        [Required]
        public string FileName { get; set; }
    }
}
