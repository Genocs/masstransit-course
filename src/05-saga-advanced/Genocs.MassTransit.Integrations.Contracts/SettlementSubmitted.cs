using System;

namespace Genocs.MassTransit.Integrations.Contracts
{

    /// <summary>
    /// This event is sent by the partner when the settlement process is completed
    /// </summary>
    public class SettlementSubmitted
    {
        public string? Id { get; set; }

        public string? Code { get; set; }
        public int AccrualMonth { get; set; }
        public int AccrualYear { get; set; }
        public DateTime ProcessedTimestamp { get; set; }
    }
}
