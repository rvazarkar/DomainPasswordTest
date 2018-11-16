using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;

namespace DomainPasswordTest
{
    internal class Options
    {
        [Option('p', DefaultValue=null)]
        public string Password { get; set; }
        
        [Option(DefaultValue = 389)]
        public int LdapPort { get; set; }

        [Option(DefaultValue = null)]
        public string DomainController { get; set; }

        [Option('d', DefaultValue = null)]
        public string Domain { get; set; }

        [Option(DefaultValue = 5)]
        public int Lockout { get; set; }

        [Option('v', DefaultValue = false)]
        public bool Verbose { get; set; }
    }
}
