using UnityEngine;

namespace Assets.FUGAS.Ads.Scripts.Helpers
{
    public static class AdUtils
    {
        /// <summary>
        /// Returns <c>true</c> on debug builds. On release is always <c>false</c>
        /// </summary>
        /// <param name="settingsValue"></param>
        /// <returns><c>true</c> on ads tests</returns>
        public static bool IsTestMode(bool settingsValue)
        {
            return settingsValue && Debug.isDebugBuild;
        }
    }
}