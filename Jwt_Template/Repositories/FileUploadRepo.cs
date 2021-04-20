using Jwt_Template.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jwt_Template.Repositories
{
    public class FileUploadRepo
    {
        public FileUploadRepo()
        {
            FileList = ViewFiles();
        }
        public List<tblFileDetails> ViewFiles()
        {
            using (DB_Entities context = new DB_Entities())
            {
                var allFiles = (from r in context.tblFileDetails select r).ToList();
                if(allFiles != null)
                    return allFiles;
            }
            return null;
        }
        public List<tblFileDetails> FileList { get; set; }
    }
}