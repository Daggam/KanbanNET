
using Microsoft.AspNetCore.Mvc;
using tl2_proyecto_2024_Daggam.Models;
using tl2_proyecto_2024_Daggam.Repositorios;
using tl2_proyecto_2024_Daggam.ViewModels;

namespace tl2_proyecto_2024_Daggam.Controllers;

public class TablerosController:Controller{
    private readonly IRepositorioTableros repositorioTableros;
    private readonly ILogger<TablerosController> logger;

    public TablerosController(IRepositorioTableros repositorioTableros, ILogger<TablerosController> logger)
    {
        this.repositorioTableros = repositorioTableros;
        this.logger = logger;
    }

    public IActionResult Index(){
        try{
            //Esto se podría mejorar para checar si existe desde la base de datos el usuario pq la sesion no la tiene en cuenta. (Podria pasar que el sistema al ser usado por varios usuarios, en uno de esos usos, un administrador borre de la BD a un usuario que este utilizando esto ) (Para eso son lo filtros)
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
        }catch(Exception e){
            logger.LogError(e.ToString());
            return RedirectToAction("RecursoInvalido","Home");  
        }
    }

    public IActionResult Crear(){
        try{
            var usuarioId = HttpContext.Session.GetInt32("usuarioId");
            if(usuarioId is null){
                return RedirectToAction("Index","Login");
            }
            var modelo = new CrearTableroViewModel(){};
            return View(modelo);
        }catch(Exception e){
            logger.LogError(e.ToString());
            return RedirectToAction("RecursoInvalido","Home");  
        }
    }
    [HttpPost]
    public IActionResult Crear(CrearTableroViewModel tableroViewModel){
        try{
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
            logger.LogInformation($"El usuario #{usuarioId} creo un tablero");
            return RedirectToAction("Index");
        }catch(Exception e){
            logger.LogError(e.ToString());
            return RedirectToAction("RecursoInvalido","Home");  
        }
    }
    public IActionResult Editar(int id){
        try{
            var usuarioId = HttpContext.Session.GetInt32("usuarioId");
            if(usuarioId is null){
                return RedirectToAction("Index","Login");
            }
            var tablero = repositorioTableros.ObtenerTablero(id); 
            //Tablero no existe o no le pertenece al usuario
            if(tablero is null) throw new Exception("El tablero no existe");
            if(tablero.IdUsuarioPropietario != usuarioId) throw new Exception("El tablero no le pertenece al usuario");
            var modelo = new ModificarTableroViewModel(){
                Id = tablero.Id,
                Nombre = tablero.Nombre,
                Descripcion = tablero.Descripcion
            };
            return View(modelo);
        }catch(Exception e){
            logger.LogError(e.ToString());
            return RedirectToAction("RecursoInvalido","Home");  
        }
        
    }

    [HttpPost]
    public IActionResult Editar(ModificarTableroViewModel modelo){
        try{
            var usuarioId = HttpContext.Session.GetInt32("usuarioId");
            if(usuarioId is null){
                return RedirectToAction("Index","Login");
            }
            var tablero = repositorioTableros.ObtenerTablero(modelo.Id); 
            //Tablero no existe o no le pertenece al usuario
            if(tablero is null) throw new Exception("El tablero no existe");
            if(tablero.IdUsuarioPropietario != usuarioId) throw new Exception("El tablero no le pertenece al usuario");
            if(!ModelState.IsValid) return View(modelo);
            
            var nuevoTablero = new Tablero(){
                Id = modelo.Id,
                IdUsuarioPropietario = (int)usuarioId,
                Nombre = modelo.Nombre,
                Descripcion = modelo.Descripcion ?? ""
            };
            repositorioTableros.Actualizar(nuevoTablero);
            logger.LogInformation($"El usuario #{usuarioId} actualizó el tablero #{tablero.Id}");
            return RedirectToAction("Index");

        }catch(Exception e){
            logger.LogError(e.ToString());
            return RedirectToAction("RecursoInvalido","Home");              
        }
    }
    [HttpPost]
    public IActionResult Borrar(int id){
        try{
            var usuarioId = HttpContext.Session.GetInt32("usuarioId");
            if(usuarioId is null){
                return RedirectToAction("Index","Login");
            }
            var tablero = repositorioTableros.ObtenerTablero(id); 
            if(tablero is null) throw new Exception("El tablero no existe");
            if(tablero.IdUsuarioPropietario != usuarioId) throw new Exception("El tablero no le pertenece al usuario");
            
            repositorioTableros.Borrar(id);
            logger.LogInformation($"El usuario #{usuarioId} borró el tablero #{id}");
            return RedirectToAction("Index");   
        }catch(Exception e){
            logger.LogError(e.ToString());
            return RedirectToAction("RecursoInvalido","Home");  
        }
        
    }
}