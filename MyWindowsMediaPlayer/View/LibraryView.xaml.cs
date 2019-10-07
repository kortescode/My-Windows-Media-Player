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
using MyWindowsMediaPlayer.ViewModel;

namespace MyWindowsMediaPlayer.View
{
    /// <summary>
    /// Interaction logic for LibraryView.xaml
    /// </summary>
    public partial class LibraryView : UserControl
    {
        public LibraryView()
        {
            InitializeComponent();
        }

        private void MusicsGrid_Drop(object sender, DragEventArgs e)
        {
            if (((LibraryViewModel)this.DataContext) != null)
            {
                string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                foreach (string file in fileList)
                    ((LibraryViewModel)this.DataContext).AddMedia(file);
            }
        }

        private void VideosGrid_Drop(object sender, DragEventArgs e)
        {
            if (((LibraryViewModel)this.DataContext) != null)
            {
                string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                foreach (string file in fileList)
                    ((LibraryViewModel)this.DataContext).AddMedia(file);
            }
        }

        private void PicturesGrid_Drop(object sender, DragEventArgs e)
        {
            if (((LibraryViewModel)this.DataContext) != null)
            {
                string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                foreach (string file in fileList)
                    ((LibraryViewModel)this.DataContext).AddMedia(file);
            }
        }
    }
}
