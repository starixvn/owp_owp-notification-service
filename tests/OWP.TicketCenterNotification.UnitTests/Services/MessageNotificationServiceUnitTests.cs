

namespace OWP.TicketCenterNotification.UnitTests.Services;

public class MessageNotificationServiceUnitTests
{
    private readonly MessageNotificationServiceImp serviceImp;
    private readonly TicketRepository repository;
    private readonly QueryMessageService queryMessageService;

    public MessageNotificationServiceUnitTests()
    {
        repository = Substitute.For<TicketRepository>();
        queryMessageService = Substitute.For<QueryMessageService>();
        serviceImp = new MessageNotificationServiceImp(queryMessageService, repository);
    }


    [Fact]
    public async void CreateMessageNotification_ReturnMessage()
    {
        // Arrange
        await repository.GetMessageUnreadByMultipleUserId(Arg.Any<List<int>>());
        await queryMessageService.QueryBySession(Arg.Any<string>());

        //Act
        var actual = await serviceImp.CreateMessageNotification(Arg.Any<NotificationSource>());

        // Assert
        Assert.True(actual.Count > 0);
    }


}
