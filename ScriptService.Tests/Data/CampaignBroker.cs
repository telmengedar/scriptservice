namespace ScriptService.Tests.Data {
    public class CampaignBroker {
        /// <summary>
        /// id of campaign
        /// </summary>
        public long CampaignId { get; set; }
        
        /// <summary>
        /// id of broker
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// moniker of broker
        /// </summary>
        public string Moniker { get; set; }

        /// <summary>
        /// broker name
        /// </summary>
        public string Name { get; set; }

    }
}