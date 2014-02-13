﻿using DIPS.Processor.Client;
using DIPS.Processor.Plugin;
using DIPS.Processor.Registry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIPS.Processor
{
    /// <summary>
    /// Represents the plugin factory used to manufacture plugins using the
    /// types loaded from the registry.
    /// </summary>
    public class RegistryFactory : IPluginFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryFactory"/>
        /// class.
        /// </summary>
        public RegistryFactory()
        {
            ProcessPluginRepository repo = new ProcessPluginRepository();
            RegistryCache.Cache.Initialize( repo );
            _activator = new AlgorithmActivator( repo );
        }


        /// <summary>
        /// Manufactures an appropriate <see cref="AlgorithmPlugin"/> given the
        /// provided <see cref="AlgorithmDefinition"/>.
        /// </summary>
        /// <param name="algorithm">The <see cref="AlgorithmDefinition"/>
        /// descriving the algorithm to create.</param>
        /// <returns>An instance of <see cref="AlgorithmPlugin"/> represented
        /// by the given definition, or null if an algorithm cannot be built
        /// with the provided definition.</returns>
        public AlgorithmPlugin Manufacture( AlgorithmDefinition algorithm )
        {
            try
            {
                return _activator.Activate( algorithm );
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// Contains the private activator.
        /// </summary>
        private AlgorithmActivator _activator;
    }
}
