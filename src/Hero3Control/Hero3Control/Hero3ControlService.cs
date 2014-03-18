using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Hero3Control
{
    public class Hero3ControlService : IVideoCameraControlService
    {
        protected string controlFormat = "http://{0}/{1}/{2}t={3}&p={4}";
        protected string videosFormat = "http://{0}:8080/videos/DCIM/100GOPRO/";
        protected GoProConfig config = null;

        public Hero3ControlService(GoProConfig config)
        {
            this.config = config;
        }


        public void StartRecording()
        {
            using (var webClient = new WebClient())
            {
                try
                {
                    webClient.DownloadString(string.Format(controlFormat, config.IpAddress, "bacpac", "SH",
                                                           config.Password, "%01"));
                }
                catch (WebException ex)
                {
                    throw new ApplicationException(ex.Status + ": unable to control camera", ex);
                }
                
            }
        }

        public void StopRecording()
        {
            using (var webClient = new WebClient())
            {
                try
                {
                    webClient.DownloadString(string.Format(controlFormat, config.IpAddress, "bacpac", "SH",
                                                           config.Password, "%00"));
                }
                catch (WebException ex)
                {
                    throw new ApplicationException(ex.Status + ": unable to control camera", ex);
                }

            }
        }

        public void RecordForSeconds(int seconds)
        {
            StartRecording();
            System.Threading.Thread.Sleep(seconds*1000);
            try
            {
                StopRecording();
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException("Error stopping camera! Still recording!", ex);
            }
        }

        public List<FileData> GetFileList()
        {
            var f = new List<FileData>();

            var fL = accessHttpPage(string.Empty);
            var doc = new HtmlDocument();
            doc.LoadHtml(fL);

            foreach (var row in doc.DocumentNode.SelectNodes("//tr"))
            {
                var m = row.SelectNodes(".//span[@class=\"date\"]");
                var l = row.SelectSingleNode(".//a[@class=\"link\"]");
                if (m == null &&  l == null)
                {
                    continue;
                }
                var p = row.SelectSingleNode(".//a[@class=\"link\"]").Attributes["href"];
                var d = row.SelectSingleNode(".//span[@class=\"date\"]");
                var s =row.SelectSingleNode(".//span[@class=\"size\"]");
                var u = row.SelectSingleNode(".//span[@class=\"unit\"]");
                    
                var fd = new FileData
                    {
                        Name = p.Value,
                        CreatedOn = DateTime.Parse(d.InnerText),
                        Size = s.InnerText,
                        SizeUnits = u.InnerText
                    };
                f.Add(fd);
            }

            return f;
        }

        public Stream GetLastFile()
        {
            var files = GetFileList();
            var file = files.Last(t => t.Name.EndsWith("MP4"));
            using (var wc = new WebClient())
            {
                return wc.OpenRead(string.Format(videosFormat, config.IpAddress) + "/" + file.Name);
            }
            return null;
        }
        

        private string accessHttpPage(string pageName)
        {
            using (var webClient = new WebClient())
            {
                var response = webClient.DownloadString(string.Format(videosFormat, config.IpAddress));
                return response;
            }
        }
    }

    public class FileData
    {
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Size { get; set; }
        public string SizeUnits { get; set; }
    }
}
