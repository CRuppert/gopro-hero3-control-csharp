using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hero3Control;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class Executions
    {

        [Test]
        public void CanGetListOfFiles()
        {
            var c = new GoProConfig()
                {
                    IpAddress = "10.5.5.9",
                    Password = "goprohero"
                };
            var controller = new Hero3ControlService(c);

            var fileList = controller.GetFileList();
            Assert.Greater(fileList.Count, 0);
        }

        [Test]
        public void CanDownloadStream()
        {
            var c = new GoProConfig()
            {
                IpAddress = "10.5.5.9",
                Password = "goprohero"
            };
            var controller = new Hero3ControlService(c);
            
            using (Stream s = controller.GetLastFile())
            {
                var ms = new MemoryStream();
                s.CopyTo(ms);
                s.Close();
                Assert.Greater(ms.Length, 0);
                ms.Seek(0, SeekOrigin.Begin);
                
                var f = File.Create("./file.mp4");
                ms.CopyTo(f);
                ms.Close();
                f.Close();
            }

        }


        [Test]
        public void FullTest()
        {
            var c = new GoProConfig()
            {
                IpAddress = "10.5.5.9",
                Password = "goprohero"
            };
            var controller = new Hero3ControlService(c);

            controller.RecordForSeconds(5);
            Thread.Sleep(1000);
            using (Stream s = controller.GetLastFile())
            {
                var ms = new MemoryStream();
                s.CopyTo(ms);
                s.Close();
                Assert.Greater(ms.Length, 0);
                ms.Seek(0, SeekOrigin.Begin);

                var f = File.Create("./fullTest.mp4");
                ms.CopyTo(f);
                ms.Close();
                f.Close();
            }

        }


        [Test]
        public void StressTest()
        {
            var config = new GoProConfig()
            {
                IpAddress = "10.5.5.9",
                Password = "goprohero"
            };
            var controller = new Hero3ControlService(config);

            controller.StopRecording();
            int lastCount = 0;

            for (int i = 0; i < 25; i++)
            {
                controller.RecordForSeconds(5);
                Thread.Sleep(1000);
                var c = controller.GetFileList().Count;
                Assert.Greater(c, lastCount);
                lastCount = c;
                using (Stream s = controller.GetLastFile())
                {
                    var ms = new MemoryStream();
                    s.CopyTo(ms);
                    s.Close();
                    Assert.Greater(ms.Length, 0);
                    ms.Seek(0, SeekOrigin.Begin);

                    var f = File.Create("./fullTest"+i+".mp4");
                    ms.CopyTo(f);
                    ms.Close();
                    f.Close();
                }
                Thread.Sleep(10000);
            }
            
        }
    }
}
