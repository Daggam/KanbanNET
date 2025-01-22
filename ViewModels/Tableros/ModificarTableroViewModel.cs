
using System.ComponentModel.DataAnnotations;

namespace tl2_proyecto_2024_Daggam.ViewModels{
    public class ModificarTableroViewModel{                 
        public int Id { get; set; }
        [Required(ErrorMessage ="El campo {0} es requerido.")]
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
    }
}