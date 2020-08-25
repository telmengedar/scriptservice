namespace ScriptService.Dto.Workflows.Nodes {

    /// <summary>
    /// parameters used for a binary operator
    /// </summary>
    public class BinaryOpParameters {

        /// <summary>
        /// left hand side operator
        /// </summary>
        public object Lhs { get; set; }

        /// <summary>
        /// right hand side operator
        /// </summary>
        public object Rhs { get; set; }

        /// <summary>
        /// operation to execute
        /// </summary>
        public BinaryOperation Operation { get; set; }


    }
}