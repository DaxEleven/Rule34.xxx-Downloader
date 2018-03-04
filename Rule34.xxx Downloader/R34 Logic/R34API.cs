using System;
using System.IO;
using System.Net;
using System.Xml;

namespace R34Downloader.Logic
{
    public static class R34API
    {
        public static string MainRequestPath { get => "https://rule34.xxx/index.php?page=dapi&s=post&q=index"; }

        public static int GetCountContent(string request)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(MainRequestPath + "&tags=" + request);
            return Convert.ToInt32(doc.DocumentElement.Attributes[0].Value);
        }

        public static void DownloadContent(string request, string path, IProgress<int> progress, IProgress<int> progress2, int quantity)
        {
            int maxPid = (quantity <= 100) ? 1 : ((quantity % 100 == 0) ? (quantity / 100 - 1) : (quantity / 100));
            for (int pid = 0; pid <= maxPid; pid++)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(MainRequestPath + "&tags=" + request + "&pid=" + pid);
                XmlElement root = doc.DocumentElement;

                int postCount = quantity - pid * 100 < 100 ? quantity - pid * 100 : 100;
                for (int i = 0; i < postCount; i++)
                {
                    string url = root.ChildNodes[i].Attributes[2].Value;
                    string filename = root.ChildNodes[i].Attributes[10].Value + url.Substring(url.LastIndexOf('.'), url.Length - url.LastIndexOf('.'));

                    if (url.Contains(".webm") && Transfer.Video)
                    {
                        DownloadContent(url, path + "\\Video\\" + filename);
                    }
                    if (url.Contains(".gif") && Transfer.Gif)
                    {
                        DownloadContent(url, path + "\\Gif\\" + filename);
                    }
                    if (!url.Contains(".webm") && !url.Contains(".gif") && Transfer.Images)
                    {
                        DownloadContent(url, path + "\\Images\\" + filename);
                    }

                    progress.Report(pid * 100 + i + 1);
                    progress2.Report(pid * 100 + i + 1);
                }
            }
        }

        public static void DownloadContent(string path, string name)
        {
            if (!File.Exists(name))
            {
                string dir = name.Substring(0, name.LastIndexOf('\\'));
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                using (WebClient client = new WebClient())
                {
                    try
                    {
                        client.DownloadFile(path, name);
                    }
                    catch { }
                }
            }
        }
    }
}