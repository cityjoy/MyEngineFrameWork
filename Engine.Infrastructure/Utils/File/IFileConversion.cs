using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Infrastructure.Utils.FileSystem
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFileConversion
    {
        void WordToPdf(string sourceFilePath, string destFilePath);
        void ExcelToPdf(string sourceFilePath, string destFilePath);
        void PptToPdf(string sourceFilePath, string destFilePath);
        void PdfToSwf(string pdfFilePath, string swfFilePath);
    }
}
