using System;
using System.Collections.Generic;
using System.Text;

namespace F2F.Sandbox
{
	/// <summary>Implementation of a file sandbox.</summary>
	public class FileSandbox : IFileSandbox
	{
		private bool _disposed = false;
		private readonly IFileLocator _fileLocator;
		private readonly string _sandboxPath;

		/// <summary>
		/// Create a new file sandbox.
		/// </summary>
		/// <param name="fileLocator">The file locator which resolves files.</param>
		public FileSandbox(IFileLocator fileLocator)
		{
			_fileLocator = fileLocator;

			_sandboxPath = System.IO.Path.Combine(GetSandboxBasePath(), Guid.NewGuid().ToString());

			System.IO.Directory.CreateDirectory(_sandboxPath);
		}

		/// <summary>
		/// Destructor
		/// </summary>
		~FileSandbox()
		{
			Dispose(false);
		}

		/// <see cref="IFileSandbox.Directory"/>
		public string Directory
		{
			get { return _sandboxPath; }
		}

		/// <see cref="IFileSandbox.FileLocator"/>
		public IFileLocator FileLocator
		{
			get { return _fileLocator; }
		}

		/// <see cref="IFileSandbox.ResolvePath(string)"/>
		public string ResolvePath(string fileName)
		{
			return System.IO.Path.Combine(_sandboxPath, fileName);
		}

		/// <see cref="IFileSandbox.ExistsFile(string)"/>
		public bool ExistsFile(string fileName)
		{
			return System.IO.File.Exists(ResolvePath(fileName));
		}

		/// <see cref="IFileSandbox.ExistsDirectory(string)"/>
		public bool ExistsDirectory(string fileName)
		{
			return System.IO.Directory.Exists(ResolvePath(fileName));
		}

		/// <see cref="IFileSandbox.CreateFile(string)"/>
		public string CreateFile(string fileName)
		{
			string sandboxFile = ResolvePath(fileName);

			CreateFileInSandbox(sandboxFile);

			return sandboxFile;
		}

		/// <see cref="IFileSandbox.CreateTempFile()"/>
		public string CreateTempFile()
		{
			string sandboxFile = GetTempFile();

			CreateFileInSandbox(sandboxFile);

			return sandboxFile;
		}

		/// <see cref="IFileSandbox.CreateTempFile(string)"/>
		public string CreateTempFile(string fileExtension)
		{
			string sandboxFile = GetTempFile(fileExtension);

			CreateFileInSandbox(sandboxFile);

			return sandboxFile;
		}

		/// <see cref="IFileSandbox.CreateDirectory(string)"/>
		public string CreateDirectory(string directoryName)
		{
			string createdDirectory = ResolvePath(directoryName);

			CreateDirectoryIfNotExists(createdDirectory);

			return createdDirectory;
		}

		/// <see cref="IFileSandbox.CreateDirectories(string[])"/>
		public IEnumerable<string> CreateDirectories(params string[] directories)
		{
			IList<string> createdDirectories = new List<string>();
			foreach (string directory in directories)
			{
				createdDirectories.Add(ProvideDirectory(directory));
			}
			return createdDirectories;
		}

		/// <see cref="IFileSandbox.GetTempFile()"/>
		public string GetTempFile()
		{
			return System.IO.Path.Combine(_sandboxPath, Guid.NewGuid().ToString());
		}

		/// <see cref="IFileSandbox.GetTempFile(string)"/>
		public string GetTempFile(string fileExtension)
		{
			return System.IO.Path.ChangeExtension(GetTempFile(), fileExtension);
		}

		/// <see cref="IFileSandbox.ProvideFile(string)"/>
		public string ProvideFile(string fileName)
		{
			if (!_fileLocator.Exists(fileName))
			{
				throw new System.IO.FileNotFoundException(String.Format("file {0} not found in sandbox", fileName));
			}

			string sandboxFile = ResolvePath(fileName);

			CreateDirectoryIfNotExists(System.IO.Path.GetDirectoryName(sandboxFile));

			if (sandboxFile != fileName)
			{
				_fileLocator.CopyTo(fileName, sandboxFile);
			}

			return sandboxFile;
		}

		/// <see cref="IFileSandbox.ProvideFiles(string[])"/>
		public IEnumerable<string> ProvideFiles(params string[] fileNames)
		{
			IList<string> resolvedFiles = new List<string>();
			foreach (string fileName in fileNames)
			{
				resolvedFiles.Add(ProvideFile(fileName));
			}
			return resolvedFiles;
		}

		/// <see cref="IFileSandbox.ProvideDirectory(string)"/>
		public string ProvideDirectory(string directoryName)
		{
			var sandboxDirectory = ResolvePath(directoryName);

			CreateDirectoryIfNotExists(sandboxDirectory);

			foreach (var file in _fileLocator.EnumeratePath(directoryName))
			{
				ProvideFile(file);
			}

			return sandboxDirectory;
		}

		/// <see cref="IFileSandbox.ProvideDirectories(string[])"/>
		public IEnumerable<string> ProvideDirectories(params string[] directoryNames)
		{
			IList<string> resolvedDirectories = new List<string>();
			foreach (string directory in directoryNames)
			{
				resolvedDirectories.Add(ProvideDirectory(directory));
			}
			return resolvedDirectories;
		}

		private static string GetSandboxBasePath()
		{
			return System.IO.Path.GetTempPath();
		}

		private void CreateDirectoryIfNotExists(string sandboxDirectory)
		{
			if (!System.IO.Directory.Exists(sandboxDirectory))
			{
				System.IO.Directory.CreateDirectory(sandboxDirectory);
			}
		}

		private void CreateFileInSandbox(string sandboxFile)
		{
			CreateDirectoryIfNotExists(System.IO.Path.GetDirectoryName(sandboxFile));

			using (System.IO.File.Create(sandboxFile))
			{
			}
		}

		/// <summary>
		/// Finalize the file sandbox.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				ReleaseUnmanagedResources();

				_disposed = true;
			}
		}

		private void ReleaseUnmanagedResources()
		{
			var di = new System.IO.DirectoryInfo(_sandboxPath);
			if (di.Exists)
			{
				di.Delete(true);
			}
		}
	}
}