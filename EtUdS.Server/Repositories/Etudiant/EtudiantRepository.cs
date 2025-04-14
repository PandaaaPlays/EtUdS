using etuds.Server.Data;
using EtUdS.Server.Repositories;
using Microsoft.EntityFrameworkCore;

namespace etuds.Server.Repositories.Etudiant;

public class EtudiantRepository: BaseRepository, IEtudiantRepository
{
    private readonly AppDbContext _context;

    public EtudiantRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Entities.Etudiant> GetByCourriel(string courriel)
    {
        return await _context.Etudiants.FirstOrDefaultAsync(e => e.Courriel == courriel);
    }

    public async Task<Entities.Etudiant> CreateEtudiant(Entities.Etudiant etudiant)
    {
        _context.Etudiants.Add(etudiant);
        await _context.SaveChangesAsync();
        return etudiant;
    }
    
}