using Azure.Identity;
using Brainz.Application.DTOs;
using Brainz.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace Brainz.Infrastructure.Services;

public class MicrosoftGraphService : IMicrosoftGraphService
{
    private readonly GraphServiceClient _graphClient;
    private readonly ILogger<MicrosoftGraphService> _logger;

    public MicrosoftGraphService(IConfiguration configuration, ILogger<MicrosoftGraphService> logger)
    {
        _logger = logger;

        var tenantId = configuration["MicrosoftGraph:TenantId"];
        var clientId = configuration["MicrosoftGraph:ClientId"];
        var clientSecret = configuration["MicrosoftGraph:ClientSecret"];

        var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        _graphClient = new GraphServiceClient(credential, new[] { "https://graph.microsoft.com/.default" });
    }

    public async Task<List<GraphUserDto>> GetAllUsersAsync()
    {
        var users = new List<GraphUserDto>();

        try
        {
            var result = await _graphClient.Users.GetAsync(config =>
            {
                config.QueryParameters.Select = new[]
                {
                    "id", "displayName", "mail", "givenName", "surname",
                    "department", "jobTitle", "accountEnabled"
                };
                config.QueryParameters.Top = 999;
                config.QueryParameters.Filter = "accountEnabled eq true";
            });

            if (result?.Value == null) return users;

            var pageIterator = PageIterator<Microsoft.Graph.Models.User, UserCollectionResponse>
                .CreatePageIterator(_graphClient, result, (user) =>
                {
                    users.Add(new GraphUserDto
                    {
                        MicrosoftId = user.Id!,
                        DisplayName = user.DisplayName ?? "",
                        Email = user.Mail ?? "",
                        GivenName = user.GivenName,
                        Surname = user.Surname,
                        Department = user.Department,
                        JobTitle = user.JobTitle
                    });
                    return true;
                });

            await pageIterator.IterateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch users from Microsoft Graph");
            throw;
        }

        return users;
    }

    public async Task<List<GraphEventDto>> GetUserEventsAsync(
        string microsoftUserId, DateTime from, DateTime to)
    {
        var events = new List<GraphEventDto>();

        try
        {
            var result = await _graphClient.Users[microsoftUserId]
                .CalendarView
                .GetAsync(config =>
                {
                    config.QueryParameters.StartDateTime = from.ToString("o");
                    config.QueryParameters.EndDateTime = to.ToString("o");
                    config.QueryParameters.Select = new[]
                    {
                        "id", "subject", "bodyPreview", "start", "end",
                        "location", "isAllDay", "organizer", "isCancelled"
                    };
                    config.QueryParameters.Top = 100;
                    config.QueryParameters.Orderby = new[] { "start/dateTime" };
                });

            if (result?.Value != null)
            {
                foreach (var evt in result.Value)
                {
                    events.Add(new GraphEventDto
                    {
                        MicrosoftEventId = evt.Id!,
                        Subject = evt.Subject ?? "(Sem assunto)",
                        BodyPreview = evt.BodyPreview,
                        StartDateTime = DateTime.Parse(evt.Start?.DateTime ?? DateTime.UtcNow.ToString("o")),
                        EndDateTime = DateTime.Parse(evt.End?.DateTime ?? DateTime.UtcNow.ToString("o")),
                        Location = evt.Location?.DisplayName,
                        IsAllDay = evt.IsAllDay ?? false,
                        OrganizerName = evt.Organizer?.EmailAddress?.Name,
                        OrganizerEmail = evt.Organizer?.EmailAddress?.Address,
                        IsCancelled = evt.IsCancelled ?? false
                    });
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch events for user {UserId}", microsoftUserId);
        }

        return events;
    }
}
