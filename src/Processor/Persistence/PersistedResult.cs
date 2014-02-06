﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIPS.Processor.Persistence
{
    /// <summary>
    /// Represents a previously persisted result.
    /// </summary>
    public class PersistedResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersistedResult"/>
        /// class.
        /// </summary>
        /// <param name="img">The <see cref="Image"/> that was persisted.</param>
        /// <param name="identifier">The identifier originally provided by the
        /// client.</param>
        /// <exception cref="ArgumentNullException">img is null</exception>
        /// <exception cref="ArgumentException">identifier is null or
        /// empty</exception>
        public PersistedResult( Image img, string identifier )
        {
            if( img == null )
            {
                throw new ArgumentNullException( "img" );
            }

            if( string.IsNullOrEmpty( identifier ) )
            {
                throw new ArgumentException( "identifier" );
            }

            Image = img;
            RestoredIdentifier = identifier;
            PersisterGeneratedIdentifier = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistedResult"/>
        /// class.
        /// </summary>
        /// <param name="img">The <see cref="Image"/> that was persisted.</param>
        /// <param name="sequenceIdentifier">The numeric ordering of the
        /// image provided by the persister, due to the client not providing
        /// an identifier.</param>
        /// <exception cref="ArgumentNullException">img is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">sequenceIdentifier
        /// is less than 0</exception>
        public PersistedResult( Image img, int sequenceIdentifier )
        {
            if( img == null )
            {
                throw new ArgumentNullException( "img" );
            }

            if( sequenceIdentifier < 0 )
            {
                throw new ArgumentOutOfRangeException(
                    "Sequence identifier must be greater than or equal to zero." );
            }

            Image = img;
            RestoredIdentifier = sequenceIdentifier.ToString();
            PersisterGeneratedIdentifier = true;
        }


        /// <summary>
        /// Gets the <see cref="Image"/> loaded from the persister.
        /// </summary>
        public Image Image
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the identifier provided to the image.
        /// </summary>
        public string RestoredIdentifier
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the restored identifier was
        /// automatically generated by the persister.
        /// </summary>
        public bool PersisterGeneratedIdentifier
        {
            get;
            private set;
        }
    }
}
