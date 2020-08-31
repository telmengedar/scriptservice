using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ScriptService.Dto.Sense;
using ScriptService.Extensions;
using ScriptService.Services;
using ScriptService.Services.Sense;

namespace ScriptService.Controllers {

    /// <summary>
    /// endpoints providing context information of installed script types
    /// </summary>
    [Route("api/v1/sense")]
    [ApiController]
    public class SenseController : ControllerBase {
        readonly IScriptSenseService senseservice;
        readonly IMethodProviderService hostprovider;

        /// <summary>
        /// creates a new <see cref="SenseController"/>
        /// </summary>
        /// <param name="senseservice">access to sense information</param>
        /// <param name="hostprovider">access to host information</param>
        public SenseController(IScriptSenseService senseservice, IMethodProviderService hostprovider) {
            this.senseservice = senseservice;
            this.hostprovider = hostprovider;
        }

        /// <summary>
        /// lists all installed host providers
        /// </summary>
        /// <returns>installed host providers</returns>
        [HttpGet("hosts")]
        public Task<PropertyInfo[]> ListHosts() {
            return senseservice.GetHostProviders();
        }

        /// <summary>
        /// lists all installed host providers
        /// </summary>
        /// <returns>installed host providers</returns>
        [HttpGet("hosts/{host}")]
        public Task<TypeInfo> GetHostInfo(string host) {
            object hostinstance = hostprovider.GetHost(host);
            return senseservice.GetTypeInfo(hostinstance.GetType().GetTypeName());
        }

        /// <summary>
        /// lists all installed host providers
        /// </summary>
        /// <returns>installed host providers</returns>
        [HttpGet("types")]
        public Task<PropertyInfo[]> ListTypes() {
            return senseservice.GetTypeProviders();
        }

        /// <summary>
        /// get type information
        /// </summary>
        /// <param name="type">type of which to get information</param>
        /// <returns>structural information about methods and properties of requested type</returns>
        [HttpGet("types/{type}")]
        public Task<TypeInfo> GetTypeInfo(string type) {
            return senseservice.GetTypeInfo(type);
        }
    }
}
