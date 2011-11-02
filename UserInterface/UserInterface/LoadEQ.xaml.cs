/* 
 * author: Matthias Hillert
 * created: 21.11.2008
 * 
 * modification history
 * --------------------
 * 
 * MHI(26.11.08)
 * added some functionality
 * 
 */
using System.Windows;
using System.Windows.Input;
using PlayControl;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for LoadEQ.xaml
    /// </summary>
    public partial class LoadEQ
    {
        private readonly PlayControler _myPlayControler;
        private string[] _eqList;

        /// <summary>
        /// Constructor Equalizer Window Load
        /// </summary>
        /// <param name="aPlayControler">not used</param>
        /// <param name="w1">not used</param>
        public LoadEQ(PlayControler aPlayControler,Window w1)
        {
            Owner = w1;
            _myPlayControler = aPlayControler;
            InitializeComponent();
        }

        /// <summary>
        /// Window has Loaded
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ClearListView();
        }

        /// <summary>
        /// loads the select EQ setting
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void mouseDoubleClick_LoadEQ(object sender, MouseButtonEventArgs e)
        {
            // get the current selected item
            string tmp = (string)listViewSaveEQ.SelectedItem;

            // and give it to the PlayControler
            if (tmp != null)
                _myPlayControler.LoadEqualizer(tmp);
         
            Close();
        }

        /// <summary>
        /// Drag the Window by just click on whitespace and drag
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void LoadEQ_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        /// <summary>
        /// deletes a EQ Setting and updates the listView
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buttonSaveEQ_delete_Click(object sender, RoutedEventArgs e)
        {
            _myPlayControler.DeleteEqualizerSetting((string)listViewSaveEQ.SelectedValue);
            
            // update the listView
            ClearListView();
        }

        /// <summary>
        /// loads the select EQ setting
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buttonSaveEQ_load_Click(object sender, RoutedEventArgs e)
        {
            // get the current selected item
            string tmp = (string)listViewSaveEQ.SelectedItem;

            // and give it to the PlayControler
            if (tmp != null)
                _myPlayControler.LoadEqualizer(tmp);

            Close();
        }

        /// <summary>
        /// closes the window
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buttonSaveEQ_close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// clears the listView and reloads it
        /// </summary>
        public void ClearListView()
        {
            _eqList = _myPlayControler.GetEQPresetList();

            listViewSaveEQ.Items.Clear();
            for (int i = 0; i < _eqList.Length; i++)
                listViewSaveEQ.Items.Add(_eqList[i]);
        }

        
    }
}
