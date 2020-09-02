using System;
using System.IO;
using System.Collections.Generic;

namespace xcopy
{
    /// <summary>
    /// Create with a source and destination folder
    /// then call RecursiveCopy() to copy the files
    /// from one folder to another.
    /// </summary>
    class XCopy
    {
        private DirectoryInfo source, dest;

        /// <summary>
        /// Copy source directory's files to destination folder
        /// with default settings.
        /// Destination does not need to exist.
        /// Files will NOT be overwritten.
        /// </summary>
        /// <param name="source">A source folder to copy files from</param>
        /// <param name="dest">A destination folder to copy files to</param>
        public XCopy(string source, string dest)
        {
            this.source = new DirectoryInfo(source);
            this.dest = new DirectoryInfo(dest);
        }

        /// <summary>
        /// Recursively copy source files to destination maintaining directory
        /// structure and using the options set at the instance level.
        /// </summary>
        public void RecursiveCopy()
        {
            // walk the path and get a list of every directory we'll need to copy
            List<DirectoryInfo> sourceDirs = GetDirsRecursively(source);
            // DebugFileList(sourceDirs);

            // Directory.CreateDirectory(dest.FullName);
            foreach (DirectoryInfo srcFolder in sourceDirs)
            {
                CreateAndCopyDestination(srcFolder);
            }
        }

        /// <summary>
        /// Give a source folder, determines its relative path to the 
        /// root source folder, then creates a similar folder at the 
        /// destination. After that, it copies all the files from the
        /// source folder to the destination folder.
        /// </summary>
        /// <param name="srcFolder">A source folder (should be subfolder of the root source folder)</param>
        private void CreateAndCopyDestination(DirectoryInfo srcFolder)
        {
            string srcRelPath = GetRelativePath(source, srcFolder);
            string destSubFolder = Path.Combine(dest.FullName, srcRelPath);
            DirectoryInfo destSubInfo = new DirectoryInfo(destSubFolder);

            // if directory doesn't exist, let the user know we're making it
            if (!destSubInfo.Exists)
            {
                Console.WriteLine(String.Format("Creating {0}", dest.FullName));
            }
            Directory.CreateDirectory(destSubFolder);
            foreach (FileInfo srcFile in srcFolder.GetFiles())
            {
                string destFilePath = Path.Combine(destSubFolder, srcFile.Name);
                Console.WriteLine(destFilePath);
                try
                {
                    File.Copy(srcFile.FullName, destFilePath);
                }
                catch (IOException)
                {
                    Console.Error.WriteLine(
                        String.Format("Already exists: '{0}'", destFilePath)
                    );
                }
            }
        }

        /// <summary>
        /// For debugging purposes. Just prints each file found in each folder
        /// of all the directories in the list given.
        /// </summary>
        /// <param name="sourceDirs">A list of folders to print the files of</param>
        private void DebugFileList(List<DirectoryInfo> sourceDirs)
        {
            foreach (DirectoryInfo srcDir in sourceDirs)
            {
                Console.WriteLine(String.Format("{0} contains...", srcDir.FullName));
                foreach (FileInfo srcFile in srcDir.GetFiles())
                {
                    Console.WriteLine(srcFile.FullName);
                }
            }
        }

        /// <summary>
        /// Return a list containing this folder and every subfolder in it.
        /// Equivalent to call GetDirsRecursively(dir, true).
        /// </summary>
        /// <param name="dir">The root folder</param>
        /// <returns>dir as well as DirectoryInfo objects of all its subfolders</returns>
        private List<DirectoryInfo> GetDirsRecursively(DirectoryInfo dir)
        {
            return GetDirsRecursively(dir, true);
        }

        /// <summary>
        /// Return a list containing a DirectoryInfo object for
        /// every subfolder of this folder (and their subfolders, recursively).
        /// If includeSelf is true, the root DirectoryInfo (the one you provided)
        /// will be included in the List.
        /// </summary>
        /// <param name="dir">The dir to get subfolders of</param>
        /// <param name="includeSelf">If true, includes the dir you provided as well</param>
        /// <returns></returns>
        private List<DirectoryInfo> GetDirsRecursively(DirectoryInfo dir, bool includeSelf)
        {

            List<DirectoryInfo> dirs = new List<DirectoryInfo>();
            if (includeSelf)
            {
                dirs.Add(dir);
            }
            dirs.AddRange(dir.GetDirectories());
            
            foreach (DirectoryInfo subdir in dir.GetDirectories()) {
                Console.WriteLine(String.Format("subdir: {0}", subdir.FullName));
                dirs.AddRange(GetDirsRecursively(subdir, false));
            }

            return dirs;
        }

        /// <summary>
        /// Given a root folder and one of its subfolders, returns
        /// the relative path between them.
        /// For example, if given C:\test\path and C:\test\path\to\here,
        /// the string returned will be "to\here".
        /// </summary>
        /// <param name="root">A root folder</param>
        /// <param name="subfolder">A subfolder of the root folder</param>
        /// <returns>The relative path between root and subfolder. Or an 
        /// empty string if root and subfolder match.</returns>
        /// <exception cref="System.ArgumentException">If subfolder is not root or 
        /// a descendent of root</exception>
        private static string GetRelativePath(DirectoryInfo root, DirectoryInfo subfolder)
        {
            string rootUpper = root.FullName.ToUpper();
            string subUpper = subfolder.FullName.ToUpper();
            if (rootUpper == subUpper)
            {
                return "";
            }
            else if (!subUpper.StartsWith(rootUpper))
            {
                throw new ArgumentException(
                    String.Format("{0} is not a subfolder of {1}", subfolder.FullName, root.FullName)
                    );
            }
            int rootLength = root.FullName.Length;
            return subfolder.FullName.Substring(rootLength + 1);
        }
    }
}
