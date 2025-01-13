using Microsoft.AspNetCore.Mvc;
namespace tl2_proyecto_2024_Daggam.Controllers;
public class UsuariosController:Controller{
    //Lista los usuarios
    public IActionResult Index(){
        return View();
    }
    public IActionResult Crear(){
        return View();
    }
};