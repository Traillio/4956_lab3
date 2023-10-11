using HelloWorld;

namespace _4956_lab3.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        float[] actual = Program.Predict("../../../../data.csv");
        float actualP1 = actual[0];
        float actualP2 = actual[1];

        float expectedP1 = 0.084682435F;
        float expectedP2 = 0.91531754F;

        Assert.Equal(expectedP1, actualP1, 7);
        Assert.Equal(expectedP2, actualP2, 7);
    }
}