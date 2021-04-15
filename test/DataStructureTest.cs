using Xunit;
using System;

namespace Test
{

    public class DataStructureTest
    {
        [Fact]
        public void DataStructureDumpTest()
        {
            var ds = new DsParameters
            {
                LdaUnknown = "R",
                LdaId = "XX",
                LdaDate = "00000000",
                LdaCompany = "12345",
                RankId = 1,
                RankNumber = 2,
                Amount = 21.50m,
                PaddedParameter = "X"
            };

            var dumpedDS = ds.Dump();

            Assert.Equal("RXX000000001234521215000X    00000000", dumpedDS);
        }

        [Fact]
        public void DataStructureClearTest()
        {
            var ds = new DsParameters
            {
                LdaId = "XX",
                LdaDate = "00000000",
                LdaCompany = "12345",
                RankId = 1,
                RankNumber = 2,
                Amount = 21.50m,
                PaddedParameter = "X"
            };

            ds.Clear();

            Assert.Equal(0, ds.Amount);
            Assert.Null(ds.PaddedParameter);
        }

        [Fact]
        public void DsEmptyDecimalPropertyLoadTest()
        {
            var ds = new DsParameters();

            ds.Load("RXX000000001234521215000X            ");
            Assert.Equal(0, ds.EmptyDecimal);
        }

        [Fact]
        public void NegativeDecimalDsDumpTest()
        {
            var ds = new DsParameters
            {
                LdaUnknown = "R",
                LdaId = "XX",
                LdaDate = "00000000",
                LdaCompany = "12345",
                RankId = 1,
                RankNumber = 2,
                Amount = -21.50m,
                PaddedParameter = "X"
            };

            var dumpedDS = ds.Dump();

            Assert.Equal("RXX00000000123452121500@X    00000000", dumpedDS);
        }

        [Fact]
        public void NegativeDecimalDsLoadTest()
        {
            var ds = new DsParameters();
            string loadedDs = "RXX00000000123452121500CX    00000000";

            ds.Clear();
            ds.Load(loadedDs);

            Assert.Equal(ds.Amount, -21.5003m);
        }

        [Fact]
        public void DecimalExceededLengthTest()
        {
            const decimal length = 6;
            const string rawData = "150321";
            string zeroIfEmpty = string.IsNullOrWhiteSpace(rawData) ? "0" : rawData;

            string amount = (Convert.ToDecimal(zeroIfEmpty) / 100m).ToString();
            if (amount.Length > length)
            {
                amount = amount.Substring(0, Convert.ToInt32(length));
            }

            Assert.True(Convert.ToDecimal(amount) > 0);
        }
    }
}
