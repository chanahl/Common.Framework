// <copyright file="FlatFileDataManager.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Common.Framework.Core.Enums;
using Common.Framework.Core.Extensions;
using Common.Framework.Core.Logging;
using Common.Framework.Data.Interfaces;

namespace Common.Framework.Data.Managers
{
    public class FlatFileDataManager : IDisposable, IFlatFileDataManager
    {
        private bool _disposed;

        public FlatFileDataManager(
            BuiltInType builtInType,
            CollectionType collectionType,
            DelimiterType delimiterType,
            string source)
        {
            BuiltInType = builtInType;
            CollectionType = collectionType;
            DelimiterType = delimiterType;
            Source = source;
            Initialize();
        }

        public BuiltInType BuiltInType { get; set; }

        public CollectionType CollectionType { get; set; }

        public DelimiterType DelimiterType { get; set; }

        public string Source { get; set; }

        public char Delimiter { get; set; }

        public dynamic Input { get; set; }

        public StreamReader Stream { get; set; }

        public TypeConverter Converter { get; set; }

        public Type GenericType { get; set; }

        public T LoadNonCollection<T>()
        {
            string line;
            var input = string.Empty;
            
            while ((line = Stream.ReadLine()) != null)
            {
                input += line;
            }

            Dispose();

            return (T)Converter.ConvertFromString(input);
        }

        public T[] LoadArray1D<T>()
        {
            // Get dimensions of data.
            var length = 0;
            while (Stream.ReadLine() != null)
            {
                length++;
            }
            
            var t = new T[length];

            // Reset stream and populate array.
            Stream.BaseStream.Seek(0, SeekOrigin.Begin);
            string line;
            var i = 0;
            while ((line = Stream.ReadLine()) != null)
            {
                t[i] = (T)Converter.ConvertFromString(line);
                i++;
            }

            Dispose();

            return t;
        }

        public T[,] LoadArray2D<T>()
        {
            // Get dimensions of data.
            string line;
            string[] values;
            var rowCount = 0;
            var colCount = 0;
            var previousColumnCount = colCount;
            while ((line = Stream.ReadLine()) != null)
            {
                values = line.Split(Delimiter);
                colCount = values.Length;

                // Data integrity check.
                if ((rowCount > 0) && (previousColumnCount != colCount))
                {
                    var warningMessage =
                        "Input data from [" + Source + "] is incomplete in row [" + rowCount + "].";
                    LogManager.Instance().LogWarningMessage(warningMessage);
                }

                rowCount++;
                previousColumnCount = colCount;
            }

            var t = new T[rowCount, colCount];

            // Reset stream and populate array.
            Stream.BaseStream.Seek(0, SeekOrigin.Begin);
            var row = 0;
            while ((line = Stream.ReadLine()) != null)
            {
                values = line.Split(Delimiter);
                for (var col = 0; col < values.Length; col++)
                {
                    t[row, col] = (T)Converter.ConvertFromString(values[col]);
                }

                row++;
            }

            Dispose();

            return t;
        }

        public T[][] LoadArrayJagged<T>()
        {
            // Get dimensions of data.
            var rowCount = 0;
            while (Stream.ReadLine() != null)
            {
                rowCount++;
            }
            
            var t = new T[rowCount][];

            // Reset stream and populate array.
            Stream.BaseStream.Seek(0, SeekOrigin.Begin);
            string line;
            var row = 0;
            while ((line = Stream.ReadLine()) != null)
            {
                var values = line.Split(Delimiter);
                t[row] = new T[values.Length];
                for (var col = 0; col < values.Length; col++)
                {
                    t[row][col] = (T)Converter.ConvertFromString(values[col]);
                }

                row++;
            }

            Dispose();

            return t;
        }

        public List<T> LoadList<T>()
        {
            var t = new List<T>();

            string line;
            while ((line = Stream.ReadLine()) != null)
            {
                var values = line.Split(Delimiter);
                t.AddRange(values.Select(value => (T)Converter.ConvertFromString(value)));
            }

            Dispose();

            return t;
        }

        public void Close()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            Close();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (Stream != null)
                {
                    Stream.Dispose();
                }
            }

