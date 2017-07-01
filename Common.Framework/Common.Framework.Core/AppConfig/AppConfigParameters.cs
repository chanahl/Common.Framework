// <copyright file="AppConfigParameters.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using Common.Framework.Core.Collections.Generic;
using Common.Framework.Core.Enums;
using Common.Framework.Core.Extensions;
using Common.Framework.Core.Logging;

namespace Common.Framework.Core.AppConfig
{
    public class AppConfigParameters
    {
        public AppConfigParameters()
        {
            Keys = GetAppConfigParameters();
        }

        public Dictionary<string, DictionaryCollection> Keys { get; set; }

        public void LogParameters()
        {
            LogManager.Instance().LogInfoMessage(typeof(AppConfigParameters).Name.Separate() + ":");

            foreach (var appConfigParameterKey in Keys)
            {
                LogManager.Instance().LogInfoMessage(string.Format("  {0}:", appConfigParameterKey.Key.Separate()));

                foreach (var collection in appConfigParameterKey.Value.Collection)
                {
                    var collectionValue = collection.Value;
                    var collectionValues = collectionValue.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    if (collectionValues.Length > 1)
                    {
                        LogManager.Instance().LogInfoMessage(string.Format("    {0}:", collection.Key));
                        var cleanCollectionValues = collectionValues.Select(values => values.Trim());
                        foreach (var cleanCollectionValue in cleanCollectionValues)
                        {
                            LogManager.Instance().LogInfoMessage(string.Format("      [{0}]", cleanCollectionValue));
                        }
                    }
                    else
                    {
                        LogManager.Instance().LogInfoMessage(
                            string.Format("    {0}: [{1}]", collection.Key, collection.Value.Trim()));
                    }
                }
            }
        }

        private static Dictionary<string, DictionaryCollection> GetAppConfigParameters()
        {
            var appConfigParameters = new Dictionary<string, DictionaryCollection>(StringComparer.OrdinalIgnoreCase);
            var appConfigParametersSection =
                ConfigurationManager.GetSection(Constants.AppConfigParametersSection) as NameValueCollection;
            if (appConfigParametersSection != null)
            {
                foreach (var key in appConfigParametersSection.AllKeys)
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    var values = appConfigParametersSection.GetValues(key).FirstOrDefault();
                    if (values == null)
                    {
                        continue;
                    }

                    var parameterValues = DictionaryCollection.CleanValues(values, DelimiterType.SemiColon);
                    var parametersCollection = CreateCollection(parameterValues);
                    appConfigParameters.Add(key, parametersCollection);
                }
            }

            return appConfigParameters;
        }

        private static DictionaryCollection CreateCollection(IEnumerable<string> parameters)
        {
            var dictionaryCollection = new DictionaryCollection();
            foreach (var parameter in parameters)
            {
                var split = parameter.Split(new[] { '=' }, 2);
                if (split[0].StartsWith("#") || split[0].StartsWith("_"))
                {
                    continue;
                }

                dictionaryCollection.Collection[split[0]] = split[1].Trim();
            }

            return dictionaryCollection;
        }
    }
}