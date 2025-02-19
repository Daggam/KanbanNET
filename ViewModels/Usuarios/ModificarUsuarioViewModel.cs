using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using tl2_proyecto_2024_Daggam.Models;

namespace tl2_proyecto_2024_Daggam.ViewModels
{
    public class ModificarUsuarioViewModel{
        public int Id { get; set; }
        [Display(Name = "Nombre de usuario")]
        [Required(ErrorMessage ="Ingrese un nombre de usuario")]
        [StringLength(maximumLength:50,MinimumLength = 3, ErrorMessage = "Ingrese un nombre de usuario entre {2} y {1} caracteres")]
        // [Remote(action:"EditarUsuarioExiste",controller:"Usuarios")]
        public string NombreDeUsuario { get; set; } = null!;
        [Display(Name ="Contraseña")]
        [Required(ErrorMessage = "Ingrese una contraseña")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name ="Rol")]
        public RolUsuario RolUsuario { get; set; } = RolUsuario.Operador;
    }
}