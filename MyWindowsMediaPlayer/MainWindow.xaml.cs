using MyWindowsMediaPlayer.Model;
using MyWindowsMediaPlayer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyWindowsMediaPlayer
{
    public partial class MainWindow : Window
    {
        #region Fields
        private bool isFullScreenEnabled = false;
        #endregion

        #region Constructors
        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }
        #endregion

        #region Events
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.MainPage.DataContext = new MainPageViewModel();
            ((MainPageViewModel)this.MainPage.DataContext).EventMediaDoubleClicked += MainPage_MediaDoubleClicked;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11)
                this.FullScreenManage();
        }

        private void MainPage_MediaDoubleClicked(object sender, EventArgs e)
        {
            this.FullScreenManage();
        }
        #endregion

        #region Methods
        private void FullScreenManage()
        {
            if (!this.isFullScreenEnabled)
            {
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
                this.isFullScreenEnabled = true;
            }
            else
            {
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = WindowState.Normal;
                this.isFullScreenEnabled = false;
            }
        }
        #endregion
    }
}
