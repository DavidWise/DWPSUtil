using DWPowerShell.Utility.Abstraction.Process;
using DWPowerShell.Utility.Abstraction.Windows;
using StaticAbstraction;
using StaticAbstraction.Reflection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DWPowerShell.Utility
{
    public class DWPSUtils
    {
        private static string _libraryFolder = null;

        public static IStaticAbstraction _diskManager { get; set; }
        public static IProcessManager _processManager { get; set; }

        static DWPSUtils()
        {
            _diskManager = new StaticAbstractionWrapper();
            _processManager = new ProcessManager();
        }

        public static string LibraryFolder
        {
            get
            {
                if (_libraryFolder == null)
                {
                    _libraryFolder = GetAssemblyFolder(Assembly.GetExecutingAssembly());
                }

                return _libraryFolder;
            }
        }


        public static string GetAssemblyFolder(Assembly assembly)
        {
            return GetAssemblyFolder(new StAbAssemblyInstance(assembly));
        }

        public static string GetAssemblyFolder(IAssemblyInstance assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            var path = assembly.Location;
            var file = _diskManager.NewFileInfo(path);

            return ForceTrailingSlash(file.DirectoryName);
        }

        public static string CurrentFolder => ForceTrailingSlash(_diskManager.Directory.GetCurrentDirectory());
        
        public static string ForceTrailingSlash(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(path, "Path cannot be null or empty");
            if (path.EndsWith("\\")) return path;
            return $"{path}\\";
        }

        [ExcludeFromCodeCoverage]
        public static string ExecuteCommandSync(string command, string runInfolder)
        {
            return ExecuteCommandSync(_diskManager, _processManager, command, runInfolder).Output;
        }


        [ExcludeFromCodeCoverage]
        public static IProcessResult ExecuteCommandSync(IStaticAbstraction diskManager, IProcessManager processManager, string command, string runInfolder)
        {
            if (diskManager == null) throw new ArgumentNullException(nameof(diskManager));
            if (processManager == null) throw new ArgumentNullException(nameof(processManager));
            if (string.IsNullOrWhiteSpace(command)) throw new ArgumentException("ExecuteCommandSync requires a valid command");
            if (!string.IsNullOrWhiteSpace(runInfolder) && !diskManager.Directory.Exists(runInfolder)) throw new DirectoryNotFoundException($"ExecuteCommandSync - '{runInfolder}' does not exist");

            IProcessResult result = null;

            var curDir = diskManager.Directory.GetCurrentDirectory();
            var changeDir = !string.IsNullOrWhiteSpace(runInfolder);
            try
            {
                if (changeDir) diskManager.Directory.SetCurrentDirectory(runInfolder);

                result = _processManager.Execute("cmd", "/c " + command, 10000);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (changeDir) diskManager.Directory.SetCurrentDirectory(curDir);

            return result;
        }

        public static bool IsFullPath(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                var cmp = StringComparison.InvariantCultureIgnoreCase;
                if (path.StartsWith("\\\\", cmp) || path.IndexOf(":\\", cmp)== 1) return true;
            }
            return false;
        }

        public static string MakeFileSystemSafe(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return null;

            var unsafechars = new string[] {":", "\\", "/", "?", "*", " ", ">", "<", "|"};
            var replacechars = new string[] {"", "_", "_", "_", "_", "_", "_", "_", "_"};

            var result = ReplaceTokens(path, unsafechars, replacechars);
            if (string.IsNullOrWhiteSpace(result)) throw new ArgumentException($"Name '{path}' could not be made safe");

            var safeChar = "_";
            var doublechar = $"{safeChar}{safeChar}";
            while (result.IndexOf(doublechar, StringComparison.InvariantCultureIgnoreCase) >= 0)
                result = result.Replace(doublechar, safeChar);

            return result;
        }

        public static string GetFullPath(string basePath, string name)
        {
            if (DWPSUtils.IsFullPath(name)) return name;
            if (string.IsNullOrWhiteSpace(name)) {
                if (DWPSUtils.IsFullPath(basePath))
                    return basePath;
                else 
                    throw new DirectoryNotFoundException("Either the base path or the file name must be a valid path");
            }

            var relPath = _diskManager.Path.Combine(basePath, name);
            var relInfo = _diskManager.NewFileInfo(relPath);
            return relInfo.FullName;
        }

        public static string GetDirectoryPath(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            var dirName = path;
            if (_diskManager.File.Exists(path))
            {
                dirName = _diskManager.NewFileInfo(path).DirectoryName;
            }
            else if(_diskManager.Directory.Exists(path))
            {
                dirName = _diskManager.NewDirectoryInfo(path).FullName;
            }
            else
            {
                throw new ApplicationException($"the item '{path}' does not exist");
            }

            if (string.IsNullOrEmpty(dirName)) throw new ArgumentException($"Unable to determine the directory for '{path}'");

            return ForceTrailingSlash(dirName);
        }

        public static string ResolveRelativePath(string basePath, string relativePath)
        {
            //~~ TODO: Need to test
            if (DWPSUtils.IsFullPath(relativePath)) return relativePath;
            if (string.IsNullOrEmpty(relativePath)) return basePath;

            var dirPath = GetDirectoryPath(basePath);
            if (dirPath == null) return relativePath;

            var tempPath = _diskManager.Path.Combine(dirPath, relativePath);
            if (_diskManager.Directory.Exists(tempPath))
            {
                return _diskManager.NewDirectoryInfo(tempPath).FullName;
            }

            return _diskManager.NewFileInfo(tempPath).FullName;
        }

        public static string[] GetEmbeddedResourceNames(Assembly assembly)
        {
            return GetEmbeddedResourceNames(new StAbAssemblyInstance(assembly));
        }

        public static string[] GetEmbeddedResourceNames(IAssemblyInstance assembly)
        {
            return assembly?.GetManifestResourceNames();
        }

        public static string GetEmbeddedResource(Assembly assembly, string resourceName)
        {
            return GetEmbeddedResource(new StAbAssemblyInstance(assembly), resourceName);
        }

        public static string GetEmbeddedResource(IAssemblyInstance assembly, string resourceName)
        {
            var names = GetEmbeddedResourceNames(assembly);
            var fullName = names.FirstOrDefault(x => x.EndsWith(resourceName, StringComparison.InvariantCultureIgnoreCase));
            if (fullName == null) return null;

            string result = null;
            using (Stream stream = assembly.GetManifestResourceStream(fullName))
            {
                if (stream != null)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }

            return result;
        }

        public static bool MakeBoolean(string value, bool defaultWhenMissing = false)
        {
            var result = defaultWhenMissing;

            if (!string.IsNullOrWhiteSpace(value))
            {
                result = false;
                var val = value.ToLower();

                var trueVals = new string[] {"true", "on", "yes", "t", "y", "1", "-1"};
                result = trueVals.Any(x => x == val);
            }

            return result;
        }
        
        public static string ReplaceToken(string text, string[] tokens, string replacement, bool removeDuplicates = false)
        {
            var replacements = new string[] {replacement ?? ""};
            return ReplaceTokens(text, tokens, replacements, removeDuplicates);
        }

        public static string ReplaceTokens(string text, string[] tokens, string[] replacements, bool removeDuplicates = false)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            if (tokens == null || tokens.Length<1) throw new ArgumentException("[tokens] for replacement must be defined");
            if (replacements == null || replacements.Length<1) throw new ArgumentException("[replacements] for replacement must be defined");
            if (tokens.Length != replacements.Length) throw new ArgumentException($"{tokens.Length} tokens defined for {replacements.Length}.  Both must have the same number of values");

            var result = text;

            for (int i = 0; i < tokens.Length; i++)
            {
                var findToken = tokens[i];
                if (!string.IsNullOrEmpty(findToken))
                {
                    var replaceToken = replacements[i]??"";

                    int splitPos = result.IndexOf(findToken, StringComparison.InvariantCultureIgnoreCase);
                    while (splitPos >= 0)
                    {
                        var leftHalf = result.Substring(0, splitPos);
                        var rightHalf = result.Substring(splitPos + findToken.Length);
                        var sep = replaceToken;
                        if (removeDuplicates && !string.IsNullOrEmpty(replaceToken) &&
                            leftHalf.EndsWith(replaceToken)) sep = "";

                        result = leftHalf + sep + rightHalf;
                        splitPos = result.IndexOf(findToken, splitPos + sep.Length,
                            StringComparison.InvariantCultureIgnoreCase);
                    }
                }
            }

            return result;
        }

        public static string BuildRelativePath(string sourcePath, string targetPath)
        {
            var winApi = new WindowsAPI(_diskManager);
            return winApi.BuildRelativePath(sourcePath, targetPath);
        }
    }
}
