namespace CcNet.Labeller
{
    internal class VersionInformation
    {
        internal string GitTreeHash { get; set; }

        internal string GitParentHash { get; set; }

        internal string GitCommitHash { get; set; }

        internal int BuildCycleNumber { get; set; }

        internal int GitCheckinCount { get; set; }

        internal string AssemblySafeLabel { get; set; }

        internal string GitAbbreviatedCommitHash
        {
            get { return GitCommitHash.Substring(0, 7); }
        }

        internal string GitLabel
        {
            get { return string.Format("{0}-{1}", BuildCycleNumber, GitAbbreviatedCommitHash); }
        }
    }
}