// <copyright file="AssemblyAppRunner.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Framework.Core.AppConfig;
using Common.Framework.Core.Extensions;
using Common.Framework.Core.Logging;

namespace Common.Framework.Core.AppRunners
{
    public abstract class AssemblyAppRunner : AppRunner
    {
        protected AssemblyAppRunner(string assemblyFullPath)
        {
            Assembly = assemblyFullPath.LoadAssembly();
            Objects = new Dictionary<string, object>();
        }

        protected Assembly Assembly { get; set; }

        private Dictionary<string, object> Objects { get; }

        protected IEnumerable<string> GetCatalogue(
            string nameSpace)
        {
            var classes = new List<string>();
            var namespaceGroups = Assembly.GetNamespaces();
            foreach (var namespaceGroup in namespaceGroups)
            {
                if (namespaceGroup == null)
                {
                    continue;
                }

                if (namespaceGroup.Contains(nameSpace) || namespaceGroup.Contains(nameSpace + "."))
                {
                    classes.AddRange(Assembly.GetClasses(namespaceGroup));
                }
            }

            return classes;
        }

        protected object Instantiate<T>(
            string name,
            params object[] arguments)
        {
            try
            {
                return Assembly.Instantiate(name, arguments);
            }
            catch (Exception exception)
            {
                LogManager.Instance().LogWarningMessage(exception.Message);
                return default(T);
            }
        }

        protected virtual void RegisterObjects<T>(
            string nameSpace,
            bool useCollectionName,
            string switchName = null,
            params object[] arguments)
        {
            var appConfigParameters = AppConfigManager.Instance().Property.AppConfigParameters;
            var appConfigParametersSection = appConfigParameters.Keys;

            var classes = GetCatalogue(nameSpace);
            foreach (var construct in classes)
            {
                foreach (var collectionName in appConfigParametersSection.Keys)
                {
                    if (useCollectionName && !construct.Equals(collectionName))
                    {
                        continue;
                    }

                    try
                    {
                        var value =
                            string.IsNullOrEmpty(switchName)
                                ? appConfigParametersSection[collectionName].Collection[construct]
                                : appConfigParametersSection[collectionName].Collection[switchName];

                        bool tryParse;
                        if (bool.TryParse(value, out tryParse))
                        {
                            if (!Convert.ToBoolean(value))
                            {
                                continue;
                            }
                        }

                        var stringIsNullOrEmpty = string.IsNullOrEmpty(value);
                        if (stringIsNullOrEmpty)
                        {
                            continue;
                        }

                        var instance = Instantiate<T>(construct, arguments);
                        RegisterInstance(instance);
                        break;
                    }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch (Exception)
                    {
                        // do nothing with exception
                    }
                }
            }

            LogObjects();
        }

        protected void LogObjects()
        {
            LogManager.Instance().LogInfoMessage("Registered [" + Objects.Count + "] object(s):");
            foreach (var obj in Objects.Select((value, index) => new { index, value }))
            {
                LogManager.Instance().LogInfoMessage(
                    string.Format("    {0}: [{1}]", obj.index + 1, obj.value.Key));
            }
        }

        protected void RegisterInstance<T>(T instance)
        {
            if (instance.Equals(null))
            {
                return;
            }

            Objects.Add(instance.GetType().Name, instance);
        }

        protected void StartObjects(string methodName)
        {
            foreach (var obj in Objects.OrderBy(key => key.Key))
            {
                LogManager.Instance().LogInfoMessage("Starting [" + obj.Key + "].");

                try
                {
                    obj.Value.GetType().GetMethod(methodName).Invoke(obj.Value, null);
                }
                catch (Exception exception)
                {
                    LogManager.Instance().LogErrorMessage(exception.Message);
                    if (exception.InnerException != null)
                    {
                        LogManager.Instance().LogErrorMessage(exception.InnerException.Message);
                    }
                }

                LogManager.Instance().LogInfoMessage("Finished [" + obj.Key + "].");
            }
        }
    }
}