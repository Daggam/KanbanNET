using tl2_proyecto_2024_Daggam.Models;

namespace tl2_proyecto_2024_Daggam.ViewModels{
    public class ListarTareaViewModel{
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = string.Empty;
        public string Color { get; set; } = null!;
        public EstadoTarea Estado { get; set; }
    }
    public class PaqueteListarViewModel{
        public IEnumerable<ListarTareaViewModel> Modelo { get; set; } = null!;
        public bool EsPropietario { get; set; }
    }
}