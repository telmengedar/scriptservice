namespace ScriptService.Tests.Data {
    public enum CampaignStatus {
        /// <summary>
        /// campaign was created
        /// </summary>
        Created,

        /// <summary>
        /// campaign is online
        /// </summary>
        Online,

        /// <summary>
        /// campaign went offline (manually or expired)
        /// </summary>
        Offline,

        /// <summary>
        /// campaign was run and is not supposed to be put online again
        /// </summary>
        Archived

    }
}