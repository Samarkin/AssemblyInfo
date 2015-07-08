using System;
using System.IO;
using AssemblyInfo.Common;
using NUnit.Framework;

namespace AssemblyInfo.Tests
{
	[TestFixture]
	public class AssemblyLoadingTests
	{
		[Test]
		public void X86AssemblyLoadTest()
		{
			var ass = new AssemblyProber(@"..\AssemblyInfo.Sample.v2.x86.dll");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.Success));
			Assert.That(ass.CLRVersion.StartsWith("v2."));
			Assert.That(ass.Architecture, Is.EqualTo("X86"));
			Assert.That(ass.GlobalAssemblyCache, Is.False);
			Assert.That(ass.Dependencies, Is.Not.Null);
		}

		[Test]
		public void AnyCPUAssemblyLoadTest()
		{
			var ass = new AssemblyProber(@"AssemblyInfo.Sample.v4.MSIL.dll");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.Success));
			Assert.That(ass.CLRVersion.StartsWith("v4."));
			Assert.That(ass.Architecture, Is.EqualTo("MSIL"));
			Assert.That(ass.GlobalAssemblyCache, Is.False);
			Assert.That(ass.Dependencies, Is.Not.Null);
		}

		[Test]
		public void X64AssemblyLoadTest()
		{
			var ass = new AssemblyProber(@"..\AssemblyInfo.Sample.v4.x64.dll");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.Success));
			Assert.That(ass.CLRVersion.StartsWith("v4."));
			Assert.That(ass.Architecture, Is.EqualTo("Amd64"));
			Assert.That(ass.GlobalAssemblyCache, Is.False);
			Assert.That(ass.Dependencies, Is.Not.Null);
		}

		[Test]
		public void NullLoadTest()
		{
			var ass = new AssemblyProber(null);
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.ArgumentError));
			Assert.That(ass.Dependencies, Is.Not.Null);
			Assert.That(ass.Debug, Is.Null);
		}

		[Test]
		public void AssemblyNameLoadingTest()
		{
			var ass = new AssemblyProber("AssemblyInfo.Sample.v4.MSIL", true);
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.Success));
			Assert.That(ass.GlobalAssemblyCache, Is.False);
		}

		[Test, Explicit] // TODO: refactor without hardcoded path
		public void LoadByAssemblyNameFromDiffDirectory()
		{
			var curDir = Environment.CurrentDirectory;
			Environment.CurrentDirectory = @"c:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\";
			try
			{
				var ass = new AssemblyProber("Microsoft.Data.Entity.Design, Version=11.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", true);
				Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.Success));
				Assert.That(ass.Location, Is.EqualTo(Path.Combine(Environment.CurrentDirectory, "Microsoft.Data.Entity.Design.dll")));
				Assert.That(ass.GlobalAssemblyCache, Is.False);
			}
			finally
			{
				Environment.CurrentDirectory = curDir;
			}
		}

		[Test]
		public void GacLoadingTest()
		{
			var ass = new AssemblyProber("System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", true);
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.Success));
			Assert.That(ass.GlobalAssemblyCache, Is.True);
		}

		[Test]
		public void EmptyStringLoadTest()
		{
			var ass = new AssemblyProber(@"");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.ArgumentError));
			Assert.That(ass.Dependencies, Is.Not.Null);
			Assert.That(ass.Debug, Is.Null);
		}

		[Test]
		public void UnexistingFileLoadTest()
		{
			var ass = new AssemblyProber(@"UnexistingFile.dll");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.FileNotFound));
			Assert.That(ass.Dependencies, Is.Not.Null);
			Assert.That(ass.Debug, Is.Null);

			ass = new AssemblyProber(@"System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.FileNotFound));
			Assert.That(ass.Dependencies, Is.Not.Null);
			Assert.That(ass.Debug, Is.Null);
		}

		[Test]
		public void NonDllFileLoadTest()
		{
			var ass = new AssemblyProber(@"Samples/NonDllFile.txt");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.ReflectionError));
			Assert.That(ass.Dependencies, Is.Not.Null);
			Assert.That(ass.Debug, Is.Null);

			ass = new AssemblyProber(@"Samples/NonDllFile.dll");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.ReflectionError));
			Assert.That(ass.Dependencies, Is.Not.Null);
			Assert.That(ass.Debug, Is.Null);
		}

		[Test]
		public void NonAssemblyDllLoadTest()
		{
			var ass = new AssemblyProber(@"Samples/Native.dll");
			Assert.That(ass.ErrorLevel, Is.EqualTo(ErrorLevel.ReflectionError));
			Assert.That(ass.Dependencies, Is.Not.Null);
			Assert.That(ass.Debug, Is.Null);
		}
	}
}
