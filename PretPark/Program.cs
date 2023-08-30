namespace Kaart
{
    public class Coordinaat
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
                return !Console.IsOutputRedirected && !Console.IsInputRedirected && !Console.IsErrorRedirected && Console.CursorLeft >= 0 && Console.CursorTop >= 0;
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
                lengteBerekend = (float)Math.Sqrt((van.x - naar.x) * (van.x - naar.x) + (van.y - naar.y) * (van.y - naar.y));
            return lengteBerekend.Value;
        }
        public void TekenConsole(ConsoleTekener t)
        {
            float deltaX = naar.x - van.x;
            float deltaY = naar.y - van.y;
            float length = Lengte();

            for (int i = 0; i < (int)length; i++)
            {
                float factor = i / length;
                int x = (int)Math.Round(van.x + deltaX * factor);
                int y = (int)Math.Round(van.y + deltaY * factor);
                t.SchrijfOp(new Coordinaat(x, y), "#");
            }

            t.SchrijfOp(new Coordinaat((int)Math.Round(van.x + deltaX * .5), (int)Math.Round(van.y + deltaY * .5)), (1000 * length).metSuffixen());
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
            Kaart k = new Kaart(30, 30);
            Pad p1 = new Pad();
            p1.van = new Coordinaat(2, 5);
            p1.naar = new Coordinaat(12, 30);
            k.VoegPadToe(p1);
            Pad p2 = new Pad();
            p2.van = new Coordinaat(26, 4);
            p2.naar = new Coordinaat(10, 5);
            k.VoegPadToe(p2);
            k.VoegItemToe(new Attractie(k, new Coordinaat(15, 15)));
            k.VoegItemToe(new Attractie(k, new Coordinaat(20, 15)));
            k.VoegItemToe(new Attractie(k, new Coordinaat(5, 18)));
            k.Teken(new ConsoleTekener());
            //Console.WriteLine("Deze kaart is schaal 1:1000");

            //Hier wordt de schaal getekend met behulp van de ConsoleTekener
            new ConsoleTekener().SchrijfOp(new Coordinaat(0, k.Hoogte + 1), "Deze kaart is schaal 1:1000");
            //Console.WriteLine("Druk op een toets om door te gaan...");
            Console.Read();
        }
    }
}