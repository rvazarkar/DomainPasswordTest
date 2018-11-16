using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Text;

namespace DomainPasswordTest
{
    class Program
    {
        internal static void Main(string[] args)
        {
            var options = new Options();

            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                try
                {
                    var l = new Ldap(options);
                    l.TestDomain();
                }
                catch
                {

                }
            }
            else
            {
                Console.WriteLine("Options failed to validate");
            }
        }
    }
}
