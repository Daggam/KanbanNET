using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using tl2_proyecto_2024_Daggam.Models;
using tl2_proyecto_2024_Daggam.Repositorios;
using tl2_proyecto_2024_Daggam.ViewModels;

namespace tl2_proyecto_2024_Daggam.Controllers;

public class TareasController:Controller{
    private readonly IRepositorioTableros repositorioTableros;
    private readonly IRepositorioUsuarios repositorioUsuarios;
    private readonly IRepositorioTareas repositorioTareas;

    public TareasController(IRepositorioTableros repositorioTableros, IRepositorioUsuarios repositorioUsuarios, IRepositorioTareas repositorioTareas){
        this.repositorioTableros = repositorioTableros;
        this.repositorioUsuarios = repositorioUsuarios;
        this.repositorioTareas = repositorioTareas;
    }
    public IActionResult Index(int? id){
        //Mi idea es la siguiente: dado un tablero, dar todas las tareas relacionadas a este.
        //Si no existe dicho tablero, retornar que no existe O podríamos tomar todas las tareas asignadas al usuario 
        //int? => si no recibe ningún numero podríamos mostrar todas las tareas asignadas al usuario.
        var usuarioId = HttpContext.Session.GetInt32("usuarioId");
        if( usuarioId is null ) return RedirectToAction("Index","Login");
        if(id is null) return RedirectToAction("RecursoInvalido","Home");
        var tablero = repositorioTableros.ObtenerTablero((int)id);
        if(tablero is null) return RedirectToAction("RecursoInvalido","Home");
        //Si soy dueño del tablero, muestro todas sin importar a quienes ha sido asignadas.
        IEnumerable<Tarea> tareas = repositorioTareas.ObtenerTareasPorTablero((int)id);
        bool esPropietario = tablero.IdUsuarioPropietario == usuarioId;
        //Si no soy el dueño, muestro todas las que se me asignaron dado a ese tablero.
        if(!esPropietario){
            tareas = tareas.Where(t => t.IdUsuarioAsignado == usuarioId);
            //Tendríamos que prohibir a aquellos usuarios quienes no son dueños ni tienen tareas asignadas.
            if(tareas.Count() == 0){
                return RedirectToAction("RecursoInvalido","Home");
            }
        }
        var modelo = tareas.Select( t => new ListarTareaViewModel(){
                Id = t.Id,
                Nombre = t.Nombre,
                Descripcion = t.Descripcion,
                Color = t.Color,
                Estado = t.Estado,
            }).ToList();
        var paquete = new PaqueteListarViewModel(){
            Modelo = modelo,
            EsPropietario = esPropietario
        };
        return View(paquete);
    }
    public IActionResult Crear(int? idTablero){
        var usuarioId = HttpContext.Session.GetInt32("usuarioId");
        if(usuarioId is null) return RedirectToAction("Index","Login");
        //Corroborar si el tablero existe o si tiene acceso a él.
        if(idTablero is null) return RedirectToAction("RecursoInvalido","Home");
        var tablero = repositorioTableros.ObtenerTablero((int)idTablero);
        if(tablero is null || tablero.IdUsuarioPropietario != usuarioId ) return RedirectToAction("RecursoInvalido","Home");
        var modelo = new CrearTareaViewModel(){
            IdTablero = (int)idTablero
        };
        return View(modelo);
    }
    [HttpPost]
    public IActionResult Crear(CrearTareaViewModel modelo){
        var usuarioId = HttpContext.Session.GetInt32("usuarioId");
        if(usuarioId is null) return RedirectToAction("Index","Login");   
        //Corroborar si el tablero existe, si el usuario de la sesion es propietario del tablero y si el usuario al cual asignamos la tarea existe
        var tablero = repositorioTableros.ObtenerTablero(modelo.IdTablero);
        bool resTablero = tablero is not null && tablero.IdUsuarioPropietario == usuarioId;
        bool resUsuario = repositorioUsuarios.ObtenerUsuario(modelo.IdUsuarioAsignado) is not null;
        if(!resTablero){
            //Podríamos mostrar un error.
            return RedirectToAction("RecursoInvalido","Home");
        }
        if(!ModelState.IsValid || !resUsuario){ 
            //Podemos mostrar un error.
            return View(modelo);
        }
        var tarea = new Tarea(){
            Nombre = modelo.Nombre,
            Descripcion = modelo.Descripcion ?? "",
            IdTablero = modelo.IdTablero,
            Color = modelo.Color!, //No puede ser nulo, lo carga automaticamente el formulario
            IdUsuarioAsignado = modelo.IdUsuarioAsignado,
            Estado = EstadoTarea.Ideas
        };
        repositorioTareas.Crear(tarea);
        return RedirectToAction("Index",new {id=modelo.IdTablero});
    }

    
    public IActionResult Editar(int id){
        var usuarioId = HttpContext.Session.GetInt32("usuarioId");
        if(usuarioId is null) return RedirectToAction("Index","Login");
        var tarea = repositorioTareas.ObtenerTarea(id);
        if(tarea==null){
            return RedirectToAction("RecursoInvalido","Home");
        }
        var modelo = new ModificarTareaViewModel(){
            Id = tarea.Id,
            Estado = tarea.Estado
        };
        return View(modelo);
    }
    [HttpPost]
    public IActionResult Editar(ModificarTareaViewModel modelo){
        var usuarioId = HttpContext.Session.GetInt32("usuarioId");
        if(usuarioId is null) return RedirectToAction("Index","Login");
        var tarea = repositorioTareas.ObtenerTarea(modelo.Id);
        //El id de la tarea esta alterado (la tarea no existe)
        //Ya de por si le pertenece al usuario y al tablero(No hay nada en el front que permita cambiarlo)
        if(!ModelState.IsValid || tarea is null){
            return View(modelo);
        }

        tarea.Estado = modelo.Estado;
        repositorioTareas.ActualizarEstado(tarea);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Borrar(int id){
        var usuarioId = HttpContext.Session.GetInt32("usuarioId");
        if(usuarioId is null) return RedirectToAction("Index","Login");
        var tarea = repositorioTareas.ObtenerTarea(id);
        if(!ModelState.IsValid || tarea is null){
            return RedirectToAction("RecursoInvalido","Home");
        }
        repositorioTareas.Borrar(id);
        return RedirectToAction("Index");
    }
    [HttpPost]
    public IActionResult ActualizarEstado([FromBody] ModificarTareaViewModel modelo){
        var usuarioId = HttpContext.Session.GetInt32("usuarioId");
        var tarea = repositorioTareas.ObtenerTarea(modelo.Id);

        if(usuarioId is null || !ModelState.IsValid || tarea is null || tarea.IdUsuarioAsignado == usuarioId) return BadRequest();
        /*
        Puede pasar lo siguiente: 
            - La tarea no pertenece al usuario
            - La tarea no pertenece al tablero
            - El usuario no tiene los permisos de mover la tarea (El usuario no es propietario del tablero)
        */
        //Corroboramos si la tarea pertenece al usuario, y si pertenece al tablero.
        
        tarea.Estado = modelo.Estado;
        repositorioTareas.ActualizarEstado(tarea);
        return Ok();
    }
}
