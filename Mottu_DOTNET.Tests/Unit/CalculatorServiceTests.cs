using Xunit;
using Mottu_DOTNET.src.Application.Services;

namespace Mottu_DOTNET.Tests.Unit;

public class CalculatorServiceTests
{
    [Fact]
    public void Add_ShouldReturnSum_WhenGivenTwoNumbers()
    {
        // Arrange
        var service = new CalculatorService();

        // Act
        var result = service.Add(2, 3);

        // Assert
        Assert.Equal(5, result);
    }
}
