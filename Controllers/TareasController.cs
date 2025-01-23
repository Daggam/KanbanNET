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
    public IActionResult Index(){
        var usuarioId = HttpContext.Session.GetInt32("usuarioId");
        if( usuarioId is null ) return RedirectToAction("Index","Login");
        var tareas = repositorioTareas.ObtenerTareasPorUsuario((int)usuarioId);
        // Listar todas las tareas asignadas a este usuario y las creadas por el?
        // var modelo = tareas.Select( t => new )
        return View();
    }

    public IActionResult Crear(){
        var usuarioId = HttpContext.Session.GetInt32("usuarioId");
        if(usuarioId is null) return RedirectToAction("Index","Login");
        var modelo = new CrearTareaViewModel();
        modelo.Tableros = TablerosAListItems((int)usuarioId);
        return View(modelo);
    }
    [HttpPost]
    public IActionResult Crear(CrearTareaViewModel modelo){
        var usuarioId = HttpContext.Session.GetInt32("usuarioId");
        if(usuarioId is null) return RedirectToAction("Index","Login");   
        //Corroboramos si el tablero elegido existe y le pertenece, ademas si el usuario elegido existe. No podemos confiar en el front. Mejorariamos si tuviesemos operaciones existe en el repositorio. Si esto pasa incluso podríamos agregar un error.
        var tablero = repositorioTableros.ObtenerTablero(modelo.IdTablero);
        bool res = tablero is not null && tablero.IdUsuarioPropietario == usuarioId && 
                  repositorioUsuarios.ObtenerUsuario(modelo.IdUsuarioAsignado) is not null;
        if(!ModelState.IsValid || !res){ 
            modelo.Tableros = TablerosAListItems((int)usuarioId);
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
        return RedirectToAction("Index");
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
    private List<SelectListItem> TablerosAListItems(int usuarioId){
        return repositorioTableros.ObtenerTablerosPorUsuario(usuarioId)
                        .Select(t => new SelectListItem
                                    {
                                        Value = t.Id.ToString(),
                                        Text = t.Nombre
                                    }).ToList();
    }
}