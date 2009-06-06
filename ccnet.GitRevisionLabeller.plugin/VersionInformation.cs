namespace CcNet.Labeller
{
    internal class VersionInformation
    {
        private const char c_delimiter = '-';

        public VersionInformation ( )
        {
        }

        public VersionInformation ( string label )
        {
            var parts = label.Split ( c_delimiter );

            BuildCycleNumber = parts.Length == 2 ? int.Parse ( parts [ 0 ] ) : default ( int );
            GitCommitHash = parts.Length == 2 ? parts [ 1 ] : parts [ 0 ];
        }

        public VersionInformation ( string gitRevision, int buildNumber )
        {
            var hashes = gitRevision.Split ( '\n' );
            GitCommitHash = hashes [ 0 ];
            GitParentHash = hashes [ 1 ];
            GitTreeHash = hashes [ 2 ];
            BuildCycleNumber = buildNumber;
        }

        internal string GitTreeHash { get; set; }

        internal string GitParentHash { get; set; }

        internal string GitCommitHash { get; set; }

        internal string GitAbbreviatedCommitHash
        {
            get { return GitCommitHash.Substring ( 0, 7 ); }
        }

        internal int BuildCycleNumber { get; set; }

        internal string Label
        {
            get { return string.Format ( "{0}{2}{1}", BuildCycleNumber, GitAbbreviatedCommitHash, c_delimiter ); }
        }
    }
}