using System;
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
            _output = new WgetOutput();

            if (String.IsNullOrEmpty(outputLine))
                return null;
            
            string[] progParts = outputLine.Split(' ');
            string strPerc = progParts.Where(p => p.Contains("%")).FirstOrDefault();

            if (!string.IsNullOrEmpty(strPerc))
                _output.PercentageComplete = Double.Parse(strPerc.Replace("%",""));

            return _output;
        }
    }
}