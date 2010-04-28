/*
 * Eksi.Tasks library - collection of NAnt tasks
 * Copyright (c) 2010 Ekşi Teknoloji Ltd. (http://www.eksiteknoloji.com)
 * Licensed under MIT License, read license.txt for details
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;

namespace Eksi.Tasks
{    
    /// <summary>
    /// JSLint task implementation
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// <jslint config="/*jslint white: false */" encoding="utf-8">
    ///   <fileset>
    ///     <include name="**/*.js" />
    ///   </fileset>
    /// </jslint>
    /// ]]>
    /// </code>
    /// </example>
    [TaskName("jslint")]
    public class JSLintTask: Task
    {
        private const string jsLintFileName = "jslint.js";
        private const string defaultJSLintConfig 
            = "/*jslint white: true, browser: true, onevar: true, undef: true, nomen: true, eqeqeq: true, plusplus: true, bitwise: true, regexp: true, strict: true, newcap: true, immed: true */";
        private const string cscriptFileName = "cscript.exe";
        private const string indentStr = "    {0}\n\r";

        private const int maxOutputLineLength = 100; 

        private readonly Encoding defaultEncoding = Encoding.UTF8;

        [BuildElement("fileset", Required=true)]
        public FileSet Files { get; set; }

        /// <summary>
        /// This is standard config string used in JSLint in format 
        /// "/*jslint x: true, y: true, z: true */" or
        /// "x: true, y: true, z: true"
        /// </summary>
        [TaskAttribute("config", Required=false)]
        [StringValidator(AllowEmpty=false)]
        public string Config { get; set; }

        [TaskAttribute("encoding", Required = false)]
        [StringValidator(AllowEmpty = false)]
        public string TextEncoding { get; set; }

        protected override void ExecuteTask()
        {
            string config = Config;
            if (config == null)
            {
                // the good parts
                config = defaultJSLintConfig;
            }
            if (!config.StartsWith("/*jslint "))
            {
                config = String.Format("/*jslint {0} */", config);
            }

            Encoding encoding = defaultEncoding;
            if (!String.IsNullOrEmpty(TextEncoding))
            {
                encoding = Encoding.GetEncoding(TextEncoding);
            }

            string systemPath = Environment.GetFolderPath(Environment.SpecialFolder.System);
            string cmd = Path.Combine(systemPath, cscriptFileName);
            string dllPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string jsLintPath = Path.Combine(dllPath, jsLintFileName);

            // use common start info among iterations
            var startInfo = new ProcessStartInfo(cmd, String.Format("//nologo {0}", jsLintPath));
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;

            bool error = false;
            foreach (var fileName in Files.FileNames)
            {
                Log(Level.Info, String.Format("Checking {0}", fileName));
                string expandedFileName = Project.ExpandProperties(fileName, Location);
                Process process = Process.Start(startInfo);
                string content = File.ReadAllText(expandedFileName);
                if (config != String.Empty)
                {
                    process.StandardInput.WriteLine(config);
                }
                process.StandardInput.Write(content);
                process.StandardInput.Close();
                while(!process.StandardError.EndOfStream)
                {
                    var sb = new StringBuilder();
                    string str = process.StandardError.ReadLine();
                    sb.AppendFormat(indentStr, str);
                    if(!process.StandardError.EndOfStream)
                    {
                        str = process.StandardError.ReadLine();
                        if (str.Length > maxOutputLineLength)
                        {
                            str = str.Remove(maxOutputLineLength) + "...";
                        }
                        sb.AppendFormat(indentStr, str);
                    }
                    Log(Level.Error, sb.ToString());
                    error = true;
                }
            }

            if (error && this.FailOnError)
            {
                throw new BuildException("JSLint found errors");
            }
        }
    }
}
