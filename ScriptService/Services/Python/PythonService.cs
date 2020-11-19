using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using ScriptService.Services.JavaScript;

namespace ScriptService.Services.Python {
    /// <inheritdoc />
    public class PythonService : IPythonService {
        readonly IScriptImportService importservice;
        readonly ITypeCreator typecreator;
        readonly ScriptEngine pythonengine=IronPython.Hosting.Python.CreateEngine();

        /// <summary>
        /// creates a new <see cref="PythonService"/>
        /// </summary>
        /// <param name="importservice">service used to import hosts</param>
        /// <param name="typecreator">service used to create custom types</param>
        public PythonService(IScriptImportService importservice, ITypeCreator typecreator) {
            this.importservice = importservice;
            this.typecreator = typecreator;
        }

        string ProcessCode(string code) {
            StringBuilder finalcode=new StringBuilder();
            List<string> realcode=new List<string>();
            List<string> imports=new List<string>();

            foreach (string line in code.Split('\n')) {
                Match match = Regex.Match(line, "^\\s*import\\s+(?<type>(Host)|(Script)|(Workflow))\\s+(?<name>[a-zA-Z0-9.]+)\\sas\\s(?<variable>[a-zA-Z0-9]+)\\s*$");
                if (match.Success) {
                    realcode.Add($"{match.Groups["variable"].Value}=load.{match.Groups["type"].Value}(\"{match.Groups["name"].Value}\")");
                }
                else {
                    match = Regex.Match(line, "^\\s*import\\s+(?<name>[a-zA-Z0-9]+)\\s*$");
                    if (match.Success) {
                        string typename = match.Groups["name"].Value;
                        if (!typecreator.Contains(typename))
                            throw new ArgumentException($"Type '{typename}' not known");
                        imports.Add(typename);
                    }
                    else {
                        if (line.Trim().StartsWith("import"))
                            throw new ArgumentException($"Invalid import statement: '{line}'");
                        if (Regex.IsMatch(line, "^\\s*clr\\s*.\\s*AddReference"))
                            throw new ArgumentException($"Importing references using clr not supported");

                        realcode.Add(line);
                    }
                }
            }

            if (imports.Count > 0) {
                finalcode.AppendLine("import clr");
                Tuple<string, Type>[] types = imports.Select(i => new Tuple<string, Type>(i, typecreator[i])).ToArray();
                foreach (Assembly assembly in types.Select(t => t.Item2.Assembly).Distinct())
                    finalcode.AppendLine($"clr.AddReference(\"{assembly.GetName().Name}\")");
                foreach (Tuple<string, Type> type in types)
                    finalcode.AppendLine($"from {type.Item2.Namespace} import {type.Item2.Name} as {type.Item1}");
            }

            foreach (string statement in realcode)
                finalcode.AppendLine(statement);

            return finalcode.ToString();
        }
        
        /// <inheritdoc />
        public ScriptSource Parse(string code) {
            return pythonengine.CreateScriptSourceFromString(ProcessCode(code), SourceCodeKind.AutoDetect);
        }

        /// <inheritdoc />
        public object Execute(ScriptSource script, IDictionary<string, object> variables) {
            ScriptScope scope = pythonengine.CreateScope(variables);

            variables.TryGetValue("log", out object logvalue);
            scope.SetVariable("load", importservice.Clone(logvalue as WorkableLogger));
            scope.SetVariable("type", typecreator);
            scope.SetVariable("await", (Func<Task,object>)Helpers.Tasks.AwaitTask);
            return script.Execute(scope);
        }
    }
}