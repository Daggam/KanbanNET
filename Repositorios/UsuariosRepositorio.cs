using Microsoft.Data.Sqlite;
using tl2_proyecto_2024_Daggam.Models;

namespace tl2_proyecto_2024_Daggam.Repositorios{
    public interface IRepositorioUsuarios{
        void Crear(Usuario usuario);
    }

    public class RepositorioUsuarios:IRepositorioUsuarios{
        private readonly string connectionString;

        public RepositorioUsuarios ( IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }
        public void Crear(Usuario usuario){
            using(var connection = new SqliteConnection(connectionString)){
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Usuario (nombre_de_usuario,password,rolusuario) VALUES (@Username,@Password,@Role);";
            command.Parameters.AddRange([
                new SqliteParameter("@Username",usuario.NombreDeUsuario),
                new SqliteParameter("@Password",usuario.Password),
                new SqliteParameter("@Role",(int)usuario.RolUsuario)
            ]);
            command.ExecuteNonQuery();
            connection.Close();
            }
        }

        
    }
}