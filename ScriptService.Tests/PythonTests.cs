using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NightlyCode.Scripting;
using NightlyCode.Scripting.Parser;
using NUnit.Framework;
using ScriptService.Dto;
using ScriptService.Dto.Scripts;
using ScriptService.Services;
using ScriptService.Services.JavaScript;
using ScriptService.Services.Python;
using ScriptService.Services.Scripts;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using PythonService = ScriptService.Services.Python.PythonService;

namespace ScriptService.Tests {

    [TestFixture, Parallelizable]
    public class PythonTests {

        public string TestMethod(NamedCode code) {
            return code.Name;
        }

        public int TestNumber(int number) {
            return number;
        }
        
        [Test, Parallelizable]
        public void TypeConversion() {
            Mock<IConfigurationSection> typeconfig = new Mock<IConfigurationSection>();
            typeconfig.SetupGet(s => s.Key).Returns("NamedCode");
            typeconfig.SetupGet(s => s.Value).Returns("ScriptService.Dto.NamedCode,ScriptService");
            
            Mock<IConfigurationSection> typesconfig=new Mock<IConfigurationSection>();
            typesconfig.Setup(s => s.GetChildren()).Returns(new[] {typeconfig.Object});
            
            Mock<IConfiguration> config = new Mock<IConfiguration>();
            config.Setup(s => s.GetSection("Types")).Returns(typesconfig.Object);

            TypeCreator creator = new TypeCreator(new NullLogger<TypeCreator>(), config.Object);
            PythonService pythonservice = new PythonService(new Mock<IScriptImportService>().Object, creator);
            
            PythonScript script = new PythonScript(pythonservice, "import NamedCode\ncode=NamedCode()\ncode.Name='Test'\ntest.TestMethod(code)");
            Assert.AreEqual("Test", script.Execute(new Dictionary<string, object> {
                ["log"] = new WorkableLogger(new NullLogger<JavascriptTests>(), null),
                ["test"] = this
            }));
        }

        [Test, Parallelizable]
        public void ReturnConstant() {
            PythonService pythonservice=new PythonService(new Mock<IScriptImportService>().Object, null);
            PythonScript script = new PythonScript(pythonservice, "7");
            Assert.AreEqual(7, script.Execute(new Dictionary<string, object> {
                ["test"] = this
            }));
        }

        [Parallelizable]
        [TestCase("clr.AddReference(\"ScriptService\")")]
        [TestCase("clr . AddReference(\"ScriptService\")")]
        public void InvalidClrCall(string line) {
            PythonService pythonservice=new PythonService(new Mock<IScriptImportService>().Object, new Mock<ITypeCreator>().Object);
            Assert.Throws<ArgumentException>(()=>new PythonScript(pythonservice, $"{line}\n7"));
        }
        
        [Test, Parallelizable]
        public void InvalidImports() {
            PythonService pythonservice=new PythonService(new Mock<IScriptImportService>().Object, new Mock<ITypeCreator>().Object);
            Assert.Throws<ArgumentException>(()=>new PythonScript(pythonservice, "import clr\n7"));
        }
        
        [Test, Parallelizable]
        public void IntCall() {
            PythonService pythonservice=new PythonService(new Mock<IScriptImportService>().Object, null);
            PythonScript script = new PythonScript(pythonservice, "test.TestNumber(7)");
            Assert.AreEqual(7, script.Execute(new Dictionary<string, object> {
                ["test"] = this
            }));
        }

        [Test, Parallelizable]
        public void CallNCScript() {
            ScriptParser parser = new ScriptParser();
            parser.Types.AddType<Script>();
            IScript ncscript = parser.Parse("parameter($data, script) return($data.name)");

            Mock<IScriptCompiler> scriptcompiler=new Mock<IScriptCompiler>();
            scriptcompiler.Setup(s => s.CompileScriptAsync(It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync(new CompiledScript {
                Instance = ncscript
            });

            Mock<IScriptImportService> importservice=new Mock<IScriptImportService>();
            importservice.Setup(s => s.Script(It.IsAny<string>(), It.IsAny<int?>())).Returns(new ScriptExecutor(new WorkableLogger(new NullLogger<JavascriptTests>(), null), scriptcompiler.Object, "Test", 1));
            importservice.Setup(s => s.Clone(It.IsAny<WorkableLogger>())).Returns(() => importservice.Object);

            PythonService pythonservice = new PythonService(importservice.Object, null);
            PythonScript script = new PythonScript(pythonservice, "script=load.Script('Test', None)\nscript.Execute({'data':{'Name':'Test'}})");
            Assert.AreEqual("Test", script.Execute(new Dictionary<string, object> {
                ["log"] = null
            }));
        }
    }
}