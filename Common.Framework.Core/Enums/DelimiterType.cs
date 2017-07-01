// <copyright file="DelimiterType.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System.ComponentModel;

namespace Common.Framework.Core.Enums
{
    public enum DelimiterType : short
    {
        [Description(":")]
        Colon,

        [Description(",")]
        Comma,

        [Description("-")]
        Hyphen,

        [Description("\n")]
        Newline,

        [Description("\0")]
        None,

        [Description(".")]
        Period,

        [Description("|")]
        Pipe,

        [Description(";")]
        SemiColon,

        [Description(" ")]
        Space
    }
}