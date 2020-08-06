using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace K3WISE_WebService
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            string path = Request.Url.LocalPath;
            path = path.Substring(1, path.Length - 1);
            if (!path.Contains(".asmx"))
            {
                Context.RewritePath(path.IndexOf('/') > 0 ? ( path.Substring(0, path.IndexOf('/')) + ".asmx" + path.Substring(path.IndexOf('/'), path.Length - path.IndexOf('/'))) : (path + ".asmx"));
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}