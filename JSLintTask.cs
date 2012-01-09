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
        private const string defaultJSLintConfig = "/*jslint white: true, browser: true, onevar: true, undef: true, nomen: true, eqeqeq: true, plusplus: true, bitwise: true, regexp: true, strict: true, newcap: true, immed: true */";
        private const string cscriptFileName = "cscript.exe";
        private const string indentStr = "    {0}\n\r";
        private const int maxOutputLineLength = 100; 
        private readonly Encoding defaultEncoding = Encoding.UTF8;
        private ProcessStartInfo startInfo;

        [BuildElement("fileset", Required=true)]
        public FileSet Files { get; set; }

        /// <summary>
        /// This is standard config string used in JSLint in format 
        /// "/*jslint x: true, y: true, z: true */" or
        /// "x: true, y: true, z: true"
        /// </summary>
        private string config;
        [TaskAttribute("config", Required = false)]
        [StringValidator(AllowEmpty = false)] 
        public string Config 
        { 
            get
            {
                if(this.config == null)
                {
                    this.config = defaultJSLintConfig;
                }
                else if (!config.StartsWith("/*jslint "))
                {
                    this.config = String.Format("/*jslint {0} */", this.config);
                }
                return this.config;           
            }
            set
            {
                this.config = value;                
            } 
        }

        [TaskAttribute("encoding", Required = false)]
        [StringValidator(AllowEmpty = false)]
        public string TextEncoding { get; set; }

        protected override void ExecuteTask()
        {
            Encoding encoding = defaultEncoding;
            if (!String.IsNullOrEmpty(TextEncoding))
            {
                encoding = Encoding.GetEncoding(TextEncoding);
            }
            string cmd;
            string jsLintPath;
            getJsLintPath(out cmd, out jsLintPath);
            createProcessStartInfo(cmd, jsLintPath);
            bool error = false;
            foreach (var fileName in Files.FileNames)
            {
                error |= runJSLint(fileName);
            }
            if (error && this.FailOnError)
            {
                throw new BuildException("JSLint found errors");
            }
        }

        private static void getJsLintPath(out string cmd, out string jsLintPath)
        {
            string systemPath = Environment.GetFolderPath(Environment.SpecialFolder.System);
            cmd = Path.Combine(systemPath, cscriptFileName);
            string dllPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            jsLintPath = Path.Combine(dllPath, jsLintFileName);
        }

        private bool runJSLint(string fileName)
        {
            Log(Level.Info, String.Format("Checking {0}", fileName));
            string expandedFileName = Project.ExpandProperties(fileName, Location);
            Process process = Process.Start(startInfo);
            string content = File.ReadAllText(expandedFileName);
            process.StandardInput.WriteLine(Config);
            process.StandardInput.Write(content);
            process.StandardInput.Close();
            bool error = false;
            while (!process.StandardError.EndOfStream)
            {
                var sb = new StringBuilder();
                string str = process.StandardError.ReadLine();
                sb.AppendFormat(indentStr, str);
                if (!process.StandardError.EndOfStream)
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
            return error;
        }

        private void createProcessStartInfo(string cmd, string jsLintPath)
        {
            startInfo = new ProcessStartInfo(cmd, String.Format("//nologo {0}", jsLintPath));
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
        }
    }
}
