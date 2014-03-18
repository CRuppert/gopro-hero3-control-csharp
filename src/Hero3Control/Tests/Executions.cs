using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
    }
}
