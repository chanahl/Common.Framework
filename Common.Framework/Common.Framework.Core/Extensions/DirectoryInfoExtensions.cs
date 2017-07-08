// <copyright file="DirectoryInfoExtensions.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Common.Framework.Core.Extensions
{
    public static class DirectoryInfoExtensions
    {
        public static FileInfo GetRelativeFile(this DirectoryInfo directoryInfo, string relativePath)
        {
            return new FileInfo(GetPath(directoryInfo, relativePath));
        }

        public static DirectoryInfo GetRelativeDirectory(this DirectoryInfo directoryInfo, string relativePath)
        {
            return new DirectoryInfo(GetPath(directoryInfo, relativePath));
        }

        public static string GetPath(this DirectoryInfo directoryInfo, string relativePath)
        {
            return NormalizePath(string.IsNullOrWhiteSpace(relativePath)
                ? directoryInfo.FullName
                : CombinePath(directoryInfo.FullName, relativePath));
        }

        private static string NormalizePath(string sourcePath)
        {
            var path = sourcePath.Trim().Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            if (path.Length < 2)
            {
                return path;
            }

            if (path[1] == ':' && (path.Length == 2 || path[2] == Path.DirectorySeparatorChar))
            {
                return Path.GetFullPath(path);
            }

            if (path[0] == Path.DirectorySeparatorChar)
            {
                var fullPath = Path.GetFullPath(path);
                return fullPath[1] == Path.DirectorySeparatorChar ? fullPath : fullPath.Substring(2);
            }

            var tokens = path.Split(
                new[] { Path.DirectorySeparatorChar },
                StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).Where(s => s.Length != 1 || s[0] != '.').ToList();

            if (tokens.Count == 0)
            {
                throw new FormatException(sourcePath);
            }

            var stack = new Stack<string>();
            var previous = "..";
            foreach (var token in tokens)
            {
                if (IsStepUp(token))
                {
                    if (IsStepUp(previous))
                    {
                        stack.Push(token);
                    }
                    else
                    {
                        stack.Pop();
                    }
                }
                else
                {
                    stack.Push(token);
                }

                previous = token;
            }

            var p = string.Join(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture), stack.ToArray().Reverse());
            return p;
        }

        private static string NormalizePathSeparators(string path)
        {
            return path.Trim().Replace(
                Path.AltDirectorySeparatorChar,
                Path.DirectorySeparatorChar).TrimEnd(new[] { Path.DirectorySeparatorChar });
        }

        private static string CombinePath(
            string path1,
            string path2)
        {
            var p1 = NormalizePathSeparators(path1);
            var p2 = NormalizePathSeparators(path2);
            var p = Path.Combine(p1 + Path.DirectorySeparatorChar, p2);
            return p;
        }

        private static bool IsStepUp(string token)
        {
            return token.Length == 2 && token[0] == '.' && token[1] == '.';
        }
    }
}