using System;
using System.Diagnostics;
using Gtk;
using System.Text;
using System.Linq;

using System.Threading.Tasks;

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
        }

        private void btnDownload_Clicked(object sender, EventArgs e)
        {
            try
            {
                btnDownload.SetStateFlags(StateFlags.Insensitive, true);
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
                    btnDownload.SetStateFlags(StateFlags.Normal, true);    
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

    }
}
