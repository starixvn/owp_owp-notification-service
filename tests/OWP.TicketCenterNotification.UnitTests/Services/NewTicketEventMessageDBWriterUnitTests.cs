
namespace OWP.TicketCenterNotification.UnitTests.Services;

public class NewTicketEventMessageDBWriterUnitTests
{
    private readonly NewTicketMatchMessageDBWriterImp writer;

    private readonly TicketRepository repository;

    public NewTicketEventMessageDBWriterUnitTests()
    {
        repository = Substitute.For<TicketRepository>();
        writer = new NewTicketMatchMessageDBWriterImp(repository);
    }

    [Fact]
    public async Task WriteMessageToDB_MessageModel_ReturnSectionId()
    {
        // Arrange
        await repository
            .InsertNewTicketMatchNotification(new InsertNewTicketMatchNotificationCriteria());

        //Act
        var actual = await writer
            .Execute(new NewTicketModel(), NotificationTemplateCode.NewTicket);

        // Assert
        Assert.True(actual.Length > 0);
    }

}