using Newtonsoft.Json.Linq;

namespace SocialTrender
{
    public class JSONUtil
    {
        public static bool SaveTo(JToken token, ref bool variable)
        {
            bool? tokenValue = token?.Value<bool>();

            if (tokenValue.HasValue)
            {
                variable = tokenValue.Value;
                return true;
            }

            return false;
        }
    }
}
