using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECA2LD
{
    public static class StringExtensions
    {
        private static string getBaseUriString(this Uri u)
        {
            return u.Scheme + "://" + u.Host + ":" + u.Port + "/";
        }

        public static string getPrototypeBaseUri(this Uri u)
        {
            return u.getBaseUriString() + "prototypes/";
        }
    }
}
