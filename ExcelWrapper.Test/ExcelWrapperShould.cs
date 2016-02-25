using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExcelWrapper;

namespace ExcelWrapper.Test
{
    [TestClass]
    public class ExcelWrapperShould
    {
        string _path;
        string _saveTo;
        IExcelFileWrapper _wrapper;

        [TestInitialize]
        public void Setup()
        {
            _path = @"c:\test\excel.xlsx";
            
            _wrapper = new ExcelFileWrapper(_path);
        }

        [TestCleanup]
        public void TearDown()
        {
            _wrapper.Close();
        }

        [TestMethod]
        public void OpenFileAtSpecifiedPathAndCopyItToSpecifiedLocation()
        {
            Assert.IsTrue(System.IO.File.Exists(_saveTo));
        }

        [TestMethod]
        [ExpectedException(typeof(System.Runtime.InteropServices.COMException))]
        public void RemoveAllSheetsInExcelFileExceptTemplate()
        {
            _wrapper.ClearSheets();
            try
            {
                _wrapper.ActivateSheet(2);
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                if (ex.Message.Contains("DISP_E_BADINDEX") || ex.Message.Contains("0x8002000B"))
                    throw ex;
            }
        }

        [TestMethod]
        public void AddNewSheetsUsingTemplate()
        {
            _wrapper.ClearSheets();
            var test = _wrapper.AddSheetFromTemplate("test");
            Assert.AreEqual(2, test);
        }

        [TestMethod]
        [Ignore]
        public void InsertRows()
        {
            //_wrapper.ClearSheets();
            //var test = _wrapper.AddSheetFromTemplate("test");
            //_wrapper.InsertRowsAt(1);
            //_wrapper.Save();
            //Assert.AreEqual(null, _wrapper.CellValue(1, 1));
            //Assert.AreEqual("Fuck you", _wrapper.CellValue(1, 2));
        }

        [TestMethod]
        [Ignore]
        public void InsertCols()
        {
            //_wrapper.ClearSheets();
            //var test = _wrapper.AddSheetFromTemplate("test");
            
            //_wrapper.InsertColumnsAt(1);
            //_wrapper.Save();
            
            //Assert.AreEqual(null, _wrapper.CellValue(1, 1));
            //Assert.AreEqual("Fuck you", _wrapper.CellValue(2, 1));
        }

        [TestMethod]
        [Ignore]
        public void SetCellValue()
        {
            //_wrapper.ClearSheets();
            
            //var test = _wrapper.AddSheetFromTemplate("test");
            //_wrapper.ActivateSheet(test);
            //_wrapper.SetCell(1, 1, "No fuck");
            //_wrapper.Save();
            //Assert.AreEqual("No fuck", _wrapper.CellValue(1, 1));
        }

        [TestMethod, Ignore] // Can't check this one !!! 
        public void MergeCells()
        {
            //_wrapper.ClearSheets();

            //var test = _wrapper.AddSheetFromTemplate("test");
            //_wrapper.ActivateSheet(test);
            //_wrapper.MergeCells(1, 1, 11);
            //_wrapper.Save();
        }
    }
}
