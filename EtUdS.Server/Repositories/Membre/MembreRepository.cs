using etuds.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace EtUdS.Server.Repositories.Membre;

public class MembreRepository : BaseRepository, IMembreRepository
{
    protected readonly AppDbContext _context;

    public MembreRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<etuds.Server.Entities.Membre>> GetAllMembresByEquipeId(int equipeId)
    {
        return await _context.Membres.Where(m => m.IdEquipe == equipeId).ToListAsync();
    }

    public async Task<IEnumerable<int>> GetIdEtudiantsNonDisponiblesPourDevoir(int devoirId)
    {
        return await _context.Membres
            .Where(m => m.Accepte &&
                        _context.Equipes.Any(e => e.Id == m.IdEquipe && e.IdDevoir == devoirId) &&
                        _context.Membres.Count(mem => mem.IdEquipe == m.IdEquipe) > 1)
            .Select(m => m.IdEtudiant)
            .ToListAsync();
    }
}