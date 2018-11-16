using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;
using System.Text;

namespace DomainPasswordTest
{
    internal class Ldap
    {
        private readonly Options _options;
        private readonly Domain _domain;
        public Ldap(Options options)
        {
            _options = options;
            _domain = GetDomain();

            if (_domain == null)
            {
                Console.WriteLine("Invalid Domain");
                throw new InvalidDomainException();
            }
        }

        internal void TestDomain()
        {
            if (_options.Password == null)
            {
                Console.WriteLine("No Password Set");
                return;
            }
            Console.WriteLine($"Starting Password Audit Using Password: {_options.Password}");
            foreach (var user in GetUserList())
            {
                if (ValidateCreds(user))
                {
                    Console.WriteLine($"[+] {user}:{_options.Password}");
                }
            }

            Console.WriteLine("Password Test Finished");
        }

        internal bool ValidateCreds(string user)
        {
            var conn = GetConnection();
            conn.AuthType = AuthType.Kerberos;
            conn.Credential = new NetworkCredential(user, _options.Password, _domain.Name);
            using (conn)
            {
                try
                {
                    conn.Bind();
                }
                catch (LdapException e)
                {
                    return false;
                }
            }

            return true;
        }

        internal IEnumerable<string> GetUserList()
        {
            var conn = GetConnection();
            if (conn == null)
            {
                yield break;
            }

            using (conn)
            {
                var request = GetRequest();

                if (request == null)
                {
                    Console.WriteLine("Unable to contact domain");
                    yield break;
                }

                var prc = new PageResultRequestControl(500);
                request.Controls.Add(prc);
                PageResultResponseControl pr = null;

                while (true)
                {
                    SearchResponse response;
                    try
                    {
                        response = (SearchResponse) conn.SendRequest(request);
                        if (response != null)
                        {
                            pr = (PageResultResponseControl) response.Controls[0];
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        yield break;
                    }

                    if (response == null || pr == null)
                        continue;

                    foreach (SearchResultEntry entry in response.Entries)
                    {
                        var bp = entry.GetProp("badpwdcount");
                        if (int.TryParse(bp, out var badpwdcount) || bp == null)
                        {
                            var user = entry.GetProp("samaccountname");
                            if (badpwdcount < _options.Lockout - 2 && user != null)
                                yield return user;
                            else
                            {

                                if (_options.Verbose)
                                {
                                    Console.WriteLine($"Skipping {user} due to badpwdcount");
                                }
                            }
                        }
                        
                    }

                    if (pr.Cookie.Length == 0)
                    {
                        yield break;
                    }

                    prc.Cookie = pr.Cookie;
                }
            }
            
        }

        internal LdapConnection GetConnection()
        {
            var dc = _options.DomainController ?? _domain.PdcRoleOwner.Name;
            var port = _options.LdapPort;

            var id = new LdapDirectoryIdentifier(dc, port, false, false);
            var conn = new LdapConnection(id) {Timeout = new TimeSpan(0, 0, 2, 0)};

            var lso = conn.SessionOptions;
            lso.Signing = true;
            lso.Sealing = true;
            lso.ReferralChasing = ReferralChasingOptions.None;
            
            return conn;
        }

        internal Domain GetDomain()
        {
            Domain domain;
            try
            {
                
                if (_options.Domain == null)
                {
                    domain = Domain.GetCurrentDomain();
                }
                else
                {
                    var context = new DirectoryContext(DirectoryContextType.Domain, _options.Domain);
                    domain = Domain.GetDomain(context);
                }
            }
            catch
            {
                domain = null;
            }

            return domain;
        }

        internal SearchRequest GetRequest()
        {
            var root = $"DC={_domain.Name.Replace(".", ",DC=")}";
            var request = new SearchRequest(root, "(&(sAMAccountType=805306368)(!(UserAccountControl:1.2.840.113556.1.4.803:=2)))",SearchScope.Subtree, null);
            var soc = new SearchOptionsControl(SearchOption.DomainScope);
            request.Controls.Add(soc);
            return request;
        }
    }

    internal class InvalidDomainException : Exception
    {
    }
}
