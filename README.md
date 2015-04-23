
## FileSandbox ##

Everyone of us certainly knows legacy code which is hard to test because it uses direct file system access. We can start to abstract every access to the file system so we are able to write unit tests. But there are situations where we want to test the *real* work with files on the hard disc. In this case we don't talk about unit tests anymore and our tests will of course take more time to execute. 

What happens if you want to test code with real files? You might have problems with tests which change your test data, so you have to copy them before. Depending on your environment (think about continuous integration systems) there will be differences between relative and absolute paths. Executing tests in parallel can also be a problem when working with the same test data. Furthermore you have to think about cleaning up your environment after test execution, e.g. deleting temporary files etc.

A FileSandbox creates a temporary directory on your local environment for each test case. With a given `FileLocator` you can automatically resolve files from e.g. Assembly resources or a network share (to name a few). Then you get the absolute path to the file independent from your environment and the actual source of the file.

For a unit testing framework this means you can execute tests even in parallel because every test will create it's own temporary directory. After test execution the temporary directory will be automatically deleted.

**Warning** - there is still one caveat here:

If you debug your tests and stop the debugger in the middle of the test or your SUT doesn't close file handles, the cleanup code won't execute and you will have trash in your temp folder then. We already have some ideas on how to solve this as well, but it's not implemented yet.

*NuGet package*:
* F2F.Sandbox

The `FileSandbox`can be used standalone or you can use the [TestFixture](https://github.com/ArtofQuality/F2F.Testing#testfixture) for the [FileSandboxFeature](https://github.com/ArtofQuality/F2F.Testing#filesandboxfeature) for one of the currently supported unit testing frameworks.

### EmptyFileLocator ###

That's the default for the FileSandbox. If you don't have to provide files because you only create new files or directories, then use this FileLocator.

```csharp
var sandbox = new FileSandbox();

// Creates an empty temporary file and returns the path to the file.
var tempFile = sandbox.CreateTempFile();
```

### ResourceFileLocator ###

With `ResourceFileLocator` you need no special deployment of test data files because everything needed is inside your test assembly. Just add files to your test assembly as *Embedded Resource* using the file properties. That's very useful for e.g. small text files.

```csharp
var sandbox = new FileSandbox(new ResourceFileLocator(GetType()));

// Locates the given file using the ResourceFileLocator, provides the
// file in the sandbox and returns the absolute path.
var absolutePath = sandbox.ProvideFile("testdata/test.txt");
```

The path `testdata/test.txt` is the path to your file inside your test assembly. The `ResourceFileLocator` will resolve it by using the given type in it's constructor. The resource name will be built by using type's namespace and the path like this: `type.Namespace + "." + Escape(path)`.

With `sandbox.Dispose()` the `FileSandbox` will delete the temporary directory and all it's containing files. Note that this will fail if your SUT didn't close file handles.

### FolderBasedFileLocator ###

The `FolderBasedFileLocator` is useful for large test data files which can be used by everyone using a network share. The FileSandbox will copy each required file to the local temporary directory. Additionally there is a `TargetFolderBasedFileLocator` which automatically points to your execution directory.

```csharp
var sandbox = new FileSandbox(new FolderBasedFileLocator(@"\\nas.local\testdata"));

// Locates the given file using the FolderBasedFileLocator, provides the
// file in the sandbox and returns the absolute path.
var absolutePath = sandbox.ProvideFile("sample/test.txt");
```

This example will copy the file *\\\nas.local\testdata\sample\test.txt* to a local temporary directory. The returned absolute path will be something like *%TEMP%\\&lt;GUID&gt;\sample\test.txt*.
