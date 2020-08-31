namespace ScriptService.Dto.Sense {

    /// <summary>
    /// info about a method parameter
    /// </summary>
    public class ParameterInfo : PropertyInfo {

        /// <summary>
        /// determines whether parameter is a reference parameter
        /// </summary>
        public bool IsReference { get; set; }

        /// <summary>
        /// determines whether parameter has a default value
        /// </summary>
        public bool HasDefault { get; set; }

        /// <summary>
        /// determines whether parameter is a params array
        /// </summary>
        public bool IsParams { get; set; }

        /// <inheritdoc />
        public override string ToString() {
            if(IsParams)
                return $"params {base.ToString()}";
            if(IsReference)
                return $"ref {base.ToString()}";
            return base.ToString();
        }
    }
}