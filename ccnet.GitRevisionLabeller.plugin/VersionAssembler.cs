using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;

namespace ccnet.GitRevisionLabeller.plugin
{
    internal static class VersionAssembler
    {
        internal static VersionInformation BuildVersion(string gitRevision, int gitCheckinCount, int major, int minor, IIntegrationResult resultFromLastBuild, bool incrementOnFailure)
        {
            var result = new VersionInformation();
            var hashes = gitRevision.Split('\n');
            result.GitCommitHash = hashes[0].Trim();
            result.GitParentHash = hashes[1].Trim();
            result.GitTreeHash = hashes[2].Trim();
            result.GitCheckinCount = gitCheckinCount;

            // major.minor.revision.rebuild
            const string format = "{0}.{1}.{2}.{3}";

            var rebuildNumber = 1;
            var previousVersion = resultFromLastBuild.LastSuccessfulIntegrationLabel;
            var parts = previousVersion.Split('.');

            int previousRebuildNumber;
            int.TryParse(parts[3], out previousRebuildNumber);

            int previousRevision;
            int.TryParse(parts[2], out previousRevision);

            if (((resultFromLastBuild.LastIntegrationStatus == IntegrationStatus.Success) || incrementOnFailure) && (gitCheckinCount == previousRevision))
            {
                rebuildNumber = previousRebuildNumber + 1;
            }

            result.BuildCycleNumber = rebuildNumber;
            var assemblySafeLabel = string.Format(format, major, minor, gitCheckinCount, rebuildNumber);
            result.AssemblySafeLabel = assemblySafeLabel;

            return result;
        }
    }
}