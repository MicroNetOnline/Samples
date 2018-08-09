using System;
using System.Windows;
using MicroNet.MMP.Shared;

namespace ImplicitFlow.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called when the Login button has been clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        void OnLoginClick(object sender, RoutedEventArgs e)
        {
            var login = new MemberZoneLogin("login.memberzonedev.org", MemberZoneClient.ClientId);
            //var login = new MemberZoneLogin("login.localtest.me:12221", "12345");

            if (login.ShowDialog() == true)
            {
                App.AccessToken = login.AccessToken;
            }
        }

        /// <summary>
        /// Called when the DisplayClaims button has been clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        async void OnDisplayClaimsClick(object sender, RoutedEventArgs e)
        {
            _listBox.Items.Clear();

            using (var client = new MemberZoneClient(new Uri("https://memberzonedev.org/"), App.AccessToken))
            {
                try
                {
                    var response = await client.GetClaimsAsync();
                    foreach (var claim in response.Claims)
                    {
                        _listBox.Items.Add($"{claim.Name}={claim.Value}");
                    }
                }
                catch (Exception exception)
                {
                    _listBox.Items.Add(exception.ToString());
                }
            }
        }
    }
}