namespace OWP.TicketCenterNotification.UnitTests.Services;

public class NewTicketSupportMessageDBWriterUnitTests
{
    private readonly NewTicketSupportMessageDBWriterImp writer;

    private readonly TicketRepository repository;

    public NewTicketSupportMessageDBWriterUnitTests()
    {
        repository = Substitute.For<TicketRepository>();
        writer = new NewTicketSupportMessageDBWriterImp(repository);
    }

    [Fact]
    public async Task WriteMessageToDB_MessageModel_ReturnSectionId()
    {
        // Arrange
        await repository
            .InsertNewTicketSupportNotification(new InsertNewTicketSupportNotificationCriteria());

        //Act
        var actual = await writer
            .Execute(new NewTicketModel(), NotificationTemplateCode.NewTicket);

        // Assert
        Assert.True(actual.Length > 0);
    }

}
