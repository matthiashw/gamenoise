using System.Windows;
using System.Windows.Input;

namespace UserInterface
{
    /// <summary>
    /// Interaktionslogik für YesNo.xaml
    /// </summary>
    public partial class YesNo
    {
        /// <summary>
        /// Constructor for Yes/No - Window
        /// </summary>
        /// <param name="owner">Owner window to center Dialog</param>
        public YesNo(Window owner)
        {
            InitializeComponent();
            Owner = owner;
        }

        /// <summary>
        /// Drag the Window by just click on whitespace and drag
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">MouseArgs</param>
        private void YesNo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch (System.Reflection.TargetInvocationException) { }
        }
        
        /// <summary>
        /// no click
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Button Arguments</param>
        private void buttonNoClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// yes click
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Button Arguments</param>
        private void buttonYesClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
