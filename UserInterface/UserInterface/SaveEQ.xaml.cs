/* 
 * author: Matthias Hillert
 * created: 21.11.2008
 * 
 * modification history
 * --------------------
 * MHI(26.11.08)
 * added save functionality
 * 
 * MHI(01.12.08)
 * added close functionality
 * 
 */
using System.Windows;
using System.Windows.Input;
using PlayControl;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for SaveEQ.xaml
    /// </summary>
    public partial class SaveEQ
    {
        private readonly PlayControler _myPlayControler;
        
        public SaveEQ(PlayControler aPlayControler,Window w1)
        {
            _myPlayControler = aPlayControler;
            
            try
            {
                Owner = w1;    
            }
            catch (System.ArgumentException)
            {}
            catch (System.InvalidOperationException)
            {}

            InitializeComponent();
        }

        /// <summary>
        /// save the Equalizer Settings under the name which was typed in the textbox
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void saveEQ_buttonSaveClick(object sender, RoutedEventArgs e)
        {
            _myPlayControler.SaveEqualizer(saveEQName.Text);
            
            Close();
        }

        /// <summary>
        /// Drag the Window by just click on whitespace and drag
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void SaveEQ_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();    
            }
            catch (System.InvalidOperationException)
            {}
            
        }

        /// <summary>
        /// close the window
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void saveEQ_buttonCloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
