using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MyWindowsMediaPlayer.ViewModel;
using MyWindowsMediaPlayer.Model;

namespace MyWindowsMediaPlayer.Dialog
{
    public static class ModifyMediaDialog
    {
        public static void ShowDialog(string title, string message, string type, Media media)
        {
            Form prompt = new Form() { Width = 400, Height = 300, Text = title };

            Label label = new Label() { Left = 20, Top = 20, Width = 80, Text = message };
            Label titleLabel = new Label() { Left = 20, Top = 50, Width = 80, Text = "Title:" };
            TextBox titleTextBox = new TextBox() { Left = 100, Top = 50, Width= 260, Text = media.Title };
            Button confirmation = new Button() { Text = "Ok", Left = 260, Width = 100, Top = 220 };
            
            confirmation.Click += (sender, e) =>
            {
                prompt.Close();
            };

            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(label);
            prompt.Controls.Add(titleLabel);
            prompt.Controls.Add(titleTextBox);

            if (type == "Music")
            {
                Label artistLabel = new Label() { Left = 20, Top = 80, Width = 80, Text = "Artist:" };
                TextBox artistTextBox = new TextBox() { Left = 100, Top = 80, Width = 260, Text = (media as Music).Artist };
                Label albumLabel = new Label() { Left = 20, Top = 110, Width = 80, Text = "Album:" };
                TextBox albumTextBox = new TextBox() { Left = 100, Top = 110, Width = 260, Text = (media as Music).Album };
                Label genreLabel = new Label() { Left = 20, Top = 140, Width = 80, Text = "Genre:" };
                TextBox genreTextBox = new TextBox() { Left = 100, Top = 140, Width = 260, Text = media.Genre };
                Label yearLabel = new Label() { Left = 20, Top = 170, Width = 80, Text = "Year:" };
                TextBox yearTextBox = new TextBox() { Left = 100, Top = 170, Width = 260, Text = media.Year };

                prompt.Controls.Add(artistLabel);
                prompt.Controls.Add(artistTextBox);
                prompt.Controls.Add(albumLabel);
                prompt.Controls.Add(albumTextBox);
                prompt.Controls.Add(genreLabel);
                prompt.Controls.Add(genreTextBox);
                prompt.Controls.Add(yearLabel);
                prompt.Controls.Add(yearTextBox);

                prompt.ShowDialog();

                media.Title = titleTextBox.Text;
                (media as Music).Artist = artistTextBox.Text;
                (media as Music).Album = albumTextBox.Text;
                media.Genre = genreTextBox.Text;
                media.Year = yearTextBox.Text;
            }
            else if (type == "Video")
            {
                Label actorLabel = new Label() { Left = 20, Top = 80, Width = 80, Text = "Actor:" };
                TextBox actorTextBox = new TextBox() { Left = 100, Top = 80, Width = 260, Text = (media as Video).Actor };
                Label directorLabel = new Label() { Left = 20, Top = 110, Width = 80, Text = "Director:" };
                TextBox directorTextBox = new TextBox() { Left = 100, Top = 110, Width = 260, Text = (media as Video).Director };
                Label genreLabel = new Label() { Left = 20, Top = 140, Width = 80, Text = "Genre:" };
                TextBox genreTextBox = new TextBox() { Left = 100, Top = 140, Width = 260, Text = media.Genre };
                Label yearLabel = new Label() { Left = 20, Top = 170, Width = 80, Text = "Year:" };
                TextBox yearTextBox = new TextBox() { Left = 100, Top = 170, Width = 260, Text = media.Year };

                prompt.Controls.Add(actorLabel);
                prompt.Controls.Add(actorTextBox);
                prompt.Controls.Add(directorLabel);
                prompt.Controls.Add(directorTextBox);
                prompt.Controls.Add(genreLabel);
                prompt.Controls.Add(genreTextBox);
                prompt.Controls.Add(yearLabel);
                prompt.Controls.Add(yearTextBox);

                prompt.ShowDialog();

                media.Title = titleTextBox.Text;
                (media as Video).Actor = actorTextBox.Text;
                (media as Video).Director = directorTextBox.Text;
                media.Genre = genreTextBox.Text;
                media.Year = yearTextBox.Text;
            }
            else
            {
                Label authorLabel = new Label() { Left = 20, Top = 80, Width = 80, Text = "Author:" };
                TextBox authorTextBox = new TextBox() { Left = 100, Top = 80, Width = 260, Text = (media as Picture).Author };
                Label genreLabel = new Label() { Left = 20, Top = 110, Width = 80, Text = "Genre:" };
                TextBox genreTextBox = new TextBox() { Left = 100, Top = 110, Width = 260, Text = media.Genre };
                Label yearLabel = new Label() { Left = 20, Top = 140, Width = 80, Text = "Year:" };
                TextBox yearTextBox = new TextBox() { Left = 100, Top = 140, Width = 260, Text = media.Year };

                prompt.Controls.Add(authorLabel);
                prompt.Controls.Add(authorTextBox);
                prompt.Controls.Add(genreLabel);
                prompt.Controls.Add(genreTextBox);
                prompt.Controls.Add(yearLabel);
                prompt.Controls.Add(yearTextBox);

                prompt.ShowDialog();

                media.Title = titleTextBox.Text;
                (media as Picture).Author = authorTextBox.Text;
                media.Genre = genreTextBox.Text;
                media.Year = yearTextBox.Text;
            }
        }
    }
}
