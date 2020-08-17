using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WallpaperManager.Tests
{
    [TestClass()]
    public class WallpaperTests
    {
        [TestMethod()]
        public void SetWallpaperTest()
        {
            Wallpaper wallpaper = new Wallpaper(new BingWallpaperGetter());
            wallpaper.SetWallpaper(Style.Fill);
            Assert.IsTrue(true);
        }
    }
}