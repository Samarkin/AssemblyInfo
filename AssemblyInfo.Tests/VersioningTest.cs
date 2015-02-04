using AssemblyInfo.Common;
using NUnit.Framework;

namespace AssemblyInfo.Tests
{
	[TestFixture]
	class VersioningTest
	{
		[Test]
		public void VersionsTest()
		{
			var ass = new AssemblyProber(@"AssemblyInfo.Sample.v4.x64.dll");
			Assert.That(ass.AssemblyVersion, Is.EqualTo("1.2.3.4"));
			Assert.That(ass.FileVersion, Is.EqualTo("2.3.4.5"));
			Assert.That(ass.ProductVersion, Is.EqualTo("2.3.4.5"));
		}
	}
}
