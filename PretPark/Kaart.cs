namespace Kaart;

public struct Coordinaat
{
    public int x { get; }
    public int y { get; }

    public Coordinaat(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
public interface Tekener
{
    public void Teken();
}

public interface Tekenbaar
{
    void TekenConsole(ConsoleTekener t);
}

public class ConsoleTekener
{
    public void Teken(Coordinaat Positie, string Text)
    {
        if (!ConsoleIsAvailable())
            throw new Exception("Console is niet beschikbaar!");
        if (Positie.x < 0 || Positie.y < 0)
            throw new Exception("Kan niet tekenen in het negatieve!");
        Console.SetCursorPosition(Positie.x, Positie.y);
        Console.WriteLine(Text);
    }
    private bool ConsoleIsAvailable()
    {
        try
        {
            return !Console.IsOutputRedirected && !Console.IsInputRedirected && !Console.IsErrorRedirected
                && Console.CursorLeft >= 0 && Console.CursorTop >= 0;
        }
        catch
        {
            return false;
        }
    }
}

public static class Float
{
    /**
     * Maak een string met een suffix van een float
     * @param number De float die omgezet moet worden naar een string
     * @return De float als string met een suffix
     */
    public static string metSuffixen(this float number)
    {
        if (number >= 1e9)
            return (number / 1e9).ToString("F1") + "B";
        if (number >= 1e6)
            return (number / 1e6).ToString("F1") + "M";
        if (number >= 1e3)
            return (number / 1e3).ToString("F1") + "K";
        return number.ToString();
    }
}

public class Pad : Tekenbaar
{
    public Coordinaat van { get; set; }
    public Coordinaat naar { get; set; }
    private float? lengteBerekend;

    public float Lengte()
    {
        if (!lengteBerekend.HasValue)
            lengteBerekend = (float)Math.Sqrt(Math.Pow(van.x - naar.x, 2) + Math.Pow(van.y - naar.y, 2));
        return lengteBerekend.Value;
    }
    /**
     * Teken het pad op de kaart
     * @param t De tekener die gebruikt moet worden om het pad te tekenen
     * De functie doet eerst een for loop 
     */
    public void TekenConsole(ConsoleTekener t)
    {
        for (int i = 0; i < (int)Lengte(); i++)
        {
            // Bereken de positie van het pad met de formule van een lijn
            float factor = i / Lengte();
            //De berekening gaat zo: van.x + (naar.x - van.x) * factor = de x positie van het pad op de kaart/
            t.Teken(new Coordinaat((int)Math.Round(van.x + (naar.x - van.x) * factor), (int)Math.Round(van.y + (naar.y - van.y) * factor)), "#");
        }
        // Teken de lengte van het pad in het midden door de lengte te delen door 2 en dan de x en y positie te berekenen
        t.Teken(new Coordinaat((int)Math.Round(van.x + (naar.x - van.x) * .5), (int)Math.Round(van.y + (naar.y - van.y) * .5)), (1000 * Lengte()).metSuffixen());
    }
}

public class Attractie : Tekenbaar
{
    private Kaart kaart;
    private Coordinaat positie;

    public Attractie(Kaart kaart, Coordinaat positie)
    {
        this.kaart = kaart;
        this.positie = positie;
    }
    /**
     * Teken de attractie op de kaart
     * @param t De tekener die gebruikt moet worden om de attractie te tekenen
     */
    public void TekenConsole(ConsoleTekener t)
    {
        t.Teken(positie, "A");
    }
}

public class Kaart
{
    public int Breedte { get; private set; }
    public int Hoogte { get; private set; }

    private Tekenbaar[] tekenbareObjecten;
    private int tekenbareCount;

    /**
     * Maak een nieuwe kaart aan
     * @param breedte De breedte van de kaart
     * @param hoogte De hoogte van de kaart
     */
    public Kaart(int breedte, int hoogte)
    {
        Breedte = breedte;
        Hoogte = hoogte;
        tekenbareObjecten = new Tekenbaar[breedte * hoogte];
        tekenbareCount = 0;
    }

    /**
     * Voeg een pad toe aan de kaart
     * @param pad Het pad dat toegevoegd moet worden aan de kaart
     */
    public void VoegPadToe(Pad pad)
    {
        if (tekenbareCount < tekenbareObjecten.Length)
            tekenbareObjecten[tekenbareCount++] = pad;
    }

    /*
     * Voeg een item toe aan de kaart
     * @param item Het item dat toegevoegd moet worden aan de kaart
     */
    public void VoegItemToe(Tekenbaar item)
    {
        if (tekenbareCount < tekenbareObjecten.Length)
        {
            tekenbareObjecten[tekenbareCount++] = item;
        }
    }
    /*
     * Teken de kaart
     * @param t De ConsoleTekener die gebruikt moet worden om de kaart te tekenen
     */
    public void Teken(ConsoleTekener t)
    {
        foreach (Tekenbaar tekenbaar in tekenbareObjecten)
        {
            if (tekenbaar != null)
                tekenbaar.TekenConsole(t);
        }
    }
}