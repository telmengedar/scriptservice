using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NightlyCode.Scripting;
using NightlyCode.Scripting.Parser;
using NUnit.Framework;
using ScriptService.Dto;
using ScriptService.Dto.Scripts;
using ScriptService.Services;
using ScriptService.Services.JavaScript;
using ScriptService.Services.Scripts;

namespace ScriptService.Tests {

    [TestFixture, Parallelizable]
    public class JavascriptTests {

        public string TestMethod(NamedCode code) {
            return code.Name;
        }

        public int TestNumber(int number) {
            return number;
        }

        public Task<string> TestTask() {
            return Task.Run(() => "test");
        }

        [Test, Parallelizable]
        public void TypeConversion() {
            JavaScript script = new JavaScript("test.TestMethod({Name:'Test'})", new ScriptImportService(null));
            Assert.AreEqual("Test", script.Execute(new Dictionary<string, object> {
                ["log"] = new WorkableLogger(new NullLogger<JavascriptTests>(), null),
                ["test"] = this
            }));
        }

        [Test, Parallelizable]
        public void IntCall() {
            JavaScript script = new JavaScript("test.TestNumber(7)", new ScriptImportService(null));
            Assert.AreEqual(7, script.Execute(new Dictionary<string, object> {
                ["log"] = new WorkableLogger(new NullLogger<JavascriptTests>(), null),
                ["test"] = this
            }));
        }

        [Test, Parallelizable]
        public void AwaitTask() {
            JavaScript script = new JavaScript("await(test.TestTask())", new ScriptImportService(null));
            Assert.AreEqual("test", script.Execute(new Dictionary<string, object> {
                ["log"] = new WorkableLogger(new NullLogger<JavascriptTests>(), null),
                ["test"] = this
            }));
        }

        [Test, Parallelizable]
        public void CallNCScript() {
            ScriptParser parser = new ScriptParser();
            parser.Types.AddType<Script>();
            IScript ncscript = parser.Parse("parameter($data, script) return($data.name)");

            Mock<IScriptCompiler> scriptcompiler=new Mock<IScriptCompiler>();
            scriptcompiler.Setup(s => s.CompileScriptAsync(It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync(new CompiledScript() {
                Instance = ncscript
            });

            Mock<IScriptImportService> importservice=new Mock<IScriptImportService>();
            importservice.Setup(s => s.Script(It.IsAny<string>(), It.IsAny<int?>())).Returns(new ScriptExecutor(new WorkableLogger(new NullLogger<JavascriptTests>(), null), scriptcompiler.Object, "Test", 1));
            importservice.Setup(s => s.Clone(It.IsAny<WorkableLogger>())).Returns(() => importservice.Object);

            JavaScript jsscript = new JavaScript("const script=load.Script('Test', null); return script.Execute({data:{Name:'Test'}});", importservice.Object);
            Assert.AreEqual("Test", jsscript.Execute(new Dictionary<string, object> {
                ["log"] = null
            }));
        }

        [Test, Parallelizable]
        public void TestTypescript() {
            Mock<IScriptImportService> importservice = new Mock<IScriptImportService>();
            importservice.Setup(s => s.Clone(It.IsAny<WorkableLogger>())).Returns(() => importservice.Object);

            ScriptCompiler compiler =new ScriptCompiler(new NullLogger<ScriptCompiler>(), null, null, null, null, null, importservice.Object, null, null);

            IScript script = compiler.CompileCode("function next(value: number): number {return value+1;} const result: number=next(8); return result;", ScriptLanguage.TypeScript);
            Assert.AreEqual(9, script.Execute(new Dictionary<string, object> {
                ["log"] = new WorkableLogger(new NullLogger<JavascriptTests>(), null)
            }));
        }
    }
}