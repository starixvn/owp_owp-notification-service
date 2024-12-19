using OWP.PreMatchNotification.Assignment.Services.OtherAssignment;

namespace OWP.PreMatchNotification.UnitTests.Assignment.OtherAssignment;

public class OtherAssignmentDetectorUnitTestsBase
{
    public AssignmentSnapshot OldSnapshot { get; set; }

    public AssignmentSnapshot NewSnapshot { get; set; }

    public OtherAssignmentDetectorUnitTestsBase()
    {
        SetUp();
    }

    private void SetUp()
    {
        OldSnapshot = new AssignmentSnapshot
        {
            LeagueName = "League",

            PreMatchLeagueId = 123,

            IsDay = true,

            Hdp = "sa",

            HdpUserId = 8,

            Ou = "sa",

            OuUserId = 8,

            HdpTakeOver = "sa",

            HdpTakeOverUserId = 8,

            OuTakeOver = "sa",

            OuTakeOverUserId = 8,

            OtherHdp = "sa",

            OtherHdpUserId = 8,

            OtherOu = "sa",

            OtherOuUserId = 8,

            OtherHdpTakeOver = "sa",

            OtherHdpTakeOverUserId = 8,

            OtherOuTakeOver = "sa",

            OtherOuTakeOverUserId = 8
        };

        NewSnapshot = new AssignmentSnapshot
        {
            LeagueName = "League",

            PreMatchLeagueId = 123,

            IsDay = true,

            Hdp = "sa",

            HdpUserId = 8,

            Ou = "sa",

            OuUserId = 8,

            HdpTakeOver = "sa",

            HdpTakeOverUserId = 8,

            OuTakeOver = "sa",

            OuTakeOverUserId = 8,

            OtherHdp = "sa",

            OtherHdpUserId = 8,

            OtherOu = "sa",

            OtherOuUserId = 8,

            OtherHdpTakeOver = "sa",

            OtherHdpTakeOverUserId = 8,

            OtherOuTakeOver = "sa",

            OtherOuTakeOverUserId = 8
        };
    }
}

public class OtherAssignmentDetectorUnitTests : IClassFixture<OtherAssignmentDetectorUnitTestsBase>
{
    private readonly OtherAssignmentDetectorUnitTestsBase setUp;

    private readonly OtherAssignmentDetectorImp otherAssignment;

    public OtherAssignmentDetectorUnitTests(OtherAssignmentDetectorUnitTestsBase setUp)
    {
        var changedValueHdpDetector = Substitute.For<ChangedValueHdpDetector>();
        var changedValueHdpTakeOverDetector = Substitute.For<ChangedValueHdpTakeOverDetector>();
        var changedValueOuDetector = Substitute.For<ChangedValueOuDetector>();
        var changedValueOuTakeOverDetector = Substitute.For<ChangedValueOuTakeOverDetector>();

        changedValueHdpDetector.DetectMain(Arg.Any<AssignmentSnapshot>(), Arg.Any<AssignmentSnapshot>(), Arg.Any<List<ChangedValue>>(), Arg.Any<int>());
        changedValueHdpDetector.DetectMain(Arg.Any<AssignmentSnapshot>(), Arg.Any<AssignmentSnapshot>(), Arg.Any<List<ChangedValue>>(), Arg.Any<int>());
        changedValueOuDetector.DetectMain(Arg.Any<AssignmentSnapshot>(), Arg.Any<AssignmentSnapshot>(), Arg.Any<List<ChangedValue>>(), Arg.Any<int>());
        changedValueOuDetector.DetectMain(Arg.Any<AssignmentSnapshot>(), Arg.Any<AssignmentSnapshot>(), Arg.Any<List<ChangedValue>>(), Arg.Any<int>());
        changedValueHdpTakeOverDetector.DetectMain(Arg.Any<AssignmentSnapshot>(), Arg.Any<AssignmentSnapshot>(), Arg.Any<List<ChangedValue>>(), Arg.Any<int>());
        changedValueHdpTakeOverDetector.DetectMain(Arg.Any<AssignmentSnapshot>(), Arg.Any<AssignmentSnapshot>(), Arg.Any<List<ChangedValue>>(), Arg.Any<int>());
        changedValueOuTakeOverDetector.DetectMain(Arg.Any<AssignmentSnapshot>(), Arg.Any<AssignmentSnapshot>(), Arg.Any<List<ChangedValue>>(), Arg.Any<int>());
        changedValueOuTakeOverDetector.DetectOther(Arg.Any<AssignmentSnapshot>(), Arg.Any<AssignmentSnapshot>(), Arg.Any<List<ChangedValue>>(), Arg.Any<int>());
        this.setUp = setUp;

        otherAssignment = new OtherAssignmentDetectorImp(changedValueHdpDetector,
           changedValueHdpTakeOverDetector,
           changedValueOuDetector,
           changedValueOuTakeOverDetector);
    }

