namespace ScriptService.Tests.Data {
    public enum JobItemStatus {
        /// <summary>
        /// broker is not targeted currently
        /// </summary>
        BrokerBlocked=-3,
        
        /// <summary>
        /// quotas have been met (eg. budget limit hit)
        /// </summary>
        HitQuota = -2,

        /// <summary>
        /// job was posted but got canceled due to expiration or manual cancellation
        /// </summary>
        Canceled = -1,

        /// <summary>
        /// job has not been posted yet
        /// </summary>
        Unposted = 0,

        /// <summary>
        /// job should get posted and will be included in generated job feeds
        /// </summary>
        Posted = 1,
    }
}