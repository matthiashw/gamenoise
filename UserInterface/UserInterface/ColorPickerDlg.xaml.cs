using System;
using System.Windows;
using System.Windows.Media;

namespace UserInterface
{
    /// <summary>
    /// Interaktionslogik für ColorPickerDlg.xaml
    /// </summary>
    public partial class ColorPickerDlg
    {
        /// <summary>
        /// Constructor for ColorPicker
        /// </summary>
        public ColorPickerDlg()
        {
            InitializeComponent();
            Width = 138;
            Height = 115;
            UpdateColor();
        }

        private Color _selectedColor;
        private SolidColorBrush _selectedColorBrush = new SolidColorBrush();
        private Byte _red;
        private Byte _green;
        private Byte _blue;


        public SolidColorBrush ColorBrush
        {
            get { return _selectedColorBrush; }
            set
            {
                _selectedColorBrush = value;
                _selectedColor = _selectedColorBrush.Color;
                sliderRed.Value = value.Color.R;
                sliderGreen.Value = value.Color.G;
                sliderBlue.Value = value.Color.B;
            }
        }

        /// <summary>
        /// Change Red Value
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">New Value</param>
        private void sliderRed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _red = (Byte)e.NewValue;
            UpdateColor();
        }

        /// <summary>
        /// Change Green Value
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">New Value</param>
        private void sliderGreen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _green = (Byte)e.NewValue;
            UpdateColor();
        }

        /// <summary>
        /// Change Blue Value 
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">Blue Value</param>
        private void sliderBlue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _blue = (Byte)e.NewValue;
            UpdateColor();
        }

        /// <summary>
        /// Update Color in Preview
        /// </summary>
        private void UpdateColor()
        {
            _selectedColor.A = 255; 
            _selectedColor.R = _red; 
            _selectedColor.G = _green; 
            _selectedColor.B = _blue;
            _selectedColorBrush = new SolidColorBrush(_selectedColor);
            colorPreview.Fill = _selectedColorBrush;
            labelColor.Content = "" + _selectedColor.R + ", " + _selectedColor.G + ", " + _selectedColor.B;
            labelColorHex.Content = _selectedColor.ToString();
        }

        /// <summary>
        /// Click OK
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        /// <summary>
        /// Click Cancel
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }




    }
}
