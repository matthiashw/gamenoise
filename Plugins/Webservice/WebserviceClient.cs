using System;
using CookComputing.XmlRpc;
using UserInterface;

namespace Webservice
{
    public delegate void WebServiceConnected();
    public delegate void WebServiceLoggedIn();

    class WebserviceClient : IWebserviceClient
    {
        private IServiceSystem _iss;
        private bool _connected;
        private bool _loggedIn;
        private Drupal _connection;
        private String _sessionID;
        public event WebServiceConnected Connected;
        public event WebServiceLoggedIn LoggedIn;

        protected virtual void OnConnected()
        {
            if (Connected != null)
                Connected();
        }

        protected virtual void OnLoggedIn()
        {
            if (LoggedIn != null)
                LoggedIn();
        }


        /// <summary>
        /// Connecting to the gamenoise Site
        /// </summary>
        /// <returns>Information about the user</returns>
        public void Connect()
        {
            //Connecting to gamenoise.de with password ... no more needed
            //NetworkCredential networkCredential = new NetworkCredential("*****", "******");

            try
            {
                _iss = XmlRpcProxyGen.Create<IServiceSystem>();
                //_iss.Credentials = networkCredential;
                _connection = _iss.Connect();
                _sessionID = _connection.sessid;

                _connected = true;
                OnConnected();
            }
            catch(Exception ex)
            {
                Logout();
                throw ex;
            }


        }

        public Object Login(String username, String password)
        {
            Drupal user;

            try
            {
                user = _iss.Login(_sessionID, username, password);
                _sessionID = user.sessid;

                _loggedIn = true;
                OnLoggedIn();
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return user;
        }

        public void Logout()
        {
            try
            {
                _iss.Logout(_sessionID);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            _sessionID = _connection.sessid;
            _connection = null;
            _connected = false;
            _loggedIn = false;
        }

        public bool IsLoggedIn()
        {
            return _loggedIn;
        }


        public Object[] GetView(String viewName, String displayId, String[] args, int offset, int limit)
        {
            Object[] view = null;
            try
            {
                view = _iss.ViewsGet(_sessionID, viewName, displayId, args, offset, limit);
            }
            catch (XmlRpcServerException ex)
            {
                new Error(ex.Message,false,null);
            }

            return view;
        }
    
        // Get the current Unix time stamp.
/*
        private String GetUnixTimestamp()
        {
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return Convert.ToString(Convert.ToUInt64(ts.TotalSeconds));
        }
*/

        // Similar to the 'user_password' function Drupal uses.
/*
        private string GetNonce(int length)
        {
            string allowedCharacters = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            StringBuilder password = new StringBuilder();
            Random rand = new Random();

            for (int i = 0; i < length; i++)
            {
                password.Append(allowedCharacters[rand.Next(0, (allowedCharacters.Length - 1))]);
            }
            return password.ToString();
        }
*/

        // Compute the hash value.
/*
        private string GetHMAC(string message, string key)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(key);
            byte[] messageByte = encoding.GetBytes(message);

            HMACSHA256 hmac = new HMACSHA256(keyByte);
            byte[] hashMessageByte = hmac.ComputeHash(messageByte);

            string sbinary = String.Empty;
            for (int i = 0; i < hashMessageByte.Length; i++)
            {
                // Converting to hex, but using lowercase 'x' to get lowercase characters
                sbinary += hashMessageByte[i].ToString("x2");
            }

            return sbinary;
        }
*/

        public bool IsConnected()
        {
            return _connected;
        }
    }
}
