using System;

namespace ScriptService.Dto.Workflows {
    
    /// <summary>
    /// identifier for workflows
    /// </summary>
    public struct WorkflowIdentifier {
        
        /// <summary>
        /// creates a new <see cref="WorkflowIdentifier"/>
        /// </summary>
        /// <param name="id">id of workflow</param>
        /// <param name="revision">workflow revision</param>
        /// <param name="name">name of workflow (optional)</param>
        public WorkflowIdentifier(long id, int revision, string name=null) {
            Id = id;
            Revision = revision;
            Name = name;
        }

        /// <summary>
        /// id of executing script
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// executing script revision
        /// </summary>
        public int Revision { get; }

        /// <summary>
        /// name of executing script
        /// </summary>
        public string Name { get; }

        bool Equals(WorkflowIdentifier other) {
            return Id == other.Id && Revision == other.Revision;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WorkflowIdentifier) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode() {
            return HashCode.Combine(Id, Revision);
        }

        /// <summary>
        /// equality operator
        /// </summary>
        /// <param name="left">lhs operant</param>
        /// <param name="right">rhs operant</param>
        /// <returns>true if lhs is equal to rhs, false otherwise</returns>
        public static bool operator ==(WorkflowIdentifier left, WorkflowIdentifier right) {
            return Equals(left, right);
        }

        /// <summary>
        /// inequality operator
        /// </summary>
        /// <param name="left">lhs operant</param>
        /// <param name="right">rhs operant</param>
        /// <returns>true if lhs is not equal to rhs, false otherwise</returns>
        public static bool operator !=(WorkflowIdentifier left, WorkflowIdentifier right) {
            return !Equals(left, right);
        }
    }
}