
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace HumaneSociety.Properties
{
    static class CSVreader
    {
        public static List<List<string>> ReadFile()
        {
            StreamReader sr = new StreamReader("C:\\Users\\jeff\\Desktop\\HumaneSociety2\\HumaneSociety\\HumaneSociety\\animals.csv");
            Regex input = new Regex(@"\d+|\w+");

            var lines = File.ReadAllLines("C:\\Users\\jeff\\Desktop\\HumaneSociety2\\HumaneSociety\\HumaneSociety\\animals.csv").ToList();
            var result = lines.Select(l => l.Split(',')).ToList();
            var output = result.Select(x => x.Select(y => input.Match(y).ToString()).ToList()).ToList();

            return output;
        }
    }
}
