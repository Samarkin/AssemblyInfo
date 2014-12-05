using System.Linq;
using NUnit.Framework;

namespace AssemblyInfo.Tests
{
	[TestFixture]
	public class ReferenceTests
	{
		[Test]
		public void DepdendencyTest()
		{
			var ass = new AssemblyProber(@"AssemblyInfo.Sample.v4.x64.dll");
			Assert.That(ass.Dependencies, Is.Not.Null);
			Assert.That(ass.Dependencies.Count(), Is.AtLeast(1));
			Assert.That(ass.Dependencies.Any(s => s.DisplayName.StartsWith("AssemblyInfo.Sample.v4.MSIL")));
		}
	}
}
