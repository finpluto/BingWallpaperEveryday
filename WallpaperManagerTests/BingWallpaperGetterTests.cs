using Microsoft.VisualStudio.TestTools.UnitTesting;
using WallpaperManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallpaperManager.Tests
{
    [TestClass()]
    public class BingWallpaperGetterTests
    {
        [TestMethod()]
        public void GetWallpaperTest()
        {
            BingWallpaperGetter getter = new BingWallpaperGetter();
            string path = getter.GetWallpaper();
            Console.WriteLine(path);
            Assert.IsTrue(true);
        }
    }
}