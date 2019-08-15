using System;
using CsvHelper;
using CsvHelper.Configuration.Attributes;

namespace IT_Talent_001
{
    class Order
    {
        [Name("Order No")]
        public String OrderNo { get; set; }

        [Name("Consignment No")]
        public String ConsignmentNo { get; set; }

        [Name("Parcel Code")]
        public String ParcelCode { get; set; }

        [Name("Consignee Name")]
        public String ConsigneeName { get; set; }

        [Name("Address 1")]
        public String Address1 { get; set; }

        [Name("Address 2")]
        public String Address2 { get; set; }

        [Name("City")]
        public String City { get; set; }

        [Name("State")]
        public String State { get; set; }

        [Name("Country Code")]
        public String CountryCode { get; set; }

        [Name("Item Quantity")]
        public String ItemQuantity { get; set; }

        [Name("Item Value")]
        public String ItemValue { get; set; }

        [Name("Item Weight")]
        public String ItemWeight { get; set; }

        [Name("Item Description")]
        public String ItemDescription { get; set; }

        [Name("Item Currency")]
        public String ItemCurrency { get; set; }
    }
}
