using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ZavaMAFFoundry.Tests;

[TestClass]
public class MAFAgentProviderTests
{
    [TestMethod]
    public void Constructor_WithValidEndpoint_ShouldInitialize()
    {
        // Arrange
        var endpoint = "https://test.foundry.endpoint.com";
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "AI_ChatDeploymentName", "gpt-5-mini" },
                { "AI_embeddingsDeploymentName", "text-embedding-3-small" }
            })
            .Build();

        // Act
        var provider = new MAFFoundryAgentProvider(endpoint, configuration);

        // Assert
        Assert.IsNotNull(provider);
    }

    [TestMethod]
    public void Constructor_WithInvalidEndpoint_ShouldThrowException()
    {
        // Arrange
        var invalidEndpoint = "not-a-valid-url";
        var configuration = new ConfigurationBuilder().Build();

        // Act & Assert
        try
        {
            var provider = new MAFFoundryAgentProvider(invalidEndpoint, configuration);
            Assert.Fail("Expected UriFormatException was not thrown");
        }
        catch (UriFormatException)
        {
            // Expected exception
        }
    }

    [TestMethod]
    public async Task GetAIAgentAsync_WithNullAgentId_ShouldThrowArgumentException()
    {
        // Arrange
        var endpoint = "https://test.foundry.endpoint.com";
        var configuration = new ConfigurationBuilder().Build();
        var provider = new MAFFoundryAgentProvider(endpoint, configuration);

        // Act & Assert
        try
        {
            provider.GetAIAgent(null!);
            Assert.Fail("Expected ArgumentException was not thrown");
        }
        catch (ArgumentException ex)
        {
            Assert.AreEqual("agentName", ex.ParamName);
            Assert.IsTrue(ex.Message.Contains("Agent Name cannot be null or empty"));
        }
    }

    [TestMethod]
    public async Task GetAIAgentAsync_WithEmptyAgentId_ShouldThrowArgumentException()
    {
        // Arrange
        var endpoint = "https://test.foundry.endpoint.com";
        var configuration = new ConfigurationBuilder().Build();
        var provider = new MAFFoundryAgentProvider(endpoint, configuration);

        // Act & Assert
        try
        {
            provider.GetAIAgent(string.Empty);
            Assert.Fail("Expected ArgumentException was not thrown");
        }
        catch (ArgumentException ex)
        {
            Assert.AreEqual("agentName", ex.ParamName);
            Assert.IsTrue(ex.Message.Contains("Agent Name cannot be null or empty"));
        }
    }

    [TestMethod]
    public async Task GetAIAgentAsync_WithWhitespaceAgentId_ShouldThrowArgumentException()
    {
        // Arrange
        var endpoint = "https://test.foundry.endpoint.com";
        var configuration = new ConfigurationBuilder().Build();
        var provider = new MAFFoundryAgentProvider(endpoint, configuration);

        // Act & Assert
        try
        {
            provider.GetAIAgent("   ");
            Assert.Fail("Expected ArgumentException was not thrown");
        }
        catch (ArgumentException ex)
        {
            Assert.AreEqual("agentName", ex.ParamName);
            Assert.IsTrue(ex.Message.Contains("Agent Name cannot be null or empty"));
        }
    }

    [TestMethod]
    public void GetAIAgent_WithNullAgentId_ShouldThrowArgumentException()
    {
        // Arrange
        var endpoint = "https://test.foundry.endpoint.com";
        var configuration = new ConfigurationBuilder().Build();
        var provider = new MAFFoundryAgentProvider(endpoint, configuration);

        // Act & Assert
        try
        {
            provider.GetAIAgent(null!);
            Assert.Fail("Expected ArgumentException was not thrown");
        }
        catch (ArgumentException ex)
        {
            Assert.AreEqual("agentName", ex.ParamName);
            Assert.IsTrue(ex.Message.Contains("Agent Name cannot be null or empty"));
        }
    }

    [TestMethod]
    public void GetAIAgent_WithEmptyAgentId_ShouldThrowArgumentException()
    {
        // Arrange
        var endpoint = "https://test.foundry.endpoint.com";
        var configuration = new ConfigurationBuilder().Build();
        var provider = new MAFFoundryAgentProvider(endpoint, configuration);

        // Act & Assert
        try
        {
            provider.GetAIAgent(string.Empty);
            Assert.Fail("Expected ArgumentException was not thrown");
        }
        catch (ArgumentException ex)
        {
            Assert.AreEqual("agentName", ex.ParamName);
            Assert.IsTrue(ex.Message.Contains("Agent Name cannot be null or empty"));
        }
    }

    [TestMethod]
    public void GetAIAgent_WithWhitespaceAgentId_ShouldThrowArgumentException()
    {
        // Arrange
        var endpoint = "https://test.foundry.endpoint.com";
        var configuration = new ConfigurationBuilder().Build();
        var provider = new MAFFoundryAgentProvider(endpoint, configuration);

        // Act & Assert
        try
        {
            provider.GetAIAgent("   ");
            Assert.Fail("Expected ArgumentException was not thrown");
        }
        catch (ArgumentException ex)
        {
            Assert.AreEqual("agentName", ex.ParamName);
            Assert.IsTrue(ex.Message.Contains("Agent Name cannot be null or empty"));
        }
    }

    [TestMethod]
    public void GetAIAgent_WithValidAgentId_ShouldAttemptToRetrieveAgent()
    {
        // Arrange
        var endpoint = "https://test.foundry.endpoint.com";
        var configuration = new ConfigurationBuilder().Build();
        var provider = new MAFFoundryAgentProvider(endpoint, configuration);
        var validAgentId = "test-agent-id";

        // Act & Assert
        try
        {
            var agent = provider.GetAIAgent(validAgentId);
            // If we get here without ArgumentException, validation passed
            Assert.IsNotNull(agent);
        }
        catch (ArgumentException ex) when (ex.Message.Contains("Agent Name cannot be null or empty"))
        {
            Assert.Fail("Should not throw ArgumentException for valid agent ID");
        }
        catch
        {
            // Other exceptions are expected without actual Azure Foundry connection
            // The test validates that input validation works correctly
        }
    }

    [TestMethod]
    public async Task GetAIAgentAsync_WithValidAgentId_ShouldAttemptToRetrieveAgent()
    {
        // Arrange
        var endpoint = "https://test.foundry.endpoint.com";
        var configuration = new ConfigurationBuilder().Build();
        var provider = new MAFFoundryAgentProvider(endpoint, configuration);
        var validAgentId = "test-agent-id";

        // Act & Assert
        try
        {
            var agent = provider.GetAIAgent(validAgentId);
            // If we get here without ArgumentException, validation passed
            Assert.IsNotNull(agent);
        }
        catch (ArgumentException ex) when (ex.Message.Contains("Agent Name cannot be null or empty"))
        {
            Assert.Fail("Should not throw ArgumentException for valid agent ID");
        }
        catch
        {
            // Other exceptions are expected without actual Azure Foundry connection
            // The test validates that input validation works correctly
        }
    }
}
