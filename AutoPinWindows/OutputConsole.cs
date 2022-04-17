using System.Windows.Forms;

[System.ComponentModel.DesignerCategory("Code")]
public class OutputConsole : Form
{
    private RichTextBox _textbox;
    public OutputConsole()
    {
        Text = "Output";
        Icon = AutoPinWindows.Properties.Resources.TerminalIcon;
        FormClosing += OutputConsoleClosing;

        _textbox = new System.Windows.Forms.RichTextBox();
        _textbox.Show();

        _textbox.Dock = System.Windows.Forms.DockStyle.Fill;
        _textbox.Location = new System.Drawing.Point(0, 0);
        Controls.Add(_textbox);

        Size = new System.Drawing.Size(Screen.PrimaryScreen.Bounds.Width/2, Screen.PrimaryScreen.Bounds.Height/2);
        StartPosition = FormStartPosition.CenterScreen;
    }

    private void OutputConsoleClosing(object sender, FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true;
            Hide();
        }
    }

    public void AppendMessge(string msg)
    {
        if (Visible)
        {
            _textbox.AppendText(msg);
            _textbox.AppendText("\n");
        }
    }
}