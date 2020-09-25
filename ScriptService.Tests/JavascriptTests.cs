using System.Collections.Generic;
using Esprima;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using ScriptService.Dto;
using ScriptService.Dto.Scripts;
using ScriptService.Services;
using ScriptService.Services.JavaScript;

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
    }
}