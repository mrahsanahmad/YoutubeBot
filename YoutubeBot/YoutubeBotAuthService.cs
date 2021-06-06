using Google.Apis.Auth.OAuth2;
using Google.Apis.YouTube.v3;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace YoutubeBotNS.YoutubeBot
{
    public class YoutubeBotAuthService : IYouTubeAuthService
    {
        public async Task<UserCredential> GetUserCredentialAsync()
        {
            UserCredential credential;
            const string clientIdPath = @"client_id.json";

            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GetClientSecret(clientIdPath),
                new[] { YouTubeService.Scope.YoutubeReadonly },
                "user",
                CancellationToken.None);

            return credential;
        }

        ClientSecrets GetClientSecret(string pathToFile)
        {
            using (var stream = new FileStream(pathToFile, FileMode.Open))
            {
                var loadedSecrets = GoogleClientSecrets.Load(stream);
                return loadedSecrets.Secrets;
            }
        }
    }
}
