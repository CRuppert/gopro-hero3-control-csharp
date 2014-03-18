﻿using System;
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

        public Dictionary<string, string> GetFileList()
        {
            throw new NotImplementedException("working on this still");
            var fL = accessHttpPage(string.Empty);
            var doc = new HtmlDocument();
            doc.LoadHtml(fL);

            foreach (var row in doc.DocumentNode.SelectNodes("//tr"))
            {
                var m = row.SelectNodes("//span[@class=\"date\"]");
                var l = row.SelectSingleNode("//a[@class=\"link\"]");
                if (m == null &&  l == null)
                {
                    continue;
                }

                var fd = new FileData
                    {
                        Path = row.SelectSingleNode("//a[@class=\"link\"]").Attributes["href"].Value,
                        CreatedOn = DateTime.Parse(row.SelectSingleNode("//span[@class=\"date\"]").InnerText),
                        Size = row.SelectSingleNode("//span[@class=\"size\"]").InnerText,
                        SizeUnits = row.SelectSingleNode("//span[@class=\"units\"]").InnerText
                    };
                string bob = fd.Path;
            }

            return new Dictionary<string, string>();
        }

        public Stream GetLastFile()
        {
            throw new NotImplementedException("working on this still");
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
        public string Path { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Size { get; set; }
        public string SizeUnits { get; set; }
    }
}