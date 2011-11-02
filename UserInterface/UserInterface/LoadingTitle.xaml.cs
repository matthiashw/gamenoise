/* 
 * author: PL
 * 
 * created: ?.12.2008
 * 
 * modification history
 * --------------------
 * 
 * AK (23.12.2008)
 * Playlist LoadingTitle CancelButton added
 * 
 * 
 * */

using System;
using System.Windows;
using System.Windows.Input;
using Interfaces;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for LoadingTitle.xaml
    /// </summary>
    public partial class LoadingTitle : IObserver 
    {
        private readonly Standard _main;
        private readonly IPlaylist _aPlaylist;
        
        public LoadingTitle(Standard w1,IPlaylist aPlaylist)
        {
            Owner = w1;
            InitializeComponent();
            _main = w1;
            _aPlaylist = aPlaylist;
        }

        /// <summary>
        /// close the window
        /// </summary>
        public void Update(Object o)
        {
            var s = o as string;
            if(s != null)
                nowLoadingTitle.Content = o;


            if (_aPlaylist.loadingRecursionDepth != 0) return;
            
            try
            {
                Hide();
            }
            catch (InvalidOperationException)
            { }
            
            _main.InsertFolderprop = false;
            _main.Update(_aPlaylist);
            _aPlaylist.regenerateList();
        }

        /// <summary>
        /// Drag the Window by just click on whitespace and drag
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void LoadingTitle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch (System.Reflection.TargetInvocationException) 
            {}
            catch (InvalidOperationException)
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

        /// <summary>
        /// Cancel Loading
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            _aPlaylist.stopRequest = true;
            try
            {
                Hide();
            }
            catch (InvalidOperationException)
            { }

            _main.InsertFolderprop = false;
            _main.Update(_aPlaylist);

        }
    }
}
