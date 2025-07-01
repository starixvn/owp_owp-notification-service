using Olympus.OWP.Log.Data.Repository.Criteria;
using OWP.Core.Models;

namespace OWP.OddsCompilerNotification;

using Core.NotificationRequests.OddsCompiler;
using MediatR;
using Services;
using Services;
using System.Net.Http;
using System.Text;

public class OddsCompilerNotificationHandler(
    OddsCompilerMessageNotificationService oddsCompilerMessage,
    OddsCompilerRepository repository,
    IHttpClientFactory httpClientFactory)
    : IRequestHandler<OddsCompilerNotifyRequest, Unit>
{
    public async Task<Unit> Handle(OddsCompilerNotifyRequest request, CancellationToken cancellationToken)
    {
        var messages = await oddsCompilerMessage.HandleMessage(request.SessionId);
        //await MessageToTrader(messages, cancellationToken);

        return Unit.Value;
    }

    private async Task MessageToTrader(IList<MessageModel> messageModels,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!messageModels.Any())
            {
                await LogDetailedPayload(ToGmt8(DateTime.Now), "[LiveDetail]-MessageToTrader - Success", "Nothing change");
                return;
            }

            using var client = httpClientFactory.CreateClient("OWPChatBot");

            var sendTasks = messageModels.Select(async msg =>
            {
                try
                {
                    var payload = new
                    {
                        channel = $"@{msg.Recipient}",
                        text = msg.MessageContent
                    };

                    var json = System.Text.Json.JsonSerializer.Serialize(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("api/v1/chat.postMessage", content, cancellationToken).ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Failed to send message to chat server: {response.StatusCode}");
                    }

                    var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                    await LogDetailedPayload(ToGmt8(DateTime.Now), $"[LiveDetail]-MessageToTrader {msg.Recipient} - Success", responseBody).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await LogDetailedPayload(ToGmt8(DateTime.Now), $"[LiveDetail]-MessageToTrader {msg.Recipient} - Error", ex.Message).ConfigureAwait(false);
                }
            });

            await Task.WhenAll(sendTasks);
        }
        catch (Exception e)
        {
            await LogDetailedPayload(ToGmt8(DateTime.Now), "[LiveDetail]-MessageToTrader - fail", e.Message);
        }

    }

    private async Task LogDetailedPayload(DateTime CreatedDate, string Action, string Payload)
    {
        await repository.WriteLog(new OwpLogCriteria()
        {
            CreatedDate = CreatedDate,
            Action = Action,
            Payload = Payload
        });
    }

    private static DateTime ToGmt8(DateTime dateTime)
    {
        return dateTime.ToUniversalTime().AddHours(8);
    }
}