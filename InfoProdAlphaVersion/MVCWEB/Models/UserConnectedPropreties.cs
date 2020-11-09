using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCWEB.Models
{
    public class UserConnectedPropreties
    {
        public int Id { get; set; }
        public String userName { get; set; }
        public String pseudoName { get; set; }

        public Byte[] Content { get; set; }

        public String ContentType { get; set; }

        public UserConnectedPropreties()
        {

        }
        public UserConnectedPropreties(int Id,String userName,String pseudoName)
        {
            this.Id = Id;
            this.userName = userName;
            this.pseudoName = pseudoName;
        }
        public void affectAttributes(DirectoryViewModel a,int Id,String Username,String pseudoName)
        {
            a.empId = "" + Id;

            a.userName = Username;
            a.pseudoNameEmp = pseudoName;
        }
        public String renderImage(byte[] content,String ContentType)
        {
            String strbase64 = Convert.ToBase64String(content);
            String Url = "data:" + ContentType + ";base64," + strbase64;

            return Url;
        }

    }
}