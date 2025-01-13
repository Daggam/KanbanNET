using tl2_proyecto_2024_Daggam.Models;

namespace tl2_proyecto_2024_Daggam.ViewModels
{
    public class CrearUsuarioViewModel{
        public string NombreDeUsuario { get; set; } = null!;
        public string Password { get; set; } = null!;
        public RolUsuario RolUsuario { get; set; } = RolUsuario.Operador;
    }
}