    [Fact]
    public void DetectChangeOtherAssignmentExceptHdp_NotChangeAssignmentHdp_hdpNotEmpty_ReturnDefaultValue()
    {
        var actual = new List<ChangedValue>();
        otherAssignment.DetectChangeOtherAssignmentExceptHdp(setUp.OldSnapshot, setUp.NewSnapshot, actual);
        DeepAssert.Equal([], actual);
    }

    [Fact]
    public void DetectChangeOtherAssignmentExceptHdp_NotChangeAssignmentHdp_hdpEmpty_ReturnDefaultValue()
    {
        setUp.OldSnapshot.Hdp = string.Empty;
        setUp.OldSnapshot.HdpUserId = 0;
        var actual = new List<ChangedValue>();
        otherAssignment.DetectChangeOtherAssignmentExceptHdp(setUp.OldSnapshot, setUp.NewSnapshot, actual);
        DeepAssert.Equal([], actual);
    }

    [Fact]
    public void DetectChangeOtherAssignmentExceptOtherHdp_NotChangeAssignmentOtherHdp_OtherHdpNotEmpty_ReturnDefaultValue()
    {
        var actual = new List<ChangedValue>();
        otherAssignment.DetectChangeAssignmentExceptOtherHdp(setUp.OldSnapshot, setUp.NewSnapshot, actual);
        DeepAssert.Equal([], actual);
    }

    [Fact]
    public void DetectChangeOtherAssignmentExceptOtherHdp_NotChangeAssignmentOtherHdp_OtherHdpEmpty_ReturnDefaultValue()
    {
        setUp.OldSnapshot.OtherHdp = string.Empty;
        setUp.OldSnapshot.OtherHdpUserId = 0;
        var actual = new List<ChangedValue>();
        otherAssignment.DetectChangeAssignmentExceptOtherHdp(setUp.OldSnapshot, setUp.NewSnapshot, actual);
        DeepAssert.Equal([], actual);
    }

    [Fact]
    public void DetectChangeOtherAssignmentExceptOu_NotChangeAssignmentOu_OuNotEmpty_ReturnDefaultValue()
    {
        var actual = new List<ChangedValue>();
        otherAssignment.DetectChangeAssignmentExceptOu(setUp.OldSnapshot, setUp.NewSnapshot, actual);
        DeepAssert.Equal([], actual);
    }

    [Fact]
    public void DetectChangeOtherAssignmentExceptOu_NotChangeAssignmentOu_OuEmpty_ReturnDefaultValue()
    {
        setUp.OldSnapshot.Ou = string.Empty;
        setUp.OldSnapshot.OuUserId = 0;
        var actual = new List<ChangedValue>();
        otherAssignment.DetectChangeAssignmentExceptOu(setUp.OldSnapshot, setUp.NewSnapshot, actual);
        DeepAssert.Equal([], actual);
    }

    [Fact]
    public void DetectChangeOtherAssignmentExceptOtherOu_NotChangeAssignmentOtherOu_OtherOuNotEmpty_ReturnDefaultValue()
    {
        var actual = new List<ChangedValue>();
        otherAssignment.DetectChangeAssignmentExceptOtherOu(setUp.OldSnapshot, setUp.NewSnapshot, actual);
        DeepAssert.Equal([], actual);
    }

    [Fact]
    public void DetectChangeOtherAssignmentExceptOtherOu_NotChangeAssignmentOtherOu_OtherOuEmpty_ReturnDefaultValue()
    {
        setUp.OldSnapshot.OtherOu = string.Empty;
        setUp.OldSnapshot.OtherOuUserId = 0;
        var actual = new List<ChangedValue>();
        otherAssignment.DetectChangeAssignmentExceptOtherOu(setUp.OldSnapshot, setUp.NewSnapshot, actual);
        DeepAssert.Equal([], actual);
    }

