using System;
using System.Diagnostics;
using Gtk;
using System.Text;
using System.Linq;

using System.Threading.Tasks;
using System.Web;

namespace bindu
{
    partial class MainWindow : Window
    {
        private ILocalStorage _storage;

        public MainWindow() : this(new Builder("MainWindow.glade")) 
        { 
            _storage = LocalStorage.GetInstance();
        }

        private MainWindow(Builder builder) : base(builder.GetObject("MainWindow").Handle)
        {
            builder.Autoconnect(this);

            DeleteEvent += Window_DeleteEvent;

            btnDownload.Clicked += btnDownload_Clicked;
/* 
            var row = new ListBoxRow();
            var b = new Box(Orientation.Horizontal, 50);
            b.Add(new Label("test 1"));
            row.Add(b);
            lstPendingDownloads.SelectionMode = SelectionMode.Single;
            lstPendingDownloads.Add(row); */
        }

        private void btnDownload_Clicked(object sender, EventArgs e)
        {
            try
            {
                string encodedUrl = HttpUtility.UrlEncode(txtDownloadUrl.Text);
                
                if (!isUrlPending(txtDownloadUrl.Text))
                {
                    _storage.GetData().PendingDownloadUrls.Add(encodedUrl);
                    _storage.Persist();
                }

                toggleInput(true);
                WgetCommand command = 
                        new WgetCommandBuilder()
                        .DestinationPrefix(txtDestinationPath.Text)
                        .DownloadUrl(txtDownloadUrl.Text)
                        .IsLinux(true)
                        .Build();

                Task.Run(()=>{
                    Process process = new Process();
                    process.StartInfo = new ProcessStartInfo(command.Command, command.Parameters);
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.CreateNoWindow = true;
                    process.OutputDataReceived += new DataReceivedEventHandler(WgetOutput_Handler);
                    

                    process.Start();
                    process.BeginOutputReadLine();

                    process.WaitForExit();
                }).GetAwaiter().OnCompleted(() => {
                    toggleInput(false);

                    _storage.GetData().PendingDownloadUrls.Remove(encodedUrl);
                    _storage.Persist();
                });
                
                
            }
            catch (Exception exception)
            {
                using(var dialog = MessageDialogFactory.CreateError(this, exception))
                    dialog.Run();
            }
        }

        private void WgetOutput_Handler(object sender, DataReceivedEventArgs e)
        {
            WgetOutputParser parser = new WgetOutputParser();
            WgetOutput output;

            Console.WriteLine(e.Data);
            if (String.IsNullOrEmpty(e.Data))
                return;

            Gtk.Application.Invoke(delegate {
                output = parser.Parse(e.Data);

                if (output != null)
                {
                    pbDownload.Fraction = output.PercentageComplete / 100;
                    lblDownloadPercentage.Text = string.Format("{0}%", output.PercentageComplete);
                }
            });
        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            //Console.WriteLine(txtDownloadUrl.Text);
            Application.Quit();
        }

        private void toggleInput(bool lockUI)
        {
            StateFlags flags = lockUI ? StateFlags.Insensitive : StateFlags.Normal;

            txtDownloadUrl.SetStateFlags(flags, true);
            txtDestinationPath.SetStateFlags(flags, true);
            btnDownload.SetStateFlags(flags, true);
        }

        private bool isUrlPending(string url)
        {
            bool result = _storage.GetData()
                .PendingDownloadUrls
                .Where(u => u == url).Any();
            return result;
        }

    }
}
