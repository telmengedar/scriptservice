using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using ScriptService.Dto.Workflows.Nodes;
using ScriptService.Services.Workflows.Nodes;
using ScriptService.Tests.Mocks;

namespace ScriptService.Tests {

    [TestFixture, Parallelizable]
    public class BinaryNodeTests {

        [Test, Parallelizable]
        public async Task TestAdd() {
            BinaryNode node=new BinaryNode(Guid.NewGuid(), null, new BinaryOpParameters {
                Lhs = "3",
                Operation = BinaryOperation.Add,
                Rhs = "7"
            }, new TestCompiler());

            Assert.AreEqual(10, await NodeTest.Execute(node));
        }

        [Test, Parallelizable]
        public async Task TestAddVariable() {
            BinaryNode node = new BinaryNode(Guid.NewGuid(),null, new BinaryOpParameters {
                Lhs = "lhs",
                Operation = BinaryOperation.Add,
                Rhs = "rhs"
            }, new TestCompiler());

            Assert.AreEqual(10, await NodeTest.Execute(node, new Dictionary<string, object> {["lhs"] = 6, ["rhs"] = 4}));
        }

    }
}