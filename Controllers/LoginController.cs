using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using tl2_proyecto_2024_Daggam.Repositorios;
using tl2_proyecto_2024_Daggam.ViewModels;
namespace tl2_proyecto_2024_Daggam.Controllers;
public class LoginController:Controller{
    private readonly IRepositorioUsuarios repositorioUsuarios;

    private readonly IDataProtector protector;
    public LoginController(IRepositorioUsuarios repositorioUsuarios, IDataProtectionProvider provider)
    {
        this.repositorioUsuarios = repositorioUsuarios;
        protector = provider.CreateProtector("Kanban.UsuariosController");
    }
    public IActionResult Index(){
        if(HttpContext.Session.GetInt32("usuarioId") is not null) return RedirectToAction("Index","Tableros");
        var model = new LoginViewModel();
        return View(model);
    }
    [HttpPost]
    public IActionResult Index(LoginViewModel loginvm){
        var usuario = repositorioUsuarios.ObtenerUsuario(loginvm.Username);
        //El usuario no existe o la contraseña es incorrecta
        //Cambiar esto por hashing
        if(usuario is null || protector.Unprotect(usuario.Password) != loginvm.Password){
            ModelState.AddModelError("UsuarioInvalido","El usuario y/o contraseña son incorrectos.");
        }
        if(!ModelState.IsValid){
            return View(loginvm);
        }
        HttpContext.Session.SetString("rol",usuario!.RolUsuario.ToString().ToLower());
        HttpContext.Session.SetInt32("usuarioId",usuario.Id);        
        return RedirectToAction("Index","Tableros");
    }

    public IActionResult Logout(){
        Response.Cookies.Delete("AuthCookie");
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }
};