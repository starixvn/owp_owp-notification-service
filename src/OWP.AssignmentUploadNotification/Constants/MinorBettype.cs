using System.Collections.Immutable;
using System.ComponentModel;

namespace OWP.AssignmentUploadNotification.Constants;

public enum MinorBettype
{
    [Description("Fast Market")]
    FastMarket = -88,

    [Description("All In One")]
    AllInOne = -89,

    [Description("Specific 15 Mins")]
    Specific15Mins = -1,

    [Description("Specific 10 Mins")]
    Specific10Mins = -2,

    [Description("Specific 15 Mins Total Of Bookings")]
    Specific15MinsTotalBookings = -8,

    [Description("Specific 15 Mins Number Of Corners")]
    Specific15MinsCorners = -5,

    [Description("Total Booking")]
    TotalBooking = -6,

    [Description("No.of Corner")]
    NoOfCorner = -3,

    [Description("All In One Total Of Bookings")]
    AllInOneTotalBookings = -189,

    [Description("All In One Number Of Corners")]
    AllInOneCorners = -289,

    [Description("Fast Market Total Of Bookings")]
    FastMarketTotalBookings = -188,

    [Description("Fast Market Number Of Corners")]
    FastMarketCorners = -288,
}

public static class MinorBettypeConst
{
    public static readonly ImmutableList<int> GenerateMinorBettypeId = ImmutableList<int>.Empty.AddRange(new List<int>
    {
        (int)MinorBettype.Specific15Mins,
        (int)MinorBettype.Specific10Mins,
        (int)MinorBettype.FastMarket,
        (int)MinorBettype.AllInOne,
        (int)MinorBettype.Specific15MinsCorners,
        (int)MinorBettype.Specific15MinsTotalBookings,
        (int)MinorBettype.AllInOneTotalBookings,
        (int)MinorBettype.FastMarketTotalBookings,
        (int)MinorBettype.AllInOneCorners,
        (int)MinorBettype.FastMarketCorners
    });
}