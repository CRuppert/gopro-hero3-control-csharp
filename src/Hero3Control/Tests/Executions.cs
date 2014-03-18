using System;
using System.Collections.Generic;
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
        public void CanGetLastFile()
        {
            var c = new GoProConfig()
                {
                    IpAddress = "10.5.5.9",
                    Password = "goprohero"
                };
            var controller = new Hero3ControlService(c);

            var file = controller.GetFileList();
        }
    }
}
