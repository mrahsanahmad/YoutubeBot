using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Http;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace YoutubeBotNS.YoutubeBot
{
    public class UnsuccessfulResponseHandler : IHttpUnsuccessfulResponseHandler
    {
        public Task<bool> HandleResponseAsync(HandleUnsuccessfulResponseArgs args)
        {
            Func<bool> f = () => {
                Console.WriteLine($"{args.Request} {args.Response} {args.TotalTries}");
                return false;
            };
            return new Task<bool>(() => f());
        }
    }
    public interface UsersInterface
    {
        bool IsConnected { get; }

        Task ConnectAsync();

        void Disconnect();

        Task<IEnumerable<string>> GetSubscribersAsync();

    }

    public class YoutubeUsersClient : UsersInterface
    {
        private readonly IYouTubeAuthService youtubeAuthService;

        public bool IsConnected { get; private set; }

        public YouTubeService YoutubeService { get; private set; }

        public YoutubeUsersClient(IYouTubeAuthService youtubeAuthService)
        {
            this.youtubeAuthService = youtubeAuthService;
        }

        public async Task ConnectAsync()
        {
            if (!this.IsConnected)
            {
                await this.InitializeAsync();
            }
        }

        public void Disconnect()
        {
            this.IsConnected = false;
        }

        public async Task<IEnumerable<string>> GetSubscribersAsync()
        {
            await ConnectAsync();
            var req = YoutubeService.Subscriptions.List("subscriberSnippet");
            req.MySubscribers = true;
            req.AddUnsuccessfulResponseHandler(new UnsuccessfulResponseHandler());
            var response = await req.ExecuteAsync();
            var names = response.Items.Select(s => s.SubscriberSnippet.Title);
            return names;
        }

        private async Task InitializeAsync()
        {
            var userCredential = await youtubeAuthService.GetUserCredentialAsync().ConfigureAwait(false);
            var initializer = new BaseClientService.Initializer
            {
                HttpClientInitializer = userCredential,
                ApplicationName = this.GetType().ToString()
            };

            this.YoutubeService = new YouTubeService(initializer);
            this.IsConnected = true;
        }
    }
}
