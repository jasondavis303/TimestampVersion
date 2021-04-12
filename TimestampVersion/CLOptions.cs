using CommandLine;
using System.Collections.Generic;

namespace TimestampVersion
{
    class CLOptions
    {
        [Option("xml-files", HelpText = "Xml file to create or update")]
        public IEnumerable<string> XmlFiles { get; set; }

        [Option("env-file", HelpText = "Github Actions environment file to create or update (only works on Ubuntu)")]
        public string EnvironmentFile { get; set; }

        [Option("set-ov", HelpText ="Set the output variable TIMESTAMP_VERSION in Github Actions (works on Ubuntu and Windows)")]
        public bool SetGAE { get; set; }

        [Option("verbose")]
        public bool Verbose { get; set; }
    }
}
