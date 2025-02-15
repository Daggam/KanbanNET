using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace tl2_proyecto_2024_Daggam.ViewModels{
    public class ReasignarTareaViewModel{
        public int Id { get; set; }
        [Required(ErrorMessage = "Debe seleccionar una de las opciones.")]
        [Display(Name ="Seleccione un usuario para reasignar la tarea:")]
        public int? UsuarioId { get; set; }
        public IEnumerable<SelectListItem> UsuariosDisponibles { get; set; } = new List<SelectListItem>();
    }
}