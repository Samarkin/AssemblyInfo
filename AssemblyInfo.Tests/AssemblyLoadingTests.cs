using NUnit.Framework;

namespace AssemblyInfo.Tests
{
	[TestFixture]
	public class AssemblyLoadingTests
	{
		[Test]
		public void X86AssemblyLoadTest()
		{
			var ass = new AssemblyProber(@"AssemblyInfo.Sample.v2.x86.dll");
			Assert.That(ass.CLRVersion.StartsWith("v2."));
			Assert.That(ass.Architecture, Is.EqualTo("X86"));
		}

		[Test]
		public void AnyCPUAssemblyLoadTest()
		{
			var ass = new AssemblyProber(@"AssemblyInfo.Sample.v4.MSIL.dll");
			Assert.That(ass.CLRVersion.StartsWith("v4."));
			Assert.That(ass.Architecture, Is.EqualTo("MSIL"));
		}

		[Test]
		public void X64AssemblyLoadTest()
		{
			var ass = new AssemblyProber(@"AssemblyInfo.Sample.v4.x64.dll");
			Assert.That(ass.CLRVersion.StartsWith("v4."));
			Assert.That(ass.Architecture, Is.EqualTo("Amd64"));
		}

		[Test]
		public void NullLoadTest()
		{
			var ass = new AssemblyProber(null);
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.ArgumentError));
		}

		[Test]
		public void EmptyStringLoadTest()
		{
			var ass = new AssemblyProber(@"");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.ArgumentError));
		}

		[Test]
		public void UnexistingFileLoadTest()
		{
			var ass = new AssemblyProber(@"UnexistingFile.dll");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.FileNotFound));
		}

		[Test]
		public void NonDllFileLoadTest()
		{
			var ass = new AssemblyProber(@"Samples\NonDllFile.txt");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.ReflectionError));
			ass = new AssemblyProber(@"Samples\NonDllFile.dll");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.ReflectionError));
		}

		[Test]
		public void NonAssemblyDllLoadTest()
		{
			var ass = new AssemblyProber(@"Samples\Native.dll");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.ReflectionError));
		}
	}
}
