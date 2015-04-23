using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using Xunit;
using Xunit.Extensions;

namespace F2F.Sandbox.IntegrationTests
{
	public class FileSandbox_Test
	{
		private IFixture Fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());

		[Fact]
		public void Directory_ShouldReturnDirectoryInWindowsTempDirectory()
		{
			// Arrange
			var fileLocator = Fixture.Create<IFileLocator>();
			var sut = new FileSandbox(fileLocator);

			var tempDirectory = Path.GetTempPath();

			// Act
			var sandboxDirectory = sut.Directory;

			// Assert
			sandboxDirectory.StartsWith(tempDirectory).Should().BeTrue();
		}

		[Fact]
		public void ResolvePath_ShouldRetunrnPathToFileInSandbox()
		{
			// Arrange
			var fileLocator = Fixture.Create<IFileLocator>();
			var fileName = Fixture.Create<string>();
			var sut = new FileSandbox(fileLocator);

			// Act
			var path = sut.ResolvePath(fileName);

			// Assert
			path.Should().Be(Path.Combine(sut.Directory, fileName));
		}

		[Fact]
		public void ExistsFile_AfterCreateFile_ShouldReturnTrue()
		{
			// Arrange
			var fileLocator = Fixture.Create<EmptyFileLocator>();
			var testName = Fixture.Create<string>();

			var sut = new FileSandbox(fileLocator);
			sut.CreateFile(testName);

			// Act && Assert
			sut.ExistsFile(testName).Should().BeTrue();
		}

		[Fact]
		public void ExistsFile_WithoutCreateFile_ShouldReturnFalse()
		{
			// Arrange
			var fileLocator = Fixture.Create<EmptyFileLocator>();
			var testName = Fixture.Create<string>();

			var sut = new FileSandbox(fileLocator);

			// Act && Assert
			sut.ExistsFile(testName).Should().BeFalse();
		}

		[Fact]
		public void ExistsDirectory_AfterCreateDirectory_ShouldReturnTrue()
		{
			// Arrange
			var fileLocator = Fixture.Create<EmptyFileLocator>();
			var testName = Fixture.Create<string>();

			var sut = new FileSandbox(fileLocator);
			sut.CreateDirectory(testName);

			// Act && Assert
			sut.ExistsDirectory(testName).Should().BeTrue();
		}

		[Fact]
		public void ExistsDirectory_WithoutCreateDirectory_ShouldReturnFalse()
		{
			// Arrange
			var fileLocator = Fixture.Create<EmptyFileLocator>();
			var testName = Fixture.Create<string>();

			var sut = new FileSandbox(fileLocator);

			// Act && Assert
			sut.ExistsDirectory(testName).Should().BeFalse();
		}

		[Theory]
		[InlineData("")]
		[InlineData("testdirectory")]
		[InlineData("testdirectory/test")]
		[InlineData("testdirectory\\test")]
		[InlineData("testdirectory\\test\\abc")]
		public void CreateDirectory_ShouldCreateDirectory(string directoryName)
		{
			// Arrange
			var fileLocator = Fixture.Create<IFileLocator>();
			var sut = new FileSandbox(fileLocator);
			var expectedPath = Path.Combine(sut.Directory, directoryName);

			// Act
			var path = sut.CreateDirectory(directoryName);

			// Assert
			path.Should().Be(expectedPath);
			Directory.Exists(path).Should().BeTrue();
		}

		[Fact]
		public void CreateFile_ShouldCreateFile()
		{
			// Arrange
			var fileLocator = Fixture.Create<IFileLocator>();
			var testName = Fixture.Create<string>();
			var sut = new FileSandbox(fileLocator);

			// Act
			var createFilePath = sut.CreateFile(testName);

			// Assert
			File.Exists(createFilePath).Should().BeTrue();
		}

		[Fact]
		public void CreateTempFile_ShouldCreateFile()
		{
			// Arrange
			var fileLocator = Fixture.Create<IFileLocator>();
			var sut = new FileSandbox(fileLocator);

			// Act
			var createdFilePath = sut.CreateTempFile();

			// Assert
			File.Exists(createdFilePath).Should().BeTrue();
		}

		[Fact]
		public void GetTempFile_ShouldReturnRandomFilePath()
		{
			// Arrange
			var fileLocator = Fixture.Create<IFileLocator>();
			var sut = new FileSandbox(fileLocator);

			// Act
			var tempFile = sut.GetTempFile();

			// Assert
			tempFile.Should().NotBeEmpty();
			tempFile.StartsWith(sut.Directory);
		}

		[Fact]
		public void GetTempFile_ShouldNotCreateTempFile()
		{
			// Arrange
			var fileLocator = Fixture.Create<IFileLocator>();
			var sut = new FileSandbox(fileLocator);

			// Act
			var tempFile = sut.GetTempFile();

			// Assert
			File.Exists(tempFile).Should().BeFalse();
		}

		[Fact]
		public void ProvideFile_ShouldProvideFileToSandbox()
		{
			// Arrange
			var fileName = Fixture.Create<string>();
			var fileLocator = Fixture.Create<IFileLocator>();
			A.CallTo(() => fileLocator.Exists(fileName)).Returns(true);

			var sut = new FileSandbox(fileLocator);

			// Act && Assert
			sut.ProvideFile(fileName).ShouldBeEquivalentTo(sut.ResolvePath(fileName));
		}

