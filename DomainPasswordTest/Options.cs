using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;

namespace DomainPasswordTest
{
    internal class Options
    {
        [Option('p', DefaultValue=null,HelpText = "Password to Test")]
        public string Password { get; set; }
        
        [Option(DefaultValue = 389, HelpText = "Port LDAP is running on")]
        public int LdapPort { get; set; }

        [Option(DefaultValue = null, HelpText = "Domain Controller to pull LDAP from. Defaults to PDC. Changing this to a non-PDC may result in locking accounts out due to badpwdcount replication")]
        public string DomainController { get; set; }

        [Option('d', DefaultValue = null, HelpText = "Domain Name to target")]
        public string Domain { get; set; }

        [Option(DefaultValue = 5, HelpText = "Lockout threshold for accounts on the domain. Use net accounts /domain to find this")]
        public int Lockout { get; set; }

        [Option('v', DefaultValue = false, HelpText = "Verbose Output")]
        public bool Verbose { get; set; }
    }
}
