using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using File = System.IO.File;
using GoogleFile = Google.Apis.Drive.v3.Data.File;

namespace TodoApp2.Core
{
    public class SessionManager : BaseViewModel
    {
        private const string s_CredentialsFolderPath = "googleToken";
        private const string s_ApplicationName = "TodoApp2";
        private const string s_Credentials = "credentials.json";
        private const string s_User = "user";
        private readonly string[] m_Scopes = { DriveService.Scope.Drive };
        private string m_EmailAddress;
        private bool m_IsLoggingInProgress;

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

        public string DisplayName { get; private set; }

        public string EmailAddress { get; private set; }

        public void LogOut()
        {
            IoC.Database.Reinitialize(false);

            m_UserCredential?.RevokeTokenAsync(CancellationToken.None);
            Directory.Delete(s_CredentialsFolderPath, true);

            DisplayName = string.Empty;
            m_EmailAddress = string.Empty;

            OnPropertyChanged(nameof(DisplayName));
            OnPropertyChanged(nameof(EmailAddress));
            OnPropertyChanged(nameof(IsLoggedIn));
        }

        public async void LogIn()
        {
            // Prevent starting another login if one is already started
            if (!m_IsLoggingInProgress)
            {
                if (await AuthenticateAndGetUserInfo())
                {
                    IoC.Database.Reinitialize(true);

                    OnPropertyChanged(nameof(IsLoggedIn));
                    OnPropertyChanged(nameof(DisplayName));
                    OnPropertyChanged(nameof(EmailAddress));
                    m_IsLoggingInProgress = false;
                }
            }
        }

        public void Download()
        {
            // Define parameters of request.
            FilesResource.ListRequest listRequest = m_Service.Files.List();
            listRequest.Fields = "nextPageToken, files(id, name)";

            IList<GoogleFile> files = listRequest.Execute().Files;

            GoogleFile onlineDbFile = files.FirstOrDefault(f => f.Name == DataAccessLayer.OnlineDatabaseName);

            if (onlineDbFile != null)
            {
                // Download file
                using (var stream = new FileStream(DataAccessLayer.OnlineDatabasePath, FileMode.OpenOrCreate))
                {
                    Google.Apis.Download.IDownloadProgress asd = m_Service.Files.Get(onlineDbFile.Id).DownloadWithStatus(stream);
                }
            }
            else
            {
                // If there is nothing on the server yet, delete the existing online db file.
                // It is probably from other account.
                if (File.Exists(DataAccessLayer.OnlineDatabasePath))
                {
                    File.Delete(DataAccessLayer.OnlineDatabasePath);
                }
            }
        }
        
        public void Upload()
        {
            if (m_Service == null)
            {
                return;
            }

            // Define parameters of request.
            FilesResource.ListRequest listRequest = m_Service.Files.List();
            listRequest.Fields = "nextPageToken, files(id, name)";

            IList<GoogleFile> files = listRequest.Execute().Files;

            GoogleFile onlineDbFile = files.FirstOrDefault(f => f.Name == DataAccessLayer.OnlineDatabaseName);

            if (onlineDbFile == null)
            {
                onlineDbFile = new GoogleFile
                {
                    Name = DataAccessLayer.OnlineDatabaseName
                };

                byte[] byteArray = System.IO.File.ReadAllBytes(DataAccessLayer.OnlineDatabasePath);

                using (var stream = new MemoryStream(byteArray))
                {
                    FilesResource.CreateMediaUpload uploadRequest = m_Service.Files.Create(onlineDbFile, stream,
                        GetMimeType(DataAccessLayer.OnlineDatabasePath));
                    uploadRequest.Upload();
                }
            }
            else
            {
                // Update file on google drive
                GoogleFile updatedFileMetadata = new GoogleFile { Name = onlineDbFile.Name };

                using (var stream = new FileStream(DataAccessLayer.OnlineDatabasePath, FileMode.OpenOrCreate))
                {
                    FilesResource.UpdateMediaUpload updateRequest = m_Service.Files.Update(updatedFileMetadata,
                        onlineDbFile.Id, stream, onlineDbFile.FileExtension);
                    updateRequest.Upload();
                }
            }
        }

        public async Task<bool> AuthenticateAndGetUserInfo()
        {
            bool success = true;
            if (string.IsNullOrEmpty(DisplayName) || string.IsNullOrEmpty(m_EmailAddress))
            {
                m_IsLoggingInProgress = true;

                success = await AuthenticateUserAsync();

                try
                {
                    // Get username 
                    var aboutRequest = m_Service.About.Get();
                    aboutRequest.Fields = "user";
                    var about = aboutRequest.Execute();
                    DisplayName = about.User.DisplayName;
                    m_EmailAddress = about.User.EmailAddress;
                }
                catch (Exception e)
                {
                    success = false;
                    DisplayName = string.Empty;
                    m_EmailAddress = string.Empty;
                }
            }

            return success;
        }

        private async Task<bool> AuthenticateUserAsync()
        {
            bool success = false;
            using (var stream = new FileStream(s_Credentials, FileMode.Open, FileAccess.Read))
            {
                var dataStorage = new FileDataStore(s_CredentialsFolderPath, true);
                var secrets = GoogleClientSecrets.Load(stream).Secrets;

                try
                {
                    // 30 sec timeout.
                    // If the login was unsuccessful during this time, the user must try it again.
                    CancellationTokenSource cts = new CancellationTokenSource(new TimeSpan(0, 0, 30));

                    m_UserCredential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        secrets,
                        m_Scopes,
                        s_User,
                        cts.Token,
                        dataStorage);

                    success = true;
                }
                catch (Exception e)
                {
                    success = false;
                    m_IsLoggingInProgress = false;
                }
            }

            // Create Drive API service.
            m_Service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = m_UserCredential,
                ApplicationName = s_ApplicationName,
            });

            return success;
        }

        private static string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = Path.GetExtension(fileName)?.ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);

            if (regKey?.GetValue("Content Type") != null)
            {
                mimeType = regKey.GetValue("Content Type").ToString();
            }

            System.Diagnostics.Debug.WriteLine(mimeType);
            return mimeType;
        }
    }
}
