namespace ScriptService.Tests.Data {
    public class RecursiveType {
        public string Name { get; set; }
        public RecursiveType Type { get; set; }
    }
}