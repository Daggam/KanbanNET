using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using tl2_proyecto_2024_Daggam.Models;
using tl2_proyecto_2024_Daggam.Repositorios;
using tl2_proyecto_2024_Daggam.ViewModels;
namespace tl2_proyecto_2024_Daggam.Controllers;

//TODO: AL CREAR UN USUARIO, CORROBORAR SI ESE NOMBRE EXISTE EN LA BASE DE DATOS. 
public class UsuariosController:Controller{
    private readonly IRepositorioUsuarios repositorioUsuarios;
    private readonly ILogger<UsuariosController> logger;
    private readonly IDataProtector protector;

    public UsuariosController(IRepositorioUsuarios repositorioUsuarios, IDataProtectionProvider provider, ILogger<UsuariosController> logger)
    {
        this.repositorioUsuarios = repositorioUsuarios;
        this.logger = logger;
        protector = provider.CreateProtector("Kanban.UsuariosController");
    }
    //Lista los usuarios
    public IActionResult Index(){
        try{
            if(HttpContext.Session.GetInt32("usuarioId") is null) return RedirectToAction("Index","Login");
            //Usuario no autorizado
            if(HttpContext.Session.GetString("rol") != "administrador") return RedirectToAction("Index","Tableros");
        
            var usuarios = repositorioUsuarios.ObtenerUsuarios();
            IEnumerable<ListarUsuarioViewModel> usuariosvm = usuarios.Select(u => 
                                                                new ListarUsuarioViewModel(){
                                                                    Id = u.Id,
                                                                    NombreDeUsuario = u.NombreDeUsuario,
                                                                    RolUsuario = u.RolUsuario})
                                                            .ToList();
            return View(usuariosvm);
        }catch(Exception e){
            logger.LogError(e.ToString());
            return RedirectToAction("RecursoInvalido","Home");  
        }
    }
    public IActionResult Crear(){
        try{
            if(HttpContext.Session.GetInt32("usuarioId") is null) return RedirectToAction("Index","Login");
            if(HttpContext.Session.GetString("rol") != "administrador") return RedirectToAction("Index","Tableros");
            var modelo = new CrearUsuarioViewModel();
            return View(modelo);
        }catch(Exception e){
            logger.LogError(e.ToString());
            return RedirectToAction("RecursoInvalido","Home");
        }
    }
    [HttpPost]
    public IActionResult Crear(CrearUsuarioViewModel usuariovm){
        try{
            int? usuarioId = HttpContext.Session.GetInt32("usuarioId");
            if(usuarioId is null) return RedirectToAction("Index","Login");
            if(HttpContext.Session.GetString("rol") != "administrador") return RedirectToAction("Index","Tableros");
            if(!ModelState.IsValid){
                return View(usuariovm);
            }
            var usuario = new Usuario(){
                NombreDeUsuario=usuariovm.NombreDeUsuario,
                Password = protector.Protect(usuariovm.Password),
                RolUsuario = usuariovm.RolUsuario
            };
            repositorioUsuarios.Crear(usuario);
            logger.LogInformation($"Se creó al usuario #{usuarioId}");
            return RedirectToAction("Index");
        }catch(Exception e){
            logger.LogError(e.ToString());
            return RedirectToAction("RecursoInvalido","Home");
        }
    }

    public IActionResult Editar(int id){
        try{
            if(HttpContext.Session.GetInt32("usuarioId") is null) return RedirectToAction("Index","Login");
            if(HttpContext.Session.GetString("rol") != "administrador") return RedirectToAction("Index","Tableros");
            var usuario = repositorioUsuarios.ObtenerUsuario(id);
            if(usuario is null){
                throw new Exception("Edición de un usuario inexistente.");
            }
            
            ModificarUsuarioViewModel usuariovm = new ModificarUsuarioViewModel(){
                Id=id,
                NombreDeUsuario = usuario.NombreDeUsuario,
                Password = "",
                RolUsuario = usuario.RolUsuario
            };
            return View(usuariovm);

        }catch(Exception e){
            logger.LogError(e.ToString());
            return RedirectToAction("RecursoInvalido","Home");
        }
    }

    [HttpPost]
    public IActionResult Editar(ModificarUsuarioViewModel usuariovm){
        try{
            int? usuarioId = HttpContext.Session.GetInt32("usuarioId");
            if(usuarioId is null) return RedirectToAction("Index","Login");
            if(HttpContext.Session.GetString("rol") != "administrador") return RedirectToAction("Index","Tableros");
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
            logger.LogInformation($"Se editó al usuario #{usuarioId}.");
            if(usuario.Id == usuarioId){
                return RedirectToAction("Logout","Login");
            }
            return RedirectToAction("Index");

        }catch(Exception e){
            logger.LogError(e.ToString());
            return RedirectToAction("RecursoInvalido","Home");
        }
    }

    [HttpPost]
    public IActionResult Borrar(int id){
        try{
            int? usuarioId = HttpContext.Session.GetInt32("usuarioId");
            if(usuarioId is null) return RedirectToAction("Index","Login");
            if(HttpContext.Session.GetString("rol") != "administrador") return RedirectToAction("Index","Tableros");
            //No puede borrarse así mismo.
            if(usuarioId == id){
                throw new Exception($"El usuario #{usuarioId} trató de borrarse asi mismo.");
            }
            //Antes de eliminar al usuario necesitamos saber si existe.
            if(!repositorioUsuarios.Existe(id)){
                throw new Exception("Eliminar un usuario inexistente.");
            }
            repositorioUsuarios.Borrar(id);
            logger.LogInformation($"Se eliminó al usuario #{id}.");
            return RedirectToAction("Index");
        }catch(Exception e){
            logger.LogError(e.ToString());
            return RedirectToAction("RecursoInvalido","Home");
        }
    }
    [HttpGet]
    public IActionResult UsuarioExiste(string nombredeusuario){
        try{
            int? usuarioId = HttpContext.Session.GetInt32("usuarioId");
            if(usuarioId is null) return RedirectToAction("Index","Login");
            if(HttpContext.Session.GetString("rol") != "administrador") return RedirectToAction("Index","Tableros");
            bool existeUsuario = repositorioUsuarios.Existe(nombredeusuario);
        
            if(existeUsuario){
                return Json($"El usuario {nombredeusuario} ya existe.");
            }
            return Json(true);
        }catch(Exception e){
            logger.LogError(e.ToString());
            return RedirectToAction("RecursoInvalido","Home");
        }
    }
};
