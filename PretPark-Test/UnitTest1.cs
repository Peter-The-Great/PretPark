namespace Pretpark_Test;

public class UnitTest1
{
    [Fact]
    public void TestPrijsToegangsKaartje()
    {
        // Arrange
        int leeftijd = 3; // Leeftijd voor de test, pas dit aan zoals nodig.

        // Act
        int prijs = PrijsToegangsKaartje(leeftijd);

        // Assert
        Assert.Equal(0, prijs); // Verwachte prijs voor de gegeven leeftijd
    }
    public int PrijsToegangsKaartje(int leeftijd)
    {
        if (leeftijd < 5)
            return 0;
        if (leeftijd < 18)
            return 5;
        return 20;
    }
}