using System;

namespace SocialTrender
{
    public struct CallbackData
    {
        public Action<string> LogMessageCallback { get; set; }
        public Action<UnityData> SearchCompleteCallback { get; set; }
        public Action SearchFailCallback { get; set; }
        public Action<ProgressData> ProgressCallback { get; set; }
    }
}
