using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Amazon.CognitoIdentity;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Plugins.AWS.SNS.Models;

namespace Xamarin.Plugins.AWS.SNS.Helpers
{
    public class SNSUtils
    {
        public enum Platform
        {
            Android,
            IOS,
            WindowsPhone
        }

        private static AWSCredentials _credentials;

        private static IAmazonSimpleNotificationService _snsClient;

        private static AWSCredentials Credentials
        {
            get
            {
                if (_credentials == null)
                    _credentials = new CognitoAWSCredentials(Constants.IdentityPoolId, Constants.CognitoRegion);
                return _credentials;
            }
        }

        private static IAmazonSimpleNotificationService SnsClient
        {
            get
            {
                if (_snsClient == null)
                    _snsClient = new AmazonSimpleNotificationServiceClient(Credentials, Constants.SnsRegion);
                return _snsClient;
            }
        }

        public static void HandleMessage(string payload)
        {
            var rootObject = JsonConvert.DeserializeObject<SNSPush>(payload);
            var displayMessage = string.Empty;
            if (Device.OS == TargetPlatform.iOS)
                displayMessage = rootObject.aps.alert;
            else if (Device.OS == TargetPlatform.Android)
                displayMessage = rootObject.message;

            if (!string.IsNullOrWhiteSpace(displayMessage))
            {
                //await UserDialogs.Instance.AlertAsync(displayMessage, "Notification", "OK");
            }


            if (rootObject.type != null)
            {
                return;
            }

            switch (rootObject.type)
            {
                case "1":
                    //Device.BeginInvokeOnMainThread(() => MessagingCenter.Send(new MainLoader(), "ShowTickets"));
                    break;
                case "2":
                    //Device.BeginInvokeOnMainThread(() => MessagingCenter.Send(new MainLoader(), "ShowTickets"));
                    break;
                case "3":
                    //Device.BeginInvokeOnMainThread(() => MessagingCenter.Send(new MainLoader(), "ReservedTickets"));
                    break;
                default:
                    break;
            }
        }


        public static async void UpdateUser(string Uid)
        {
            if (!string.IsNullOrWhiteSpace(Settings.Arnsns))
            {
                var updateToken = new SetEndpointAttributesRequest();
                updateToken.Attributes.Add("CustomUserData", Uid);
                updateToken.EndpointArn = Settings.Arnsns;
                try
                {
                    var update = await SnsClient.SetEndpointAttributesAsync(updateToken);
                }
                catch
                {
                    // Someone has deleted the ARN from the server manually, need to uninstall the app to get a new ARN, or eventually re-register the device here
                }
            }
        }

        public static async Task RegisterDevice(Platform platform, string registrationId)
        {
            var arn = string.Empty;
            var _endpointArn = string.Empty;
            switch (platform)
            {
                case Platform.Android:
                    arn = Constants.AndroidPlatformApplicationArn;
                    break;
                case Platform.IOS:
                    arn = Constants.iOSPlatformApplicationArn;
                    break;
            }
            var response = new CreatePlatformEndpointResponse();
            var userData = "";

            try
            {
                if (!string.IsNullOrWhiteSpace(Settings.Uid))
                {
                    // Add Your User Data Here
                    //userData = Settings.Uid;
                }

                if (string.IsNullOrEmpty(Settings.Arnsns))
                {
                    response = await SnsClient.CreatePlatformEndpointAsync(new CreatePlatformEndpointRequest
                    {
                        Token = registrationId,
                        PlatformApplicationArn = arn,
                        CustomUserData = userData
                    });

                    Settings.Arnsns = response.EndpointArn;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex);
            }

            if (!string.IsNullOrWhiteSpace(Settings.Arnsns))
            {
                var results = new GetEndpointAttributesResponse();

                try
                {
                    var test = new GetEndpointAttributesRequest();
                    test.EndpointArn = Settings.Arnsns;


                    results = await SnsClient.GetEndpointAttributesAsync(test);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error" + ex);
                }


                if (results.Attributes == null || results.Attributes.Count == 0)
                {
                    response = await SnsClient.CreatePlatformEndpointAsync(new CreatePlatformEndpointRequest
                    {
                        Token = registrationId,
                        PlatformApplicationArn = arn,
                        CustomUserData = userData
                    });

                    Settings.Arnsns = response.EndpointArn;
                }
                else if (results.Attributes["Token"] != registrationId)
                {
                    var updateToken = new SetEndpointAttributesRequest();
                    updateToken.Attributes.Add("Token", registrationId);
                    updateToken.Attributes.Add("Enabled", "true");
                    updateToken.EndpointArn = Settings.Arnsns;

                    var update = await SnsClient.SetEndpointAttributesAsync(updateToken);
                }
            }

            _endpointArn = response.EndpointArn;
        }
    }
}