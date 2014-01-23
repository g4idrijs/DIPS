﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIPS.Processor.Plugin
{
    /// <summary>
    /// Represents errors that occur within the plugin system.
    /// </summary>
    public class PluginException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginException"/> class.
        /// </summary>
        public PluginException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginException"/> class with
        /// the reason for the Exception.
        /// </summary>
        /// <param name="reason">The reason for the error.</param>
        public PluginException( string reason )
            : base( reason )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginException"/> class with
        /// the reason for the Exception, and the inner Exception that occured.
        /// </summary>
        /// <param name="reason">The reason for the error.</param>
        /// <param name="innerException">The internal Exception that is the cause for this
        /// Exception.</param>
        public PluginException( string reason, Exception innerException )
            : base( reason, innerException )
        {
        }
    }
}
