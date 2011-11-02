using System;

namespace Webservice
{
    public interface IWebserviceClient
    {
        event WebServiceConnected Connected;
        event WebServiceLoggedIn LoggedIn;
        bool IsConnected();
        bool IsLoggedIn();
        Object[] GetView(String viewName, String displayId, String[] args, int offset, int limit);
    }
}
