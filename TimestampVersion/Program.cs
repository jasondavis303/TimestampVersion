using CommandLine;
using System;
using System.Collections.Generic;
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
                Console.WriteLine();
                Console.WriteLine("Timestamp Version Generator");
                Console.WriteLine();

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
                        //var node = doc.Root.Descendants("Version").First();
                        var node = doc
                            .Root
                            .Descendants()
                            .Where(item => item.Name.LocalName.Equals("Version", StringComparison.CurrentCultureIgnoreCase))
                            .First();

                        node.SetValue(version);
                    }
                    catch
                    {
                        throw new Exception("Version not found in existing xml file: " + xmlFile);
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
        
            foreach(var aiFile in opts.AssemblyInfoFiles)
            {
                try { Directory.CreateDirectory(Path.GetDirectoryName(aiFile)); }
                catch { }

                if (File.Exists(aiFile))
                {
                    bool reflection = false;
                    bool assemblyVersion = false;
                    bool assemblyFileVersion = false;
                    var lines = File.ReadAllLines(aiFile).ToList();
                    for(int i = 0; i < lines.Count; i++)
                    {
                        if (lines[i].Trim().Equals("using System.Reflection;"))
                            reflection = true;

                        if(lines[i].StartsWith("[assembly: AssemblyVersion("))
                        {
                            lines[i] = $"[assembly: AssemblyVersion({version})]";
                            assemblyVersion = true;
                        }

                        if(lines[i].StartsWith("[assembly: AssemblyFileVersion("))
                        {
                            lines[i] = $"[assembly: AssemblyFileVersion({version})]";
                            assemblyFileVersion = true;
                        }
                    }

                    if (!reflection)
                        lines.Insert(0, "using System.Reflection;");

                    if (!assemblyVersion)
                        lines.Add($"[assembly: AssemblyVersion(\"{version}\")]");

                    if (!assemblyFileVersion)
                        lines.Add($"[assembly: AssemblyFileVersion(\"{version}\")]");

                    File.WriteAllLines(aiFile, lines);
                }
                else
                {
                    var lines = new List<string>
                    {
                        "using System.Reflection;",
                        $"[assembly: AssemblyVersion(\"{version}\")]",
                        $"[assembly: AssemblyFileVersion(\"{version}\")]"
                    };

                    File.WriteAllLines(aiFile, lines);
                }
            }
        }
    }
}
