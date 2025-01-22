using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using tl2_proyecto_2024_Daggam.Models;
using tl2_proyecto_2024_Daggam.Repositorios;
using tl2_proyecto_2024_Daggam.ViewModels;
namespace tl2_proyecto_2024_Daggam.Controllers;
public class UsuariosController:Controller{
    private readonly IRepositorioUsuarios repositorioUsuarios;
    private readonly IDataProtector protector;

    public UsuariosController(IRepositorioUsuarios repositorioUsuarios, IDataProtectionProvider provider)
    {
        this.repositorioUsuarios = repositorioUsuarios;
        protector = provider.CreateProtector("Kanban.UsuariosController");
    }
    //Lista los usuarios
    public IActionResult Index(){
        if(HttpContext.Session.GetInt32("usuarioId") is null) return RedirectToAction("Index","Login");
        //Usuario no autorizado
        if(HttpContext.Session.GetString("rol") != "administrador") return RedirectToAction("Index","Home");
    
        var usuarios = repositorioUsuarios.ObtenerUsuarios();
        IEnumerable<ListarUsuarioViewModel> usuariosvm = usuarios.Select(u => 
                                                            new ListarUsuarioViewModel(){
                                                                Id = u.Id,
                                                                NombreDeUsuario = u.NombreDeUsuario,
                                                                RolUsuario = u.RolUsuario})
                                                        .ToList();
        return View(usuariosvm);
    }
    public IActionResult Crear(){
        if(HttpContext.Session.GetInt32("usuarioId") is null) return RedirectToAction("Index","Login");

        if(HttpContext.Session.GetString("rol") != "administrador") return RedirectToAction("Index","Home");
        var modelo = new CrearUsuarioViewModel();
        return View(modelo);
    }
    [HttpPost]
    public IActionResult Crear(CrearUsuarioViewModel usuariovm){
        if(HttpContext.Session.GetInt32("usuarioId") is null) return RedirectToAction("Index","Login");

        if(HttpContext.Session.GetString("rol") != "administrador") return RedirectToAction("Index","Home");
        if(!ModelState.IsValid){
            return View(usuariovm);
        }
        var usuario = new Usuario(){
            NombreDeUsuario=usuariovm.NombreDeUsuario,
            Password = protector.Protect(usuariovm.Password),
            RolUsuario = usuariovm.RolUsuario
        };
        repositorioUsuarios.Crear(usuario);
        return RedirectToAction("Index");
    }

    public IActionResult Editar(int id){
        if(HttpContext.Session.GetInt32("usuarioId") is null) return RedirectToAction("Index","Login");

        if(HttpContext.Session.GetString("rol") != "administrador") return RedirectToAction("Index","Home");

        var usuario = repositorioUsuarios.ObtenerUsuario(id);
        if(usuario is null){
            return RedirectToAction("Index"); // Cambiar a una vista de error.
        }
        
        ModificarUsuarioViewModel usuariovm = new ModificarUsuarioViewModel(){
            Id=id,
            NombreDeUsuario = usuario.NombreDeUsuario,
            Password = "",
            RolUsuario = usuario.RolUsuario
        };
        return View(usuariovm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Editar(ModificarUsuarioViewModel usuariovm){
        if(HttpContext.Session.GetInt32("usuarioId") is null) return RedirectToAction("Index","Login");

        if(HttpContext.Session.GetString("rol") != "administrador") return RedirectToAction("Index","Home");

        if(!ModelState.IsValid){
            return View(usuariovm);
        }
        Usuario usuario = new Usuario(){
            Id = usuariovm.Id,
            NombreDeUsuario = usuariovm.NombreDeUsuario,
            Password = protector.Protect(usuariovm.Password),
            RolUsuario = usuariovm.RolUsuario
        };
        repositorioUsuarios.Actualizar(usuario);
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Borrar(int id){
        if(HttpContext.Session.GetInt32("usuarioId") is null) return RedirectToAction("Index","Login");

        if(HttpContext.Session.GetString("rol") != "administrador") return RedirectToAction("Index","Home");

        //antes de eliminar el usuario válidar si existe el usuario
        repositorioUsuarios.Borrar(id);
        return RedirectToAction("Index");
    }
};
