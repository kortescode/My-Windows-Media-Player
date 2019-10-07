using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MyWindowsMediaPlayer.ViewModel;

namespace MyWindowsMediaPlayer.Dialog
{
    public static class LibraryPathsDialog
    {
        public static void ShowDialog(string title, string message, List<MyWindowsMediaPlayer.Model.Path> paths, LibraryViewModel vm)
        {
            Form prompt = new Form() { Width = 400, Height = 300, Text = title };

            Label label = new Label() { Left = 20, Top = 20, Width = 150, Text = message };
            ListBox listBox = new ListBox() { Left = 20, Top = 50, Width = 200, Height = 150, DataSource = paths, DisplayMember = "PathSource" };
            Button add = new Button() { Text = "Add", Left = 260, Width = 100, Top = 50 };
            Button del = new Button() { Text = "Del", Left = 260, Width = 100, Top = 90 };
            Button confirmation = new Button() { Text = "Ok", Left = 260, Width = 100, Top = 220 };

            add.Click += (sender, e) =>
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = "Open a folder which contains medias to add";
                dialog.ShowDialog();
                string path = dialog.SelectedPath;
                if (path != "")
                {
                    for (int i = 0; i < paths.Count; i++)
                    {
                        if (path == paths[i].PathSource.OriginalString)
                        {
                            MessageBox.Show("Selected path already exists.", "Path Already Exists Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    int idMax = (paths.Count > 0) ? paths.OrderByDescending(item => item.Id).First().Id + 1 : 1;
                    MyWindowsMediaPlayer.Model.Path newPath = new MyWindowsMediaPlayer.Model.Path(idMax, new Uri(path));
                    Cursor.Current = Cursors.WaitCursor;
                    vm.AddMediasFromPath(newPath);
                    Cursor.Current = Cursors.Default;
                    paths.Add(newPath);
                    listBox.DataSource = null;
                    listBox.DataSource = paths;
                    listBox.DisplayMember = "PathSource";
                }
            };

            del.Click += (sender, e) =>
            {
                if (listBox.SelectedItem != null)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    vm.DelMediasFromPath(listBox.SelectedItem as MyWindowsMediaPlayer.Model.Path);
                    Cursor.Current = Cursors.Default;
                    paths.Remove(listBox.SelectedItem as MyWindowsMediaPlayer.Model.Path);
                    listBox.DataSource = null;
                    listBox.DataSource = paths;
                    listBox.DisplayMember = "PathSource";
                }
            };
            
            confirmation.Click += (sender, e) =>
            {
                prompt.Close();
            };

            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(label);
            prompt.Controls.Add(listBox);
            prompt.Controls.Add(add);
            prompt.Controls.Add(del);
            
            prompt.ShowDialog();
        }
    }
}
