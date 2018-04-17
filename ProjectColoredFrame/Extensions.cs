﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace ProjectColoredFrame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal static class Extensions
    {
        public static bool IsNullOrEmpty<T>(this IList<T> @this)
        {
            if (@this == null)
                return true;

            return @this.Count == 0;
        }
    }
}
