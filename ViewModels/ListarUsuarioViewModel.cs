using tl2_proyecto_2024_Daggam.Models;

namespace tl2_proyecto_2024_Daggam.ViewModels
{
    public class ListarUsuarioViewModel{
        public int Id { get; set; }
        public string NombreDeUsuario { get; set; } = null!;
        public RolUsuario RolUsuario { get; set; }
    }
}