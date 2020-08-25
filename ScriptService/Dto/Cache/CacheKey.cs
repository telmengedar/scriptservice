using System;

namespace ScriptService.Dto.Cache {


    /// <summary>
    /// key used for an object cache
    /// </summary>
    public readonly struct CacheKey<T> {

        /// <summary>
        /// creates a new <see cref="CacheKey{T}"/>
        /// </summary>
        /// <param name="objectType">type of object</param>
        /// <param name="id">id of object</param>
        /// <param name="revision">object revision</param>
        public CacheKey(Type objectType, T id, int revision) {
            ObjectType = objectType;
            Id = id;
            Revision = revision;
        }

        public Type ObjectType { get; }

        public T Id { get; }

        public int Revision { get; }

        bool Equals(CacheKey<T> other) {
            return ObjectType == other.ObjectType && Id.Equals(other.Id) && Revision == other.Revision;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != GetType()) return false;
            return Equals((CacheKey<T>) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode() {
            return HashCode.Combine(ObjectType, Id, Revision);
        }

        /// <inheritdoc />
        public override string ToString() {
            return $"{ObjectType} {Id}.{Revision}";
        }
    }
}