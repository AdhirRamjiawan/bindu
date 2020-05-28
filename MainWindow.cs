using System;
using System.Diagnostics;
using Gtk;
using System.Text;
using UI = Gtk.Builder.ObjectAttribute;

namespace bindu
{
    class MainWindow : Window
    {
        [UI] private Entry txtDownloadUrl = null;
        [UI] private Entry txtDestinationPath = null;
        [UI] private Button btnDownload = null;

        private int _counter;

        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetObject("MainWindow").Handle)
        {
            builder.Autoconnect(this);

            DeleteEvent += Window_DeleteEvent;

            btnDownload.Clicked += btnDownload_Clicked;
        }

        private void btnDownload_Clicked(object sender, EventArgs e)
        {
            try
            {
                WgetCommand command = 
                    new WgetCommandBuilder()
                    .DestinationPrefix(txtDestinationPath.Text)
                    .DownloadUrl(txtDownloadUrl.Text)
                    .Build();

                var startInfo = new ProcessStartInfo(command.Command, command.Parameters);
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.CreateNoWindow = true;

                var process = Process.Start(startInfo);
                StringBuilder outputBuilder = new StringBuilder();

                while(!process.StandardOutput.EndOfStream)
                    outputBuilder.Append(process.StandardOutput.ReadLine());


                Console.WriteLine(outputBuilder.ToString());

                process.WaitForExit();
            }
            catch (Exception exception)
            {
                MessageDialog dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Close, exception.Message);
                dialog.Run();
                dialog.Dispose();
            }
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            //Console.WriteLine(txtDownloadUrl.Text);
            Application.Quit();
        }

    }
}
