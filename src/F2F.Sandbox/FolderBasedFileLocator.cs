using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace F2F.Sandbox
{
	/// <summary>
	/// File locator that locates files in a given directory.
	/// </summary>
	public class FolderBasedFileLocator : IFileLocator
	{
		private readonly string _baseDirectory;

		/// <summary>
		/// Creates a new instance
		/// </summary>
		/// <param name="baseDirectory"></param>
		public FolderBasedFileLocator(string baseDirectory)
		{
			if (String.IsNullOrEmpty(baseDirectory))
				throw new ArgumentException("baseDirectory is null or empty.", "baseDirectory");

			_baseDirectory = baseDirectory;
		}

		/// <summary>
		/// Query if 'fileName' exists.
		/// </summary>
		/// <param name="fileName">The file to locate.</param>
		/// <returns>true if file exists, false if it fails.</returns>
		public bool Exists(string fileName)
		{
			return File.Exists(MakeAbsolutePath(fileName));
		}

		/// <summary>
		/// See <see cref="F2F.Sandbox.IFileLocator.EnumeratePath(string)"/>
		/// </summary>
		public IEnumerable<string> EnumeratePath(string path)
		{
			string searchPath = Path.Combine(_baseDirectory, path);
			if (Directory.Exists(searchPath))
			{
				var result = new List<string>();
				var allFiles = Directory.GetFiles(searchPath, "*.*", SearchOption.AllDirectories);

				foreach (var file in allFiles)
				{
					result.Add(file.Remove(0, _baseDirectory.Length + 1));  // +1 to remove the trailing backslash
				}

				return result;
			}
			else if (File.Exists(searchPath))
			{
				return new[] { path };
			}
			else
			{
				return new string[0];
			}
		}

		/// <summary>
		/// See <see cref="F2F.Sandbox.IFileLocator.CopyTo(string, string)"/>
		/// </summary>
		public void CopyTo(string fileName, string destinationPath)
		{
			string sourcePath = MakeAbsolutePath(fileName);

			File.Move(sourcePath, destinationPath);
		}

		private string MakeAbsolutePath(string fileName)
		{
			return Path.Combine(_baseDirectory, fileName);
		}
	}
}