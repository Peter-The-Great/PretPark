namespace PretPark;

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

public static class ExtensionMethods
{
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

    public void TekenConsole(ConsoleTekener t)
    {
        for (int i = 0; i < (int)Lengte(); i++)
        {
            float factor = i / Lengte();
            t.Teken(new Coordinaat((int)Math.Round(van.x + (naar.x - van.x) * factor), (int)Math.Round(van.y + (naar.y - van.y) * factor)), "#");
        }
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

    public Kaart(int breedte, int hoogte)
    {
        Breedte = breedte;
        Hoogte = hoogte;
        tekenbareObjecten = new Tekenbaar[breedte * hoogte];
        tekenbareCount = 0;
    }

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