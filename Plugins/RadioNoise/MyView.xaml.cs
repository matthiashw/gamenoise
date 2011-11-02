using System.Windows;
using Interfaces;
using System;
using System.Windows.Data;
using System.ComponentModel;


namespace RadioNoise
{
    /// <summary>
    /// Interaktionslogik für MyView.xaml
    /// </summary>
    public partial class MyView
    {
        readonly IPluginHost _myHost;

        private readonly IPlugin _myPlugin;
        private readonly IPlugin _aWebservicePlugin;
        private readonly StreamManager _aStreamManagerAll;
        private readonly StreamManager _aStreamManagerFavorites;
        private readonly Webservice.IWebserviceClient _aWebserviceClient;
        private Webservice.WebServiceLoggedIn _aLoggedInHandler;
        private bool _refreshScheduler;
        private int resolves = 0;

        public MyView(IPluginHost myHost, IPlugin myPlugin, Webservice.IWebserviceClient aWebserviceClient, IPlugin aWebservicePlugin)
        {
            InitializeComponent();


            _aWebserviceClient = aWebserviceClient;
            _myHost = myHost;
            _myPlugin = myPlugin;
            _aStreamManagerAll = new StreamManager(aWebserviceClient);
            _aStreamManagerFavorites = new StreamManager(aWebserviceClient);
            _aWebservicePlugin = aWebservicePlugin;

            //Init Binding
            ICollectionView viewAll = CollectionViewSource.GetDefaultView(_aStreamManagerAll.GetStreams());
            new TextSearchFilter(viewAll, textBoxSearch);

            ICollectionView viewFavorites = CollectionViewSource.GetDefaultView(_aStreamManagerFavorites.GetStreams());
            new TextSearchFilter(viewFavorites, textBoxSearch);

            listView.DataContext = viewAll;
            Binding bindAll = new Binding();
            listView.SetBinding(System.Windows.Controls.ItemsControl.ItemsSourceProperty, bindAll);

            listViewFavorites.DataContext = viewFavorites;
            Binding bindFav = new Binding();
            listViewFavorites.SetBinding(System.Windows.Controls.ItemsControl.ItemsSourceProperty, bindFav);

            _aStreamManagerAll.Load(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Gamenoise\\streamsAll.xml");
            _aStreamManagerFavorites.Load(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Gamenoise\\streamsFavorite.xml");

        }

    


        private void RefreshList()
        {
            if (!_aWebserviceClient.IsLoggedIn())
            {
                _refreshScheduler = true;
                _aLoggedInHandler = new Webservice.WebServiceLoggedIn(AWebserviceClientLoggedIn);
                _aWebserviceClient.LoggedIn += _aLoggedInHandler;
                _myHost.ShowPlugin(_aWebservicePlugin);
                return;
            }

            /*
            String[] fields = new String[3];
            fields[0] = "node_title";
            fields[1] = "node_data_field_streamlink_field_streamlink_url";
            fields[2] = "node_data_field_streamlink_field_bitrate_value";
            */
            String displayId = "default";
            String[] args = new String[0];
            Object[] streams = _aWebserviceClient.GetView("streamlibrary2", displayId, args, 0, 0);


            // before adding streams remove the old ones to prevent duplicates
            _aStreamManagerAll.RemoveAll();

            // now add them to streamlist
            foreach (CookComputing.XmlRpc.XmlRpcStruct stream in streams)
            {
                _aStreamManagerAll.AddStream(new Stream(
                        stream["node_title"].ToString(),
                        stream["node_data_field_streamlink_field_streamlink_url"].ToString(),
                        "",
                        stream["node_data_field_streamlink_field_bitrate_value"].ToString()));
            }

            streams = _aWebserviceClient.GetView("flag_streams", displayId, args, 0, 0);

            // before adding streams remove the old ones to prevent duplicates
            _aStreamManagerFavorites.RemoveAll();

            // now add them to streamlist
            foreach (CookComputing.XmlRpc.XmlRpcStruct stream in streams)
            {
                _aStreamManagerFavorites.AddStream(new Stream(
                        stream["node_title"].ToString(),
                        stream["node_data_field_streamlink_field_streamlink_url"].ToString(),
                        "",
                        stream["node_data_field_streamlink_field_bitrate_value"].ToString()));
            }

            _aStreamManagerAll.Save(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Gamenoise\\streamsAll.xml");
            _aStreamManagerFavorites.Save(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Gamenoise\\streamsFavorite.xml");
        }

        void AWebserviceClientLoggedIn()
        {
            _aWebserviceClient.LoggedIn -= _aLoggedInHandler;
            if (!_refreshScheduler) 
                return;
            
            _myHost.pluginClose(_aWebservicePlugin);
            RefreshList();
        }

        private void buttonPluginRNadd_Click(object sender, RoutedEventArgs e)
        {
            _myHost.getPlaylist().addSong(String.Empty, textBoxPluginRNStream.Text, String.Empty, textBoxPluginRNStream.Text, 5999);
        }

        private void buttonPluginRNclose_Click(object sender, RoutedEventArgs e)
        {
            _myHost.pluginClose(_myPlugin);
        }

        private void buttonPluginRNPlay_Click(object sender, RoutedEventArgs e)
        {
            _myHost.getPlayControler().Play();
        }

        private void buttonPluginRNStop_Click(object sender, RoutedEventArgs e)
        {
            _myHost.getPlayControler().Stop();
        }

        private void buttonPluginRNloadStreams_Click(object sender, RoutedEventArgs e)
        {
            RefreshList();
            
        }

        private void listView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            String url = ((Stream)listView.SelectedItem).Url;
            //TODO
            if (url != null)
            {
                int index = _myHost.getPlaylist().addSong(String.Empty, ((Stream) listView.SelectedItem).Title,
                                                          String.Empty, url, 5999);
                _myHost.getPlaylist().setNowPlayingPosition(index);
                _myHost.getPlayControler().Stop();
                _myHost.getPlayControler().Play();
            }
        }

        private void expander_Expanded(object sender, RoutedEventArgs e)
        {
            gridWrapper.Height = 350;
        }

        private void expander_Collapsed(object sender, RoutedEventArgs e)
        {
            gridWrapper.Height = 110;
        }

        private void buttonPluginRNaddSearch_Click(object sender, RoutedEventArgs e)
        {
            textBoxSearch.Clear();
        }

        private void buttonPluginRNclearSearch_Click(object sender, RoutedEventArgs e)
        {
            _aStreamManagerAll.RemoveAll();
            _aStreamManagerFavorites.RemoveAll();
        }
    }
}
