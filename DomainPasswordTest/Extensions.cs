using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Text;

namespace DomainPasswordTest
{
    static class Extensions
    {
        public static string GetProp(this SearchResultEntry result, string prop)
        {
            if (!result.Attributes.Contains(prop))
                return null;

            return result.Attributes[prop][0].ToString();
        }
    }
}
