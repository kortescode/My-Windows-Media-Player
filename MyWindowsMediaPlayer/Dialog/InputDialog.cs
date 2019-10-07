using System.Windows.Forms;

namespace MyWindowsMediaPlayer.Dialog
{
    public static class InputDialog
    {
        public static string ShowDialog(string title, string message, string text)
        {
            Form prompt = new Form() { Width = 400, Height = 150, Text = title };

            Label label = new Label() { Left = 20, Top = 20, Width = 80, Text = message };
            TextBox textBox = new TextBox() { Left = 100, Top = 20, Width= 260, Text = text };
            Button cancel = new Button() { Text = "Cancel", Left = 150, Width = 100, Top = 70 };
            Button confirmation = new Button() { Text = "Ok", Left = 260, Width = 100, Top = 70 };

            cancel.Click += (sender, e) =>
            {
                textBox.Text = "";
                prompt.Close();
            };
            
            confirmation.Click += (sender, e) =>
            {
                prompt.Close();
            };

            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);
            prompt.Controls.Add(label);
            prompt.Controls.Add(textBox);
            
            prompt.ShowDialog();

            return textBox.Text;
        }
    }
}
