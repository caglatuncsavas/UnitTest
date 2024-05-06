namespace ValueSamples.Tests.UnitTest;

using CalculatorLibrary;
using FluentAssertions;
using System.Configuration;
using ValueSamples = CalculatorLibrary.ValueSamples;
public class ValueSamplesTests
{
    //Arrange
    private readonly ValueSamples _sut = new();
    [Fact]
    public void StringAssertionExample()
    {
        //Act
        var fullName = _sut.FullName;

        //Assert
        fullName.Should().Be("Çağla Tunç Savaş");
        fullName.Should().NotBeEmpty();
        fullName.Should().StartWith("Çağla");
        fullName.Should().EndWith("Savaş");
    }

    [Fact]
    public void NumberAssertionExample()
    {
        //Act
        var age = _sut.Age;

        //Assert
        age.Should().Be(30);
        age.Should().BePositive();
        age.Should().BeGreaterThan(20);
        age.Should().BeLessThan(40);
        age.Should().BeInRange(20, 50);
    }

    [Fact]
    public void ObjectAssertionExample()
    {
        //Act
        var expectedUser = new User()
        {
            FullName = "Çağla Tunç Savaş",
            Age = 30,
            DateOfBirth = new DateOnly(1994, 10, 10)
        };

        var user = _sut.user;

        //Assert
        //BeEquivalentTo referası siliyor ve sadece değerleri üzerinden karşılaştırma yapar.
        user.Should().BeEquivalentTo(expectedUser);
    }

    [Fact]
    public void EnumrableObjectAssertionExample()
    {
        //Arrange
        var expected = new User
        {
            FullName = "Çağla Tunç Savaş",
            Age = 30,
            DateOfBirth = new(1994, 10, 10)
        };

        //Act
        var users = _sut.Users.As<User[]>();

        //Assert
        users.Should().ContainEquivalentOf(expected);
        users.Should().HaveCount(2);
        users.Should().Contain(p=> p.FullName.StartsWith("Çağla") && p.Age > 20);

    }

    [Fact]
    public void EnumrableNumberAssertionExample()
    {
        //Act
        var numbers = _sut.Numbers.As<int[]>();

        //Assert
        numbers.Should().Contain(30);
      
    }

    [Fact]
    public void ExceptionThrownAssertionExample()
    {
        //Act
        Action result = () => _sut.Divide(1, 0);

        //Assert
        result.Should().Throw<DivideByZeroException>();
        //.WithMessage("Attempted to divide by zero.");
    }

    [Fact]
    public void EventRaisedAssertionExample()
    {
        //Arrange
        var monitorSubject = _sut.Monitor();

        //Act
        _sut.RaiseExampleEvent();

        //Assert
        monitorSubject.Should().Raise("ExampleEvent");
    }

    [Fact]
    public void TestingInternalMembersExample()
    {
        //Act
        var number = _sut.InternalSecretNumber;

        //Assert
        number.Should().Be(10);
    }



    //out ref
    //public void TestMethod()
    //{
    //    int a = 10;
    //    Test2(ref a);

    //    Console.WriteLine(a);
    //}

    //public void Test2(ref int x)
    //{
    //    x += 10;
    //}
}
