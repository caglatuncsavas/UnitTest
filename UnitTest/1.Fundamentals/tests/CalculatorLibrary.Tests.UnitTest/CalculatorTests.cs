using CalculatorLibrary;
using FluentAssertions;
using Xunit.Abstractions;

namespace TestProject1;

public class CalculatorTests : IDisposable, IAsyncLifetime
{
    #region Arrange
    //Arrange
    private readonly Calculator _sut = new();//System Under Test
    private readonly Guid _guid = Guid.NewGuid();
    private readonly ITestOutputHelper _outputHelper;

    public CalculatorTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _outputHelper.WriteLine("Constructor is working...");
    }
    public async Task InitializeAsync()
    {
        _outputHelper.WriteLine("InitializeAsync is working...");
        await Task.Delay(1);
    }
    #endregion
  
    [Fact]
    public void Add_ShouldAddTwoNumbers_WhenTwoNumbersAreInteger()
    {
        //Act
        var result = _sut.Add(1, 2);

        //Assert
        //Assert.Equal(3, result);
        result.Should().Be(3);
        result.Should().NotBe(4);
    }

    [Fact]
    public void Substract_ShouldSubtstractTwoNumbers_WhenTwoNumbersAreInteger()
    {
        //Act
        var result = _sut.Subtract(1, 2);

        //Assert
        //Assert.Equal(-1, result);
        result.Should().Be(-1);
        result.Should().NotBe(3);
    }

    [Fact]
    public void Multiply_ShouldMultiplyTwoNumbers_WhenTwoNumbersAreInteger()
    {
        //Act
        var result = _sut.Multiply(1, 2);

        //Assert
        result.Should().Be(2);
        result.Should().NotBe(3);
    }

    [Theory]
    [InlineData(4, 2, 2)]
    [InlineData(8, 2, 4)]
    [InlineData(0, 0, 0, Skip = "Sýfýr sýfýra bölünemez.")]
    public void Divide_ShouldDivideTwoNumbers_WhenTwoNumbersAreInteger(int a, int b, int expected)
    {
        //Act
        var result = _sut.Divide(a, b);//0,0 5,2

        //Assert
        result.Should().Be(expected);
    }

    #region Test
    [Fact(Skip = "Bu metot artýk kullanýlmýyor!")]
    public void Test1()
    {
        _outputHelper.WriteLine(_guid.ToString());
    }

    [Fact(Skip = "Bu metot artýk kullanýlmýyor!")]
    public void Test2()
    {
        _outputHelper.WriteLine(_guid.ToString());
    }
    #endregion

    #region Dispose
    public void Dispose()//Genelde integration testlerde yazýlýr
    {
        _outputHelper.WriteLine("Dispose is working...");
    }

    public async Task DisposeAsync()
    {
        _outputHelper.WriteLine("DisposeASync is working...");
        await Task.Delay(1);
    }
    #endregion

}