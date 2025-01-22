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
        if(HttpContext.Session.GetInt32("usuarioId") is null ) return RedirectToAction("Index","Login");
        return View();
    }

    public IActionResult Crear(){
        var usuarioId = HttpContext.Session.GetInt32("usuarioId");
        if(usuarioId is null) return RedirectToAction("Index","Login");
        var modelo = new CrearTareaViewModel();
        modelo.Tableros = repositorioTableros.ObtenerTableros()
                        .Where( t => t.IdUsuarioPropietario == usuarioId)
                        .Select(t => new SelectListItem
                                    {
                                        Value = t.Id.ToString(),
                                        Text = t.Nombre
                                    }).ToList();
        
        return View(modelo);
    }
    [HttpPost]
    public IActionResult Crear([FromForm] CrearTareaViewModel modelo){
        var usuarioId = HttpContext.Session.GetInt32("usuarioId");
        if(usuarioId is null) return RedirectToAction("Index","Login");   
        //Corroboramos si el tablero elegido existe y le pertenece, ademas si el usuario elegido existe. No podemos confiar en el front. Mejorariamos si tuviesemos operaciones existe en el repositorio. Si esto pasa incluso podríamos agregar un error.
        var tablero = repositorioTableros.ObtenerTablero(modelo.IdTablero);
        bool res = tablero is not null && tablero.IdUsuarioPropietario == usuarioId && 
                  repositorioUsuarios.ObtenerUsuario(modelo.IdUsuarioAsignado) is not null;
        if(!ModelState.IsValid || !res) return View(modelo);
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
}