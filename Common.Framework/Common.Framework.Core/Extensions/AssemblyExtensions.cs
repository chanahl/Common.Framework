// <copyright file="AssemblyExtensions.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Common.Framework.Core.Enums;

namespace Common.Framework.Core.Extensions
{
    public static class AssemblyExtensions
    {
        public static string GetAssemblyFileVersion(this string assemblyInfoFile)
        {
            const string AssemblyVersionPattern =
                @"\[assembly\: AssemblyVersion\(""(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})""\)\]";

            var match = Regex.Match(File.ReadAllText(assemblyInfoFile), AssemblyVersionPattern);

            if (!match.Success)
            {
                return string.Empty;
            }

            var major = match.Groups[1].Value;
            var minor = match.Groups[2].Value;
            var build = match.Groups[3].Value;
            var revision = match.Groups[4].Value;
            return string.Format("{0}.{1}.{2}.{3}", major, minor, build, revision);
        }

        public static string GetAssemblyName(this AssemblyType assemblyType)
        {
            switch (assemblyType)
            {
                case AssemblyType.Calling:
                    return Assembly.GetCallingAssembly().GetName().Name;
                case AssemblyType.Entry:
                    return Assembly.GetEntryAssembly().GetName().Name;
                case AssemblyType.Executing:
                    return Assembly.GetExecutingAssembly().GetName().Name;
                default:
                    return string.Empty;
            }
        }

        public static string GetAssemblyVersion(this string assemblyInfoFile)
        {
            if (!File.Exists(assemblyInfoFile))
            {
                return string.Empty;
            }

            var fileLines = File.ReadAllLines(assemblyInfoFile);
            var versionInfoLines = fileLines.Where(t => t.Contains("[assembly: AssemblyVersion"));
            foreach (var item in versionInfoLines)
            {
                var version = item.Substring(item.IndexOf('(') + 2, item.LastIndexOf(')') - item.IndexOf('(') - 3);
                if (!string.IsNullOrEmpty(version))
                {
                    return version;
                }
            }

            return string.Empty;
        }

        public static List<string> GetClasses(
            this Assembly assembly,
            string nameSpace)
        {
            var classes = RetrieveClasses(assembly, nameSpace);
            return classes;
        }

        public static List<string> GetClasses(
            this AssemblyType assemblyType,
            string nameSpace)
        {
            var assembly = GetAssemblyType(assemblyType);
            var classes = RetrieveClasses(assembly, nameSpace);
            return classes;
        }

        public static IEnumerable<Type> GetDerivedTypesFor(
            this AssemblyType assemblyType,
            Type baseType)
        {
            var assembly = GetAssemblyType(assemblyType);

            return assembly.GetTypes()
                .Where(baseType.IsAssignableFrom)
                .Where(t => baseType != t);
        }

        public static List<string> GetNamespaces(this Assembly assembly)
        {
            var namespaces = RetrieveNamespaces(assembly);
            return namespaces;
        }

        public static List<string> GetNamespaces(this AssemblyType assemblyType)
        {
            var assembly = GetAssemblyType(assemblyType);
            var namespaces = RetrieveNamespaces(assembly);
            return namespaces;
        }

        public static object Instantiate(
            this Assembly assembly,
            string className,
            params object[] arguments)
        {
            var instance = CreateInstance(assembly, className, arguments);
            return instance;
        }

        public static object Instantiate(
            this AssemblyType assemblyType,
            string className,
            params object[] arguments)
        {
            var assembly = GetAssemblyType(assemblyType);
            var instance = CreateInstance(assembly, className, arguments);
            return instance;
        }

        public static Assembly LoadAssembly(this string assemblyFullPath)
        {
            if (!File.Exists(assemblyFullPath))
            {
                var errorMessage = "Could not find assembly [" + assemblyFullPath + "].";
                throw new FileNotFoundException(errorMessage);
            }

            try
            {
                ////return Assembly.LoadFile(assemblyFullPath);
                var content = File.ReadAllBytes(assemblyFullPath);
                return AppDomain.CurrentDomain.Load(content);
            }
            catch (FileLoadException fileLoadException)
            {
                throw new FileNotFoundException(fileLoadException.Message);
            }
        }

        private static object CreateInstance(
            this Assembly assembly,
            string className,
            params object[] arguments)
        {
            var type = assembly.GetTypes().First(t => t.Name == className);

            try
            {
                var hasDefaultConstructor = HasDefaultConstructor(type);
                return
                    arguments.Any() && !hasDefaultConstructor
                        ? Activator.CreateInstance(type, arguments)
                        : Activator.CreateInstance(type);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static Assembly GetAssemblyType(AssemblyType assemblyType)
        {
            switch (assemblyType)
            {
                case AssemblyType.Calling:
                    return Assembly.GetCallingAssembly();
                case AssemblyType.Entry:
                    return Assembly.GetEntryAssembly();
                case AssemblyType.Executing:
                    return Assembly.GetExecutingAssembly();
                default:
                    return null;
            }
        }

        private static bool HasDefaultConstructor(Type type)
        {
            return type.GetConstructor(Type.EmptyTypes) != null;
        }

        private static List<string> RetrieveClasses(
            Assembly assembly,
            string nameSpace)
        {
            var namespaces = new List<string>();
            foreach (var type in assembly.GetTypes())
            {
                if (type.Namespace == nameSpace)
                {
                    namespaces.Add(type.Name);
                }
            }

            var classes = new List<string>();
            foreach (var classname in namespaces)
            {
                classes.Add(classname);
            }

            return classes;
        }

        private static List<string> RetrieveNamespaces(Assembly assembly)
        {
            var namespaces = new List<string>();
            var groups = assembly
                .GetTypes()
                .GroupBy(type => type.Namespace)
                .OrderBy(name => name.Key);
            foreach (var group in groups)
            {
                namespaces.Add(group.Key);
            }

            return namespaces;
        }
    }
}