using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;

namespace ccnet.GitRevisionLabeller.plugin.Tests
{
    [TestFixture]
    public class RevisionLabellerTests
    {
        [Test]
        public void GitRevisionLabeller_Should_HandleUnknownLabel()
        {
            var lastIntegrationResult = new Mock<IIntegrationResult>();
            lastIntegrationResult.Setup(l => l.LastSuccessfulIntegrationLabel).Returns("UNKNOWN");

            var target = new GitRevisionLabeller();
            target.Run(lastIntegrationResult.Object);
        }
    }
}
