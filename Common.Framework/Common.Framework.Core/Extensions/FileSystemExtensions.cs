// <copyright file="FileSystemExtensions.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Framework.Core.AppConfig;
using Common.Framework.Core.Enums;
using Common.Framework.Core.Logging;

namespace Common.Framework.Core.Extensions
{
    public static class FileSystemExtensions
    {
        public static void CleanDirectory(
            this string targetDirectory,
            bool deleteTargetDirectory)
        {
            try
            {
                if (!Directory.Exists(targetDirectory))
                {
                    return;
                }

                DeleteDirectory(targetDirectory, deleteTargetDirectory);
                LogManager.Instance().LogInfoMessage("Cleaned directory [" + targetDirectory + "].");
            }
            catch (Exception exception)
            {
                LogManager.Instance().LogErrorMessage(exception.Message);
            }
        }

        public static void CopyDirectoryTo(
            this string source,
            string destination)
        {
            if (!Directory.Exists(source))
            {
                return;
            }

            if (destination[destination.Length - 1] != Path.DirectorySeparatorChar)
            {
                destination += Path.DirectorySeparatorChar;
            }

            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            var files = Directory.GetFileSystemEntries(source);
            foreach (var file in files)
            {
                if (Directory.Exists(file))
                {
                    file.CopyDirectoryTo(destination + Path.GetFileName(file));
                }
                else
                {
                    var destFileName = destination + Path.GetFileName(file);
                    if (File.Exists(destFileName))
                    {
                        File.Delete(destFileName);
                    }

                    try
                    {
                        File.Copy(file, destFileName);
                        LogManager.Instance().LogDebugMessage("Copied file [" + file + "] to [" + destination + "].");
                    }
                    catch (IOException ioexception)
                    {
                        LogManager.Instance().LogErrorMessage(ioexception.Message);
                        LogManager.Instance().LogErrorMessage("Failed to copy file [" + file + "] to [" + destination + "].");
                    }
                }
            }
        }

        public static void CopyDirectoryTo(
            this string source,
            string destination,
            string exclude)
        {
            if (!Directory.Exists(source))
            {
                return;
            }

            if (destination[destination.Length - 1] != Path.DirectorySeparatorChar)
            {
                destination += Path.DirectorySeparatorChar;
            }

            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            var files = Directory.GetFileSystemEntries(source).Where(name => !name.EndsWith(exclude));
            foreach (var file in files)
            {
                if (Directory.Exists(file))
                {
                    file.CopyDirectoryTo(destination + Path.GetFileName(file), exclude);
                }
                else
                {
                    var destFileName = destination + Path.GetFileName(file);
                    if (File.Exists(destFileName))
                    {
                        File.Delete(destFileName);
                    }

                    File.Copy(file, destFileName);
                    LogManager.Instance().LogDebugMessage("Copied file [" + file + "] to [" + destination + "].");
                }
            }
        }

        public static void CopyFileTo(
            this string source,
            string destination)
        {
            if (!File.Exists(source))
            {
                return;
            }

            if (File.Exists(destination))
            {
                File.SetAttributes(destination, FileAttributes.Normal);
                File.Delete(destination);
            }

            File.Copy(source, destination);
            File.SetAttributes(destination, FileAttributes.Normal);

            LogManager.Instance().LogDebugMessage("Copied file [" + source + "] to [" + destination + "].");
        }

        public static void CopyLatest(
            this IDictionary<FileInfo, FileInfo> sources,
            bool globalOverride = false)
        {
            foreach (var file in sources.Select((value, index) => new { index, value }))
            {
                try
                {
                    var source = file.value.Key;
                    var destination = file.value.Value;

                    var destinationDirectory = destination.DirectoryName;
                    if (destinationDirectory != null && !Directory.Exists(destinationDirectory))
                    {
                        Directory.CreateDirectory(destinationDirectory);
                    }

                    if (FileIsLocked(source))
                    {
                        continue;
                    }

                    if (destination.Exists)
                    {
                        if (FileIsLocked(destination))
                        {
                            continue;
                        }

                        if (globalOverride)
                        {
                            File.Delete(destination.FullName);
                        }
                        else
                        {
                            if (source.LastWriteTime <= destination.LastWriteTime && !destination.Length.Equals(0))
                            {
                                if (AppConfigManager.Instance().Property.AppConfigCore.IsDebugMode)
                                {
                                    LogManager.Instance().LogDebugMessage(
                                        "Destination file [" + file.index + "] [" + destination.FullName + "] already updated.");
                                }

                                continue;
                            }

                            if (source.LastWriteTime > destination.LastWriteTime || destination.Length.Equals(0))
                            {
                                File.Delete(destination.FullName);
                            }
                        }
                    }

                    File.Copy(source.FullName, destination.FullName);
                    LogManager.Instance().LogInfoMessage(
                        "Copied file [" + file.index + "] [" + destination.FullName + "].");
                }
                catch (Exception exception)
                {
                    LogManager.Instance().LogErrorMessage(exception.Message);
                }
            }
        }

