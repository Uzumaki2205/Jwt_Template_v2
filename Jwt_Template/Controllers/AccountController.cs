using Jwt_Template.Filters;
using Jwt_Template.Models;
using Jwt_Template.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Jwt_Template.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
            if (Session["UserName"] != null)
                return RedirectToAction("FileUpload", "Account");
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(string username, string password)
        {
            var parameters = new Dictionary<string, string> { { "username", username }, { "password", password } };
            var encodedContent = new FormUrlEncodedContent(parameters);

            HttpResponseMessage response = await RequestHelper.PostRequest("api/AccountAPI/Login", encodedContent);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Session["UserName"] = username;
                var parameters1 = new Dictionary<string, string> { { "username", username } };
                var encodedContent1 = new FormUrlEncodedContent(parameters1);

                using (var res = await RequestHelper.PostRequest("api/Token/GenerateToken", encodedContent1))
                {
                    if (res.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var token = await res.Content.ReadAsStringAsync();
                        Session["Token"] = token.Replace("\"", "").Replace("\\", "");
                    }
                }

                return RedirectToAction("Dashboard");
            }

            return View();
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.RemoveAll(); // it will clear the session at the end of request
            return RedirectToAction("Login", "Account");
        }

        public async Task<ActionResult> Dashboard()
        {
            if (Session["UserName"] == null || Session["Token"] == null)
                return RedirectToAction("Login");

            HttpResponseMessage response = 
                await RequestHelper.GetRequestWithToken("/api/Account/Dashboard", Session["Token"].ToString());
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return View();

            return RedirectToAction("Login");
        }

        public async Task<ActionResult> FileUpload(tblFileDetails model)
        {
            if (Session["Token"] == null || Session["UserName"] == null) 
            {
                Session.RemoveAll();
                return RedirectToAction("Login", "Account");
            }
            
            string token = Session["Token"].ToString();

            HttpResponseMessage response = await RequestHelper.GetRequestWithToken("api/Files/FileUpload", token);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //var list = new FileUploadRepo();
                //if (list != null)
                //{
                //    model.FileList = list.FileList;
                //    return View(model);
                //}
                //else return View();

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

            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase files)
        {
            string ext = Path.GetExtension(files.FileName);

            if (ext == ".doc" || ext == ".docx")
            {
                try
                {
                    if (!Directory.Exists(Server.MapPath($"~/Template")))
                        Directory.CreateDirectory(Server.MapPath($"~/Template"));

                    string newFileName = files.FileName;

                    using (DB_Entities db = new DB_Entities())
                    {
                        var isExist = db.tblFileDetails.Where(a => a.FILENAME.Equals(files.FileName)).FirstOrDefault();
                        if (isExist != null)
                            newFileName = InfoVuln.GetInstance().TimeStamp + newFileName;
                    }

                    string path = Path.Combine(Server.MapPath($"~/Template"), newFileName);
                    var fileUrl = Url.Content(Path.Combine($"~/Template/", newFileName));

                    files.SaveAs(path);

                    using (var context = new DB_Entities())
                    {
                        var t = new tblFileDetails
                        {
                            FILENAME = files.FileName,
                            FILEURL = fileUrl,
                        };

                        context.tblFileDetails.Add(t);
                        context.SaveChanges();
                    }
                    return RedirectToAction("FileUpload");
                }
                catch (Exception)
                {
                    TempData["msg"] = "<script>alert('Upload file error!!!');</script>";
                    ModelState.AddModelError("", "Error In Add File. Please Try Again !!!");
                }
            }

            return RedirectToAction("FileUpload", "Account");
        }

        public ActionResult DeleteFile(string fileName)
        {
            var entites = new DB_Entities();
            var itemToRemove = entites.tblFileDetails.SingleOrDefault(x => x.FILENAME == fileName); //returns a single item.

            if (itemToRemove != null)
            {
                entites.tblFileDetails.Remove(itemToRemove);
                entites.SaveChanges();

                if (System.IO.File.Exists(Server.MapPath($"~/Template/{fileName}")))
                    System.IO.File.Delete(Server.MapPath($"~/Template/{fileName}"));
            }

            return RedirectToAction("FileUpload");
        }

        public ActionResult DownloadFile(string filePath)
        {
            string fullName = Server.MapPath("~" + filePath);

            byte[] fileBytes = GetFile(fullName);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filePath);
        }

        private byte[] GetFile(string s)
        {
            FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new IOException(s);
            return data;
        }
    }
}