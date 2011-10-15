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
    /// Minimizes JS and CSS files in a directory
    /// Uses YUI Compressor .NET
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    ///   <jsmin todir="d:\deploy" suffix="-min">
    ///     <fileset>
    ///       <include name="js/*.js" />
    ///       <include name="css/*.css" />
    ///     </fileset>
    ///   </jsmin>
    /// ]]>
    /// </code>
    /// </example>
    [TaskName("jsmin")]
    public class JSMinTask: MinifyTask
    {
        protected override void Minify(string fileName, string outputFileName)
        {
            Log(Level.Info, "Minifying JavaScript {0} -> {1}", fileName, outputFileName);
            var compressor = new JavaScriptCompressor(File.ReadAllText(fileName));
            string output = compressor.Compress();
            File.WriteAllText(outputFileName, output);
        }
    }
}
