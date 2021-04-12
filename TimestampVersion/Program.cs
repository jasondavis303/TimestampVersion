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

            if(opts.Verbose)
                Console.WriteLine($"Generated TIMESTAMP_VERSION={version}");

            if(opts.SetGAE)
            {
                if (opts.Verbose)
                    Console.WriteLine("Setting output variable");
                Console.Out.WriteLine($"::set-output name=TIMESTAMP_VERSION::{version}");
            }
            
            if (!string.IsNullOrWhiteSpace(opts.EnvironmentFile))
            {
                if(opts.Verbose)
                    Console.WriteLine("Saving to: {0}", opts.EnvironmentFile);
                try { Directory.CreateDirectory(Path.GetDirectoryName(opts.EnvironmentFile)); }
                catch { }
                File.AppendAllLines(opts.EnvironmentFile, new string[] { $"TIMESTAMP_VERSION={version}" });
            }

            foreach (var xmlFile in opts.XmlFiles)
            {
                try { Directory.CreateDirectory(Path.GetDirectoryName(xmlFile)); }
                catch { }

                if (File.Exists(xmlFile))
                {
                    var doc = XDocument.Load(xmlFile);
                    try
                    {
                        var node = doc.Root.Descendants("Version").First();
                        node.SetValue(version);
                    }
                    catch
                    {
                        throw new Exception("Version not found in existing xml file");
                    }

                    if (opts.Verbose)
                        Console.WriteLine("Saving to: {0}", xmlFile);
                    doc.Save(xmlFile);
                }
                else
                {
                    if (opts.Verbose)
                        Console.WriteLine("Saving to: {0}", xmlFile);
                    File.WriteAllText(xmlFile, $"<Version>{version}</Version>");
                }
            }
        }
    }
}
