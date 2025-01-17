using System.ComponentModel.DataAnnotations;

namespace tl2_proyecto_2024_Daggam.ViewModels{
    public class LoginViewModel{
        [Required(ErrorMessage ="Ingrese un nombre de usuario")]
        [Display(Name ="Nombre de usuario")]
        public string Username { get; set; } = null!;
        [Required(ErrorMessage ="La contraseña es requerida.")]
        [DataType(DataType.Password)]
        [Display(Name ="Contraseña")]
        public string Password { get; set; } = null!;
    }
}