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

    [Fact]
    public void TestLaplacianSmoothing()
    {
        Console.WriteLine("Hello World!");
        Dictionary<string, int> jointCounts = new Dictionary<string, int>
        {
            { "0", 1 },
            { "1", 1 },
            { "2", 1 },
            { "3", 1 },
            { "4", 1 },
            { "5", 1 },
            { "6", 1 },
            { "7", 1 },
            { "8", 1 },
            { "9", 1 },
            { "10", 1 }
        };

        Program.applyLaplacianSmoothing(jointCounts);


        for (int i = 0; i < 10; i++)
        {
            Assert.Equal(2, jointCounts[i.ToString()]);
        }
    }
}