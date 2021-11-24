[![release](https://github.com/jasondavis303/TimestampVersion/actions/workflows/release.yml/badge.svg)](https://github.com/jasondavis303/TimestampVersion/actions/workflows/release.yml)

Nuget Library: https://www.nuget.org/packages/TimestampVersion/


Windows Exe: https://github.com/jasondavis303/TimestampVersion/releases/latest/download/tsv.exe

Linux Exe: https://github.com/jasondavis303/TimestampVersion/releases/latest/download/tsv

<pre>
Usage: tsv.exe [options]

  --xml-files             Xml files to create or update

  --assemblyinfo-files    AssemblyInfo.cs files to create or update

  --env-file              Github Actions environment file to create or update 
                          (only works on Ubuntu)

  --set-ov                Set the output variable TIMESTAMP_VERSION in Github Actions
                          (works on Ubuntu and Windows)

  --msix                  Set version to be compatible with MSIX packages

  --verbose

  --help                  Display this help screen.

  --version               Display version information.
</pre>