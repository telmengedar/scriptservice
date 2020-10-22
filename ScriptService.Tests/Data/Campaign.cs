using System;
using ScriptService.Dto;

namespace ScriptService.Tests.Data {
    public class Campaign {
        /// <summary>
        /// targeted brokers
        /// </summary>
        public CampaignBroker[] Brokers { get; set; }

        /// <summary>
        /// jobs handled by campaign
        /// </summary>
        public CampaignItemTargetDetails[] Items { get; set; }

        /// <summary>
        /// id of campaign
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// id of source which manages this campaign
        /// </summary>
        public long? SourceId { get; set; }

        /// <summary>
        /// name of campaign
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// budget for the whole campaign
        /// </summary>
        /// <remarks>
        /// the campaign is only valid if the sum of all job budgets does not exceed the budget of the campaign
        /// </remarks>
        public decimal Budget { get; set; }

        /// <summary>
        /// price per application
        /// </summary>
        [AllowPatch]
        public decimal Cpa { get; set; }

        /// <summary>
        /// sum of paid clicks for campaign
        /// </summary>
        public decimal ClickCosts { get; set; }

        /// <summary>
        /// start of campaign
        /// </summary>
        public DateTime Start { get; set; }
        
        /// <summary>
        /// expiration of campaign
        /// </summary>
        /// <remarks>
        /// the campaign is supposed to go offline automatically when it is expired
        /// </remarks>
        public DateTime? Expiration { get; set; }

        /// <summary>
        /// status of campaign
        /// </summary>
        public CampaignStatus Status { get; set; }

    }
}