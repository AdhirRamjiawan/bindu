using System;
using System.Text.RegularExpressions;
using System.Linq;

namespace bindu
{
    public class WgetOutput
    {
        public double PercentageComplete {get;set;}
        public double DownloadRate {get;set;}

    }

    public class WgetOutputParser
    {
        public WgetOutput _output;

        public WgetOutputParser()
        {
        }

        public WgetOutput Parse(string outputLine)
        {
            //"     0K .......... .......... .......... .......... ..........  1% 79.4K 34s"
            Regex regex = new Regex(@"^(\s)*[0-9]*K(.)*[0-9]*%(.)*$");
            _output = new WgetOutput();

            if (String.IsNullOrEmpty(outputLine))
                return null;
            
            if (!regex.Matches(outputLine).Any())
                return null;

            string[] progParts = outputLine.Split(' ');
            string strPerc = progParts.Where(p => p.Contains("%")).FirstOrDefault();

            if (!string.IsNullOrEmpty(strPerc))
                _output.PercentageComplete = Double.Parse(strPerc.Replace("%",""));

            return _output;
        }
    }
}