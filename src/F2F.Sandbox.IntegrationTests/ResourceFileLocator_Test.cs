using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Xunit.Extensions;
using FluentAssertions;
using AutoFixture;
using AutoFixture.AutoFakeItEasy;

namespace F2F.Sandbox.IntegrationTests
{
	public class ResourceFileLocator_Test : IDisposable
	{
		private string _tempDirectory;
		private IFixture Fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());

		public ResourceFileLocator_Test()
		{
			_tempDirectory = Path.Combine(Path.GetTempPath(), Fixture.Create<string>());
			if (!Directory.Exists(_tempDirectory))
			{
				Directory.CreateDirectory(_tempDirectory);
			}
		}

		public void Dispose()
		{
			if (Directory.Exists(_tempDirectory))
			{
				Directory.Delete(_tempDirectory, true);
			}
		}

		[Theory]
		[InlineData("testdata/test.txt")]
		[InlineData("testdata\\test.txt")]
		[InlineData("testdata\\test\\test.txt")]
		[InlineData("testdata/test/test.txt")]
		public void Exists_IfFileExists_ShouldReturnTrue(string fileName)
		{
			// Arrange

			var sut = new ResourceFileLocator(GetType());

			// Act && Assert
			sut.Exists(fileName).Should().BeTrue();
		}

		public static IEnumerable<object[]> FileNames
		{
			get
			{
				yield return new object[]
				{
					"",
					new[] { "testdata\\test.txt", "testdata\\test\\test.txt" }
				};
				yield return new object[]
				{
					".",
					new[] { "testdata\\test.txt", "testdata\\test\\test.txt" }
				};
				yield return new object[]
				{
					"testdata",
					new[] { "testdata\\test.txt", "testdata\\test\\test.txt" }
				};
				yield return new object[]
				{
					"testdata\\test\\",
					new[] { "testdata\\test\\test.txt" },
				};
				yield return new object[]
				{
					"testdata\\test",
					new[] { "testdata\\test\\test.txt" },
				};
				yield return new object[]
				{
					"testdata/test",
					new[] { "testdata\\test\\test.txt" },
				};
				yield return new object[]
				{
					"testdata/test/",
					new[] { "testdata\\test\\test.txt" },
				};
				yield return new object[]
				{
					"testdata/test\\",
					new[] { "testdata\\test\\test.txt" },
				};
				yield return new object[]
				{
					"testdata\\test/",
					new[] { "testdata\\test\\test.txt" },
				};
			}
		}

		[Theory]
		[MemberData("FileNames")]
		public void EnumeratePath_ShouldReturnExpectedFiles(string path, string[] expectedFileNames)
		{
			var sut = new ResourceFileLocator(GetType());

			// Act && Assert
			sut.EnumeratePath(path).Should().BeEquivalentTo(expectedFileNames);
		}

		[Theory]
		[InlineData("testdata\\test.txt", "dst.txt")]
		[InlineData("testdata/test.txt", "dst.txt")]
		[InlineData("testdata\\test\\test.txt", "dst.txt")]
		[InlineData("testdata/test/test.txt", "dst.txt")]
		[InlineData("testdata\\test/test.txt", "dst.txt")]
		[InlineData("testdata/test\\test.txt", "dst.txt")]
		public void CopyTo_ShouldCopySrcFileToDstFile(string src, string dst)
		{
			// Arrange
			var sut = new ResourceFileLocator(GetType());
			var dstFile = Path.Combine(_tempDirectory, dst);

			// Act
			sut.CopyTo(src, dstFile);

			// Assert
			File.Exists(Path.Combine(_tempDirectory, dstFile)).Should().BeTrue();
		}

		//[Theory]
		//[InlineData("src.txt", "src.txt")]
		//[InlineData("input/src.txt", "input/src.txt")]
		//public void CopyTo_WhenSrcIsSameAsDestination_ShouldThrow(string src, string dst)
		//{
		//	// Arrange
		//	var sut = new FolderBasedFileLocator(_directory);
		//	CreateFile(src);

		//	Action a = () => sut.CopyTo(src, dst);

		//	// Act && Assert
		//	a.ShouldThrow<IOException>();
		//}

		//private void CreateFile(string fileName)
		//{
		//	var filePath = Path.Combine(_directory, fileName);

		//	var directoryPath = Path.GetDirectoryName(filePath);
		//	if (!Directory.Exists(directoryPath))
		//	{
		//		Directory.CreateDirectory(directoryPath);
		//	}

		//	using (File.Create(filePath))
		//	{
		//	}
		//}
	}
}