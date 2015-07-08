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
			var ass = new AssemblyProber(@"..\AssemblyInfo.Sample.v4.x64.dll");
			Assert.That(ass.AssemblyVersion, Is.EqualTo("1.2.3.4"));
			Assert.That(ass.FileVersion, Is.EqualTo("2.3.4.5"));
			Assert.That(ass.ProductVersion, Is.EqualTo("2.3.4.5"));
		}

		[Test]
		public void DebugTest()
		{
			var ass = new AssemblyProber(@"..\AssemblyInfo.Sample.v2.x86.dll");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.Success));
			Assert.That(ass.Debug, Is.True);
		}

		[Test]
		public void ReleaseTest()
		{
			var ass = new AssemblyProber(@"..\AssemblyInfo.Sample.v4.x64.dll");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.Success));
			Assert.That(ass.Debug, Is.False);
		}

		[Test]
		public void DebugReleaseTest()
		{
			var ass = new AssemblyProber(@"AssemblyInfo.Sample.v4.MSIL.dll");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.Success));
#if DEBUG
			Assert.That(ass.Debug, Is.True);
#else
			Assert.That(ass.Debug, Is.False);
#endif
		}
	}
}
