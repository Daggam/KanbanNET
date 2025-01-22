using Microsoft.Data.Sqlite;
using tl2_proyecto_2024_Daggam.Models;

namespace tl2_proyecto_2024_Daggam.Repositorios;

public interface IRepositorioTareas{
    void Crear(Tarea tarea);
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
}