using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
using ScriptService.Services.Lua;
using ScriptService.Services.Python;
using ScriptService.Services.Scripts;
using LuaImportService = ScriptService.Tests.Mocks.LuaImportService;

namespace ScriptService.Tests {

    [TestFixture, Parallelizable]
    public class LuaTests {

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
        public void AwaitTask() {
            //PythonService pythonservice=new PythonService(new Mock<IScriptImportService>().Object, null);
            LuaScript script = new LuaScript("return await(test.TestTask())", new LuaService(null, null));
            Assert.AreEqual("test", script.Execute(new Dictionary<string, object> {
                ["test"] = this
            }));
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

            LuaScript script = new LuaScript("return test:TestMethod(new(\"NamedCode\", {Name=\"Test\"}))", new LuaService(null, creator));
            Assert.AreEqual("Test", script.Execute(new Dictionary<string, object> {
                ["log"] = new WorkableLogger(new NullLogger<JavascriptTests>(), null),
                ["test"] = this
            }));
        }

        [Test, Parallelizable]
        public void ReturnConstant() {
            //PythonService pythonservice=new PythonService(new Mock<IScriptImportService>().Object, null);
            LuaScript script = new LuaScript("return 7", new LuaService(null, null));
            Assert.AreEqual(7, script.Execute(new Dictionary<string, object> {
                ["test"] = this
            }));
        }
        
        [Test, Parallelizable]
        public void IntCall() {
//            PythonService pythonservice=new PythonService(new Mock<IScriptImportService>().Object, null);
            LuaScript script = new LuaScript("return test:TestNumber(7)", new LuaService(null, null));
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

            LuaImportService importservice=new LuaImportService();
            importservice.AddScript("Test", new ScriptExecutor(new WorkableLogger(new NullLogger<JavascriptTests>(), null), scriptcompiler.Object, "Test", 1));
            
            Mock<IServiceProvider> serviceprovider=new Mock<IServiceProvider>();
            serviceprovider.Setup(s => s.GetService(typeof(IScriptCompiler))).Returns(scriptcompiler.Object);
            
            //PythonService pythonservice = new PythonService(importservice.Object, null);
            LuaScript script = new LuaScript("script=load:Script(\"Test\", nil)\nreturn script:Execute({data={Name=\"Test\"}})", new LuaService(serviceprovider.Object, null));
            Assert.AreEqual("Test", script.Execute(new Dictionary<string, object> {
                ["log"] = null
            }));
        }
    }
}