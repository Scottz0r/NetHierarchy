﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetHierarchy
{
    internal static class Helpers
    {
        /// <summary>
        /// Extension method to help with null checking.
        /// </summary>
        internal static void ArgumentNullCheck(this object obj, string name)
        {
            if (obj == null)
                throw new ArgumentNullException(name);
        }
    }
}
