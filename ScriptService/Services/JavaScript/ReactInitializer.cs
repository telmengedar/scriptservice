using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Core;
using React;
using React.TinyIoC;

namespace ScriptService.Services.JavaScript {

    /// <summary>
    /// initializes react
    /// </summary>
    public static class ReactInitializer {

        static ReactInitializer() {
            JsEngineSwitcher.Current.DefaultEngineName = ChakraCoreJsEngine.EngineName;
            JsEngineSwitcher.Current.EngineFactories.AddChakraCore();
            InitReact();
        }

        static void InitReact() {
            Initializer.Initialize(registration => registration.AsSingleton());
            TinyIoCContainer container = AssemblyRegistration.Container;
            // Register some components that are normally provided by the integration library
            // (eg. React.AspNet or React.Web.Mvc6)
            container.Register<ICache, NullCache>();
            container.Register<IFileSystem, SimpleFileSystem>();
        }

        /// <summary>
        /// static initializer method
        /// </summary>
        public static void Initialize() {
            // the static ctor actually does everything
        }
    }
}