using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Logging;
using NightlyCode.AspNetCore.Services.Errors.Exceptions;
using NightlyCode.Scripting.Parser;
using ScriptService.Extensions;
using MethodInfo = ScriptService.Dto.Sense.MethodInfo;
using ParameterInfo = ScriptService.Dto.Sense.ParameterInfo;
using PropertyInfo = ScriptService.Dto.Sense.PropertyInfo;
using TypeInfo = ScriptService.Dto.Sense.TypeInfo;

namespace ScriptService.Services.Sense {

    /// <inheritdoc />
    public class ScriptSenseService : IScriptSenseService {
        readonly ILogger<ScriptSenseService> logger;
        readonly IScriptParser parser;
        readonly IMethodProviderService methodprovider;
        readonly ConcurrentDictionary<Assembly, XmlDocument> assemblydocs = new ConcurrentDictionary<Assembly, XmlDocument>();

        /// <summary>
        /// creates a new <see cref="ScriptSenseService"/>
        /// </summary>
        /// <param name="logger">access to logging</param>
        /// <param name="parser">parser used to get script environment information</param>
        /// <param name="methodprovider">provider for installed host types</param>
        public ScriptSenseService(ILogger<ScriptSenseService> logger, IScriptParser parser, IMethodProviderService methodprovider) {
            this.logger = logger;
            this.parser = parser;
            this.methodprovider = methodprovider;
        }

        string GetReturnType(System.Reflection.MethodInfo method, Type hosttype) {
            if(method.ReturnType.IsGenericParameter) {
                if(method.ReturnType.GenericParameterPosition < hosttype.GenericTypeArguments.Length)
                    return hosttype.GenericTypeArguments[method.ReturnType.GenericParameterPosition].GetTypeName();
                // TODO: see whether it makes sense to resolve the generic type
                return typeof(object).GetTypeName();
            }

            if(method.ReturnType.ContainsGenericParameters) {
                if(method.ReturnType.BaseType == typeof(Array) && hosttype.GenericTypeArguments.Length > 0)
                    return hosttype.GenericTypeArguments[0].MakeArrayType().GetTypeName();
                return typeof(object).GetTypeName();
            }

            return method.ReturnType != typeof(void) ? method.ReturnType.GetTypeName() : null;
        }

        IEnumerable<MethodInfo> RetrieveExtensionMethods(Type argumenttype) {
            if(argumenttype == null)
                yield break;

            Type currenttype = argumenttype;
            do {
                Type typetocheck = currenttype.IsGenericType ? currenttype.GetGenericTypeDefinition() : currenttype;
                foreach(System.Reflection.MethodInfo methodInfo in parser.Extensions.GetExtensions(typetocheck)) {
                    yield return new MethodInfo {
                        Name = methodInfo.Name,
                        Parameters = methodInfo.GetParameters().Skip(1).Select(mp => new ParameterInfo {
                            Name = mp.Name,
                            Type = mp.ParameterType.GetTypeName(),
                            HasDefault = mp.HasDefaultValue,
                            IsReference = mp.ParameterType.IsByRef,
                            IsParams = Attribute.IsDefined(mp, typeof(ParamArrayAttribute))
                        }).ToArray(),
                        Returns = GetReturnType(methodInfo, argumenttype)
                    };
                }

                currenttype = currenttype.BaseType;
            } while(currenttype != typeof(object) && currenttype != null);

            foreach(Type iftype in argumenttype.GetInterfaces()) {
                Type basetype = iftype;
                if(iftype.IsGenericType)
                    basetype = iftype.GetGenericTypeDefinition();

                foreach(System.Reflection.MethodInfo methodInfo in parser.Extensions.GetExtensions(basetype)) {
                    yield return new MethodInfo {
                        Name = methodInfo.Name,
                        Parameters = methodInfo.GetParameters().Skip(1).Select(mp => new ParameterInfo {
                            Name = mp.Name,
                            Type = mp.ParameterType.GetTypeName(),
                            HasDefault = mp.HasDefaultValue,
                            IsReference = mp.ParameterType.IsByRef,
                            IsParams = Attribute.IsDefined(mp, typeof(ParamArrayAttribute))
                        }).ToArray(),
                        Returns = GetReturnType(methodInfo, iftype)
                    };
                }
            }
        }

        string DetermineElementType(Type type) {
            if(type.IsArray)
                return type.GetElementType().GetTypeName();

            Type enumerabletype;
            if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                enumerabletype = type;
            else
                enumerabletype = type.GetInterfaces().FirstOrDefault(t => t == typeof(IEnumerable<>));

            if(enumerabletype != null)
                return enumerabletype.GetGenericArguments().FirstOrDefault().GetTypeName();

            return null;
        }

        IEnumerable<System.Reflection.MethodInfo> GetMethods(Type type) {
            foreach(System.Reflection.MethodInfo method in type.GetMethods())
                yield return method;

            if(type.IsInterface)
                foreach(System.Reflection.MethodInfo method in typeof(object).GetMethods())
                    yield return method;
        }

