using System;
using System.Threading.Tasks;
using WallpaperManager;

namespace BingWallpaperEveryday
{
    class Program
    {
        static readonly string programName = "BingWallpaperEveryday";
        static void Main(string[] args)
        {
            MainAsync().Wait();
        }

        static async Task MainAsync()
        {
            Wallpaper wallpaper = new Wallpaper(new BingWallpaperGetter());
            bool continueFlag = true;
            while (continueFlag)
            {
                try
                {
                    await wallpaper.SetWallpaper(Style.Fill);
                    continueFlag = false;
                }
                catch (Exception)
                {
                    await Task.Delay(30000);
                }
            }
        }
    }
}
