using etuds.Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace etuds.Server.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Devoir> Devoirs { get; set; }
    public DbSet<Cours> Cours { get; set; }
    public DbSet<Seance> Seances { get; set; }
    public DbSet<Equipe> Equipes { get; set; }
    public DbSet<Membre> Membres { get; set; }
    public DbSet<Soumission> Soumissions { get; set; }
    public DbSet<Etudiant> Etudiants { get; set; }
    public DbSet<Inscription> Inscriptions { get; set; }
    public DbSet<Professeur> Professeurs { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<NoteDeCours> NotesDeCours { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<ConversationParticipant> ConversationParticipants { get; set; }
    public DbSet<MessageNonLu> MessageNonLus { get; set; }
    public DbSet<Message> Messages { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Devoir>().ToTable("devoir");
        modelBuilder.Entity<Cours>().ToTable("cours");
        modelBuilder.Entity<Etudiant>().ToTable("etudiant");
        modelBuilder.Entity<Seance>().ToTable("seance");
        modelBuilder.Entity<Soumission>().ToTable("soumission");
        modelBuilder.Entity<Membre>().ToTable("membre");
        modelBuilder.Entity<Equipe>().ToTable("equipe");
        modelBuilder.Entity<Inscription>().ToTable("inscription");
        modelBuilder.Entity<Professeur>().ToTable("professeur");
        modelBuilder.Entity<Document>().ToTable("document");
        modelBuilder.Entity<NoteDeCours>().ToTable("note_de_cours");
        modelBuilder.Entity<Conversation>().ToTable("conversation");
        modelBuilder.Entity<ConversationParticipant>().ToTable("conversation_participant");
        modelBuilder.Entity<MessageNonLu>().ToTable("message_non_lu");
        modelBuilder.Entity<Message>().ToTable("message");
    }
}