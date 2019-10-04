using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Stuff.HTTPManager;

namespace Stuff
{
    class Program
    {
        public static void Main(string[] args)
        {
            HTTPManager manager = new HTTPManager();
            httpAnswer result = manager.Authenticate("alfred", "nobel");
            if (result.d.error)
            {
                Console.WriteLine("Error " + result.d.errorNumber.ToString() + " " + result.d.message);
            }
            else
            {
                string token = result.d.message.Replace("\"", "");
                Console.WriteLine(token);

                result = manager.sendHttpRequest("getData", "",token);
                if (result.d.error)
                {
                    Console.WriteLine("Error " + result.d.errorNumber.ToString() + " " + result.d.message);
                }
                else
                {
                    Console.WriteLine(result.d.message);
                }

            }

            Console.ReadKey();
        }
    }

}
