using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace bindu
{
    partial class MainWindow
    {
        [UI] private Entry txtDownloadUrl = null;
        [UI] private Entry txtDestinationPath = null;
        [UI] private Button btnDownload = null;
        [UI] private ProgressBar pbDownload = null;
        [UI] private Label lblDownloadPercentage = null;
    }
}