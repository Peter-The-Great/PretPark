namespace Authenticatie;

public class VerificatieToken
{
    public Guid Token { get; private set; }
    public DateTime VerloopDatum { get; private set; }

    public VerificatieToken()
    {
        Token = Guid.NewGuid();
        VerloopDatum = DateTime.Now.AddDays(3); // Token verloopt na 3 dagen
    }
}

public class Gebruiker
{
    public string Email { get; set; }
    public string Wachtwoord { get; set; }
    public VerificatieToken VerificatieToken { get; set; }

    public bool Geverifieerd()
    {
        return VerificatieToken == null || DateTime.Now <= VerificatieToken.VerloopDatum;
    }
}

public class EmailService
{
    /**
     * Simulatie van het versturen van een verificatie-email.
     * In een echte implementatie zou dit een asynchrone taak zijn.
     */
    public void StuurVerificatieEmail(string email, Guid token)
    {
        Console.WriteLine($"Verificatie-email verstuurd naar: {email}, Token: {token}");
    }
}
/**
 * Simulatie van een database context.
 * * In een echte implementatie zou dit een database context zijn.
 * */
public interface IGebruikerContext
{
    int AantalGebruikers();
    Gebruiker GetGebruiker(int index);
    void NieuweGebruiker(Gebruiker gebruiker);
}

public class GebruikerContext : IGebruikerContext
{
    private List<Gebruiker> gebruikers = new List<Gebruiker>();

    public int AantalGebruikers()
    {
        return gebruikers.Count;
    }

    public Gebruiker GetGebruiker(int index)
    {
        return gebruikers[index];
    }

    public void NieuweGebruiker(Gebruiker gebruiker)
    {
        gebruikers.Add(gebruiker);
    }
}

public class GebruikerService
{
    private readonly IGebruikerContext gebruikerContext;
    private readonly EmailService emailService;

    public GebruikerService(IGebruikerContext gebruikerContext, EmailService emailService)
    {
        this.gebruikerContext = gebruikerContext;
        this.emailService = emailService;
    }

    public void Registreer(string email, string wachtwoord)
    {
        var gebruiker = new Gebruiker
        {
            Email = email,
            Wachtwoord = wachtwoord,
            VerificatieToken = new VerificatieToken()
        };

        gebruikerContext.NieuweGebruiker(gebruiker);
        emailService.StuurVerificatieEmail(email, gebruiker.VerificatieToken.Token);
    }

    public bool Login(string email, string wachtwoord)
    {
        var gebruiker = gebruikerContext.GetGebruiker(0); // Eenvoudige implementatie voor testdoeleinden
        return gebruiker != null && gebruiker.Email == email && gebruiker.Wachtwoord == wachtwoord && gebruiker.Geverifieerd();
    }

    public bool Verifieer(Guid token)
    {
        var gebruiker = gebruikerContext.GetGebruiker(0); // Eenvoudige implementatie voor testdoeleinden
        if (gebruiker != null && gebruiker.VerificatieToken != null && gebruiker.VerificatieToken.Token == token && gebruiker.Geverifieerd())
        {
            gebruiker.VerificatieToken = null;
            return true;
        }
        return false;
    }
}