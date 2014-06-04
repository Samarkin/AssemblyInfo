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
			Assert.That(ass.Dependencies, Is.Not.Null);
		}

		[Test]
		public void AnyCPUAssemblyLoadTest()
		{
			var ass = new AssemblyProber(@"AssemblyInfo.Sample.v4.MSIL.dll");
			Assert.That(ass.CLRVersion.StartsWith("v4."));
			Assert.That(ass.Architecture, Is.EqualTo("MSIL"));
			Assert.That(ass.Dependencies, Is.Not.Null);
		}

		[Test]
		public void X64AssemblyLoadTest()
		{
			var ass = new AssemblyProber(@"AssemblyInfo.Sample.v4.x64.dll");
			Assert.That(ass.CLRVersion.StartsWith("v4."));
			Assert.That(ass.Architecture, Is.EqualTo("Amd64"));
			Assert.That(ass.Dependencies, Is.Not.Null);
		}

		[Test]
		public void NullLoadTest()
		{
			var ass = new AssemblyProber(null);
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.ArgumentError));
			Assert.That(ass.Dependencies, Is.Not.Null);
		}

		[Test]
		public void EmptyStringLoadTest()
		{
			var ass = new AssemblyProber(@"");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.ArgumentError));
			Assert.That(ass.Dependencies, Is.Not.Null);
		}

		[Test]
		public void UnexistingFileLoadTest()
		{
			var ass = new AssemblyProber(@"UnexistingFile.dll");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.FileNotFound));
			Assert.That(ass.Dependencies, Is.Not.Null);
		}

		[Test]
		public void NonDllFileLoadTest()
		{
			var ass = new AssemblyProber(@"Samples\NonDllFile.txt");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.ReflectionError));
			Assert.That(ass.Dependencies, Is.Not.Null);
			ass = new AssemblyProber(@"Samples\NonDllFile.dll");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.ReflectionError));
			Assert.That(ass.Dependencies, Is.Not.Null);
		}

		[Test]
		public void NonAssemblyDllLoadTest()
		{
			var ass = new AssemblyProber(@"Samples\Native.dll");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.ReflectionError));
			Assert.That(ass.Dependencies, Is.Not.Null);
		}
	}
}
