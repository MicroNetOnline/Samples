using System;
using System.Web;
using System.Windows;
using System.Windows.Navigation;

namespace ImplicitFlow.Wpf
{
    /// <summary>
    /// Interaction logic for MemberZoneLogin.xaml
    /// </summary>
    public partial class MemberZoneLogin
    {
        const string CallbackEndpoint = "http://localhost:54322";

        readonly string _server;
        readonly string _clientId;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="server">The address of the server.</param>
        /// <param name="clientId">The Client ID.</param>
        public MemberZoneLogin(string server, string clientId)
        {
            _server = server;
            _clientId = clientId;

            InitializeComponent();
        }

        /// <summary>
        /// Called when the Window has loaded.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            var redirectUri = new Uri(CallbackEndpoint);
            var uri = $"https://{_server}/oauth/authorize?client_id={_clientId}&response_type=token&redirect_uri={redirectUri}";

            _webBroswer.Navigate(uri);
        }

        /// <summary>
        /// Called when the browser has navigated to an address.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        void OnBrowserNavigated(object sender, NavigationEventArgs e)
        {
            // ensure this is our callback URL
            if (e.Uri.AbsoluteUri.StartsWith(CallbackEndpoint) == false)
            {
                return;
            }

            AccessToken = HttpUtility.ParseQueryString(e.Uri.Fragment.Substring(1))["access_token"];
            DialogResult = true;
        }

        /// <summary>
        /// Returns the access token if the login succeeded.
        /// </summary>
        public string AccessToken { get; private set; }
    }
}