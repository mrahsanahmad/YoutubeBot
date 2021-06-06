using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;

namespace YoutubeBotNS.YoutubeBot
{
    public interface IYouTubeAuthService
    {
        Task<UserCredential> GetUserCredentialAsync();
    }
}
