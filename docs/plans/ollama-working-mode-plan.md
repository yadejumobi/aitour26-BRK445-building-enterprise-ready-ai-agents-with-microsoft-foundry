# Plan: Add Ollama Local LLM Working Mode

## Goal

Add a new working mode that creates agents using local LLMs via Ollama, with `ministral-3` for chat and `all-minilm` for embeddings.

## Overview

This plan adds a new `MafOllama` working mode that uses Ollama for local LLM inference, following the established patterns in `ZavaMAFLocal` and `ZavaMAFFoundry`.

---

## Implementation Steps

### Step 1: Add `MafOllama` Enum Value

**File:** `src/ZavaWorkingModes/WorkingMode.cs`

- Add `MafOllama` to the `WorkingMode` enum
- Extend `WorkingModeProvider` with:
  - Short name: `"maf_ollama"`
  - Display name: `"MAF Ollama"`
  - Description: `"Microsoft Agent Framework using locally hosted Ollama models (ministral-3 for chat, all-minilm for embeddings)"`
  - Icon: `Icons.Material.Filled.Memory` (or similar)
  - Include in `AllModes()` enumeration

---

### Step 2: Create `ZavaMAFOllama` Class Library

**Location:** `src/ZavaMAFOllama/`

**NuGet Dependencies:**

- `OllamaSharp` (latest stable)
- `Microsoft.Agents.AI.Abstractions` (1.0.0-preview.251219.1)
- `Microsoft.Agents.AI.AIAgents` (1.0.0-preview.251219.1)
- `Microsoft.Agents.AI.Workflows` (1.0.0-preview.251219.1)
- `Microsoft.Agents.AI.Templates` (1.0.0-preview.251219.1)
- `Microsoft.Agents.AI.Hosting` (1.0.0-alpha.251219.1)
- `Microsoft.Extensions.AI` (1.18.0-beta.2)

**Files to Create:**

#### 2.1 `ZavaMAFOllama.csproj`

Project file with package references.

#### 2.2 `MAFOllamaAgentProvider.cs`

```csharp
public class MAFOllamaAgentProvider
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IChatClient _chatClient;

    public MAFOllamaAgentProvider(IServiceProvider serviceProvider, IChatClient chatClient);
    public AIAgent GetAgentByName(string agentName);
    public AIAgent GetLocalAgentByName(AgentType agent);
    public Workflow GetLocalWorkflowByName(string workflowName);
}
```

#### 2.3 `MAFOllamaAgentExtensions.cs`

```csharp
public static class MAFOllamaAgentExtensions
{
    public static WebApplicationBuilder AddMAFOllamaAgents(this WebApplicationBuilder builder);
    public static WebApplicationBuilder AddMAFOllamaWorkflows(this WebApplicationBuilder builder);
}
```

**Configuration:**

- Environment variable: `Ollama__Endpoint` (default: `http://localhost:11434`)
- Chat model: `ministral-3`
- Embedding model: `all-minilm`

---

### Step 3: Add Controllers to Demo Services

#### 3.1 SingleAgentDemo

**File:** `src/SingleAgentDemo/Controllers/SingleAgentControllerMAFOllama.cs`

- Route: `/api/singleagent/maf_ollama`
- Pattern from: `SingleAgentControllerMAFLocal.cs`
- Inject: `MAFOllamaAgentProvider`

**File:** `src/SingleAgentDemo/Program.cs`

- Add: `builder.AddMAFOllamaAgents();`

#### 3.2 MultiAgentDemo

**File:** `src/MultiAgentDemo/Controllers/MultiAgentControllerMAFOllama.cs`

- Route: `/api/multiagent/maf_ollama`
- Pattern from: `MultiAgentControllerMAFLocal.cs`
- Inject: `MAFOllamaAgentProvider`

**File:** `src/MultiAgentDemo/Program.cs`

- Add: `builder.AddMAFOllamaAgents();`

---

### Step 4: Update Store Frontend Services

**File:** `src/Store/Services/SingleAgentService.cs`

Add to switch statement:

```csharp
WorkingMode.MafOllama => "maf_ollama",
```

**File:** `src/Store/Services/MultiAgentService.cs`

Add to switch statement:

```csharp
WorkingMode.MafOllama => "maf_ollama",
```

> **Note:** `AgentFrameworkSelector.razor` will auto-populate from `WorkingModeProvider.GetAllModes()` - no changes needed.

---

### Step 5: Configure ZavaAppHost

**File:** `src/ZavaAppHost/Program.cs`

Options:

1. **Add Ollama as Aspire container:**

   ```csharp
   var ollama = builder.AddContainer("ollama", "ollama/ollama")
       .WithEndpoint(port: 11434, targetPort: 11434, name: "ollama-api");
   ```