    [Fact]
    public void DetectChangeOtherAssignmentExceptHdpTOver_NotChangeAssignmentHdpTOver_HdpTOverNotEmpty_ReturnDefaultValue()
    {
        var actual = new List<ChangedValue>();
        otherAssignment.DetectChangeAssignmentExceptHdpTOver(setUp.OldSnapshot, setUp.NewSnapshot, actual);
        DeepAssert.Equal([], actual);
    }

    [Fact]
    public void DetectChangeOtherAssignmentExceptHdpTOver_NotChangeAssignmentHdpTOver_HdpTOverEmpty_ReturnDefaultValue()
    {
        setUp.OldSnapshot.HdpTakeOver = string.Empty;
        setUp.OldSnapshot.HdpTakeOverUserId = 0;
        var actual = new List<ChangedValue>();
        otherAssignment.DetectChangeAssignmentExceptHdpTOver(setUp.OldSnapshot, setUp.NewSnapshot, actual);
        DeepAssert.Equal([], actual);
    }

    [Fact]
    public void DetectChangeOtherAssignmentExceptOtherHdpTOver_NotChangeAssignmentOtherHdpTOver_OtherHdpTOverNotEmpty_ReturnDefaultValue()
    {
        var actual = new List<ChangedValue>();
        otherAssignment.DetectChangeAssignmentExceptOtherHdpTOver(setUp.OldSnapshot, setUp.NewSnapshot, actual);
        DeepAssert.Equal([], actual);
    }

    [Fact]
    public void DetectChangeOtherAssignmentExceptOtherHdpTOver_NotChangeAssignmentOtherHdpTOver_OtherHdpTOverEmpty_ReturnDefaultValue()
    {
        setUp.OldSnapshot.OtherHdpTakeOver = string.Empty;
        setUp.OldSnapshot.OtherHdpTakeOverUserId = 0;
        var actual = new List<ChangedValue>();
        otherAssignment.DetectChangeAssignmentExceptOtherHdpTOver(setUp.OldSnapshot, setUp.NewSnapshot, actual);
        DeepAssert.Equal([], actual);
    }

    [Fact]
    public void DetectChangeOtherAssignmentExceptOuTOver_NotChangeAssignmentOuTOver_OuTOverNotEmpty_ReturnDefaultValue()
    {
        var actual = new List<ChangedValue>();
        otherAssignment.DetectChangeAssignmentExceptOuTOver(setUp.OldSnapshot, setUp.NewSnapshot, actual);
        DeepAssert.Equal([], actual);
    }

    [Fact]
    public void DetectChangeOtherAssignmentExceptOuTOver_NotChangeAssignmentOuTOver_OuTOverEmpty_ReturnDefaultValue()
    {
        setUp.OldSnapshot.OuTakeOver = string.Empty;
        setUp.OldSnapshot.OuTakeOverUserId = 0;
        var actual = new List<ChangedValue>();
        otherAssignment.DetectChangeAssignmentExceptOuTOver(setUp.OldSnapshot, setUp.NewSnapshot, actual);
        DeepAssert.Equal([], actual);
    }

    [Fact]
    public void DetectChangeOtherAssignmentExceptOtherOuTOver_NotChangeAssignmentOtherOuTOver_OtherOuTOverNotEmpty_ReturnDefaultValue()
    {
        var actual = new List<ChangedValue>();
        otherAssignment.DetectChangeAssignmentExceptOtherOuTOver(setUp.OldSnapshot, setUp.NewSnapshot, actual);
        DeepAssert.Equal([], actual);
    }

    [Fact]
    public void DetectChangeOtherAssignmentExceptOtherOuTOver_NotChangeAssignmentOtherOuTOver_OtherOuTOverEmpty_ReturnDefaultValue()
    {
        setUp.OldSnapshot.OtherOuTakeOver = string.Empty;
        setUp.OldSnapshot.OtherOuTakeOverUserId = 0;
        var actual = new List<ChangedValue>();
        otherAssignment.DetectChangeAssignmentExceptOtherOuTOver(setUp.OldSnapshot, setUp.NewSnapshot, actual);
        DeepAssert.Equal([], actual);
    }
}