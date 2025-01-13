namespace tl2_proyecto_2024_Daggam.Models
{
    public enum RolUsuario{
        Administrador=1,
        Operador
    }

    public class Usuario{
        public int Id { get; set; }
        public string NombreDeUsuario { get; set; } = null!;
        public string Password { get; set; } = null!;
        public RolUsuario RolUsuario { get; set; }
    }
}