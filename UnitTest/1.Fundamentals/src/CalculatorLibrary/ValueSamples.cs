namespace CalculatorLibrary;
public class ValueSamples
{
    public string FullName = "Çağla Tunç Savaş";
    public int Age = 30;
    public User user = new()
    {
        FullName = "Çağla Tunç Savaş",
        Age = 30,
        DateOfBirth = new DateOnly(1994, 10, 10)
    };

    public IEnumerable<User> Users = new[]
    {
         new User()
        {
            FullName = "Çağla Tunç Savaş",
            Age = 30,
            DateOfBirth = new (1994, 10, 10)
        },
         new User()
         {
            FullName = "John Doe",
            Age = 28,
            DateOfBirth = new (1996, 10, 10)
        }
    };

    public IEnumerable<int> Numbers = new[] { 10, 20, 30 };
    public float Divide(int a, int b)
    {
        if(a == 0 || b == 0)
        {
            throw new DivideByZeroException();
        }
           
        return a / b;
    }

    public event EventHandler ExampleEvent;
    public virtual void RaiseExampleEvent()
    {
        ExampleEvent(this, EventArgs.Empty);
    }

    internal int InternalSecretNumber = 10;
    
       

}

public sealed class User
{
    public string FullName { get; set; } = string.Empty;
    public int Age { get; set; }
    public DateOnly DateOfBirth { get; set; }
}
