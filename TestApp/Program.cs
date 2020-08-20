using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xal.Extensions;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var s = JsonConvert.SerializeObject(new { a = 10 });
            Console.WriteLine(s);
        }
    }
}