            _disposed = true;
        }

        private void Initialize()
        {
            InitializeStreamReader();
            InitializeTypeConverter();
            InitializeDelimiterType();
            InitializeCollectionType();
        }

        private void InitializeStreamReader()
        {
            if (Source.Length == 0 || string.IsNullOrEmpty(Source))
            {
                const string ErrorMessage = "Provided source is empty.  Cannot open stream.";
                LogManager.Instance().LogErrorMessage(ErrorMessage);
                throw new ApplicationException(ErrorMessage);
            }

            Stream = new StreamReader(Environment.ExpandEnvironmentVariables(Source));
        }

        private void InitializeTypeConverter()
        {
            switch (BuiltInType)
            {
                case BuiltInType.Boolean:
                    Converter = TypeDescriptor.GetConverter(typeof(bool));
                    GenericType = typeof(bool);
                    break;
                case BuiltInType.Byte:
                    Converter = TypeDescriptor.GetConverter(typeof(byte));
                    GenericType = typeof(byte);
                    break;
                case BuiltInType.Character:
                    Converter = TypeDescriptor.GetConverter(typeof(char));
                    GenericType = typeof(char);
                    break;
                case BuiltInType.Decimal:
                    Converter = TypeDescriptor.GetConverter(typeof(decimal));
                    GenericType = typeof(decimal);
                    break;
                case BuiltInType.Double:
                    Converter = TypeDescriptor.GetConverter(typeof(double));
                    GenericType = typeof(double);
                    break;
                case BuiltInType.Float:
                    Converter = TypeDescriptor.GetConverter(typeof(float));
                    GenericType = typeof(float);
                    break;
                case BuiltInType.Integer:
                    Converter = TypeDescriptor.GetConverter(typeof(int));
                    GenericType = typeof(int);
                    break;
                case BuiltInType.Long:
                    Converter = TypeDescriptor.GetConverter(typeof(long));
                    GenericType = typeof(long);
                    break;
                case BuiltInType.Object:
                    Converter = TypeDescriptor.GetConverter(typeof(object));
                    GenericType = typeof(object);
                    break;
                case BuiltInType.Short:
                    Converter = TypeDescriptor.GetConverter(typeof(short));
                    GenericType = typeof(short);
                    break;
                case BuiltInType.SignedByte:
                    Converter = TypeDescriptor.GetConverter(typeof(sbyte));
                    GenericType = typeof(sbyte);
                    break;
                case BuiltInType.String:
                    Converter = TypeDescriptor.GetConverter(typeof(string));
                    GenericType = typeof(string);
                    break;
                case BuiltInType.UnsignedInteger:
                    Converter = TypeDescriptor.GetConverter(typeof(uint));
                    GenericType = typeof(uint);
                    break;
                case BuiltInType.UnsignedLong:
                    Converter = TypeDescriptor.GetConverter(typeof(ulong));
                    GenericType = typeof(ulong);
                    break;
                case BuiltInType.UnsignedShort:
                    Converter = TypeDescriptor.GetConverter(typeof(ushort));
                    GenericType = typeof(ushort);
                    break;
                default:
                    var errorMessage =
                        "Built-in type [" + BuiltInType + "] not supported.";
                    LogManager.Instance().LogErrorMessage(errorMessage);
                    throw new ApplicationException(errorMessage);
            }
        }

        private void InitializeDelimiterType()
        {
            Delimiter = Convert.ToChar(DelimiterType.GetDescription());
        }

        private void InitializeCollectionType()
        {
            var t = typeof(IFlatFileDataManager);
            MethodInfo method;
            MethodInfo genericMethod;
            switch (CollectionType)
            {
                case CollectionType.NonCollection:
                    method = t.GetMethod("LoadNonCollection");
                    genericMethod = method.MakeGenericMethod(GenericType);
                    Input = genericMethod.Invoke(this, null);
                    break;
                case CollectionType.Array1D:
                    method = t.GetMethod("LoadArray1D");
                    genericMethod = method.MakeGenericMethod(GenericType);
                    Input = genericMethod.Invoke(this, null);
                    break;
                case CollectionType.Array2D:
                    method = t.GetMethod("LoadArray2D");
                    genericMethod = method.MakeGenericMethod(GenericType);
                    Input = genericMethod.Invoke(this, null);
                    break;
                case CollectionType.ArrayJagged:
                    method = t.GetMethod("LoadArrayJagged");
                    genericMethod = method.MakeGenericMethod(GenericType);
                    Input = genericMethod.Invoke(this, null);
                    break;
                case CollectionType.List:
                    method = t.GetMethod("LoadList");
                    genericMethod = method.MakeGenericMethod(GenericType);
                    Input = genericMethod.Invoke(this, null);
                    break;
                default:
                    var errorMessage =
                        "Collection type [" + CollectionType + "] not supported.";
                    LogManager.Instance().LogErrorMessage(errorMessage);
                    throw new ApplicationException(errorMessage);
            }
        }
    }
}