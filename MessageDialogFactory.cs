using Gtk;
using System;

namespace bindu
{
    public class MessageDialogFactory
    {
        public static MessageDialog CreateError(Window window, Exception exception)
        {
            return new MessageDialog(window, DialogFlags.Modal, MessageType.Error, ButtonsType.Close, exception.Message);
        }        
    }
}