using etuds.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace EtUdS.Server.Repositories.Equipe;

public class EquipeRepository : BaseRepository, IEquipeRepository
{
    protected readonly AppDbContext _context;

    public EquipeRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }
    
    public async Task<etuds.Server.Entities.Equipe> GetByMembreId(int membreId)
    {
        var membre = await _context.Membres
            .FirstOrDefaultAsync(m => m.Id == membreId);
        
        var equipe = await _context.Equipes
            .FirstOrDefaultAsync(e => e.Id == membre.IdEquipe);

        return equipe;
    }
}