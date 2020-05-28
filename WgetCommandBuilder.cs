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
        private bool _isLinux {get;set;}

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

        public WgetCommandBuilder IsLinux(bool isLinux)
        {
            _isLinux = isLinux;
            return this;
        }

        public WgetCommand Build()
        {
            WgetCommand command = new WgetCommand();

            string standardOutput = _isLinux ? "-o /dev/stdout" : "";

            /* This command will only work on bash or linux at the moment. */
            command.Parameters =  string.Format("wget {0} -P {1} -c {2} ", standardOutput, _prefix, _url);
            return command;
        }

    }
}