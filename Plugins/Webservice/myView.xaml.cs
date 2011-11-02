using System;
using System.Windows;
using System.Windows.Input;

namespace Webservice
{
    /// <summary>
    /// Interaktionslogik für myView.xaml
    /// </summary>
    partial class MyView
    {
        private readonly WebserviceClient _webservice;
        private readonly Interfaces.IPluginHost _aHost;
        readonly Interfaces.IPlugin _aPlugin;

        public MyView(Interfaces.IPlugin aPlugin, WebserviceClient webservice, Interfaces.IPluginHost aHost)
        {
            InitializeComponent();

            _aPlugin = aPlugin;
            _webservice = webservice;
            _aHost = aHost;
            textBoxStatus.Text = "Disconnected";
        }

        private void buttonRegister_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.gamenoise.de/user/register");
        }

        private void buttonLogin_Click(object sender, RoutedEventArgs e)
        {
            textBoxStatus.Text = "Connecting ...";

            try
            {
                _webservice.Connect();
                _webservice.Login(textBoxUserName.Text, textBoxPassword.Password);
                textBoxStatus.Text = "Connected!";
            }
            catch (Exception ex)
            {
                textBoxStatus.Text = "Connection failed:\n" + ex.Message;
            }
        }

        private void buttonDisconnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _webservice.Logout();
            }
            catch (Exception)
            {}

            textBoxStatus.Text = "Disconnected";
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            _aHost.pluginClose(_aPlugin);
        }

        private void textBoxPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                buttonLogin_Click(sender, e);
            }
        }
    }
}
