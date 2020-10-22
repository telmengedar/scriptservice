using System;
using System.Net.Mime;

namespace ScriptService.Tests.Data {
    public class CampaignItemTargetDetails {
        
        /// <summary>
        /// unique item target id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// id of campaign item
        /// </summary>
        public long ItemId { get; set; }

        /// <summary>
        /// broker at which item is targeted at
        /// </summary>
        public long BrokerId { get; set; }
        
        /// <summary>
        /// used budget reported by broker
        /// </summary>
        public decimal UsedBudget { get; set; }

        /// <summary>
        /// clicks originating from paid placement
        /// </summary>
        public long PaidClicks { get; set; }

        /// <summary>
        /// clicks originating by random visitors
        /// </summary>
        public long NaturalClicks { get; set; }
        
        /// <summary>
        /// status about job posting
        /// </summary>
        public JobItemStatus Status { get; set; }

        /// <summary>
        /// time when target was taken online
        /// </summary>
        public DateTime? Start { get; set; }

        /// <summary>
        /// time when target was taken offline
        /// </summary>
        public DateTime? End { get; set; }

        /// <summary>
        /// id of campaign
        /// </summary>
        public long CampaignId { get; set; }

        /// <summary>
        /// job title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// date job was posted originally
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// date when validity job expires
        /// </summary>
        public DateTime? Expiration { get; set; }

        /// <summary>
        /// unique reference number
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// url where you can look up job
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// job description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// description of what is required of a candidate
        /// </summary>
        public string Requirements { get; set; }

        /// <summary>
        /// description what the employer has to offer
        /// </summary>
        public string Offers { get; set; }

        /// <summary>
        /// type of description (eg. html)
        /// </summary>
        public ContentType? DescriptionType { get; set; }

        /// <summary>
        /// salary information
        /// </summary>
        public string Salary { get; set; }

        /// <summary>
        /// type of job
        /// </summary>
        public JobType JobType { get; set; }

        /// <summary>
        /// job experience
        /// </summary>
        public string Experience { get; set; }

        /// <summary>
        /// categories linked to job
        /// </summary>
        public string Categories { get; set; }

        /// <summary>
        /// sought education level 
        /// </summary>
        public string Education { get; set; }

        /// <summary>
        /// id of feed where this was encountered last
        /// </summary>
        public Guid? FeedId { get; set; }

        /// <summary>
        /// type of remote option
        /// </summary>
        public string RemoteType { get; set; }

        /// <summary>
        /// job publisher
        /// </summary>
        public CompanyData Publisher { get; set; }

        /// <summary>
        /// company which offers the job
        /// </summary>
        public CompanyData Company { get; set; }

        /// <summary>
        /// location of job
        /// </summary>
        public AddressData Location { get; set; }

        /// <summary>
        /// time job is posted (or was posted)
        /// </summary>
        public TimeSpan PostTime { get; set; } // nullable would lead to a bug in utf8 deserialization

        /// <summary>
        /// cpc to use for this item
        /// </summary>
        public decimal? Cpc { get; set; }

    }
}