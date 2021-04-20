using Jwt_Template.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jwt_Template.Repositories
{
    public class AccountRepo
    {
        public tblUser checkUser(string username, string password)
        {
            tblUser u = GetUser(username, password);
            if (u != null)
                return u;
            return null;
        }

        private tblUser GetUser(string username, string password)
        {
            using (DB_Entities db = new DB_Entities())
            {
                var obj = db.tblUser.Where(a => a.Username.Equals(username)
                    && a.Password.Equals(password)).FirstOrDefault();
                if (obj != null)
                    return obj;
            }
            return null;
        }

        //public tblUser GetUserAccount(string username)
        //{
        //    using (DB_Entities db = new DB_Entities())
        //    {
        //        var obj = db.tblUser.Where(a => a.Username.Equals(username)).FirstOrDefault();
        //        if (obj != null)
        //            return obj;
        //    }
        //    return null;
        //}
    }
}