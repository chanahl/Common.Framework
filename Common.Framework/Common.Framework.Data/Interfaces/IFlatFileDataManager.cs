// <copyright file="IDataManager.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using Common.Framework.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Common.Framework.Data.Interfaces
{
    public interface IFlatFileDataManager : IDisposable
    {
        BuiltInType BuiltInType { get; set; }

        CollectionType CollectionType { get; set; }

        DelimiterType DelimiterType { get; set; }

        string Source { get; set; }

        char Delimiter { get; set; }

        dynamic Input { get; set; }

        StreamReader Stream { get; set; }

        TypeConverter Converter { get; set; }

        Type GenericType { get; set; }

        T LoadNonCollection<T>();

        T[] LoadArray1D<T>();

        T[,] LoadArray2D<T>();

        T[][] LoadArrayJagged<T>();

        List<T> LoadList<T>();
    }
}