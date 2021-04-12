using CommandLine;

namespace TimestampVersion
{
    class CLOptions
    {
        [Option("xml-file", HelpText = "Xml file to create or update")]
        public string XmlFile { get; set; }

        [Option("env-file", HelpText = "Github Actions environment file to create or update")]
        public string EnvironmentFile { get; set; }
    }
}
