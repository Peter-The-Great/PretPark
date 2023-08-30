namespace Kaart
{
    public interface ICoordinaat
    {
        int x { get; }
        int y { get; }
    }
    public class Coordinaat<T> where T : ICoordinaat
    {
        public T x { get; set; }
        public T y { get; set; }

        public Coordinaat(T x, T y)
        {
            this.x = x;
            this.y = y;
        }
    }
    public struct Coordinaat
    {
        public int x { get; set; }
        public int y { get; set; }

        public Coordinaat(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public interface Tekenbaar
    {
        void TekenConsole(ConsoleTekener t);
    }

    public class ConsoleTekener
    {
        public void SchrijfOp(Coordinaat Positie, string Text)
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
                t.SchrijfOp(new Coordinaat((int)Math.Round(van.x + (naar.x - van.x) * factor), (int)Math.Round(van.y + (naar.y - van.y) * factor)), "#");
            }
            t.SchrijfOp(new Coordinaat((int)Math.Round(van.x + (naar.x - van.x) * .5), (int)Math.Round(van.y + (naar.y - van.y) * .5)), (1000 * Lengte()).metSuffixen());
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
            t.SchrijfOp(positie, "A");
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

    class Program
    {
        static void Main(string[] args)
        {
            //Hier wordt de kaart getekend met behulp van de ConsoleTekener
            Kaart k = new Kaart(30, 30);
            //Hier worden de paden en attracties toegevoegd
            Pad p1 = new Pad();
            //Met de coordinaten van de paden en attracties
            p1.van = new Coordinaat(2, 5);
            p1.naar = new Coordinaat(12, 30);
            //Het pad wordt toegevoegd aan de kaart
            k.VoegPadToe(p1);
            //Een Tweede pad wordt toegevoegd aan de kaart
            Pad p2 = new Pad();
            p2.van = new Coordinaat(26, 4);
            p2.naar = new Coordinaat(10, 5);
            k.VoegPadToe(p2);
            //Er worden 3 attracties toegevoegd aan de kaart
            k.VoegItemToe(new Attractie(k, new Coordinaat(15, 15)));
            k.VoegItemToe(new Attractie(k, new Coordinaat(20, 15)));
            k.VoegItemToe(new Attractie(k, new Coordinaat(5, 18)));
            
            //Hierna wordt een console tekener aangemaakt
            k.Teken(new ConsoleTekener());

            //Hier wordt de schaal getekend met behulp van de ConsoleTekener
            new ConsoleTekener().SchrijfOp(new Coordinaat(0, k.Hoogte + 1), "Deze kaart is schaal 1:1000");
            //Console.WriteLine("Druk op een toets om door te gaan...");
            Console.Read();
        }
    }
}