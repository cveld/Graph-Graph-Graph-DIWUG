using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventManagerDemo.Utils
{
    public class FileHelper
    {
        static public string GetAbsolutePath(Controller c, string local_path)
        {
            var absolute_path = local_path[0] == '~' ? c.Server.MapPath(local_path) : local_path;
            return absolute_path;
        }
    }
}