namespace tl2_proyecto_2024_Daggam.Models
{
    public class Tablero{
        public int Id { get; set; }
        public int IdUsuarioPropietario { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
    }
}