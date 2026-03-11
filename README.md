# A2A-Orchestrator

A .NET-based orchestrator for managing and coordinating multiple AI agents using Microsoft's Agent-to-Agent (A2A) framework with Azure OpenAI integration.

## Overview

The A2A-Orchestrator is a console application that enables coordinated interactions between multiple remote AI agents. It acts as a central hub that discovers remote agents, converts their capabilities into tools, and creates a main orchestrator agent that can intelligently delegate tasks across these agents.

## Features

- **Remote Agent Discovery**: Automatically discovers and retrieves capabilities from remote A2A agents
- **Dynamic Tool Conversion**: Converts agent capabilities into reusable tools for the orchestrator
- **Intelligent Orchestration**: Uses OpenAI's GPT models to coordinate complex multi-agent workflows
- **Interactive Console Interface**: User-friendly CLI for real-time interaction with the orchestration system
- **Comprehensive Logging**: Built-in logging support via Microsoft.Extensions.Logging

## Prerequisites

- **.NET 10.0** or later
- **Azure OpenAI** API credentials
- **Remote A2A Agents** running and accessible via HTTP endpoints
- PowerShell or compatible shell for configuration

## Configuration

The application requires the following configuration settings (via environment variables or User Secrets):

| Setting | Description | Example |
|---------|-------------|---------|
| `A2A-Orchestrator:ApiKey` | Azure OpenAI API Key | `your-api-key` |
| `A2A-Orchestrator:ModelId` | OpenAI Model ID to use | `gpt-4-turbo` |
| `A2A-Orchestrator:AgentUrls` | Semicolon-separated URLs of remote agents | `http://agent1:5000;http://agent2:5001` |

### Setting Up Configuration

#### Using User Secrets (Development):
```bash
dotnet user-secrets init
dotnet user-secrets set "A2A-Orchestrator:ApiKey" "your-api-key"
dotnet user-secrets set "A2A-Orchestrator:ModelId" "your-model-id"
dotnet user-secrets set "A2A-Orchestrator:AgentUrls" "http://agent1:5000;http://agent2:5001"
```

#### Using Environment Variables:
```bash
$env:A2A_Orchestrator:ApiKey = "your-api-key"
$env:A2A_Orchestrator:ModelId = "your-model-id"
$env:A2A_Orchestrator:AgentUrls = "http://agent1:5000;http://agent2:5001"
```

## Installation & Usage

### Build the Project

```bash
dotnet build
```

### Run the Application

```bash
dotnet run
```

### Interactive Session

Once running, the application presents an interactive prompt:

```
User (:q or quit to exit): [your message]
```

- Enter your request or query
- The orchestrator will coordinate with remote agents
- Results will be displayed in the console
- Type `:q` or `quit` to exit

## How It Works

1. **Agent Discovery**: The orchestrator connects to configured remote agent URLs and retrieves their capabilities using the A2A Card Resolver
2. **Tool Conversion**: Agent capabilities are converted into a tool set that the orchestrator can utilize
3. **Orchestration**: The main orchestrator agent is created with these tools and establishes a session
4. **Execution**: User messages are processed by the orchestrator, which delegates to appropriate remote agents as needed
5. **Response**: Results are aggregated and presented back to the user

## Architecture

### Key Components

- **HostClientAgent**: Central orchestration logic that manages agent discovery, tool conversion, and session handling
- **AIAgent**: Individual AI agent instances orchestrated by the main agent
- **AITool**: Abstraction of agent capabilities as tools
- **AgentSession**: Session state management for multi-turn conversations

## Dependencies

- **Azure.AI.OpenAI**: Azure OpenAI client library
- **Microsoft.Agents.AI.A2A**: Agent-to-Agent framework
- **Microsoft.Agents.AI.Abstractions**: AI agent abstractions
- **Microsoft.Agents.AI.OpenAI**: OpenAI integration for agents
- **Microsoft.Extensions.Configuration**: Configuration management
- **Microsoft.Extensions.Logging**: Logging framework

## Project Structure

```
A2A-Orchestrator/
├── Program.cs                    # Main application entry point
├── A2A-Orchestrator.csproj      # Project configuration
├── README.md                     # This file
└── obj/                          # Build artifacts
```

## Docker Support

The project includes Docker support configuration (Windows containers). To build a Docker image:

```dockerfile
# Build from Dockerfile (requires appropriate Dockerfile)
docker build -t a2a-orchestrator .
docker run -e A2A_Orchestrator:ApiKey=<key> a2a-orchestrator
```

## Error Handling

The application includes comprehensive error handling:

- Connection failures to remote agents are logged
- API errors are captured and reported
- Invalid user input is validated
- All errors are logged to the console with timestamps

## Logging

Logging is configured at the Information level by default. Logs include:

- Agent discovery operations
- Configuration loading
- Runtime errors and exceptions
- Agent interaction details

## Development

### Prerequisites for Development

- Visual Studio 2026 or later (or Visual Studio Code)
- .NET 10.0 SDK
- Azure OpenAI account with API access

### Project Configuration

User Secrets ID: `b1d92496-d8b2-452f-884f-0e6d9052d6b6`

This project uses:
- Nullable reference types enabled
- Implicit using statements enabled
- SDK-style project format

## Contributing

Contributions are welcome! Please ensure:

1. Code follows existing style conventions
2. All changes are tested
3. Logging is appropriate for debugging
4. Configuration-sensitive values use environment variables or User Secrets

## License

See LICENSE file for details.

## Support

For issues or questions:
- Check configuration settings are correct
- Verify remote agent endpoints are accessible
- Review application logs for error details
- Ensure Azure OpenAI credentials are valid

---

**Repository**: https://github.com/rpbs/A2A-Orchestrator