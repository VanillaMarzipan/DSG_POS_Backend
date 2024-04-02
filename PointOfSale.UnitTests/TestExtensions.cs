using System;

namespace PosTransactionManager.IntegrationTests
{
    public static class AssertDateTime
    {
        public static void Equal(DateTime? expectedDate, DateTime? actualDate, TimeSpan maximumDelta)
        {
            if (expectedDate == null && actualDate == null)
                return;
            else if (expectedDate == null)
                throw new NullReferenceException("The expected date was null");
            else if (actualDate == null)
                throw new NullReferenceException("The actual date was null");

            double totalSecondsDifference = Math.Abs(((DateTime)actualDate - (DateTime)expectedDate).TotalSeconds);

            if (totalSecondsDifference > maximumDelta.TotalSeconds)
                throw new Exception($"Expected Date: {expectedDate}, Actual Date: {actualDate} \nExpected Delta: {maximumDelta}, Actual Delta in seconds- {totalSecondsDifference}");
        }
    }
}
