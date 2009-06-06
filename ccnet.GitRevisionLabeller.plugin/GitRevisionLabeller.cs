using System;
using System.IO;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;

namespace CcNet.Labeller
{
    /// <summary>
    /// Generates the CCNet label. The resultant label is accessible from 
    /// apps such as MSBuild via the <c>$(CCNetLabel)</c> property , and NAnt via 
    /// the <c>${CCNetLabel}</c> property.
    /// </summary>
    /// <remarks>
    /// <para>
    /// In addition it sets the following environment variables:
    /// <ul>
    /// <li>CCNetGitCommitHash</li>
    /// <li>CCNetGitParentHash</li>
    /// <li>CCNetGitTreeHash</li>
    /// <li>CCNetBuildCycleNumber</li>
    /// <li>CCNetGitRepositoryPath</li>
    /// </ul>
    /// </para>
    /// </remarks>
    [ ReflectorType ( "gitRevisionLabeller" ) ]
    public class GitRevisionLabeller : ILabeller
    {
        #region Properties

        /// <summary>
        /// Gets or sets the path to the Git executable.
        /// </summary>
        /// <remarks>
        /// By default, the labeller checks the <c>PATH</c> environment variable.
        /// </remarks>
        /// <value>The executable.</value>
        [ ReflectorProperty ( "executable", Required = false ) ]
        public string Executable = "git";

        /// <summary>
        /// Gets or sets the path to the Git working directory.
        /// </summary>
        /// <value>The working directory.</value>
        [ ReflectorProperty ( "workingDirectory", Required = true ) ]
        public string WorkingDirectory;

        /// <summary>
        /// Gets or sets a value indicating whether the build number should 
        /// increment on failed build.
        /// </summary>
        /// <value><c>true</c> if the build number should increment on failure; otherwise, <c>false</c>.</value>
        [ ReflectorProperty ( "incrementOnFailure", Required = false ) ]
        public bool IncrementOnFailure { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Runs the task, given the specified <see cref="IIntegrationResult"/>, in the specified <see cref="IProject"/>.
        /// </summary>
        /// <param name="result">The label for the current build.</param>
        public void Run ( IIntegrationResult result )
        {
            result.Label = Generate ( result );
        }

        /// <summary>
        /// Returns the label to use for the current build.
        /// </summary>
        /// <param name="resultFromLastBuild">IntegrationResult from last build used to determine the next label.</param>
        /// <returns>The label for the current build.</returns>
        public string Generate ( IIntegrationResult resultFromLastBuild )
        {
            var repositoryFullPath = GetRepositoryFullPath ( resultFromLastBuild );

            var versionInformation = VersionAssembler.BuildVersion ( GetOriginHash ( resultFromLastBuild ),
                                                                     resultFromLastBuild,
                                                                     IncrementOnFailure );

            SetEnvironmentVariable ( "CCNetGitCommitHash", versionInformation.GitCommitHash );
            SetEnvironmentVariable ( "CCNetGitParentHash", versionInformation.GitParentHash );
            SetEnvironmentVariable ( "CCNetGitTreeHash", versionInformation.GitTreeHash );
            SetEnvironmentVariable ( "CCNetBuildCycleNumber", versionInformation.BuildCycleNumber );
            SetEnvironmentVariable ( "CCNetGitRepositoryPath", repositoryFullPath );

            return versionInformation.Label;
        }

        private static void SetEnvironmentVariable ( string name, int @value )
        {
            SetEnvironmentVariable ( name, @value.ToString ( ) );
        }

        private static void SetEnvironmentVariable ( string name, string @value )
        {
            Environment.SetEnvironmentVariable ( name, @value );

            Log.Info ( string.Format ( "{0}: {1}", name, @value ) );
        }

        private string GetOriginHash ( IIntegrationResult result )
        {
            var buffer = new ProcessArgumentBuilder ( );

            buffer.AddArgument ( "log" );
            buffer.AddArgument ( "origin/master" );
            buffer.AddArgument ( "--date-order" );
            buffer.AddArgument ( "-1" );
            buffer.AddArgument ( "--pretty=format:'%H%n%P%n%T'" );

            return
                new ProcessExecutor ( ).Execute ( NewProcessInfo ( buffer.ToString ( ), BaseWorkingDirectory ( result ) ) )
                    .StandardOutput.Trim ( ).Replace ( "'", "" );
        }

        private string GetRepositoryFullPath ( IIntegrationResult result )
        {
            var buffer = new ProcessArgumentBuilder ( );

            buffer.AddArgument ( "remote" );
            buffer.AddArgument ( "--verbose" );

            var pathInfo =
                new ProcessExecutor ( ).Execute ( NewProcessInfo ( buffer.ToString ( ), BaseWorkingDirectory ( result ) ) )
                    .StandardOutput.Trim ( ).Replace ( "origin", "" ).TrimStart ( );

            return Directory.Exists ( pathInfo ) ? Path.GetFullPath ( pathInfo ) : pathInfo;
        }

        private ProcessInfo NewProcessInfo ( string args, string dir )
        {
            Log.Info ( "Calling git " + args );

            var processInfo = new ProcessInfo ( Executable, args, dir ) { StreamEncoding = Encoding.UTF8 };

            return processInfo;
        }

        private string BaseWorkingDirectory ( IIntegrationResult result )
        {
            return result.BaseFromWorkingDirectory ( WorkingDirectory );
        }

        #endregion
    }
}