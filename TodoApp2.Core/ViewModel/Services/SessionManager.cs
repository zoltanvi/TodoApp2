using System;
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
        private bool m_IsLoggingInProgress;

        private DriveService m_DriveService;
        private UserCredential m_UserCredential;
        private readonly MessageService m_MessageService;

        public SessionManager(MessageService messageService)
        {
            m_MessageService = messageService;
        }

        public string DisplayName { get; private set; } = string.Empty;
        public string EmailAddress { get; private set; } = string.Empty;
        public bool IsLoggedIn => OnlineMode && 
                                  !string.IsNullOrEmpty(DisplayName) && 
                                  !string.IsNullOrEmpty(EmailAddress);
        public bool OnlineMode => Directory.Exists(s_CredentialsFolderPath) &&
                                  Directory.EnumerateFiles(s_CredentialsFolderPath)
                                      .Any(f => f.EndsWith("TokenResponse-user"));

        public async void LogOut()
        {
            await IoC.Database.Reinitialize();

            await m_UserCredential.RevokeTokenAsync(CancellationToken.None);
            Directory.Delete(s_CredentialsFolderPath, true);

            DisplayName = string.Empty;
            EmailAddress = string.Empty;

            OnPropertyChanged(nameof(DisplayName));
            OnPropertyChanged(nameof(EmailAddress));
            OnPropertyChanged(nameof(IsLoggedIn));
            m_IsLoggingInProgress = false;
            m_MessageService.ShowSuccess("Logged out successfully.", TimeSpan.FromSeconds(3));
        }

        public async void LogIn()
        {
            // Prevent starting another login if one is already started
            if (!m_IsLoggingInProgress)
            {
                m_MessageService.ShowInfo("Login started.", TimeSpan.FromSeconds(3));
                
                if (await AuthenticateUserAsync())
                {
                    await IoC.Database.Reinitialize(true);

                    OnPropertyChanged(nameof(IsLoggedIn));
                    OnPropertyChanged(nameof(DisplayName));
                    OnPropertyChanged(nameof(EmailAddress));
                    m_IsLoggingInProgress = false;
                    m_MessageService.ShowSuccess("Logged in successfully.", TimeSpan.FromSeconds(3));
                }
                else
                {
                    m_MessageService.ShowError("Login failed. Please check the network connection!");
                }
            }
            else
            {
                m_MessageService.ShowWarning("Login in progress.", TimeSpan.FromSeconds(2));
            }
        }

        public void Download()
        {
            if (SafeGetServerDatabaseFile(out var onlineDatabaseFile))
            {
                if (onlineDatabaseFile != null)
                {
                    // Download file
                    using (var stream = new FileStream(DataAccessLayer.OnlineDatabasePath, FileMode.OpenOrCreate))
                    {
                        var progress = m_DriveService.Files.Get(onlineDatabaseFile.Id).DownloadWithStatus(stream);
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
        }

        public void Upload()
        {
            // If the list request could not be executed, skip uploading
            if (m_DriveService != null && SafeGetServerDatabaseFile(out var onlineDatabaseFile))
            {
                if (onlineDatabaseFile == null)
                {
                    // Upload file to google drive
                    onlineDatabaseFile = new GoogleFile { Name = DataAccessLayer.OnlineDatabaseName };

                    using (var stream = new FileStream(DataAccessLayer.OnlineDatabasePath, FileMode.OpenOrCreate))
                    {
                        var uploadRequest = m_DriveService.Files.Create(onlineDatabaseFile, stream, null);
                        uploadRequest.Upload();
                    }
                }
                else
                {
                    // Update file on google drive
                    GoogleFile updatedFileMetadata = new GoogleFile { Name = onlineDatabaseFile.Name };

                    using (var stream = new FileStream(DataAccessLayer.OnlineDatabasePath, FileMode.OpenOrCreate))
                    {
                        var updateRequest = m_DriveService.Files.Update(updatedFileMetadata, onlineDatabaseFile.Id, stream, null);
                        updateRequest.Upload();
                    }
                }
            }
        }

        public async Task<bool> AuthenticateUserAsync()
        {
            bool success = false;
            if (string.IsNullOrEmpty(DisplayName) || string.IsNullOrEmpty(EmailAddress))
            {
                m_IsLoggingInProgress = true;
                
                if (await TryAuthenticateUserAsync())
                {
                    success = UpdateUserInfo();
                    if (!success)
                    {
                        m_IsLoggingInProgress = false;
                    }
                }
            }

            return success;
        }

        /// <summary>
        /// Authenticates the user. May require user action!
        /// </summary>
        /// <returns>Returns true if the user authentication was successful, false otherwise.</returns>
        private async Task<bool> TryAuthenticateUserAsync()
        {
            bool success = true;
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
                }
                catch (OperationCanceledException e)
                {
                    success = false;
                    m_IsLoggingInProgress = false;
                    m_MessageService.ShowError("Login timed out. Please try again!");
                }
                catch (Exception e)
                {
                    success = false;
                    m_IsLoggingInProgress = false;
                    m_MessageService.ShowError("Login failed.");
                }
            }

            // Create Drive API service.
            m_DriveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = m_UserCredential,
                ApplicationName = s_ApplicationName,
            });

            return success;
        }

        /// <summary>
        /// Gets the database file from google drive.
        /// </summary>
        /// <param name="onlineDatabaseFile">The database file. Null if getting it was not successful.</param>
        /// <returns>Returns true if getting the file was successful, false otherwise.</returns>
        private bool SafeGetServerDatabaseFile(out GoogleFile onlineDatabaseFile)
        {
            FilesResource.ListRequest listRequest = m_DriveService.Files.List();
            listRequest.Fields = "nextPageToken, files(id, name)";

            bool success = true;
            onlineDatabaseFile = null;

            try
            {
                var files = listRequest.Execute().Files;
                onlineDatabaseFile = files?.FirstOrDefault(f => f.Name == DataAccessLayer.OnlineDatabaseName);
            }
            catch (Exception e)
            {
                success = false;
            }

            return success;
        }

        /// <summary>
        /// Updates the user info (<see cref="DisplayName"/>, <see cref="EmailAddress"/>).
        /// </summary>
        /// <returns>Returns true if updating the user info was successful, false otherwise.</returns>
        private bool UpdateUserInfo()
        {
            bool success = true;
            try
            {
                var aboutRequest = m_DriveService.About.Get();
                aboutRequest.Fields = "user";
                var about = aboutRequest.Execute();

                DisplayName = about.User.DisplayName;
                EmailAddress = about.User.EmailAddress;
            }
            catch (Exception e)
            {
                success = false;
                DisplayName = string.Empty;
                EmailAddress = string.Empty;
            }

            return success;
        }
    }
}
