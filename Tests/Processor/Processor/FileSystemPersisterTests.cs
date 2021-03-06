﻿using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DIPS.Processor.Persistence;
using DIPS.Processor;
using DIPS.Processor.Client;
using DIPS.Processor.Client.JobDeployment;
using System.Drawing;
using DIPS.Processor.Plugin;
using System.IO;
using System.Linq;

namespace DIPS.Tests.Processor
{
    /// <summary>
    /// Summary description for AppDataPersisterTests
    /// </summary>
    [TestClass]
    public class FileSystemPersisterTests
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the current ticket to use in tests.
        /// </summary>
        private JobTicket CurrentTicket
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current directory to use in testing.
        /// </summary>
        private string CurrentDirectory
        {
            get;
            set;
        }


        /// <summary>
        /// Initializer called before tests.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            JobRequest r = new JobRequest( new TestDefinition() );
            CurrentTicket = new JobTicket( r, new TestCancellation() );
            CurrentDirectory = Directory.GetCurrentDirectory() + "/TestOutput";
        }

        /// <summary>
        /// Cleaner called between tests.
        /// </summary>
        [TestCleanup]
        public void Clean()
        {
            // Delete all files in our test directory.
            if( Directory.Exists( CurrentDirectory ) )
            {
                Directory.Delete( CurrentDirectory, true );
            }
        }


        /// <summary>
        /// Tests constructing the persister with a null target path.
        /// </summary>
        [TestMethod]
        [ExpectedException( typeof( ArgumentException ) )]
        public void TestConstructor_NullPath()
        {
            FileSystemPersister persister = new FileSystemPersister( null );
        }

        /// <summary>
        /// Tests constructing the persister with an empty target path.
        /// </summary>
        [TestMethod]
        [ExpectedException( typeof( ArgumentException ) )]
        public void TestConstructor_EmptyPath()
        {
            FileSystemPersister persister = new FileSystemPersister( string.Empty );
        }

        /// <summary>
        /// Tests persisting an image with no identifier.
        /// </summary>
        [TestMethod]
        public void TestPersist_NoIdentifier()
        {
            FileSystemPersister persister = new FileSystemPersister( CurrentDirectory );
            Image toPersist = CurrentTicket.Request.Job.GetInputs().First().Input;
            object identifier = null;
            persister.Persist( CurrentTicket.JobID, toPersist, identifier );

            // A file should exist within this path.
            string path = string.Format( @"{0}/{{{1}}}", CurrentDirectory, CurrentTicket.JobID );
            bool fileExists = Directory.GetFiles( path ).Count() == 1;
            Assert.IsTrue( fileExists );

            // Persist again, should have 2 files
            persister.Persist( CurrentTicket.JobID, toPersist, identifier );

            fileExists = Directory.GetFiles( path ).Count() == 2;
            Assert.IsTrue( fileExists );
        }

        /// <summary>
        /// Tests persisting an image with an identifier.
        /// </summary>
        [TestMethod]
        public void TestPersist_WithIdentifier()
        {
            string id = "test";
            FileSystemPersister persister = new FileSystemPersister( CurrentDirectory );
            Image toPersist = CurrentTicket.Request.Job.GetInputs().First().Input;
            persister.Persist( CurrentTicket.JobID, toPersist, id );

            Guid theID = persister.GetPersistedIdentifier( CurrentTicket.JobID, id );
            string path = string.Format( @"{0}/{{{1}}}/{2}.png", CurrentDirectory, CurrentTicket.JobID, theID );
            bool fileExists = File.Exists( path );
            Assert.IsTrue( fileExists );
        }

        /// <summary>
        /// Tests loading results from an empty directory.
        /// </summary>
        [TestMethod]
        public void TestLoadAll_EmptyDirectory()
        {
            FileSystemPersister persister = new FileSystemPersister( CurrentDirectory );
            var results = persister.Load( CurrentTicket.JobID );

            Assert.IsNotNull( results );
            Assert.IsFalse( results.Any() );
        }

        /// <summary>
        /// Tests loading a persisted result.
        /// </summary>
        [TestMethod]
        public void TestLoadAll_OneResult()
        {
            string id = "test";
            FileSystemPersister persister = new FileSystemPersister( CurrentDirectory );
            Image toPersist = CurrentTicket.Request.Job.GetInputs().First().Input;
            persister.Persist( CurrentTicket.JobID, toPersist, id );
            var results = persister.Load( CurrentTicket.JobID );
            Guid persistedID = persister.GetPersistedIdentifier( CurrentTicket.JobID, id );

            Assert.IsTrue( results.Any() );
            Assert.AreEqual( 1, results.Count() );

            PersistedResult r = results.First();
            Assert.AreEqual( id, r.Identifier );
        }

        /// <summary>
        /// Tests loading a result by ID when there are no saved files
        /// </summary>
        [TestMethod]
        public void TestLoadByID_NoFiles()
        {
            FileSystemPersister persister = new FileSystemPersister( CurrentDirectory );
            var result = persister.Load( CurrentTicket.JobID, "test" );

            Assert.IsNull( result );
        }

        /// <summary>
        /// Tests loading a result by ID when a file with the ID does not exist.
        /// </summary>
        [TestMethod]
        public void TestLoadByID_OneFileMismatchingID()
        {
            string id = "test";
            FileSystemPersister persister = new FileSystemPersister( CurrentDirectory );
            Image toPersist = CurrentTicket.Request.Job.GetInputs().First().Input;
            persister.Persist( CurrentTicket.JobID, toPersist, id );
            var result = persister.Load( CurrentTicket.JobID, "unknown" );

            Assert.IsNull( result );
        }

        /// <summary>
        /// Tests loading a result by ID where there is a match.
        /// </summary>
        [TestMethod]
        public void TestLoadByID_OneFileMatchingID()
        {
            string id = "test";
            FileSystemPersister persister = new FileSystemPersister( CurrentDirectory );
            Image toPersist = CurrentTicket.Request.Job.GetInputs().First().Input;
            persister.Persist( CurrentTicket.JobID, toPersist, id );
            var result = persister.Load( CurrentTicket.JobID, id );

            Assert.IsNotNull( result );
            Assert.AreEqual( id, result.Identifier );
        }

        /// <summary>
        /// Tests deleting a result that has not been created
        /// </summary>
        [TestMethod]
        public void TestDeleteAll_DirectoryDoesntExist()
        {
            FileSystemPersister persister = new FileSystemPersister( CurrentDirectory );
            Guid testGuid = Guid.NewGuid();
            bool deleted = persister.Delete( testGuid );

            Assert.IsFalse( deleted );
        }

        /// <summary>
        /// Tests deleting all results within a directory
        /// </summary>
        [TestMethod]
        public void TestDeleteAll_DirectoryExists()
        {
            string id = "test";
            FileSystemPersister persister = new FileSystemPersister( CurrentDirectory );
            Image toPersist = CurrentTicket.Request.Job.GetInputs().First().Input;
            persister.Persist( CurrentTicket.JobID, toPersist, id );
            bool deleted = persister.Delete( CurrentTicket.JobID );

            string path = string.Format( @"{0}/{{{1}}}", persister.TargetDirectory, CurrentTicket.JobID );
            Assert.IsTrue( deleted );
            Assert.IsFalse( Directory.Exists( path ) );
        }

        /// <summary>
        /// Tests deleting a specific result that does not exist.
        /// </summary>
        [TestMethod]
        public void TestDeleteByID_FileDoesntExist()
        {
            FileSystemPersister persister = new FileSystemPersister( CurrentDirectory );
            Guid testGuid = Guid.NewGuid();
            bool deleted = persister.Delete( testGuid, "test" );

            Assert.IsFalse( deleted );
        }

        /// <summary>
        /// Tests deleting a specific result that exists
        /// </summary>
        [TestMethod]
        public void TestDeleteByID_FileExists()
        {
            string id = "test";
            FileSystemPersister persister = new FileSystemPersister( CurrentDirectory );
            Image toPersist = CurrentTicket.Request.Job.GetInputs().First().Input;
            persister.Persist( CurrentTicket.JobID, toPersist, id );
            Guid providedID = persister.GetPersistedIdentifier( CurrentTicket.JobID, id );

            string dir = string.Format( @"{0}/{{{1}}}", persister.TargetDirectory, CurrentTicket.JobID );
            string path = string.Format( @"{0}/{1}.png", dir, persister.GetPersistedIdentifier( CurrentTicket.JobID, id ) );
            Assert.IsTrue( File.Exists( path ) );

            bool deleted = persister.Delete( CurrentTicket.JobID, id );
            Assert.IsTrue( deleted );
            Assert.IsFalse( File.Exists( path ) );
        }


        class TestDefinition : IJobDefinition
        {
            public PipelineDefinition GetAlgorithms()
            {
                return new PipelineDefinition( new AlgorithmDefinition[] {
                    new AlgorithmDefinition("Test", null ) } );
            }

            public IEnumerable<JobInput> GetInputs()
            {
                return new[] { new JobInput( Image.FromFile( "img.bmp" ) ) };
            }
        }

        class TestAlgorithm : AlgorithmPlugin
        {
            public override void Run( object obj )
            {
                Output = Input;
            }
        }

        class TestCancellation : ITicketCancellationHandler
        {
            public ITicketCancellationHandler Successor
            {
                get;
                set;
            }

            public bool Handle( IJobTicket ticket )
            {
                return false;
            }
        }
    }
}
