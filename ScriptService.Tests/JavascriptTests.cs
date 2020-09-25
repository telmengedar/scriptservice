using System.Collections.Generic;
using Esprima;
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

        [Test, Parallelizable]
        public void TypeConversion() {
            JavaScript script = new JavaScript(new JavaScriptParser("test.TestMethod({Name:'Test'})").ParseScript(), new JavascriptImportService(null));
            Assert.AreEqual("Test", script.Execute(new Dictionary<string, object> {
                ["log"] = new WorkableLogger(new NullLogger<JavascriptTests>(), null),
                ["test"] = this
            }));
        }

        [Test, Parallelizable]
        public void IntCall() {
            JavaScript script = new JavaScript(new JavaScriptParser("test.TestNumber(7)").ParseScript(), new JavascriptImportService(null));
            Assert.AreEqual(7, script.Execute(new Dictionary<string, object> {
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

            Mock<IJavascriptImportService> importservice=new Mock<IJavascriptImportService>();
            importservice.Setup(s => s.Script(It.IsAny<string>(), It.IsAny<int?>())).Returns(new ScriptExecutor(new WorkableLogger(new NullLogger<JavascriptTests>(), null), scriptcompiler.Object, "Test", 1));
            importservice.Setup(s => s.Clone(It.IsAny<WorkableLogger>())).Returns(() => importservice.Object);

            JavaScript jsscript = new JavaScript(new JavascriptParser().Parse("const script=load.Script('Test', null); return script.Execute({data:{Name:'Test'}})"), importservice.Object);
            Assert.AreEqual("Test", jsscript.Execute(new Dictionary<string, object> {
                ["log"] = null
            }));
        }
    }
}