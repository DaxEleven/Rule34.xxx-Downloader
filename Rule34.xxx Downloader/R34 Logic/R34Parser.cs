using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace R34Downloader.Logic
{
    public static class R34Parser
    {
        public static string MainRequestPath { get => "https://rule34.xxx/index.php?page=post&s=list&tags="; }
        public static int PidValue { get => 42; }

        public static HtmlWeb web = new HtmlWeb();

        public static bool IsSomethingFound(string request)
        {
            HtmlDocument doc = web.Load(MainRequestPath + request);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//div[@class='content']//span[@class='thumb']");

            if (nodes == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static int GetMaxPid(string request)
        {
            HtmlDocument doc = web.Load(MainRequestPath + request);
            HtmlNode nodes = doc.DocumentNode.SelectSingleNode("//div[@class='pagination']//a[@alt='last page']");

            if (nodes == null)
            {
                return 0;
            }
            else
            {
                string pidString = nodes.GetAttributeValue("href", "");
                return Convert.ToInt32(pidString.Substring(pidString.LastIndexOf('=') + 1, pidString.Length - pidString.LastIndexOf('=') - 1));
            }
        }

        public static int GetCountContent(string request, int maxPid)
        {
            HtmlDocument doc = web.Load(MainRequestPath + request + "&pid=" + maxPid);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//div[@class='content']//span[@class='thumb']/a");
            HtmlNodeCollection nodes2 = doc.DocumentNode.SelectNodes("//div[@class='content']/div/h1");

            if (nodes != null && maxPid == 0)
            {
                return nodes.Count;
            }
            else if (nodes != null && maxPid != 0)
            {
                return maxPid + nodes.Count;
            }
            if (nodes2 != null)
            {
                return -1;
            }
            return -1;
        }

        public static void DownloadPages(string request, string path, IProgress<int> progress, IProgress<int> progress2, int quantity)
        {
            int maxPages = quantity, residue = PidValue;
            if (quantity < PidValue)
            {
                maxPages = PidValue;
                residue = quantity;
            }
            for (int pid = 0; pid < maxPages; pid += PidValue)
            {
                HtmlDocument doc = web.Load(MainRequestPath + request + "&pid=" + pid);
                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//div[@class='content']//span[@class='thumb']/a");

                DownloadPosts(GetPostLinks(nodes), path, pid, residue, maxPages, progress, progress2);
            }
        }

        public static void DownloadPosts(List<string> postList, string path, int pid, int residue, int maxPages, IProgress<int> progress, IProgress<int> progress2)
        {
            int maxPosts = postList.Count;
            if (maxPages - pid < PidValue)
            {
                maxPosts = maxPages - pid;
            }
            else if (maxPages - pid == PidValue)
            {
                maxPosts = residue;
            }
            for (int i = 0; i < maxPosts; i++)
            {
                HtmlDocument doc2 = web.Load("https://rule34.xxx/" + postList.ElementAt(i));
                HtmlNode videoNode = doc2.DocumentNode.SelectSingleNode("//video[@id='gelcomVideoPlayer']/source");
                HtmlNode imageNode = doc2.DocumentNode.SelectSingleNode("//div[@class='content']//img[@id='image']");

                if (videoNode != null && Transfer.Video)
                {
                    string videoPath = videoNode.GetAttributeValue("src", "");
                    DownloadContent(videoPath, path + "\\Video\\" + postList.ElementAt(i).Split('=')[3] + videoPath.Substring(videoPath.LastIndexOf('.'), videoPath.Length - videoPath.LastIndexOf('.')));
                }
                else if (imageNode != null)
                {
                    string imagePath = imageNode.GetAttributeValue("src", "");
                    string id = imagePath.Split('?')[1];
                    imagePath = imagePath.Substring(0, imagePath.LastIndexOf('?'));
                    string filename = id + imagePath.Substring(imagePath.LastIndexOf('.'), imagePath.Length - imagePath.LastIndexOf('.'));

                    if (imagePath.Contains(".gif") && Transfer.Gif)
                    {
                        DownloadContent(imagePath, path + "\\Gif\\" + filename);
                    }
                    else if (!imagePath.Contains(".gif") && Transfer.Images)
                    {
                        DownloadContent(imagePath, path + "\\Images\\" + filename);
                    }
                }

                progress.Report(pid + i + 1);
                progress2.Report(pid + i + 1);
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

        public static List<string> GetPostLinks(HtmlNodeCollection nodes)
        {
            List<string> posts = new List<string>();
            foreach (HtmlNode node in nodes)
            {
                posts.Add(node.GetAttributeValue("href", "").Replace("&amp;", "&"));
            }
            return posts;
        }
    }
}