using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

namespace WallpaperManager
{
    public interface IWallpaperGetter
    {
        string GetWallpaper();
    }

    public enum Style
    {
        Fill,
        Fit,
        Span,
        Tile,
        Center,
        Stretch
    }

    public class Wallpaper
    {
        IWallpaperGetter getter;

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public Wallpaper(IWallpaperGetter getter)
        {
            this.getter = getter;
        }

        // source: https://stackoverflow.com/questions/1061678/change-desktop-wallpaper-using-code-in-net
        public void SetWallpaper(Style style)
        {
            string wallpaperPath = getter.GetWallpaper();
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            if (style == Style.Fill)
            {
                key.SetValue(@"WallpaperStyle", 10.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == Style.Fit)
            {
                key.SetValue(@"WallpaperStyle", 6.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == Style.Span) // Windows 8 or newer only!
            {
                key.SetValue(@"WallpaperStyle", 22.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == Style.Stretch)
            {
                key.SetValue(@"WallpaperStyle", 2.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == Style.Tile)
            {
                key.SetValue(@"WallpaperStyle", 0.ToString());
                key.SetValue(@"TileWallpaper", 1.ToString());
            }
            if (style == Style.Center)
            {
                key.SetValue(@"WallpaperStyle", 0.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                wallpaperPath,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE) == 0)
            {
                string errorMessage = new Win32Exception(Marshal.GetLastWin32Error()).Message;
                throw new Win32Exception(errorMessage);
            }
        }
    }

    public class BingWallpaperGetter : IWallpaperGetter
    {
        private static readonly string bingWallpaperApi = "https://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1&mkt=zh-CN";
        private Uri imgUri;

        private void FetchWallpaperUri()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(bingWallpaperApi);
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (Stream s = response.GetResponseStream())
            using (StreamReader sr = new StreamReader(s))
            {
                JObject json = (JObject)JToken.ReadFrom(new JsonTextReader(sr));
                Uri baseUri = new Uri("https://www.bing.com");
                imgUri = new Uri(baseUri, (string)json["images"][0]["url"]);
            }
        }
        public string GetWallpaper()
        {
            FetchWallpaperUri();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(imgUri);
            request.Method = "GET";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (Stream s = response.GetResponseStream())
            {
                System.Drawing.Image img = System.Drawing.Image.FromStream(s);
                string tempPath = Path.GetTempFileName();
                img.Save(tempPath);
                return tempPath;
            }
        }
    }
}
