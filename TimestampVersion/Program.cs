using CommandLine;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace TimestampVersion
{
    class Program
    {
        static int Main(string[] args)
        {
            int ret = 0;
            
            try
            {
                Parser.Default.ParseArguments<CLOptions>(args)
                    .WithParsed(opts => Run(opts))
                    .WithNotParsed(opts => ret = -1);
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.WriteLine();
            }

            return ret;
        }

        static void Run(CLOptions opts)
        {
            var version = Generator.Generate();

            Console.WriteLine($"TIMESTAMP_VERSION={version}");


            if (!string.IsNullOrWhiteSpace(opts.EnvironmentFile))
            {
                Console.WriteLine("Saving to: {0}", opts.EnvironmentFile);
                try { Directory.CreateDirectory(Path.GetDirectoryName(opts.EnvironmentFile)); }
                catch { }
                File.AppendAllLines(opts.EnvironmentFile, new string[] { $"TIMESTAMP_VERSION={version}" });
            }
        
            if(!string.IsNullOrWhiteSpace(opts.XmlFile))
            {
                try { Directory.CreateDirectory(Path.GetDirectoryName(opts.XmlFile)); }
                catch { }

                if (File.Exists(opts.XmlFile))
                {
                    var doc = XDocument.Load(opts.XmlFile);
                    try
                    {
                        var node = doc.Root.Descendants("Version").First();
                        node.SetValue(version);
                    }
                    catch 
                    {
                        throw new Exception("Version not found in existing xml file");
                    }
                    Console.WriteLine("Saving to: {0}", opts.XmlFile);
                    doc.Save(opts.XmlFile);
                }
                else
                {
                    File.WriteAllText(opts.XmlFile, $"<Version>{version}</Version>"); 
                }
            }
        }
    }
}
