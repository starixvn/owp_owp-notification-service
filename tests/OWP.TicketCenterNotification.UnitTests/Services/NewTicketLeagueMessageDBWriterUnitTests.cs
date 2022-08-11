namespace OWP.TicketCenterNotification.UnitTests.Services;

public class NewTicketLeagueMessageDBWriterUnitTests
{
    private readonly NewTicketLeagueMessageDBWriterImp writer;

    private readonly TicketRepository repository;

    public NewTicketLeagueMessageDBWriterUnitTests()
    {
        repository = Substitute.For<TicketRepository>();
        writer = new NewTicketLeagueMessageDBWriterImp(repository);
    }

    [Fact]
    public async void WriteMessageToDB_MessageModel_ReturnSectionId()
    {
        // Arrange
        await repository
            .InsertNewTicketLeagueNotification(new InsertNewTicketLeagueNotificationCriteria());

        //Act
        var actual = await writer
            .Execute(new NewTicketModel(), NotificationTemplateCode.NewTicket);

        // Assert
        Assert.True(actual.Length > 0);
    }
}

