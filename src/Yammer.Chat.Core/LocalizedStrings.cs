using Yammer.Chat.Core.Resources;

namespace Yammer.Chat.Core
{
    /// <summary>
    /// Provides access to string resources.
    /// </summary>
    public class LocalizedStrings
    {
        private static AppResources _localizedResources = new AppResources();

        public AppResources Resources { get { return _localizedResources; } }
    }
}