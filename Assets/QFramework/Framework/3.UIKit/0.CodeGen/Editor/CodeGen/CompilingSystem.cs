using System;
using System.IO;
using UnityEngine;

namespace QFramework.CodeGen
{
    public class CompilingSystem 
    {
        public static void GenerateFile(FileInfo fileInfo, CodeFileGenerator codeFileGenerator)
        {
            // Get the path to the directory
            var directory = Path.GetDirectoryName(fileInfo.FullName);
            // Create it if it doesn't exist
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            try
            {
                // Write the file
                File.WriteAllText(fileInfo.FullName, codeFileGenerator.ToString());
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                Debug.Log("Coudln't create file " + fileInfo.FullName);
            }
        }
    }
}