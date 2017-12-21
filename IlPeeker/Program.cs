using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronRust;
using IronRust.Asts;

namespace IlPeeker {

    using static Environment;

    public class Program {
        
        private static async Task Main(string[] args)
        {
            var x = -129;
            
            //var makeVariable = new VariableAst( "x", "int32" );
            //var loadVariable = new IntegerAst(5);
            //var bind = new BindingAst("x", loadVariable);
            //var block = new ICodeAcceptable[]
            //{
            //    makeVariable,
            //    bind
            //};

            //var func = new FunctionAst("Main", Accecibility.Public, true, "void", true, block);
            //var codeGenerator = new IlGenerator();
            //func.Accept(codeGenerator);

            //Console.WriteLine(codeGenerator.Result);

            var code = $"fn hoge() {{{NewLine}" +
                       $"    let mut x = 5;{NewLine}" +
                       $"    x = 10;{NewLine}" +
                       $"}}{NewLine}";
            var lexer = new Lexer( code );
            var parser = new Parser( lexer );
            var result = parser.Parse();

            var codeGenerator = new IlGenerator();
            result.Accept( codeGenerator );

            Console.WriteLine(codeGenerator.Result);

            var tmpPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var filePath = OutputToTmpFile( tmpPath, codeGenerator );

            await CompileIl( filePath, tmpPath );
            

            Process.Start( tmpPath );
        }

        private static string OutputToTmpFile(string tmpPath, IlGenerator codeGenerator)
        {
            Directory.CreateDirectory(tmpPath);
            var filePath = Path.Combine(tmpPath, "tmp.txt");
            File.WriteAllText(filePath, codeGenerator.Result);
            return filePath;
        }

        private static async Task CompileIl(string filePath, string tmpPath)
        {
            var process = CreateProcess("ilasm", $"{filePath} /dll");

            process.Start();
            //await Task.Delay(2000);

            //var exePath = Path.Combine(tmpPath, "tmp.exe");

            //var executeProcess = CreateProcess(exePath);
            //executeProcess.Start();

            //var output = executeProcess.StandardOutput.ReadToEnd();
            //Console.WriteLine(output);
        }


        private static Process CreateProcess( string name, string args = null )
        {
            return new Process
            {
                StartInfo =
                {
                    FileName = name,
                    Arguments = args,
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };
        }
    }
}
