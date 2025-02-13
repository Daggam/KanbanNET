using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using tl2_proyecto_2024_Daggam.Repositorios;
using tl2_proyecto_2024_Daggam.ViewModels;
namespace tl2_proyecto_2024_Daggam.Controllers;
public class LoginController:Controller{
    private readonly IRepositorioUsuarios repositorioUsuarios;
    private readonly ILogger<LoginController> logger;
    private readonly IDataProtector protector;
    public LoginController(IRepositorioUsuarios repositorioUsuarios,ILogger<LoginController> logger, IDataProtectionProvider provider)
    {
        this.repositorioUsuarios = repositorioUsuarios;
        this.logger = logger;
        protector = provider.CreateProtector("Kanban.UsuariosController");
    }
    public IActionResult Index(){
        try{
            if(HttpContext.Session.GetInt32("usuarioId") is not null) return RedirectToAction("Index","Tableros");
            var model = new LoginViewModel();
            return View(model);

        }catch(Exception e){
            logger.LogError(e.ToString());
            return RedirectToAction("RecursoInvalido","Home");
        }
    }
    [HttpPost]
    public IActionResult Index(LoginViewModel loginvm){
        try{
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
            logger.LogInformation($"El usuario #{usuario.Id} inició sesión.");
            return RedirectToAction("Index","Tableros");
        }catch(Exception e){
            logger.LogError(e.ToString());
            return RedirectToAction("RecursoInvalido","Home");
        }
    }
    public IActionResult Logout(){
        try{
            var usuarioId = HttpContext.Session.GetInt32("usuarioId");
            Response.Cookies.Delete("AuthCookie");
            HttpContext.Session.Clear();
            logger.LogInformation($"El usuario #{usuarioId} cerró sesión.");
            return RedirectToAction("Index");
        }catch(Exception e){
            logger.LogError(e.ToString());
            return RedirectToAction("RecursoInvalido","Home");
        }
    }
};