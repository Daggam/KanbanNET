
using Microsoft.AspNetCore.Mvc;
using tl2_proyecto_2024_Daggam.Models;
using tl2_proyecto_2024_Daggam.Repositorios;
using tl2_proyecto_2024_Daggam.ViewModels;

namespace tl2_proyecto_2024_Daggam.Controllers;

public class TablerosController:Controller{
    private readonly IRepositorioTableros repositorioTableros;

    public TablerosController(IRepositorioTableros repositorioTableros)
    {
        this.repositorioTableros = repositorioTableros;
    }

    public IActionResult Index(){
        //Esto se podría mejorar para checar si existe desde la base de datos el usuario pq la sesion no la tiene en cuenta. (Podria pasar que el sistema al ser usado por varios usuarios, en uno de esos usos, un administrador borre de la BD a un usuario que este utilizando esto ) (Para eso son lo filtros)
        //Cambiar: Debo mostrar los tableros los cuales tengo asignados tareas y los que cree.
        var usuarioId = HttpContext.Session.GetInt32("usuarioId");
        if(usuarioId is null){
            return RedirectToAction("Index","Login");
        }
        var tableros = repositorioTableros.ListarTableros((int)usuarioId);
        IEnumerable<ListarTableroViewModel> tablerosViewModel = tableros.Select( t => new ListarTableroViewModel(){
            Id = t.Id,
            Nombre = t.Nombre,
            Descripcion = t.Descripcion,
            IdUsuarioPropietario = t.IdUsuarioPropietario
        });
        return View(tablerosViewModel);
    }

    public IActionResult Crear(){
        var usuarioId = HttpContext.Session.GetInt32("usuarioId");
        if(usuarioId is null){
            return RedirectToAction("Index","Login");
        }
        var modelo = new CrearTableroViewModel(){};
        return View(modelo);
    }
    [HttpPost]
    public IActionResult Crear(CrearTableroViewModel tableroViewModel){
        var usuarioId = HttpContext.Session.GetInt32("usuarioId");
        if(usuarioId is null){
            return RedirectToAction("Index","Login");
        }
        if(!ModelState.IsValid){
            return View(tableroViewModel);
        }
        var tablero = new Tablero(){
            IdUsuarioPropietario = (int)usuarioId,
            Nombre = tableroViewModel.Nombre,
            Descripcion = tableroViewModel.Descripcion ?? ""
        };
        repositorioTableros.Crear(tablero);
        return RedirectToAction("Index");
    }
    public IActionResult Editar(int id){
        var usuarioId = HttpContext.Session.GetInt32("usuarioId");
        if(usuarioId is null){
            return RedirectToAction("Index","Login");
        }
        var tablero = repositorioTableros.ObtenerTablero(id); 
        //Tablero no existe o no le pertenece al usuario
        if(tablero is null || tablero.IdUsuarioPropietario != usuarioId){
            return RedirectToAction("RecursoInvalido","Home");
        }
        var modelo = new ModificarTableroViewModel(){
            Id = tablero.Id,
            Nombre = tablero.Nombre,
            Descripcion = tablero.Descripcion
        };
        return View(modelo);
    }

    [HttpPost]
    public IActionResult Editar(ModificarTableroViewModel modelo){
        var usuarioId = HttpContext.Session.GetInt32("usuarioId");
        if(usuarioId is null){
            return RedirectToAction("Index","Login");
        }
        //Podria cambiarlo por un metodo como Pertenece(tableroId,usuarioId) que me retorne un true cuando le corresponda el tableroId al usuarioId
        var tablero = repositorioTableros.ObtenerTablero(modelo.Id); 
        //Tablero no existe o no le pertenece al usuario
        if(tablero is null || tablero.IdUsuarioPropietario != usuarioId){
            return RedirectToAction("RecursoInvalido","Home");
        }
        if(!ModelState.IsValid){
            return View(modelo);
        }

        var nuevoTablero = new Tablero(){
            Id = modelo.Id,
            IdUsuarioPropietario = (int)usuarioId,
            Nombre = modelo.Nombre,
            Descripcion = modelo.Descripcion ?? ""
        };
        repositorioTableros.Actualizar(nuevoTablero);
        return RedirectToAction("Index");
    }
    [HttpPost]
    public IActionResult Borrar(int id){
        var usuarioId = HttpContext.Session.GetInt32("usuarioId");
        if(usuarioId is null){
            return RedirectToAction("Index","Login");
        }
        var tablero = repositorioTableros.ObtenerTablero(id); 
        if(tablero is not null && tablero.IdUsuarioPropietario == usuarioId){
            repositorioTableros.Borrar(id);
        }
        return RedirectToAction("Index");   
    }
}