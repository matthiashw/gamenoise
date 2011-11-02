/* 
 * author: Andreas Knöpfle  
 * created: 30.11.2008
 * 
 * modification history
 * --------------------
 * 02.12.08 MHO:
 * Replaced show with showdialog
 * 
 */

using System;
using System.Windows;
using System.Windows.Input;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for Error.xaml
    /// </summary>
    public partial  class Error
    {
        private readonly bool _shutdown;

        /// <summary>
        /// General Error Screen Constructor
        /// </summary>
        /// <param name="errortext">Text To Show</param>
        /// <param name="shutdown">Should Player shut down?</param>
        /// <param name="owner">Owner Window if any else null</param>
        public Error(string errortext, bool shutdown, Window owner)
        {
            try
            {
                Owner = owner;
            }
            catch (InvalidOperationException)
            {}
            catch(ArgumentException)
            {}

            _shutdown = shutdown;
            InitializeComponent();
            errorText.Text = errortext;
            ShowDialog();  
        }

        /// <summary>
        /// Move Error Screen
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void Error_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        /// <summary>
        /// User Clicked OK 
        /// Either Application Shutdown od Window Hide
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void Error_OK(object sender, RoutedEventArgs e)
        {
            if (_shutdown)
                Application.Current.Shutdown();
            else
                Close();
        }
    }
}
