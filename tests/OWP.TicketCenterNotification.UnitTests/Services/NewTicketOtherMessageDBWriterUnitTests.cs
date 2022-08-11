namespace OWP.TicketCenterNotification.UnitTests.Services;

public class NewTicketOtherMessageDBWriterUnitTests
{
    private readonly NewTicketOtherMessageDBWriterImp writer;

    private readonly TicketRepository repository;

    public NewTicketOtherMessageDBWriterUnitTests()
    {
        repository = Substitute.For<TicketRepository>();
        writer = new NewTicketOtherMessageDBWriterImp(repository);
    }

    [Fact]
    public async void WriteMessageToDB_MessageModel_ReturnSectionId()
    {
        // Arrange
        await repository
            .InsertNewTicketOtherNotification(new InsertNewTicketOtherNotificationCriteria());

        //Act
        var actual = await writer
            .Execute(new NewTicketModel(), NotificationTemplateCode.NewTicket);

        // Assert
        Assert.True(actual.Length > 0);
    }

}
