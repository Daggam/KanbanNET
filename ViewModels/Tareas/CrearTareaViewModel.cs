using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace tl2_proyecto_2024_Daggam.ViewModels{
    public class CrearTareaViewModel{
        // [Display(Name ="Tablero")]
        // [Required(ErrorMessage = "El campo {0} es requerido.")]
        public int IdTablero { get; set; }
        // public List<SelectListItem> Tableros {get;set;} = new List<SelectListItem>();
        [Required(ErrorMessage ="El campo {0} es requerido.")]
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string? Color { get; set; } = string.Empty;
        [Display(Name ="Asignar tarea a")]
        public int IdUsuarioAsignado { get; set; }       
        public IEnumerable<SelectListItem> UsuariosDisponibles {get;set;} = new List<SelectListItem>();
    }
}