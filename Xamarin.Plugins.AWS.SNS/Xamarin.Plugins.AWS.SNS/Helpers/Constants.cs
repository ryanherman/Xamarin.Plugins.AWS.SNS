using Amazon;

namespace Xamarin.Plugins.AWS.SNS.Helpers
{
    public class Constants
    {
        //identity pool id for cognito credentials
        public const string IdentityPoolId = "";

        //sns android platform arn
        public const string AndroidPlatformApplicationArn = "";

        //sns ios platform arn
        public const string iOSPlatformApplicationArn = "";

        //project id for android gcm
        public const string GoogleConsoleProjectId = "";

        //set your regions here
        public static RegionEndpoint CognitoRegion = RegionEndpoint.USEast1;
        public static RegionEndpoint SnsRegion = RegionEndpoint.USWest1;
    }
}