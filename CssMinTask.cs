/*
 * Eksi.Tasks library - collection of NAnt tasks
 * Copyright (c) 2010 Ekşi Teknoloji Ltd. (http://www.eksiteknoloji.com)
 * Licensed under MIT License, read license.txt for details
 */

using System.IO;
using NAnt.Core;
using NAnt.Core.Attributes;
using Yahoo.Yui.Compressor;

namespace Eksi.Tasks
{
    /// <summary>
    /// Minimizes CSS files in a directory
    /// Uses YUI Compressor .NET
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    ///   <cssmin todir="d:\deploy" suffix="-min">
    ///     <fileset>
    ///       <include name="css/*.css" />
    ///     </fileset>
    ///   </jsmin>
    /// ]]>
    /// </code>
    /// </example>
    [TaskName("cssmin")]
    public class CssMinTask: MinifyTask
    {
        protected override void Minify(string fileName, string outputFileName)
        {
            Log(Level.Info, "Minifying CSS {0} -> {1}", fileName, outputFileName);
            string input = File.ReadAllText(fileName);
            string output = CssCompressor.Compress(input, 80, CssCompressionType.Hybrid);
            File.WriteAllText(outputFileName, output);
        }
    }
}
