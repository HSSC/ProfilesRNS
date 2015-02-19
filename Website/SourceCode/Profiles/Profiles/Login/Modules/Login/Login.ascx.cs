/*  
 
    Copyright (c) 2008-2012 by the President and Fellows of Harvard College. All rights reserved.  
    Profiles Research Networking Software was developed under the supervision of Griffin M Weber, MD, PhD.,
    and Harvard Catalyst: The Harvard Clinical and Translational Science Center, with support from the 
    National Center for Research Resources and Harvard University.


    Code licensed under a BSD License. 
    For details, see: LICENSE.txt 
  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Web.UI.HtmlControls;
using System.Configuration;

using Profiles.Login.Utilities;
using Profiles.Framework.Utilities;

namespace Profiles.Login.Modules.Login
{
    public partial class Login : System.Web.UI.UserControl
    {
        Framework.Utilities.SessionManagement sm;
        
        protected Boolean doLogin(string u, string pw) {
        	Profiles.Login.Utilities.DataIO data = new Profiles.Login.Utilities.DataIO();
        	Profiles.Login.Utilities.User user = new Profiles.Login.Utilities.User();
        	
        	user.UserName = u;
        	user.Password = pw;
        	
        	if(data.UserLogin(ref user)) {
        		//emit log message with username date and time and userid(?)
        		
        		
        		Framework.Utilities.DebugLogging.AlwaysLog("login "
                	                                     + u + " " + user.UserName
                	                                     + " " 
                	                                     + sm.Session().UserID
        			+ " "
        			+ DateTime.Now.ToLongDateString()
        			+ " TIME: " 
        			+ DateTime.Now.ToLongTimeString() 
        		);
                Framework.Utilities.DebugLogging.Log("loggedin !!! "
                	                                     + u + " " + user.UserName
                	                                     + " " 
                	                                     + sm.Session().UserID);
        		
        		
        		sm.Session().UserName = u;
        		
        		
        		Framework.Utilities.Cache.AlterDependency(sm.Session().SessionID);
        		if(Request.QueryString["edit"] == "true")
        			Response.Redirect(Root.Domain + "/edit/" + sm.Session().NodeID.ToString());
        		else
        			Response.Redirect(Root.Domain + "/search");
        		return true;
        	} else {
        		//log failure message, maybe show a message to the user
        		Response.Redirect(Root.Domain + "/about");
        		return false;
        	}
        	return false;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            	String eppn = Request.Headers["eppn"];
            	//Framework.Utilities.DebugLogging.AlwaysLog("login page: eppn: " + eppn);
            	
            	if ( Request.QueryString == null || Request.QueryString.Count == 0) {
  
            		if(!String.IsNullOrEmpty(eppn)) {
            			if ( doLogin(eppn,eppn) ) {
            					Framework.Utilities.DebugLogging.AlwaysLog("suceeded login: eppn: " + eppn
        							+ " "  + DateTime.Now.ToLongDateString()
        			+ " TIME: " 
        			+ DateTime.Now.ToLongTimeString() 
        		);
            					
            					
            					
            			} else {
            					Framework.Utilities.DebugLogging.AlwaysLog("failed to login: eppn: " + eppn
            				                                          
            				       	+ " "
        			+ DateTime.Now.ToLongDateString()
        			+ " TIME: " 
        			+ DateTime.Now.ToLongTimeString() 
        		);                                   
            				                                         
            			}
            		} else {
            			Framework.Utilities.DebugLogging.AlwaysLog("login page, no user id. going to search.");
  	         		Framework.Utilities.DebugLogging.Log("login page, no user id. going to search.");
            		 Response.Redirect(Root.Domain + "/search");
            		}
            	} else
            	
            	
            	
            	
            	
            	

                if (Request.QueryString["method"].ToString() == "logout")
                {

                    sm.SessionLogout();
                    sm.SessionDestroy();
                    Response.Redirect(Root.Domain + "/search");
                }
                else if (Request.QueryString["method"].ToString() == "login" && sm.Session().PersonID > 0)
                {
                    if (Request.QueryString["redirectto"] == null && Request.QueryString["edit"] == "true")
                    {
                        if (Request.QueryString["editparams"] == null)
                            Response.Redirect(Root.Domain + "/edit/" + sm.Session().NodeID);
                        else
                            Response.Redirect(Root.Domain + "/edit/default.aspx?subject=" + sm.Session().NodeID + "&" + Request.QueryString["editparams"]);
                    }
                    else
                        Response.Redirect(Request.QueryString["redirectto"].ToString());
                }
            }


        }

        public Login() { }
        public Login(XmlDocument pagedata, List<ModuleParams> moduleparams, XmlNamespaceManager pagenamespaces)
        {
            sm = new Profiles.Framework.Utilities.SessionManagement();
            LoadAssets();
        }

        protected void cmdSubmit_Click(object sender, EventArgs e)
        {

            Profiles.Login.Utilities.DataIO data = new Profiles.Login.Utilities.DataIO();

            if (Request.QueryString["method"].ToString() == "login")
            {
                Profiles.Login.Utilities.User user = new Profiles.Login.Utilities.User();
                user.UserName = txtUserName.Text.Trim();
                user.Password = txtPassword.Text.Trim();

                if (data.UserLogin(ref user))
                {
                	
                		Framework.Utilities.DebugLogging.AlwaysLog("CMDLCIK: login "
                	                                     + " " + user.UserName
                	                                     + " " 
                	                                     + sm.Session().UserID
        			+ " "
        			+ DateTime.Now.ToLongDateString()
        			+ " TIME: " 
        			+ DateTime.Now.ToLongTimeString() 
        		);
                	
                	
                	
                	
                	
                    Framework.Utilities.Cache.AlterDependency(sm.Session().SessionID);
                    if (Request.QueryString["edit"] == "true")
                        if (Request.QueryString["editparams"] == null)
                            Response.Redirect(Root.Domain + "/edit/" + sm.Session().NodeID);
                        else
                            Response.Redirect(Root.Domain + "/edit/default.aspx?subject=" + sm.Session().NodeID + "&" + Request.QueryString["editparams"]);
                    else
                        Response.Redirect(Request.QueryString["redirectto"].ToString());

                }
                else
                {
                    lblError.Text = "Login failed, please try again";
                    txtPassword.Text = "";
                    txtPassword.Focus();
                }





            }

        }

        private void LoadAssets()
        {
            HtmlLink Searchcss = new HtmlLink();
            Searchcss.Href = Root.Domain + "/Search/CSS/search.css";
            Searchcss.Attributes["rel"] = "stylesheet";
            Searchcss.Attributes["type"] = "text/css";
            Searchcss.Attributes["media"] = "all";
            Page.Header.Controls.Add(Searchcss);

            // Inject script into HEADER
            Literal script = new Literal();
            script.Text = "<script>var _path = \"" + Root.Domain + "\";</script>";
            Page.Header.Controls.Add(script);

            //Response.Write("<script>var _path = \"" + Root.Domain + "\";</script>");


        }


    }
}