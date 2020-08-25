using NightlyCode.Database.Entities.Attributes;

namespace ScriptService.Dto {

    /// <summary>
    /// obsolete object revision
    /// </summary>
    public class ArchivedObject {

        /// <summary>
        /// object type to archive
        /// </summary>
        [Unique("objectrevision")]
        public string Type { get; set; }

        /// <summary>
        /// archived object id
        /// </summary>
        [Unique("objectrevision")]
        public long ObjectId { get; set; }

        /// <summary>
        /// archived object revision
        /// </summary>
        [Unique("objectrevision")]
        public int Revision { get; set; }

        /// <summary>
        /// archived object data
        /// </summary>
        public byte[] Data { get; set; }
    }
}