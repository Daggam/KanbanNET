using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SQLitePCL;
using tl2_proyecto_2024_Daggam.Models;
using tl2_proyecto_2024_Daggam.Repositorios;
using tl2_proyecto_2024_Daggam.ViewModels;

namespace tl2_proyecto_2024_Daggam.Controllers;

public class TareasController:Controller{
    private readonly IRepositorioTableros repositorioTableros;
    private readonly IRepositorioUsuarios repositorioUsuarios;
    private readonly IRepositorioTareas repositorioTareas;
    private readonly ILogger<TareasController> logger;

    public TareasController(IRepositorioTableros repositorioTableros, IRepositorioUsuarios repositorioUsuarios, IRepositorioTareas repositorioTareas, ILogger<TareasController> logger){
        this.repositorioTableros = repositorioTableros;
        this.repositorioUsuarios = repositorioUsuarios;
        this.repositorioTareas = repositorioTareas;
        this.logger = logger;
    }
    public IActionResult Index(int? id){
        try{
            var usuarioId = HttpContext.Session.GetInt32("usuarioId");
            if( usuarioId is null ) return RedirectToAction("Index","Login");
            if(id is null) throw new Exception($"El usuario #{usuarioId} quiso acceder a una página inexistente");
            var tablero = repositorioTableros.ObtenerTablero((int)id);
            if(tablero is null) throw new Exception($"El usuario #{usuarioId} quiso acceder a un tablero nulo.");
            //Si soy dueño del tablero, muestro todas sin importar a quienes ha sido asignadas.
            IEnumerable<Tarea> tareas = repositorioTareas.ObtenerTareasPorTablero((int)id);
            bool esPropietario = tablero.IdUsuarioPropietario == usuarioId;
            //Si no soy el dueño, muestro todas las que se me asignaron dado a ese tablero.
            if(!esPropietario){
                tareas = tareas.Where(t => t.IdUsuarioAsignado == usuarioId);
                //Tendríamos que prohibir a aquellos usuarios quienes no son dueños ni tienen tareas asignadas.
                if(tareas.Count() == 0){
                    throw new Exception($"El usuario #{usuarioId} no tiene acceso a este tablero");
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
        }catch(Exception e){
            logger.LogError(e.ToString());
            return RedirectToAction("RecursoInvalido","Home");
        }
        
    }
    public IActionResult Crear(int? idTablero){
        try{
            var usuarioId = HttpContext.Session.GetInt32("usuarioId");
            if(usuarioId is null) return RedirectToAction("Index","Login");
            if(idTablero is null) throw new Exception($"El usuario #{usuarioId} puso un idTablero nulo");
            var tablero = repositorioTableros.ObtenerTablero((int)idTablero);
            if(tablero is null || tablero.IdUsuarioPropietario != usuarioId ) throw new Exception($"El usuario #{usuarioId} quiso acceder a un tablero nulo o no le pertenece");
            var modelo = new CrearTareaViewModel(){
                IdTablero = (int)idTablero,
                UsuariosDisponibles = repositorioUsuarios.ObtenerUsuarios().Where(u=>u.Id!=usuarioId).Select(u => new SelectListItem(u.NombreDeUsuario,u.Id.ToString())).ToList()
            };
            return View(modelo);    
        }catch(Exception e){
            logger.LogError(e.ToString());
            return RedirectToAction("RecursoInvalido","Home");
        }
    }
    [HttpPost]
    public IActionResult Crear(CrearTareaViewModel modelo){
        try{
            var usuarioId = HttpContext.Session.GetInt32("usuarioId");
            if(usuarioId is null) return RedirectToAction("Index","Login");   
            //Corroborar si el tablero existe, si el usuario de la sesion es propietario del tablero y si el usuario al cual asignamos la tarea existe y no es el propietario
            var tablero = repositorioTableros.ObtenerTablero(modelo.IdTablero);
            bool resTablero = tablero is not null && tablero.IdUsuarioPropietario == usuarioId;
            var usuario = repositorioUsuarios.ObtenerUsuario(modelo.IdUsuarioAsignado);
            bool resUsuario = usuario is not null && usuario.Id != usuarioId;
            if(!resTablero || !resUsuario) throw new Exception($"El usuario #{usuarioId} no pudo crear una tarea.");
            if(!ModelState.IsValid){ 
                modelo.UsuariosDisponibles = repositorioUsuarios.ObtenerUsuarios().Where(u=>u.Id!=usuarioId).Select(u => new SelectListItem(u.NombreDeUsuario,u.Id.ToString())).ToList();
                return View(modelo);
            }
            var tarea = new Tarea(){
                Nombre = modelo.Nombre,
                Descripcion = modelo.Descripcion ?? "",
                IdTablero = modelo.IdTablero,
                Color = modelo.Color!, //No puede ser nulo, lo carga automaticamente el formulario
                IdUsuarioAsignado = modelo.IdUsuarioAsignado,
                Estado = EstadoTarea.Ideas,
            };
            
            repositorioTareas.Crear(tarea);
            logger.LogInformation($"El usuario #{usuarioId} creó exitosamente una tarea.");
            return RedirectToAction("Index",new {id=modelo.IdTablero});
        }catch(Exception e){
            logger.LogError(e.ToString());
            return RedirectToAction("RecursoInvalido","Home");
        }

    }

    public IActionResult Reasignar(int id){
        try{
            var usuarioId = HttpContext.Session.GetInt32("usuarioId");
            if(usuarioId is null) return RedirectToAction("Index","Login");   
            //Corroboramos si la tarea existe y tenemos permiso de modificarla
            var tarea = repositorioTareas.ObtenerTarea(id);
            bool permiso = (repositorioTableros.ObtenerTablero(tarea?.IdTablero ?? -1)?.IdUsuarioPropietario ?? -1) == usuarioId;
            if(tarea is null || !permiso) throw new Exception($"El usuario #{usuarioId} no tiene permisos para estar en la página de reasignación o su tarea no existe.");
            var modelo = new ReasignarTareaViewModel(){
                Id = id,
                UsuarioId = tarea.IdUsuarioAsignado,
                UsuariosDisponibles = repositorioUsuarios.ObtenerUsuarios().Where(u=>u.Id!=usuarioId).Select(u => new SelectListItem(u.NombreDeUsuario,u.Id.ToString())).ToList()
            };
            return View(modelo);
        }catch(Exception e){
            logger.LogError(e.ToString());
            return RedirectToAction("RecursoInvalido","Home");
        }
    }

    [HttpPost]
    public IActionResult Reasignar(ReasignarTareaViewModel modelo){
        try{
            var usuarioId = HttpContext.Session.GetInt32("usuarioId");
            if(usuarioId is null) return RedirectToAction("Index","Login");   
            //Corroboramos si la tarea existe y tenemos permiso de modificarla
            var tarea = repositorioTareas.ObtenerTarea(modelo.Id);
            bool permiso = (repositorioTableros.ObtenerTablero(tarea?.IdTablero ?? -1)?.IdUsuarioPropietario ?? -1) == usuarioId;
            if(tarea is null || !permiso || !ModelState.IsValid) throw new Exception($"El usuario #{usuarioId} no pudo reasignar al usuario.");
            //No puede ser nulo (Pusimos required en el viewmodel)
            repositorioTareas.ReasginarUsuario(modelo.Id,(int)modelo.UsuarioId!);
            logger.LogInformation($"El usuario {usuarioId} reasignó exitosamente una tarea.");
            return RedirectToAction("Index",new {id=tarea.IdTablero});
        }catch(Exception e){
            logger.LogError(e.ToString());
            return RedirectToAction("RecursoInvalido","Home");            
        }
    }

    [HttpPost]
    public IActionResult Borrar(int id){
        try{
            var usuarioId = HttpContext.Session.GetInt32("usuarioId");
            if(usuarioId is null) return RedirectToAction("Index","Login");
            var tarea = repositorioTareas.ObtenerTarea(id);
            //No puedo borrar tareas de un tablero ajeno. El unico capaz de borrar tareas es el propietario.
            var tablero = repositorioTableros.ObtenerTablero(tarea?.IdTablero ?? -1);
            if(!ModelState.IsValid || tarea is null || tablero!.IdUsuarioPropietario != usuarioId){
                throw new Exception($"El usuario #{usuarioId} no trató de borrar la tarea #{id} pero no pudo.");
            }
            repositorioTareas.Borrar(id);
            logger.LogInformation($"El usuario {usuarioId} borró la tarea #{id}.");
            return RedirectToAction("Index", new {id = tablero.Id});
        }catch(Exception e){
            logger.LogError(e.ToString());
            return RedirectToAction("RecursoInvalido","Home");
        }
    }
    [HttpPost]
    public IActionResult ActualizarEstado([FromBody] ModificarTareaViewModel modelo){
        /*
            Primera alternativa:
            Quienes pueden mover tareas:
                - Creador del tablero (actua como especie de administrador del tablero)
                - Usuario con sus tareas asignadas.
                (Complicado puesto a que no iría en tiempo real)
            Segunda alternativa:
                - Usuarios con sus tareas asignadas. 
                (Más complejo puesto a que si el creador del tablero se autoasigna una tarea, podrá mover sus tareas pero no la de los demás)
                (Si hago esto: debo cambiar el viewmodel para agregar el propietarioTareaId, para cada carta cargada, permitir que solo se muevan las tareas de aquellos usuarios que tengan permiso)
            Tercera alternativa:
                -Usuarios con tareas asignadas (El creador del tablero no puede autoasignarse tareas.)
        */
        /*
        Puede pasar lo siguiente: 
            - La tarea no existe.
            - La tarea no pertenece al usuario
            - El usuario no tiene los permisos de mover la tarea (El usuario no es propietario del tablero)
        */
        try{
            var usuarioId = HttpContext.Session.GetInt32("usuarioId");
            var tarea = repositorioTareas.ObtenerTarea(modelo.Id);
            var tablero = repositorioTableros.ObtenerTablero(tarea?.IdTablero ?? -1);
            if(!ModelState.IsValid || usuarioId is null || tarea is null || tarea.IdUsuarioAsignado != usuarioId || tablero!.IdUsuarioPropietario == usuarioId) throw new Exception($"El usuario #{usuarioId} no pudo mover la tarea.");
            tarea.Estado = modelo.Estado;
            repositorioTareas.ActualizarEstado(tarea);
            logger.LogTrace($"El usuario #{usuarioId} cambió el estado de la tarea.");
            return Ok();
        }catch(Exception e){
            logger.LogError(e.ToString());
            return RedirectToAction("RecursoInvalido","Home");
        }
    }
}
