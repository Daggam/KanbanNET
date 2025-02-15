namespace tl2_proyecto_2024_Daggam.Models
{
        public enum EstadoTarea{
        Ideas=1,
        ToDo,
        Doing,
        Review,
        Done
    }
    public class Tarea{
        public int Id { get; set; }
        public int IdTablero { get; set; }
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public string Color { get; set; } = null!; // Tiene un color por defecto.
        public EstadoTarea Estado { get; set; }
        public int? IdUsuarioAsignado { get; set; }
    }
}