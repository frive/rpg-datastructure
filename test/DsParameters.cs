using Rpg;

namespace Test
{
    public class DsParameters : DataStructure
    {
        [CharType(1)]
        [Index]
        public string LdaUnknown { get; set; }

        [CharType(2)]
        [Index]
        public string LdaId { get; set; }

        [CharType(8)]
        [Index]
        public string LdaDate { get; set; }

        [CharType(5)]
        [Index]
        public string LdaCompany { get; set; }

        [FixedDecimalType(1, 0)]
        [Index]
        public decimal RankNumber { get; set; }

        [FixedDecimalType(1, 0)]
        [Index]
        public decimal RankId { get; set; }

        [FixedDecimalType(6, 4)]
        [Index]
        public decimal Amount { get; set; }

        [CharType(5)]
        [Index]
        public string PaddedParameter { get; set; }

        [FixedDecimalType(8, 0)]
        [Index]
        public decimal EmptyDecimal { get; set; }
    }
}
