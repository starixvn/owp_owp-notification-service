using System.ComponentModel;

namespace OWP.AssignmentUploadNotification.Constants
{
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
    }
}