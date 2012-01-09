/*
 * Eksi.Tasks library - collection of NAnt tasks
 * Copyright (c) 2010 Ekşi Teknoloji Ltd. (http://www.eksiteknoloji.com)
 * Licensed under MIT License, read license.txt for details
 */

using System;
using System.IO;
using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;

namespace Eksi.Tasks
{
    /// <summary>
    /// This is the base task to derive all "minimizers" from.
    /// </summary>
    public abstract class MinifyTask : Task
    {
        [TaskAttribute("suffix", Required = false)]
        public string Suffix { get; set; }

        [TaskAttribute("todir", Required = false)]
        public string DestPath { get; set; }

        [BuildElement("fileset", Required = true)]
        public FileSet Files { get; set; }

        protected override void ExecuteTask()
        {
            string suffix = Suffix;
            string destPath = DestPath;
            foreach (var fileName in Files.FileNames)
            {
                string outputFileName = Path.GetFileNameWithoutExtension(fileName)
                        + (suffix ?? String.Empty) + Path.GetExtension(fileName);
                string outputPath = destPath ?? Path.GetDirectoryName(fileName);
                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }
                outputFileName = Path.Combine(outputPath, outputFileName);
                Minify(fileName, outputFileName);
            }
        }

        protected abstract void Minify(string inputFileName, string outputFileName);
    }
}
