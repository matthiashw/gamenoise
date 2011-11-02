using System;
using CookComputing.XmlRpc;

namespace Webservice
{
    [XmlRpcUrl("http://www.gamenoise.de/services/xmlrpc")] 
    public interface IServiceSystem : IXmlRpcProxy
    {
        [XmlRpcMethod("system.connect")]
        Drupal Connect();

        [XmlRpcMethod("user.login")]
        Drupal Login(string sessid, string username, string password);

        [XmlRpcMethod("user.logout")]
        Object Logout(string sessid);

        [XmlRpcMethod("views.get")]
        Object[] ViewsGet(string sessid, String viewName, String displayId, String[] args, int offset, int limit);
    }

    public class Drupal
    {
        public string sessid;
        public Object user;
    }

}
