using System;

namespace bindu
{

    public class WgetCommand
    {
        public string Command {get {return "wget";} }
        public string Parameters {get;set;}
    }

    public class WgetCommandBuilder
    {
        private string _prefix {get;set;}
        private string _url {get;set;}

        public WgetCommandBuilder()
        {
        }

        public WgetCommandBuilder DestinationPrefix(string prefix)
        {
            _prefix = String.IsNullOrEmpty(prefix) ? "" : prefix;
            return this;
        }

        public WgetCommandBuilder DownloadUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new Exception("Wget: Download url is required!");

            _url = url;
            return this;
        }

        public WgetCommand Build()
        {
            WgetCommand command = new WgetCommand();
            command.Parameters =  string.Format("wget -P {0} -c {1}", _prefix, _url);
            return command;
        }

    }
}