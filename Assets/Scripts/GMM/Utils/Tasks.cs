using System.Threading.Tasks;

namespace GMM
{
    namespace Utils
    {
        public static class Tasks
        {
            public static async Task Blink()
            {
                await Task.Yield();
                // Task.Delay(TimeSpan.FromMilliseconds(1));
            }
        }
    }
}