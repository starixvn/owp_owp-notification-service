namespace OWP.TicketCenterNotification.UnitTests.Services;

public class NewRecipientMessageDBWriterUnitTests
{
    private readonly NewRecipientMessageDBWriterImp writer;
    private readonly TicketRepository repository;

    public NewRecipientMessageDBWriterUnitTests()
    {
        repository = Substitute.For<TicketRepository>();
        writer = new NewRecipientMessageDBWriterImp(repository);
    }

    [Fact]
    public async void WriteMessageToDB_InsertSingleRecipientRequest_ReturnSectionId()
    {
        // Arrange
        await repository
            .InsertNewRecipientNotification(new InsertNewRecipientNotificationCriteria());

        //Act
        var actual = await writer
            .Execute(new NewRecipientModel());

        // Assert
        Assert.True(actual.Length > 0);
    }
}

