
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
        var usuarioId = HttpContext.Session.GetInt32("usuarioId");
        if(usuarioId is null){
            return RedirectToAction("Index","Login");
        }
        var tableros = repositorioTableros.ObtenerTableros((int)usuarioId);
        IEnumerable<ListarTableroViewModel> tablerosViewModel = tableros.Select( t => new ListarTableroViewModel(){
            Id = t.Id,
            Nombre = t.Nombre,
            Descripcion = t.Descripcion
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
    public IActionResult Editar(){
        return View();
    }
    public IActionResult Borrar(){
        return RedirectToAction("Index");   
    }
}