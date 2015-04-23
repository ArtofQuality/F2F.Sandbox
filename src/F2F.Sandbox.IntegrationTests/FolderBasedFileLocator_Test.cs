using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F2F.Sandbox;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using Xunit;
using Xunit.Extensions;

namespace F2F.Sandbox.IntegrationTests
{
	public class FolderBasedFileLocator_Test : IDisposable
	{
		private string _directory;
		private IFixture Fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());

		public FolderBasedFileLocator_Test()
		{
			_directory = Path.Combine(Path.GetTempPath(), Fixture.Create<string>());
			if (!Directory.Exists(_directory))
			{
				Directory.CreateDirectory(_directory);
			}
		}

		public void Dispose()
		{
			if (Directory.Exists(_directory))
			{
				Directory.Delete(_directory, true);
			}
		}

		[Fact]
		public void Exists_IfNoFileExists_ShouldReturnFalse()
		{
			// Arrange
			var fileName = Fixture.Create<string>();
			var directoryName = Fixture.Create<string>();

			var sut = new FolderBasedFileLocator(directoryName);

			// Act && Assert
			sut.Exists(fileName).Should().BeFalse();
		}

		public static IEnumerable<object[]> FileNames
		{
			get
			{
				yield return new object[]
				{
					"",
					new string[] { },
					new string[] { }
				};
				yield return new object[]
				{
					"",
					new[] { "test/test.txt" },
					new[] { "test\\test.txt" }
				};
				yield return new object[]
				{
					".",
					new[] { "test/test.txt" },
					new[] { ".\\test\\test.txt" }
				};
				yield return new object[]
				{
					"test",
					new[] { "test/test.txt" },
					new[] { "test\\test.txt" }
				};
				yield return new object[]
				{
					"testFailed",
					new[] { "test/test.txt" },
					new string[] { }
				};
				yield return new object[]
				{
					"testFailed",
					new[] { "test/test.txt" },
					new string[] { }
				};
				yield return new object[]
				{
					"test/test",
					new[] { "test/test/test.txt" },
					new string[] { "test\\test\\test.txt" }
				};
				yield return new object[]
				{
					"test/test",
					new[] { "test/test.txt", "test/test/test.txt" },
					new string[] { "test\\test\\test.txt" }
				};
				yield return new object[]
				{
					"test",
					new[] { "test/test.txt", "test/test/test.txt" },
					new string[] { "test\\test.txt", "test\\test\\test.txt"  }
				};
				yield return new object[]
				{
					"test/test",
					new[] { "test/test.txt", "test/test/test.txt", "test/test2/test.txt" },
					new string[] { "test\\test\\test.txt"  }
				};

				yield return new object[]
				{
					"test/test2",
					new[] { "test/test.txt", "test/test/test.txt", "test/test2/test.txt" },
					new string[] { "test\\test2\\test.txt"  }
				};
				yield return new object[]
				{
					"",
					new[] { "testc.txt", "test/test.txt", "test/test/test.txt", "test/test2/test.txt" },
					new string[] { "testc.txt", "test\\test.txt", "test\\test\\test.txt", "test\\test2\\test.txt" }
				};
			}
		}

		[Theory]
		[PropertyData("FileNames")]
		public void EnumeratePath_ShouldReturnExpectedFiles(string directoryName, string[] fileNames, string[] expectedFileNames)
		{
			// Arrange
			fileNames.ToList().ForEach(f => CreateFile(f));

			var sut = new FolderBasedFileLocator(_directory);

			// Act && Assert
			sut.EnumeratePath(directoryName).Should().BeEquivalentTo(expectedFileNames);
		}

		[Theory]
		[InlineData("src.txt", "dst.txt")]
		[InlineData("input/src.txt", "output/dst.txt")]
		public void CopyTo_ShouldCopySrcFileToDstFile(string src, string dst)
		{
			// Arrange
			var sut = new FolderBasedFileLocator(_directory);
			CreateFile(src);

			// Act
			sut.CopyTo(src, dst);

			// Assert
			File.Exists(Path.Combine(_directory, dst)).Should().BeTrue();
			File.Exists(Path.Combine(_directory, src)).Should().BeTrue();
		}

		[Theory]
		[InlineData("src.txt", "src.txt")]
		[InlineData("input/src.txt", "input/src.txt")]
		public void CopyTo_WhenSrcIsSameAsDestination_ShouldThrow(string src, string dst)
		{
			// Arrange
			var sut = new FolderBasedFileLocator(_directory);
			CreateFile(src);

			Action a = () => sut.CopyTo(src, dst);

			// Act && Assert
			a.ShouldThrow<IOException>();
		}

		private void CreateFile(string fileName)
		{
			var filePath = Path.Combine(_directory, fileName);

			var directoryPath = Path.GetDirectoryName(filePath);
			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}

			using (File.Create(filePath))
			{
			}
		}
	}
}