using Jwt_Template.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Jwt_Template.Controllers
{
    public class FillDocxController : Controller
    {
        private string CurrentDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}";
        // GET: FillDocx
        public ActionResult Index(tblFileDetails model)
        {
            using (DB_Entities context = new DB_Entities())
            {
                var allFiles = (from r in context.tblFileDetails select r).ToList();
                if (allFiles != null)
                {
                    model.FileList = allFiles;
                    return View(model);
                }  
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Generate(string nameTemplate, HttpPostedFileBase jsonName)
        {
            string ext = Path.GetExtension(jsonName.FileName);
            if (ext == ".json")
            {
                var timeStamp = InfoVuln.GetInstance().TimeStamp;
                if (!Directory.Exists(Server.MapPath($"~/UploadedFiles/Json/{timeStamp}")))
                    Directory.CreateDirectory(Server.MapPath($"~/UploadedFiles/Json/{timeStamp}"));

                string path = Path.Combine(Server.MapPath($"~/UploadedFiles/Json/{timeStamp}"), jsonName.FileName);

                jsonName.SaveAs(path);

                var parameters = new Dictionary<string, string> { { "NameTemplate", nameTemplate }, { "Jsonpath", path } };
                var encodedContent = new FormUrlEncodedContent(parameters);

                HttpResponseMessage response =
                    await RequestHelper.PostRequest($"api/FillDocx/Fill", encodedContent);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string filename = await response.Content.ReadAsStringAsync();
                    if(filename == "null")
                    {
                        TempData["msg"] = "<script>alert('Json format or template format error, please reformat your file again!!!');</script>";
                        return RedirectToAction("Index");
                    }
                        
                    filename = filename.Replace("\\","").Replace("\"","");
                    Session["filename"] = filename;

                    return RedirectToAction("Download", new { filename = filename });
                }     
            }

            TempData["msg"] = "<script>alert('Upload file error or Not exist template!!!');</script>";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Download()
        {
            if (Session["filename"] == null)
            {
                TempData["msg"] = "<script>alert('File Download not exist!!!');</script>";
                return RedirectToAction("Index");
            }

            string fileName = Session["filename"].ToString();
            Session.Remove("filename");

            var filepath = $"{CurrentDirectory}Renders/{fileName}";

            var memory = new MemoryStream();
            using (var stream = new FileStream(filepath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            var ext = Path.GetExtension(filepath).ToLowerInvariant();
            memory.Position = 0;

            return File(memory, GetMinetype()[ext], Path.GetFileName(filepath));
        }
        private Dictionary<string, string> GetMinetype()
        {
            return new Dictionary<string, string>
            {
                {".docx", "application/vnd.ms-word" },
                {".doc", "application/vnd.ms-word" },
            };
        }
    }
}