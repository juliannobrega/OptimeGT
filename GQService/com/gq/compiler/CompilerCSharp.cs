using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using Microsoft.Extensions.FileProviders;
using GQService.com.gq.log;

namespace GQService.com.gq.compiler
{
    public class CompilerCSharp
    {
        public enum SourceTypeEnum
        {
            File,
            Text
        }

        public string PathBase { get; private set; }

        protected List<string> Reference { get; set; }

        public SourceTypeEnum SourceType { get; set; }

        public string Source { get; set; }

        public Assembly CompiledAssembly { get; private set; }

        public CompilerCSharp()
        {
            Reference = new List<string>();

            string path = Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", "").Replace(":/", ":\\").Replace("/", "\\");
            var idx = path.LastIndexOf('\\') + 1;

            PathBase = path.Substring(0, idx);
            //path = path.Substring(0, path.Length - "AplicacionCentral.dll".Length);
        }

        public void AddReferencia(string referencia)
        {
            Reference.Add(referencia);
        }

        public Type GetClass(string className)
        {
            if (CompiledAssembly == null)
            {
                GenerateCode(this);
            }
            return CompiledAssembly.GetType(className);
        }

        public MethodInfo GetMethod(string className, string methodName)
        {
            var type = GetClass(className);
            return type.GetMethod(methodName);
        }

        public object Invoke(string className, string methodName, params object[] parameters)
        {
            var type = GetClass(className);
            var method = type.GetMethod(methodName);

            object obj = Activator.CreateInstance(type);

            var result = method.Invoke(obj, parameters);

            return result;
        }

        public static void GenerateCode(CompilerCSharp code)
        {
            try
            {
                CSharpCodeProvider provider = new CSharpCodeProvider();
                CompilerParameters cp = new CompilerParameters();

                cp.GenerateExecutable = false;
                cp.GenerateInMemory = true;
                cp.IncludeDebugInformation = true;

                foreach (string s in code.Reference)
                {
                    if (s != "")
                        cp.ReferencedAssemblies.Add(s);
                }

                CompilerResults compilerResults = null;

                switch (code.SourceType)
                {
                    case SourceTypeEnum.File:
                        {
                            compilerResults = provider.CompileAssemblyFromFile(cp, code.Source);
                            break;
                        }
                    case SourceTypeEnum.Text:
                        {
                            compilerResults = provider.CompileAssemblyFromSource(cp, code.Source);
                            break;
                        }
                }

                if (compilerResults.Errors.HasErrors == true)
                {
                    var ex = new ExceptionCompilerCSharp();
                    ex.CompilerResults = compilerResults;
                    Log.Error("GenerateCode", ex);
                    throw ex;
                }

                code.CompiledAssembly = compilerResults.CompiledAssembly;
            }
            catch (Exception ex)
            {
                Log.Error("GenerateCode", ex);
                throw;
            }
            
        }
    }

    public class ExceptionCompilerCSharp : Exception
    {
        public CompilerResults CompilerResults { get; internal set; }
    }
}