        public static void CreateEmptyFile(this string filename)
        {
            var directory = Path.GetDirectoryName(filename);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (File.Exists(filename))
            {
                LogManager.Instance().LogInfoMessage("Not creating file [" + filename + "] since it already exists.");
                return;
            }

            File.Create(filename).Dispose();
            LogManager.Instance().LogInfoMessage("Created file [" + filename + "].");
        }

        public static void CreateFile(
            this string directory,
            string name,
            string extension)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var filename = Path.Combine(directory, name + "." + extension);
            File.Create(filename).Dispose();

            LogManager.Instance().LogDebugMessage("Created [" + filename + "].");
        }

        public static void CreateFile(
            this string directory,
            string name,
            string extension,
            string contents,
            bool append = false)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var filename = Path.Combine(directory, name + "." + extension);
            using (var file = new StreamWriter(filename, append))
            {
                file.WriteLine(contents);
            }

            LogManager.Instance().LogDebugMessage(
                string.Format(
                    "{0} [{1}].",
                    append ? "Appended to" : "Created",
                    filename));
        }

        public static void DeleteDirectory(
            this string targetDirectory,
            bool deleteTargetDirectory)
        {
            if (!Directory.Exists(targetDirectory))
            {
                return;
            }

            var directories = Directory.GetDirectories(targetDirectory);
            var files = Directory.GetFiles(targetDirectory);

            foreach (var file in files)
            {
                try
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
                catch (Exception exception)
                {
                    LogManager.Instance().LogWarningMessage(exception.Message);
                }
            }

            foreach (var directory in directories)
            {
                directory.DeleteDirectory(deleteTargetDirectory);
            }

            if (deleteTargetDirectory)
            {
                try
                {
                    Directory.Delete(targetDirectory, false);
                }
                catch (Exception exception)
                {
                    LogManager.Instance().LogWarningMessage(exception.Message);
                }
            }
        }

        public static void DeleteFile(this string file)
        {
            if (!File.Exists(file))
            {
                return;
            }

            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);

            LogManager.Instance().LogDebugMessage("Deleted file [" + file + "].");
        }

        public static void DeleteFiles(this List<FileInfo> files)
        {
            foreach (var file in files)
            {
                try
                {
                    file.Attributes = FileAttributes.Normal;
                    File.Delete(file.FullName);
                }
                catch (Exception exception)
                {
                    var errorMessage = "Could not delete file [" + file + "].  " + exception.Message;
                    LogManager.Instance().LogErrorMessage(errorMessage);
                }
            }
        }

        public static bool DirectoryOrFileExists(this string path)
        {
            return Directory.Exists(path) || File.Exists(path);
        }

        public static void EnsureDirectory(
            this string path,
            bool isDirectory = false)
        {
            if (isDirectory)
            {
                if (path == null || Directory.Exists(path))
                {
                    return;
                }

                Directory.CreateDirectory(path);
                LogManager.Instance().LogInfoMessage("Created directory [" + path + "].");
                return;
            }

            var directory = Path.GetDirectoryName(path);
            if (directory == null || Directory.Exists(directory))
            {
                return;
            }

            Directory.CreateDirectory(directory);
            LogManager.Instance().LogInfoMessage("Created directory [" + directory + "].");
        }

        public static List<string> FindAllFiles(this string directory)
        {
            var files = new List<string>();
            try
            {
                foreach (var f in Directory.GetFiles(directory))
                {
                    files.Add(f);
                }

                foreach (var d in Directory.GetDirectories(directory))
                {
                    files.AddRange(d.FindAllFiles());
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance().LogErrorMessage(exception.Message);
            }

            return files;
        }

        public static List<string> FindAllFiles(
            this string directory,
            string directorySearchPattern,
            string fileSearchPattern)
        {
            var files = new List<string>();
            try
            {
                if (string.IsNullOrEmpty(fileSearchPattern))
                {
                    foreach (var f in Directory.GetFiles(directory))
                    {
                        files.Add(f);
                    }
                }
                else
                {
                    foreach (var f in Directory.GetFiles(directory, fileSearchPattern))
                    {
                        files.Add(f);
                    }
                }

                if (string.IsNullOrEmpty(directorySearchPattern))
                {
                    foreach (var d in Directory.GetDirectories(directory))
                    {
                        files.AddRange(d.FindAllFiles(directorySearchPattern, fileSearchPattern));
                    }
                }
                else
                {
                    foreach (var d in Directory.GetDirectories(directory, directorySearchPattern))
                    {
                        files.AddRange(d.FindAllFiles(directorySearchPattern, fileSearchPattern));
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance().LogErrorMessage(exception.Message);
            }

            return files;
        }

        public static string GetExtension(this string filePath)
        {
            var extension = Path.GetExtension(filePath);
            if (extension == null)
            {
                return null;
            }

            extension = extension.Trim();
            return extension.StartsWith(".") ? extension.Substring(1) : extension;
        }

        public static List<FileInfo> GetFiles(
            this string path,
            params string[] extensions)
        {
            var list = new List<FileInfo>();
            foreach (var extension in extensions)
            {
                list.AddRange(new DirectoryInfo(path).GetFiles("*" + extension).Where(p =>
                    p.Extension.Equals(extension, StringComparison.CurrentCultureIgnoreCase))
                    .ToArray());
            }

            return list;
        }

        public static object IsDirectoryOrFile(this string path)
        {
            if (File.Exists(path))
            {
                return new FileInfo(path);
            }

            if (Directory.Exists(path))
            {
                return new DirectoryInfo(path);
            }

            LogManager.Instance().LogErrorMessage("[" + path + "] is not a valid file nor folder.");
            return null;
        }

        public static void RemoveFiles(
            this string directory,
            int filesToKeep,
            string searchPattern = null,
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var files =
                string.IsNullOrEmpty(searchPattern)
                    ? Directory.GetFiles(directory).OrderByDescending(name => name)
                        .ToList()
                    : Directory.GetFiles(directory, searchPattern, searchOption)
                        .OrderByDescending(name => name)
                        .ToList();
            var fileCount = files.Count;
            if (fileCount <= filesToKeep)
            {
                LogManager.Instance().LogInfoMessage(
                    "Not cleaning as the number of files [" +
                    fileCount +
                    "] is less than or equal to the number of files to keep [" +
                    filesToKeep + "].");
                return;
            }

            files.RemoveRange(0, filesToKeep);
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file)
                {
                    Attributes = FileAttributes.Normal
                };
                File.Delete(fileInfo.FullName);
            }
        }

        public static void RemoveDirectories(
            this string rootDirectory,
            string searchPattern,
            int directoriesToKeep)
        {
            var directories = Directory.GetDirectories(rootDirectory, "*" + searchPattern + "*")
                .OrderByDescending(name => name).ToList();
            var directoriesCount = directories.Count;
            if (directoriesCount <= directoriesToKeep)
            {
                return;
            }

            directories.RemoveRange(0, directoriesToKeep);
            foreach (var directory in directories)
            {
                DeleteDirectory(directory, true);
            }
        }

        public static void RemoveSubdirectories(
            this string rootDirectory,
            int subdirectoriesToKeep,
            string searchPattern = null,
            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var subdirectories =
                string.IsNullOrEmpty(searchPattern)
                    ? Directory.GetDirectories(rootDirectory).OrderByDescending(name => name)
                        .ToList()
                    : Directory.GetDirectories(rootDirectory, searchPattern, searchOption)
                        .OrderByDescending(name => name)
                        .ToList();
            var subdirectoriesCount = subdirectories.Count;
            if (subdirectoriesCount <= subdirectoriesToKeep)
            {
                LogManager.Instance().LogInfoMessage(
                    "Not cleaning as the number of directories [" +
                    subdirectoriesCount +
                    "] is less than or equal to the number of directories to keep [" +
                    subdirectoriesToKeep + "].");
                return;
            }

            subdirectories.RemoveRange(0, subdirectoriesToKeep);
            foreach (var subdirectory in subdirectories)
            {
                subdirectory.DeleteDirectory(true);
                LogManager.Instance().LogInfoMessage("Deleted directory [" + subdirectory + "].");
            }
        }

        public static void WriteLine(
            this string file,
            string line,
            bool append)
        {
            using (var streamWriter = new StreamWriter(file, append))
            {
                streamWriter.WriteLine(line);
            }
        }

        private static bool FileIsLocked(FileSystemInfo file)
        {
            try
            {
                using (var fileStream = File.Open(file.FullName, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    fileStream.Close();
                }

                return false;
            }
            catch (IOException exception)
            {
                var errorFileName = AssemblyType.Entry.GetAssemblyName();
                var logDirectory = AppConfigManager.Instance().Property.AppConfigCore.LogDirectory;
                var date = DateTime.Now.ToString(Constants.DateFormatIso8601);
                var errorFile = Path.Combine(logDirectory, errorFileName) + "_" + date + ".err";

                var streamWriter = new StreamWriter(errorFile, true);
                streamWriter.WriteLine(exception.Message);
                streamWriter.Close();
                LogManager.Instance().LogWarningMessage(exception.Message);
                return true;
            }
        }
    }
}