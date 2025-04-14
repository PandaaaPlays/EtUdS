using System.Net;
using System.Net.Mail;
using Npgsql;

namespace EtUdS.Server.Services.Message;

public class RappelService : BackgroundService
{
    private readonly IConfiguration _configuration;

    public RappelService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckAndSendEmails();
            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
        }
    }

    private async Task CheckAndSendEmails()
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var conn = new NpgsqlConnection(connectionString))
            {
                await conn.OpenAsync();

                string query = @"
            SELECT DISTINCT ON (c.id)
                c.titre,
                e.courriel
            FROM message_non_lu mn
            JOIN message m 
                ON mn.id_message = m.id
            JOIN etudiant e 
                ON mn.id_etudiant = e.id
            JOIN conversation c 
                ON m.id_conversation = c.id
            WHERE mn.rappel_envoye = false
                AND m.date_envoi <= NOW() - INTERVAL '1 hour'
            ORDER BY c.id, m.date_envoi DESC";

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var courrielsToSend = new List<(string titre, string courriel)>();

                    while (await reader.ReadAsync())
                    {
                        courrielsToSend.Add((
                            reader.GetString(0),
                            reader.GetString(1)
                        ));
                    }

                    foreach (var (titre, courriel) in courrielsToSend)
                    {
                        await SendEmail(titre, courriel);
                        await MarkAsSent(courriel);
                    }
                }
            }
        } 
        catch (NpgsqlException ex)
        {
            Console.WriteLine($"[CheckAndSendEmails] Erreur de connexion à PostgreSQL : {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CheckAndSendEmails] Erreur inattendue : {ex.Message}");
        }
    }
    
    private async Task MarkAsSent(string courriel)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        using (var conn = new NpgsqlConnection(connectionString))
        {
            await conn.OpenAsync();

            string updateQuery = @"
            UPDATE message_non_lu
            SET rappel_envoye = true
            WHERE id_etudiant = (
                SELECT id FROM etudiant WHERE courriel = @courriel LIMIT 1
            ) AND rappel_envoye = false;
        ";

            using (var cmd = new NpgsqlCommand(updateQuery, conn))
            {
                cmd.Parameters.AddWithValue("@courriel", courriel);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }

    private async Task SendEmail(string titre, string courriel)
    {
        try
        {
            var smtpServer = "smtp.gmail.com";
            var smtpPort = 587;
            var smtpUser = "etuds.noreply@gmail.com";
            var smtpPassword = "otgw shva wvjc fswd";

            var from = smtpUser;

            using (var client = new SmtpClient(smtpServer, smtpPort))
            {
                client.Credentials = new NetworkCredential(smtpUser, smtpPassword);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(from),
                    Subject = $"[ÉtUdS] Nouveau message dans la conversation {titre}",
                    Body =
                        $"Vous avez un nouveau message dans la conversation: {titre}\n\nCliquez ici pour le voir: https://etuds.com",
                    IsBodyHtml = false,
                };
                mailMessage.To.Add(courriel);

                await client.SendMailAsync(mailMessage);
            }
        }
        catch (SmtpException smtpEx)
        {
            Console.WriteLine($"SMTP Error: {smtpEx.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General Error: {ex.Message}");
        }
    }
}