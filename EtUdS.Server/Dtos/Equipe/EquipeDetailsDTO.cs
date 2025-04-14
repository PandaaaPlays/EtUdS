using etuds.Server.Entities;

namespace EtUdS.Server.Dtos;

public class EquipeDetailsDTO
{
    public Equipe Equipe { get; set; }
    public List<MembreDetailsDTO> Etudiants { get; set; }
}