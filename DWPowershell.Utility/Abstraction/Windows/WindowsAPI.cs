using StaticAbstraction;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace DWPowerShell.Utility.Abstraction.Windows
{
    public interface IWindowsAPI
    {
        string BuildRelativePath(string sourcePath, string targetPath);
    }

    public class WindowsAPI : IWindowsAPI
    {
        [DllImport("shlwapi.dll", SetLastError = true)]
        private static extern int PathRelativePathTo(StringBuilder pszPath, string pszFrom, int dwAttrFrom, string pszTo, int dwAttrTo);
        private const int FILE_ATTRIBUTE_DIRECTORY = 0x10;
        private const int FILE_ATTRIBUTE_NORMAL = 0x80;

        protected IStaticAbstraction _diskManager = null;

        public WindowsAPI() : this(null)
        {
        }

        public WindowsAPI(IStaticAbstraction diskManager)
        {
            _diskManager = diskManager ?? new StaticAbstractionWrapper();
        }

        public string BuildRelativePath(string sourcePath, string targetPath)
        {
            int fromAttr = GetPathAttribute(sourcePath);
            int toAttr = GetPathAttribute(targetPath);

            StringBuilder path = new StringBuilder(260); // MAX_PATH
            if (PathRelativePathTo(
                    path,
                    sourcePath,
                    fromAttr,
                    targetPath,
                    toAttr) == 0)
            {
                throw new ArgumentException("Paths must have a common prefix");
            }
            return path.ToString();
        }

        private int GetPathAttribute(string path)
        {
            var di = _diskManager.NewDirectoryInfo(path);
            if (di.Exists)
            {
                return FILE_ATTRIBUTE_DIRECTORY;
            }

            var fi = _diskManager.NewFileInfo(path);
            if (fi.Exists)
            {
                return FILE_ATTRIBUTE_NORMAL;
            }
            else if (!string.IsNullOrEmpty(fi.Extension))
            {
                // guess that if it has an extension that it should be a file
                return FILE_ATTRIBUTE_NORMAL;
            }

            throw new FileNotFoundException();
        }
    }
}
