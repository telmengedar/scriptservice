using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NightlyCode.AspNetCore.Services.Convert;
using NightlyCode.Scripting.Extensions;
using NightlyCode.Scripting.Parser;
using ScriptService.Dto;
using ScriptService.Dto.Workflows.Nodes;
using ScriptService.Errors;
using ScriptService.Services.Scripts;

namespace ScriptService.Services.Workflows.Nodes {

    /// <summary>
    /// node used to initialize state variables
    /// </summary>
    public class StartNode : InstanceNode {
        readonly IScriptCompiler compiler;

        /// <summary>
        /// creates a new <see cref="StartNode"/>
        /// </summary>
        /// <param name="nodeName">name of node</param>
        /// <param name="parameters">parameters for workflow start</param>
        /// <param name="compiler">parser used to convert parameter data</param>
        public StartNode(string nodeName, StartParameters parameters, IScriptCompiler compiler)
            : base(nodeName) {
            this.compiler = compiler;
            Parameters = parameters;
        }

        /// <summary>
        /// parameters for start node
        /// </summary>
        public StartParameters Parameters { get; }

        /// <inheritdoc />
        public override async Task<object> Execute(WorkableLogger logger, IVariableProvider variables, IDictionary<string, object> state, CancellationToken token) {
            if (Parameters?.Parameters?.Length > 0) {
                foreach (ParameterDeclaration parameter in Parameters.Parameters) {
                    Type type = (await compiler.CompileCodeAsync(parameter.Type, ScriptLanguage.NCScript)).Execute<Type>();
                    
                    if (!state.TryGetValue(parameter.Name, out object parametervalue)) {
                        if (string.IsNullOrEmpty(parameter.Default))
                            throw new WorkflowException($"Missing required parameter '{parameter.Name}'");

                        parametervalue = await (await compiler.CompileCodeAsync(parameter.Default, ScriptLanguage.NCScript)).ExecuteAsync(state, token);
                    }
                    
                    if(parametervalue == null) {
                        if (type.IsValueType)
                            throw new WorkflowException($"Unable to pass null to the value parameter '{parameter.Name}'");
                        state[parameter.Name] = null;
                    }
                    else {
                        if(!type.IsInstanceOfType(parametervalue)) {
                            try {
                                if(type.IsArray) {
                                    Type elementtype = type.GetElementType();
                                    if(parametervalue is IEnumerable enumeration) {
                                        object[] items = enumeration.Cast<object>().ToArray();
                                        Array array = Array.CreateInstance(elementtype, items.Length);
                                        int index = 0;
                                        foreach (object item in items) {
                                            if (item is IDictionary dic)
                                                array.SetValue(dic.ToType(elementtype), index++);
                                            else array.SetValue(Converter.Convert(item, elementtype), index++);
                                        }

                                        parametervalue = array;
                                    }
                                    else {
                                        Array array = Array.CreateInstance(elementtype, 1);
                                        array.SetValue(Converter.Convert(parametervalue, elementtype), 0);
                                        parametervalue = array;
                                    }
                                }
                                else {
                                    if (parametervalue is IDictionary dic)
                                        parametervalue = dic.ToType(type);
                                    else if (parametervalue is IDictionary<string, object> gendic)
                                        parametervalue = gendic.ToType(type);
                                    else parametervalue = Converter.Convert(parametervalue, type);
                                }
                            }
                            catch(Exception e) {
                                throw new WorkflowException($"Unable to convert parameter '{parametervalue}' to '{type.Name}'", e);
                            }
                        }
                        state[parameter.Name] = parametervalue;
                    }

                }
            }
            return Task.FromResult((object)null);
        }
    }
}