using System;

namespace CompositeC1Contrib.Email.GoogleAnalytics
{
    public class GoogleAnalyticsEventsProcessorOptions
    {
        private string _trackerCode;

        public Func<string> TrackerCodeResolver { private get; set; }

        public string TrackerCode
        {
            get
            {
                if (TrackerCodeResolver != null)
                {
                    return TrackerCodeResolver();
                }

                return _trackerCode;
            }

            set
            {
                _trackerCode = value;

                TrackerCodeResolver = null;
            }
        }
    }
}
