namespace OWP.TicketCenterNotification.UnitTests.Services;

public class NewMessageDBWriterUnitTests
{
    private readonly NewMessageDBWriterImp writer;
    private readonly TicketRepository repository;

    public NewMessageDBWriterUnitTests()
    {
        repository = Substitute.For<TicketRepository>();
        writer = new NewMessageDBWriterImp(repository);
    }

    [Fact]
    public async Task WriteMessageToDB_NewMessageRequest_ReturnSectionId()
    {
        // Arrange
        await repository.InsertNewMessageNotification(new InsertNewMessageNotificationCriteria());

        //Act
        var actual = await writer
            .Execute(new NewMessageModel());

        // Assert
        Assert.True(actual.Length > 0);
    }

}
