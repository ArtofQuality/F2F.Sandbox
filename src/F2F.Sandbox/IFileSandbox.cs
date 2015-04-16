using System;
using System.Collections.Generic;
using System.Text;

namespace F2F.Sandbox
{
	/// <summary>Interface for a file sandbox.</summary>
	public interface IFileSandbox : IDisposable
	{
		/// <summary>Gets the path of the sandbox directory.</summary>
		/// <value>The directory path.</value>
		string Directory { get; }

		/// <summary>
		/// The current file locator
		/// </summary>
		IFileLocator FileLocator { get; }

		/// <summary>Resolves a path in the sandbox for the given file.</summary>
		/// <param name="fileName">The file to resolve.</param>
		/// <returns>The resolved file path.</returns>
		string ResolvePath(string fileName);

		/// <summary>Checks whether the given file exists in the sandbox.</summary>
		/// <param name="fileName">The file name.</param>
		/// <returns>true if it exists, false if not.</returns>
		bool ExistsFile(string fileName);

		/// <summary>Checks whether the given directory exists in the sandbox.</summary>
		/// <param name="fileName">The file name.</param>
		/// <returns>true if it exists, false if not.</returns>
		bool ExistsDirectory(string fileName);

		/// <summary>Creates an empty file and returns the path to the file.</summary>
		/// <returns>The path to the new file.</returns>
		string CreateFile(string fileName);

		/// <summary>Creates an empty temporary file and returns the path to the file.</summary>
		/// <returns>The path to the new file.</returns>
		string CreateTempFile();

		/// <summary>Creates an empty temporary file with the given extension and returns the path to the file.</summary>
		/// <param name="fileExtension">The file extension.</param>
		/// <returns>The path to the new file.</returns>
		string CreateTempFile(string fileExtension);

		/// <summary>Creates a directory in the sandbox, if it does not exist yet.</summary>
		/// <param name="directoryName">The directory to create.</param>
		/// <returns>The path to the created directory in the sandbox.</returns>
		string CreateDirectory(string directoryName);

		/// <summary>Creates the given directories and returns the absolute path to the directories in the sandbox.</summary>
		/// <param name="directories">The directories to create.</param>
		/// <returns>The resolved file paths.</returns>
		IEnumerable<string> CreateDirectories(params string[] directories);

		/// <summary>Returns the name of a temporary file in the sandbox. The file will not be created!</summary>
		/// <returns>The path to the new file.</returns>
		string GetTempFile();

		/// <summary>Returns the name of a temporary file with the given extension in the sandbox. The file will not be created!</summary>
		/// <param name="fileExtension">The file extension.</param>
		/// <returns>The path to the new file.</returns>
		string GetTempFile(string fileExtension);

		/// <summary>Locates the given fileName using the current IFileLocator and provides the file in the sandbox.</summary>
		/// <param name="fileName">The file to provide.</param>
		/// <returns>The resolved file path.</returns>
		string ProvideFile(string fileName);

		/// <summary>Locates the given files using the current IFileLocator and provides the files in the sandbox.</summary>
		/// <param name="fileNames">The files to provide.</param>
		/// <returns>The resolved file paths.</returns>
		IEnumerable<string> ProvideFiles(params string[] fileNames);

		/// <summary>
		/// Locates the given directory using the current IFileLocator and provides the complete directory
		/// (including all subdirectories and contained files) in the sandbox. The absolute path to the directory
		/// in the sandbox will be returned. The directoryName will be used relative to the root of the
		/// file locator,
		/// i.e. if the directoryName is
		///     dir1\subdir1\subdir2
		/// then the directory in the sandbox will be
		///     sandboxdir\dir1\subdir1\subdir2
		/// </summary>
		/// <param name="directoryName">The directory to provide.</param>
		/// <returns>The absolute path to the provided directory</returns>
		string ProvideDirectory(string directoryName);

		/// <summary>
		/// Locates the given directories using the current IFileLocator and provides the complete directories
		/// (including all subdirectories and contained files) in the sandbox. The absolute path to the directories
		/// in the sandbox will be returned. The directoryNames will be used relative to the root of the
		/// file locator,
		/// i.e. if the directoryName is
		///     dir1\subdir1\subdir2
		/// then the directory in the sandbox will be
		///     sandboxdir\dir1\subdir1\subdir2
		/// </summary>
		/// <param name="directoryNames">The directories to provide.</param>
		/// <returns>The absolute paths to the provided directories</returns>
		IEnumerable<string> ProvideDirectories(params string[] directoryNames);
	}
}