namespace Classy.DotNet.Responses
{
    public class ProfessionalInfoView
    {
        public string Category { get; set; }
        public string TaxId { get; set; }
        public string CompanyName { get; set; }
        public ExtendedContactInfoView CompanyContactInfo { get; set; }
        public int SettlementPeriodInDays { get; set; }
        public int RollingReservePercent { get; set; }
        public int RollingReserveTimeInDays { get; set; }
        public string DefaultCulture { get; set; }

        // TODO: can we delete this ? Doesn't really seem to be in use...
        public BankAccountView PaymentDetails { get; set; }
    }
}
