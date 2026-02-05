using A2A;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using System.Reflection;

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.SetMinimumLevel(LogLevel.Information);
});

var logger = loggerFactory.CreateLogger("A2A-Orchestrator");

// Retrieve configuration settings
IConfigurationRoot configRoot = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddUserSecrets(Assembly.GetExecutingAssembly())
    .Build();

var apiKey = configRoot["A2A-Orchestrator:ApiKey"] ?? throw new ArgumentException("A2A-Orchestrator:ApiKey must be provided");
var modelId = configRoot["A2A-Orchestrator:ModelId"] ?? "gpt-4.1";
var agentUrls = configRoot["A2A-Orchestrator:AgentUrls"] ?? "http://localhost:5000/;http://localhost:5001/;http://localhost:5002/";

// Create the Host agent
var hostAgent = new HostClientAgent(loggerFactory, modelId, apiKey, agentUrls!.Split(";"));

// passo 1: Retrieve agents from remote and convert to tools
IEnumerable<AIAgent> agents = await hostAgent.RetrieveAgentsFromRemoteAsync();

var tools = hostAgent.ConvertAgentsToTools();

// passo 2: Create orchestrator agent with tools
AIAgent mainAgent = hostAgent.CreateOrchestratorAgent(tools);

AgentSession session = await mainAgent.GetNewSessionAsync(CancellationToken.None);

try
{
    while (true)
    {
        // Get user message
        Console.Write("\nUser (:q or quit to exit): ");
        string? message = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(message))
        {
            Console.WriteLine("Request cannot be empty.");
            continue;
        }

        if (message is ":q" or "quit")
        {
            break;
        }

        var agentResponse = await mainAgent.RunAsync(message, session, cancellationToken: CancellationToken.None);
        foreach (var chatMessage in agentResponse.Messages)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\nAgent: {chatMessage.Text}");
            Console.ResetColor();
        }
    }
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred while running the A2A-Orchestrator");
    return;
}

internal class HostClientAgent(ILoggerFactory loggerFactory, string modelId, string apiKey, string[] a2aAgents)
{
    private ILogger _logger = loggerFactory.CreateLogger("HostClientAgent");

    List<AIAgent> aIAgents = [];

    public async Task<IEnumerable<AIAgent>> RetrieveAgentsFromRemoteAsync()
    {
        _logger.LogInformation("Reading A2A agents from remote urls");       

        foreach (var agentUrl in a2aAgents)
        {
            var uri = new Uri(agentUrl);

            var cardResolver = new A2ACardResolver(uri);

            _logger.LogInformation($"Reading remove agent: {agentUrl}");

            AIAgent agent = await cardResolver.GetAIAgentAsync();

            _logger.LogInformation($"Reading remove agent: {agentUrl}");

            aIAgents.Add(agent);
        }

        return aIAgents;
    }

    public List<AITool> ConvertAgentsToTools() => aIAgents.Select(agent => (AITool)agent.AsAIFunction()).ToList();

    public AIAgent CreateOrchestratorAgent(List<AITool> tools)
    {
        var endpointEnv = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");

        if (string.IsNullOrWhiteSpace(endpointEnv))
        {
            throw new InvalidOperationException("Environment variable 'AZURE_OPENAI_ENDPOINT' is not set. Please set it to your Azure OpenAI endpoint (e.g. https://your-resource.openai.azure.com/).");
        }

        // basicamente estamos criando um agente que tem acesso a outros agentes (convertidos em tools)
        var mainAgent = new AzureOpenAIClient(new Uri(endpointEnv), new AzureKeyCredential(apiKey))
            .GetChatClient(modelId)
            .AsAIAgent(instructions: "You specialize in handling queries for users and using your tools to provide answers.", name: "HostClient", tools: tools);

        return mainAgent;
    }
    
}