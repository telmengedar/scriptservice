using System;
using System.Collections.Generic;
using NUnit.Framework;
using ScriptService.Extensions;

namespace ScriptService.Tests {

    [TestFixture, Parallelizable]
    public class WorkflowServiceTests {

        [Test, Parallelizable]
        public void ObjectDeserializeOutcome() {
            string json = "{\"id\": \"guid\", \"name\": \"bla\", \"type\": \"Start\"}";
            object result = json.Deserialize<object>();
            Assert.That(result is IDictionary<string, object>);
        }

        [Test, Parallelizable]
        public void ArrayDeserializeOutcome() {
            string json = "[{\"id\": \"guid\", \"name\": \"bla\", \"type\": \"Start\"}]";
            object result = json.Deserialize<object>();
            Assert.That(result is List<object> array);
            array = (List<object>) result; // what a hack ...
            Assert.AreEqual(1, array.Count);
            Assert.That(array[0] is IDictionary<string, object>);

        }
    }
}