        TypeInfo AnalyseType(Type type) {
            TypeInfo typeinfo = new TypeInfo {
                Name = type.GetTypeName(),
                Description = GetTypeDoc(type),
                ElementType = DetermineElementType(type),
                Properties = type.GetProperties().Where(pi => pi.GetIndexParameters().Length == 0).Select(pi => new PropertyInfo {
                    Name = pi.Name,
                    Description = GetPropertyDoc(pi),
                    Type = pi.PropertyType.GetTypeName()
                }).ToArray(),
                Methods = GetMethods(type).Where(mi => !mi.IsSpecialName).Select(mi => {
                    return new MethodInfo {
                        Name = mi.Name,
                        Description = GetMethodDoc(mi),
                        Parameters = mi.GetParameters().Select(mp => new ParameterInfo {
                            Name = mp.Name,
                            Description = GetParameterDoc(mi, mp),
                            Type = mp.ParameterType.GetTypeName(),
                            HasDefault = mp.HasDefaultValue,
                            IsReference = mp.ParameterType.IsByRef,
                            IsParams = Attribute.IsDefined(mp, typeof(ParamArrayAttribute))
                        }).ToArray(),
                        Returns = GetReturnType(mi, type)
                    };
                }).Concat(RetrieveExtensionMethods(type)).ToArray(),
                Indexer = type.GetProperties().Where(pi => pi.GetIndexParameters().Length > 0).Select(pi => {
                    return new MethodInfo {
                        Description = GetIndexerDoc(pi),
                        Parameters = pi.GetIndexParameters().Select(ip => new ParameterInfo {
                            Name = ip.Name,
                            Description = GetParameterDoc(pi, ip),
                            Type = ip.ParameterType.GetTypeName(),
                            HasDefault = ip.HasDefaultValue,
                            IsReference = ip.ParameterType.IsByRef,
                            IsParams = Attribute.IsDefined(ip, typeof(ParamArrayAttribute))
                        }).ToArray(),
                        Returns = pi.PropertyType.IsGenericParameter ? typeof(object).GetTypeName() : pi.PropertyType.GetTypeName()
                    };
                }).ToArray()
            };

            return typeinfo;
        }

        XmlDocument GetAssemblyDoc(Assembly assembly) {
            if(assembly == null)
                return null;

            if(assemblydocs.TryGetValue(assembly, out XmlDocument doc))
                return doc;

            string path = Path.GetFileNameWithoutExtension(assembly.Location) + ".xml";
            if(!File.Exists(path))
                return assemblydocs[assembly] = null;

            try {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(path);
                return assemblydocs[assembly] = xmldoc;
            }
            catch(Exception e) {
                logger.LogWarning(e, $"Unable to load xml doc for {assembly.FullName}");
                return null;
            }
        }

        string ExtractSummary(XmlNode summary) {
            if(summary == null)
                return null;

            StringBuilder text = new StringBuilder();
            foreach(XmlNode child in summary.ChildNodes) {
                switch(child.NodeType) {
                case XmlNodeType.Text:
                    text.Append(child.Value.Trim());
                    break;
                case XmlNodeType.Element:
                    switch(child.Name) {
                    case "see":
                        string crefvalue = child.Attributes?["cref"]?.Value ?? "";
                        if(crefvalue.Length > 2)
                            crefvalue = crefvalue.Substring(2);
                        text.Append(crefvalue);
                        break;
                    }

                    break;
                }

                text.Append(' ');
            }

            if(text.Length > 0)
                --text.Length;

            return text.ToString();
        }

        XmlNode GetMethodDocFromType(Type type, System.Reflection.MethodInfo method) {
            XmlDocument doc = GetAssemblyDoc(type.Assembly);
            string membername = $"{type.FullName}.{method.Name}({string.Join(",", method.GetParameters().Select(p => p.ParameterType.FullName))})";

            XmlNode doctag = doc?.SelectSingleNode($"doc/members/member[@name='M:{membername}']");
            if(doctag == null)
                return null;

            if(doctag.ChildNodes.Cast<XmlNode>().Any(c => c.Name == "inheritdoc")) {
                foreach(Type interfacetype in type.GetInterfaces()) {
                    XmlNode summarydoc = GetMethodDocFromType(interfacetype, method);
                    if(summarydoc != null)
                        return summarydoc;
                }

                if(type.BaseType != null && type != typeof(object)) {
                    XmlNode summarydoc = GetMethodDocFromType(type.BaseType, method);
                    if(summarydoc != null)
                        return summarydoc;
                }
                return null;
            }

            return doctag;
        }

