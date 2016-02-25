using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TicketDataModel;
using TicketManager;
using System.Collections.Generic;
using System.Linq;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
         
        [TestMethod, Ignore]
        public void TestMethod1()
        {
            var f = new List<TicketDataModel.JobFile>();
            var file1 = new TicketDataModel.JobFile {
                FileName = "file1.doc",
                FilePath = @"Z:\Компании\ЦО\тралала\"
            };
            var file2 = new TicketDataModel.JobFile {
                FileName = "file2.doc",
                FilePath = @"Z:\Компании\тралала\"
            };
            var file3 = new TicketDataModel.JobFile
            {
                FileName = "file3.doc",
                FilePath = @"Z:\Компании\тралала\"
            };

            f.Add(file1); f.Add(file2); f.Add(file3);

            var ft = new FileTree();
            ft.CreateTreeFromPaths(f);
            foreach (var n in ft.Nodes)
                Console.WriteLine("id = " + n.ID + " parent = " + n.ParentID + " name " + n.Name);
        }
    }
}
