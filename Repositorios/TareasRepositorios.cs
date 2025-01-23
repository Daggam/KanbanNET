using Microsoft.Data.Sqlite;
using tl2_proyecto_2024_Daggam.Models;

namespace tl2_proyecto_2024_Daggam.Repositorios;

public interface IRepositorioTareas{
    void Crear(Tarea tarea);
    IEnumerable<Tarea> ObtenerTareasPorUsuario(int usuarioId);
    IEnumerable<Tarea> ObtenerTareasPorTablero(int tableroId);
    Tarea? ObtenerTarea(int id);
    void ActualizarEstado(Tarea tarea);
    void Borrar(int id);
}
public class RepositorioTareas:IRepositorioTareas{
    
    private readonly string connectionString;
    public RepositorioTareas(IConfiguration configuration){
        connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
    }
    public void Crear(Tarea tarea){
        using(var connection = new SqliteConnection(connectionString)){
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO Tarea 
                                    (id_tablero,nombre,estado,descripcion,color,id_usuario_asignado)
                                    VALUES (@IdTablero,@Nombre,@Estado,@Descripcion,@Color,@IdUsuarioAsignado)";
            command.Parameters.AddRange([
                new SqliteParameter("@IdTablero",tarea.IdTablero),
                new SqliteParameter("@Nombre",tarea.Nombre),
                new SqliteParameter("@Estado",(int)tarea.Estado),
                new SqliteParameter("@Descripcion",tarea.Descripcion),
                new SqliteParameter("@Color",tarea.Color),
                new SqliteParameter("@IdUsuarioAsignado",tarea.IdUsuarioAsignado)
            ]);
            command.ExecuteNonQuery();
            connection.Close();
        }
    }

    public IEnumerable<Tarea> ObtenerTareasPorUsuario(int usuarioId){
        using(var connection = new SqliteConnection(connectionString)){
            connection.Open();
            ICollection<Tarea> tarea = new List<Tarea>();
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM Tarea WHERE id_usuario_asignado=@UsuarioId";
            command.Parameters.AddWithValue("@UsuarioId",usuarioId);
            using(var reader = command.ExecuteReader()){
                while(reader.Read()){
                    tarea.Add(new Tarea(){
                        Id = reader.GetInt32(0),
                        IdTablero = reader.GetInt32(1),
                        Nombre = reader.GetString(2),
                        Estado=(EstadoTarea) reader.GetInt16(3),
                        Descripcion = reader.GetString(4),
                        Color = reader.GetString(5),
                        IdUsuarioAsignado = reader.GetInt32(6)
                    });
                }
            }
            connection.Close();
            return tarea.AsEnumerable();
        }
    }
    
    public IEnumerable<Tarea> ObtenerTareasPorTablero(int tableroId){
        using(var connection = new SqliteConnection(connectionString)){
            connection.Open();
            ICollection<Tarea> tarea = new List<Tarea>();
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM Tarea WHERE id_tablero=@TableroId";
            command.Parameters.AddWithValue("@TableroId",tableroId);
            using(var reader = command.ExecuteReader()){
                while(reader.Read()){
                    tarea.Add(new Tarea(){
                        Id = reader.GetInt32(0),
                        IdTablero = reader.GetInt32(1),
                        Nombre = reader.GetString(2),
                        Estado=(EstadoTarea) reader.GetInt16(3),
                        Descripcion = reader.GetString(4),
                        Color = reader.GetString(5),
                        IdUsuarioAsignado = reader.GetInt32(6)
                    });
                }
            }
            connection.Close();
            return tarea.AsEnumerable();
        }
    }
    public Tarea? ObtenerTarea(int id){
        using(var connection = new SqliteConnection(connectionString)){
            connection.Open();
            Tarea? tarea = null;
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM Tarea WHERE id=@Id";
            command.Parameters.AddWithValue("@Id",id);
            using(var reader = command.ExecuteReader()){
                while(reader.Read()){
                    tarea = new Tarea(){
                        Id = id,
                        IdTablero = reader.GetInt32(1),
                        Nombre = reader.GetString(2),
                        Estado=(EstadoTarea) reader.GetInt16(3),
                        Descripcion = reader.GetString(4),
                        Color = reader.GetString(5),
                        IdUsuarioAsignado = reader.GetInt32(6)
                    };
                }
            }
            connection.Close();
            return tarea;
        }
    }

    public void ActualizarEstado(Tarea tarea){
        using(var connection = new SqliteConnection(connectionString)){
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"UPDATE Tarea SET estado=@Estado WHERE id=@Id ";
            command.Parameters.AddRange([
                new SqliteParameter("@Estado",(int)tarea.Estado),
                new SqliteParameter("@Id",tarea.Id)
            ]);
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
    public void Borrar(int id){
        using(var connection = new SqliteConnection(connectionString)){
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM Tarea WHERE id=@Id";
            command.Parameters.AddRange([
                new SqliteParameter("@Id",id)
            ]);
            command.ExecuteNonQuery();
            connection.Close();
        }   
    }
}