        XmlNode GetIndexerDocFromType(Type type, System.Reflection.PropertyInfo method) {
            XmlDocument doc = GetAssemblyDoc(type.Assembly);
            string membername = $"{type.FullName}.{method.Name}({string.Join(",", method.GetIndexParameters().Select(p => p.ParameterType.FullName))})";

            XmlNode doctag = doc?.SelectSingleNode($"doc/members/member[@name='P:{membername}']");
            if(doctag == null)
                return null;

            if(doctag.ChildNodes.Cast<XmlNode>().Any(c => c.Name == "inheritdoc")) {
                foreach(Type interfacetype in type.GetInterfaces()) {
                    XmlNode summarydoc = GetIndexerDocFromType(interfacetype, method);
                    if(summarydoc != null)
                        return summarydoc;
                }

                if(type.BaseType != null && type != typeof(object)) {
                    XmlNode summarydoc = GetIndexerDocFromType(type.BaseType, method);
                    if(summarydoc != null)
                        return summarydoc;
                }
                return null;
            }

            return doctag;
        }

        string GetParameterDoc(System.Reflection.MethodInfo method, System.Reflection.ParameterInfo parameter) {
            if(method.DeclaringType == null)
                return null;

            XmlNode methoddoc = GetMethodDocFromType(method.DeclaringType, method);

            XmlNode paramdoc = methoddoc?.SelectSingleNode($"param[@name='{parameter.Name}']");
            return ExtractSummary(paramdoc);
        }

        string GetParameterDoc(System.Reflection.PropertyInfo method, System.Reflection.ParameterInfo parameter) {
            if(method.DeclaringType == null)
                return null;

            XmlNode methoddoc = GetIndexerDocFromType(method.DeclaringType, method);

            XmlNode paramdoc = methoddoc?.SelectSingleNode($"param[@name='{parameter.Name}']");
            return ExtractSummary(paramdoc);
        }

        string GetIndexerDoc(System.Reflection.PropertyInfo indexer) {
            if(indexer.DeclaringType == null)
                return null;

            XmlNode methoddoc = GetIndexerDocFromType(indexer.DeclaringType, indexer);
            XmlNode remarksnode = methoddoc?.SelectSingleNode("remarks");
            if(remarksnode != null)
                return ExtractSummary(methoddoc.SelectSingleNode("summary")) + "\n" + ExtractSummary(remarksnode);
            return ExtractSummary(methoddoc?.SelectSingleNode("summary"));
        }

        string GetMethodDoc(System.Reflection.MethodInfo method) {
            if(method.DeclaringType == null)
                return null;

            XmlNode methoddoc = GetMethodDocFromType(method.DeclaringType, method);
            XmlNode remarksnode = methoddoc?.SelectSingleNode("remarks");
            if(remarksnode != null)
                return ExtractSummary(methoddoc.SelectSingleNode("summary")) + "\n" + ExtractSummary(remarksnode);
            return ExtractSummary(methoddoc?.SelectSingleNode("summary"));
        }

        string GetPropertyDoc(System.Reflection.PropertyInfo pi) {
            if(pi.DeclaringType == null)
                return null;

            XmlDocument doc = GetAssemblyDoc(pi.DeclaringType.Assembly);

            XmlNode propertydoc = doc?.SelectSingleNode($"doc/members/member[@name='P:{pi.DeclaringType.FullName}.{pi.Name}']");
            XmlNode remarksnode = propertydoc?.SelectSingleNode("remarks");
            if(remarksnode != null)
                return ExtractSummary(propertydoc.SelectSingleNode("summary")) + "\n" + ExtractSummary(remarksnode);
            return ExtractSummary(propertydoc?.SelectSingleNode("summary"));
        }

        string GetTypeDoc(Type type) {
            if (type == null)
                return null;
            XmlDocument doc = GetAssemblyDoc(type.Assembly);

            XmlNode typedoc = doc?.SelectSingleNode($"doc/members/member[@name='T:{type.FullName}']");

            XmlNode remarksnode = typedoc?.SelectSingleNode("remarks");
            if(remarksnode != null)
                return ExtractSummary(typedoc.SelectSingleNode("summary")) + "\n" + ExtractSummary(remarksnode);
            return ExtractSummary(typedoc?.SelectSingleNode("summary"));
        }

        /// <inheritdoc />
        public Task<TypeInfo> GetTypeInfo(string typename) {
            Type type = Type.GetType(typename);
            if(type == null)
                throw new NotFoundException(typeof(Type), typename);
            return Task.FromResult(AnalyseType(type));
        }

        /// <inheritdoc />
        public Task<PropertyInfo[]> GetTypeProviders() {
            return Task.FromResult(parser.Types.ProvidedTypes.Select(t => new PropertyInfo {
                Name = t,
                Type = parser.Types.GetType(t).ProvidedType.GetTypeName()
            }).ToArray());
        }

        /// <inheritdoc />
        public Task<PropertyInfo[]> GetHostProviders() {
            return Task.FromResult(methodprovider.Hosts.Select(h => new PropertyInfo {
                Name = h.Key,
                Description = GetTypeDoc(h.Value?.GetType()),
                Type = h.Value?.GetType().GetTypeName()
            }).ToArray());
        }
    }
}