		[Fact]
		public void ProvideFile_IfFileDoesNotExist_ShouldThrow()
		{
			// Arrange
			var fileName = Fixture.Create<string>();
			var fileLocator = Fixture.Create<IFileLocator>();
			A.CallTo(() => fileLocator.Exists(fileName)).Returns(false);

			var sut = new FileSandbox(fileLocator);

			// Act
			Action a = () => sut.ProvideFile(fileName);

			// Assert
			a.ShouldThrow<FileNotFoundException>();
		}

		[Theory]
		[InlineData("test")]
		[InlineData("test/test2")]
		[InlineData("test/test.txt")]
		public void ProvideDirectory_IfEnumeratePathReturnEmptyList_ShouldProvideEmptyDirectoryToSandbox(string directoryName)
		{
			// Arrange
			var fileLocator = Fixture.Create<IFileLocator>();
			A.CallTo(() => fileLocator.EnumeratePath(directoryName)).Returns(Enumerable.Empty<string>());

			var sut = new FileSandbox(fileLocator);

			var expectedDirectoryPath = Path.Combine(sut.Directory, directoryName);

			// Act && Assert
			sut.ProvideDirectory(directoryName).Should().Be(expectedDirectoryPath);
			Directory.Exists(expectedDirectoryPath).Should().BeTrue();
		}

		[Fact]
		public void ProvideDirectory_IfEnumeratePathReturnFilledList_ShouldProvideDirectoryWithFilesToSandbox()
		{
			// Arrange
			var directoryName = Fixture.Create<string>();
			var fileLocator = Fixture.Create<IFileLocator>();
			var files = Fixture.CreateMany<string>(3);

			A.CallTo(() => fileLocator.EnumeratePath(directoryName)).Returns(files);
			A.CallTo(() => fileLocator.Exists(A<string>.Ignored)).Returns(true);

			var sut = new FileSandbox(fileLocator);

			// Act
			sut.ProvideDirectory(directoryName);

			// Assert
			A.CallTo(() => fileLocator.CopyTo(A<string>.Ignored, A<string>.Ignored)).MustHaveHappened(Repeated.Exactly.Times(3));
		}

		[Fact]
		public void ProvideDirectory_ShouldCreateSubDirectoryWithIncludedFiles()
		{
			// Arrange
			var directoryName = "testdata";
			var fileLocator = Fixture.Create<IFileLocator>();
			var files = new[] { "testdata/test/test2.txt", "testdata/abc/sample.txt", "testdata/heinz.doc" };

			A.CallTo(() => fileLocator.EnumeratePath(directoryName)).Returns(files);
			A.CallTo(() => fileLocator.Exists(A<string>.Ignored)).Returns(true);

			var sut = new FileSandbox(fileLocator);

			// Act
			sut.ProvideDirectory(directoryName);

			// Assert
			Directory.Exists(Path.Combine(sut.Directory, "testdata/test")).Should().BeTrue();
			Directory.Exists(Path.Combine(sut.Directory, "testdata/abc")).Should().BeTrue();
		}

		[Fact]
		public void ProvideDirectory_IfFilesDoNotExist_ShouldThrow()
		{
			// Arrange
			var directoryName = Fixture.Create<string>();
			var fileLocator = Fixture.Create<IFileLocator>();
			var files = Fixture.CreateMany<string>(3);

			A.CallTo(() => fileLocator.EnumeratePath(directoryName)).Returns(files);
			A.CallTo(() => fileLocator.Exists(A<string>.Ignored)).Returns(false);

			var sut = new FileSandbox(fileLocator);

			// Act
			Action a = () => sut.ProvideDirectory(directoryName);

			// Assert
			a.ShouldThrow<FileNotFoundException>();
		}

		[Fact]
		public void ProvideDirectories_ShouldCreateDirectories()
		{
			// Arrange
			var fileLocator = Fixture.Create<IFileLocator>();
			var directories = Fixture.CreateMany<string>(3);
			var files = Fixture.CreateMany<string>(3);

			A.CallTo(() => fileLocator.EnumeratePath("")).Returns(files);
			A.CallTo(() => fileLocator.Exists(A<string>.Ignored)).Returns(true);

			var sut = new FileSandbox(fileLocator);

			// Act
			Action a = () => sut.CreateDirectories(directories.ToArray());
			a.Invoke();

			var directoryList = directories.ToList();

			// Assert
			Directory.Exists(Path.Combine(sut.Directory, directoryList[0])).Should().BeTrue();
			Directory.Exists(Path.Combine(sut.Directory, directoryList[1])).Should().BeTrue();
			Directory.Exists(Path.Combine(sut.Directory, directoryList[2])).Should().BeTrue();
		}

		[Fact]
		public void Dispose_ShouldDeleteSanboxDirectory()
		{
			// Arrange
			var fileLocator = Fixture.Create<IFileLocator>();
			string sandboxDirectory;
			using (var sut = new FileSandbox(fileLocator))
			{
				sandboxDirectory = sut.Directory;
				Directory.Exists(sandboxDirectory).Should().BeTrue();
			}

			Directory.Exists(sandboxDirectory).Should().BeFalse();
		}
	}
}