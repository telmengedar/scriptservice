using System;

namespace ScriptService.Dto.Workflows {
    
    /// <summary>
    /// identifier of a node
    /// </summary>
    public struct NodeIdentifier {
        
        /// <summary>
        /// creates a new <see cref="NodeIdentifier"/>
        /// </summary>
        /// <param name="id">id of node</param>
        /// <param name="name">name of node</param>
        public NodeIdentifier(Guid id, string name) {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// id of node
        /// </summary>
        public Guid Id { get; }
        
        /// <summary>
        /// name of node
        /// </summary>
        public string Name { get; }

        bool Equals(NodeIdentifier other) {
            return Id.Equals(other.Id);
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            return obj is NodeIdentifier other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode() {
            return Id.GetHashCode();
        }
    }
}