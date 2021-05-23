using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp2.Core
{
    public class SessionManager
    {
        private const string s_CredentialsFolderPath = "googleToken";

        public string LoggedInUserName { get; set; }

        public bool OnlineMode
        {
            get
            {
                var exists = Directory.Exists(s_CredentialsFolderPath);
                return exists && Directory.EnumerateFiles(s_CredentialsFolderPath).Any(f => f.EndsWith("TokenResponse-user"));
            }
        }

    }
}
