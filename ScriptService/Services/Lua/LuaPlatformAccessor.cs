using System.IO;
using System.Text;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Platforms;

namespace ScriptService.Services.Lua {
    public class LuaPlatformAccessor : IPlatformAccessor {
        
        public CoreModules FilterSupportedCoreModules(CoreModules module) {
            return module & ~(CoreModules.OS_System | CoreModules.IO);
        }

        public string GetEnvironmentVariable(string envvarname) {
            throw new System.NotImplementedException();
        }

        public bool IsRunningOnAOT() {
            return false;
        }

        /// <inheritdoc />
        public string GetPlatformName() {
            return "NC Script Engine";
        }

        public void DefaultPrint(string content) {
            throw new System.NotImplementedException();
        }

        public string DefaultInput(string prompt) {
            throw new System.NotImplementedException();
        }

        public Stream IO_OpenFile(Script script, string filename, Encoding encoding, string mode) {
            throw new System.NotImplementedException();
        }

        public Stream IO_GetStandardStream(StandardFileType type) {
            throw new System.NotImplementedException();
        }

        public string IO_OS_GetTempFilename() {
            throw new System.NotImplementedException();
        }

        public void OS_ExitFast(int exitCode) {
            throw new System.NotImplementedException();
        }

        public bool OS_FileExists(string file) {
            throw new System.NotImplementedException();
        }

        public void OS_FileDelete(string file) {
            throw new System.NotImplementedException();
        }

        public void OS_FileMove(string src, string dst) {
            throw new System.NotImplementedException();
        }

        public int OS_Execute(string cmdline) {
            throw new System.NotImplementedException();
        }
    }
}