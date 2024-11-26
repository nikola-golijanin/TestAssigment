using System.ComponentModel.DataAnnotations;

namespace TestAssigmentAPI.Options;

public record Client(string Name, string ApiKey);

public record ClientsOptions
{
    [MinLength(1, ErrorMessage = "At least one client must be defined")]
    public List<Client> Clients { get; init; } = [];

    public static bool ValidateClients(ClientsOptions options) =>
        options.Clients.All(ValidateClient);

    private static bool ValidateClient(Client client) =>
       !string.IsNullOrWhiteSpace(client.ApiKey) && !string.IsNullOrEmpty(client.Name);
}
