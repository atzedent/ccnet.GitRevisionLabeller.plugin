using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;

namespace CcNet.Labeller
{
    internal static class VersionAssembler
    {
        internal static VersionInformation BuildVersion ( string gitRevision, IIntegrationResult resultFromLastBuild,
                                                        bool incrementOnFailure )
        {
            var buildNumber = 1;
            var previousVersion = new VersionInformation ( resultFromLastBuild.LastSuccessfulIntegrationLabel );
            var abbreviatedGitRevision = new VersionInformation ( gitRevision ).GitAbbreviatedCommitHash;

            if ( ( ( resultFromLastBuild.LastIntegrationStatus == IntegrationStatus.Success ) ||
                   incrementOnFailure ) && ( abbreviatedGitRevision == previousVersion.GitAbbreviatedCommitHash ) )
            {
                buildNumber = previousVersion.BuildCycleNumber + 1;
            }

            return new VersionInformation ( gitRevision, buildNumber );
        }
    }
}