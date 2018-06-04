using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Rlc2
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var dir = "s:\\";
                var files = Directory.GetFiles(dir, "RJFAF*").ToList();
                files.RemoveAll(x => !Regex.Match(Path.GetFileName(x), @"^RJFAF\d\d\d.[A-Z]{3}$").Success);
                files.RemoveAll(x => Regex.Match(x, "zip", RegexOptions.IgnoreCase).Success);
                var extMap = files.ToLookup(x => Path.GetExtension(x), StringComparer.OrdinalIgnoreCase);
                var dups = extMap.Where(x => x.Count() > 1);
                foreach (var entry in dups)
                {
                    Console.Error.WriteLine($"More than one RJFAF file with extension {entry.Key}");
                }

                var locfile = extMap[".rlc"].FirstOrDefault();

                foreach (var line in File.ReadAllLines(locfile))
                {
                    if (line.Length == 127)
                    {
                        var railcard = line.Substring(0, 3);
                        if (line.Substring(4, 8) == "31122999")
                        {
                            continue;
                        }
                        var maxPassengers = Convert.ToInt32(line.Substring(56, 3));
                        var minPassengers = Convert.ToInt32(line.Substring(59, 3));
                        var maxHolders = Convert.ToInt32(line.Substring(62, 3));
                        var minHolders = Convert.ToInt32(line.Substring(65, 3));
                        var maxAccAdults = Convert.ToInt32(line.Substring(68, 3));
                        var minAccAdults = Convert.ToInt32(line.Substring(71, 3));
                        var maxAdults = Convert.ToInt32(line.Substring(74, 3));
                        var minAdults = Convert.ToInt32(line.Substring(77, 3));
                        var maxChildren = Convert.ToInt32(line.Substring(80, 3));
                        var minChildren = Convert.ToInt32(line.Substring(83, 3));

                        if (maxAccAdults > 0 && minAdults > 1)
                        {
                            Console.WriteLine($"{railcard} : max acc {maxAccAdults} min adults {minAdults}");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                var progname = Path.GetFileNameWithoutExtension(codeBase);
                Console.Error.WriteLine(progname + ": Error: " + ex.Message);
            }

        }
    }
}
