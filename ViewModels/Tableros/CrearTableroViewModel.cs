
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace tl2_proyecto_2024_Daggam.ViewModels{
    public class CrearTableroViewModel{
        [Required(ErrorMessage ="El {0} es requerido")]
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
    }
}