2. **Or use external connection string:**

   ```csharp
   var ollamaEndpoint = builder.AddConnectionString("ollamaEndpoint");
   ```

Pass to services:

```csharp
.WithEnvironment("Ollama__Endpoint", ollama.GetEndpoint("ollama-api"))
```

---

### Step 6: Validate Client-to-Service Endpoints

Verify endpoint paths match between:

| Component | Path Pattern |
|-----------|--------------|
| Frontend SingleAgentService | `/api/singleagent/maf_ollama/analyze` |
| Backend SingleAgentControllerMAFOllama | `/api/singleagent/maf_ollama/analyze` |
| Frontend MultiAgentService | `/api/multiagent/maf_ollama/...` |
| Backend MultiAgentControllerMAFOllama | `/api/multiagent/maf_ollama/...` |

---

## Files to Create (New)

| File | Description |
|------|-------------|
| `src/ZavaMAFOllama/ZavaMAFOllama.csproj` | Project file |
| `src/ZavaMAFOllama/MAFOllamaAgentProvider.cs` | Agent provider class |
| `src/ZavaMAFOllama/MAFOllamaAgentExtensions.cs` | DI extensions |
| `src/SingleAgentDemo/Controllers/SingleAgentControllerMAFOllama.cs` | Single agent controller |
| `src/MultiAgentDemo/Controllers/MultiAgentControllerMAFOllama.cs` | Multi agent controller |

---

## Files to Modify (Existing)

| File | Change |
|------|--------|
| `src/ZavaWorkingModes/WorkingMode.cs` | Add `MafOllama` enum + metadata |
| `src/SingleAgentDemo/Program.cs` | Register Ollama agents |
| `src/MultiAgentDemo/Program.cs` | Register Ollama agents |
| `src/Store/Services/SingleAgentService.cs` | Add endpoint mapping |
| `src/Store/Services/MultiAgentService.cs` | Add endpoint mapping |
| `src/ZavaAppHost/Program.cs` | Add Ollama resource/config |
| `src/BRK445-Zava-Aspire.slnx` | Add ZavaMAFOllama project reference |

---

## Open Questions / Considerations

### 1. Ollama Container Management

- **Option A:** Add Ollama as Aspire container resource (`builder.AddContainer("ollama", ...)`)
- **Option B:** Expect user to run Ollama externally and provide endpoint URL

**Recommendation:** Option A for development, Option B for production flexibility.

### 2. Fallback Behavior

Should the mode gracefully degrade or throw clear error messages when Ollama server isn't running?

**Recommendation:** Fail fast with clear instructions on how to start Ollama and pull required models.

### 3. Model Availability Validation

Should `AddMAFOllamaAgents()` verify that `ministral-3` and `all-minilm` models are pulled in Ollama before registering agents?

**Recommendation:** Add startup health check that logs warnings if models are missing, with instructions:

```
ollama pull ministral-3
ollama pull all-minilm
```

### 4. Service Registration Order

Since `ZavaMAFLocal` currently depends on `IChatClient` being registered by `ZavaMAFFoundry`, consider:

- Making `ZavaMAFOllama` self-contained (registers its own `IChatClient`)
- Or using keyed services to allow multiple `IChatClient` implementations

**Recommendation:** Use keyed services:

```csharp
builder.Services.AddKeyedSingleton<IChatClient>("ollama", ollamaChatClient);
```

### 5. Potential Improvements to Existing Code

After reviewing the codebase, these improvements could enhance the solution:

1. **Consolidate agent provider interfaces:** Create `IAgentProvider` interface that `MAFLocalAgentProvider`, `MAFFoundryAgentProvider`, and `MAFOllamaAgentProvider` all implement.

2. **Configuration abstraction:** Use `IOptions<OllamaConfiguration>` pattern instead of direct environment variable reads.

3. **Health checks:** Add ASP.NET Core health checks for Ollama connectivity.

4. **Model configurability:** Allow model names to be configured via appsettings.json instead of hardcoded.

---

## Default Configuration

```json
{
  "Ollama": {
    "Endpoint": "http://localhost:11434",
    "ChatModel": "ministral-3",
    "EmbeddingModel": "all-minilm"
  }
}
```

---

## Testing Checklist

- [ ] Ollama container starts successfully in Aspire
- [ ] `ministral-3` model responds to chat requests
- [ ] `all-minilm` model generates embeddings
- [ ] SingleAgentDemo `/api/singleagent/maf_ollama/analyze` works
- [ ] MultiAgentDemo workflows execute successfully
- [ ] Frontend mode selector shows "MAF Ollama" option
- [ ] Frontend correctly routes to Ollama endpoints
- [ ] Error handling when Ollama is unavailable
- [ ] Model pulling instructions are clear in logs
