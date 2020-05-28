using System;
using System.Diagnostics;
using Gtk;
using System.Text;
using System.Linq;
using UI = Gtk.Builder.ObjectAttribute;
using System.Threading.Tasks;

namespace bindu
{
    class MainWindow : Window
    {
        [UI] private Entry txtDownloadUrl = null;
        [UI] private Entry txtDestinationPath = null;
        [UI] private Button btnDownload = null;
        [UI] private ProgressBar pbDownload = null;

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
                btnDownload.SetStateFlags(StateFlags.Insensitive, true);
                Task.Run(()=>{
                    WgetCommand command = 
                        new WgetCommandBuilder()
                        .DestinationPrefix(txtDestinationPath.Text)
                        .DownloadUrl(txtDownloadUrl.Text)
                        .Build();

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
                MessageDialog dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Close, exception.Message);
                dialog.Run();
                dialog.Dispose();
            }
        }

        private void WgetOutput_Handler(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
            if (String.IsNullOrEmpty(e.Data))
                return;

            Gtk.Application.Invoke(delegate {
                string[] progParts = e.Data.Split(' ');
                double perc =  0;
                
                string strPerc = progParts.Where(p => p.Contains("%")).FirstOrDefault();

                if (!string.IsNullOrEmpty(strPerc))
                {
                    perc = Double.Parse(strPerc.Replace("%",""));
                    pbDownload.Fraction = perc / 100;
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
