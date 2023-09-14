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
    public string? Email { get; set; }
    public string? Wachtwoord { get; set; }
    public VerificatieToken? VerificatieToken { get; set; }

    public bool Geverifieerd()
    {
        return VerificatieToken == null || DateTime.Now <= VerificatieToken.VerloopDatum;
    }
}

public class EmailService
{
    // <summary>
    // Een <c>StuurVerificatieEmail</c> functie om een verificatie-email te sturen.
    // <summary>
    public void StuurVerificatieEmail(string email, Guid token)
    {
        Console.WriteLine($"Verificatie-email verstuurd naar: {email}, Token: {token}");
    }
}
// <summary>
//  <c>IGebruikerContext</c> In een echte implementatie zou dit een database context zijn.
// <summary>
public interface IGebruikerContext
{
    int AantalGebruikers();
    Gebruiker GetGebruiker(int index);
    void NieuweGebruiker(Gebruiker gebruiker);
}

/// <summary>
/// Simulatie van een database context.
/// </summary>
public class GebruikerContext : IGebruikerContext
{
    private List<Gebruiker> gebruikers = new List<Gebruiker>();

    public int AantalGebruikers()
    {
        return gebruikers.Count;
    }

    public Gebruiker GetGebruiker(int index)
    {
        if (index >= 0 && index < AantalGebruikers())
        {
            return gebruikers[index];
        }
        else
        {
            return null;
        }
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
        //Hier halen we de gebruiker op uit de database
        var gebruiker = gebruikerContext.GetGebruiker(0); // Eenvoudige implementatie voor testdoeleinden
        //Wat we hier doen is kijken of de gebruiker niet null is en of de email en wachtwoord gelijk zijn aan de meegegeven email en wachtwoord en of de gebruiker geverifieerd is
        return gebruiker != null && gebruiker.Email == email && gebruiker.Wachtwoord == wachtwoord && gebruiker.Geverifieerd();
    }

    public bool Verifieer(Guid token)
    {
        var gebruiker = gebruikerContext.GetGebruiker(0); // Eenvoudige implementatie voor testdoeleinden
        //Als de gebruiker bestaat en de token is niet null en de token is gelijk aan de token die is meegegeven en de gebruiker is geverifieerd
        if (gebruiker != null && gebruiker.VerificatieToken != null && gebruiker.VerificatieToken.Token == token && gebruiker.Geverifieerd())
        {
            gebruiker.VerificatieToken = null;
            return true;
        }
        return false;
    }
}