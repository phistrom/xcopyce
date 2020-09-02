using System;

namespace xcopy
{
    /// <summary>
    /// Recursively copies files from a given source directory to a destination.
    /// </summary>
    class Program
    {
        static string Usage = @"Usage:
xcopy.exe [srcdir] [dstdir]
There are no flags... yet. Will NOT 
overwrite existing files. Copies all files
from a source directory to a destination 
directory. Will create destination 
directory structure as needed.";

        /// <summary>
        /// Currently only reads two parameters: srcFolder and dstFolder.
        /// </summary>
        /// <param name="args">Index 0 must be source folder, Index 1 must be destination.</param>
        /// <returns>0 if successful, -1 if invalid number of arguments</returns>
        static int Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine(Usage);
                Console.ReadLine();
            }

            string source = args[0].Trim();
            string dest = args[1].Trim();
            Console.WriteLine(String.Format("Source: '{0}'", source));
            Console.WriteLine(String.Format("Destination: '{0}'", dest));
            XCopy xcopy = new XCopy(source, dest);
            xcopy.RecursiveCopy();
            return 0;
        }
    }
}
