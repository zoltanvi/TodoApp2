using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace TodoApp2.Core
{
    public class SessionManager : BaseViewModel
    {
        private const string s_CredentialsFolderPath = "googleToken";
        private const string s_ApplicationName = "TodoApp2";
        private const string s_Credentials = "credentials.json";
        private const string s_User = "user";
        private readonly string[] m_Scopes = { DriveService.Scope.Drive };

        private string m_DisplayName;
        private string m_EmailAddress;

        private DriveService m_Service;
        private UserCredential m_UserCredential;

        public bool IsLoggedIn
        {
            get
            {
                bool loggedIn = false;
                
                if (OnlineMode)
                {
                    loggedIn = DisplayName != string.Empty && EmailAddress != string.Empty;
                }

                return loggedIn;
            }
        }

        public bool OnlineMode
        {
            get
            {
                var exists = Directory.Exists(s_CredentialsFolderPath);
                return exists && Directory.EnumerateFiles(s_CredentialsFolderPath).Any(f => f.EndsWith("TokenResponse-user"));
            }
        }

        public string DisplayName
        {
            get
            {
                if (OnlineMode)
                {
                    GetUserInfo();
                }
                return m_DisplayName;
            }
        }

        public string EmailAddress
        {
            get
            {
                if (OnlineMode)
                {
                    GetUserInfo();
                }
                return m_EmailAddress;
            }
        }

        public void LogOut()
        {
            m_UserCredential?.RevokeTokenAsync(CancellationToken.None);
            Directory.Delete(s_CredentialsFolderPath, true);

            m_DisplayName = string.Empty;
            m_EmailAddress = string.Empty;

            IoC.Database.Reinitialize(OnlineMode);

            OnPropertyChanged(nameof(DisplayName));
            OnPropertyChanged(nameof(EmailAddress));
            OnPropertyChanged(nameof(IsLoggedIn));
        }

        public void LogIn()
        {
            GetUserInfo();

            IoC.Database.Reinitialize(OnlineMode);

            OnPropertyChanged(nameof(IsLoggedIn));
            OnPropertyChanged(nameof(DisplayName));
            OnPropertyChanged(nameof(EmailAddress));
        }

        private void GetUserInfo()
        {
            if (string.IsNullOrEmpty(m_DisplayName) || string.IsNullOrEmpty(m_EmailAddress))
            {
                AuthenticateUser();

                try
                {
                    // Get username 
                    var aboutRequest = m_Service.About.Get();
                    aboutRequest.Fields = "user";
                    var about = aboutRequest.Execute();
                    m_DisplayName = about.User.DisplayName;
                    m_EmailAddress = about.User.EmailAddress;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Need to reauthenticate.");
                    m_DisplayName = string.Empty;
                    m_EmailAddress = string.Empty;
                }
            }
        }

        private void AuthenticateUser()
        {
            using (var stream = new FileStream(s_Credentials, FileMode.Open, FileAccess.Read))
            {
                var dataStorage = new FileDataStore(s_CredentialsFolderPath, true);
                var secrets = GoogleClientSecrets.Load(stream).Secrets;

                m_UserCredential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    secrets,
                    m_Scopes,
                    s_User,
                    CancellationToken.None,
                    dataStorage).Result;
            }

            // Create Drive API service.
            m_Service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = m_UserCredential,
                ApplicationName = s_ApplicationName,
            });
        }
    }
}
