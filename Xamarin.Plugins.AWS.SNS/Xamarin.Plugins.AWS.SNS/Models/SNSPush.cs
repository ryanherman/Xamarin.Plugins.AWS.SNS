namespace Xamarin.Plugins.AWS.SNS.Models
{
    public class SNSPush
    {
        public Aps aps { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public string from { get; set; }
        public string message { get; set; }
        public string collapse_key { get; set; }

        public class Aps
        {
            public string alert { get; set; }
        }
    }
}