using Microsoft.Data.Sqlite;
using tl2_proyecto_2024_Daggam.Models;

namespace tl2_proyecto_2024_Daggam.Repositorios{
    public interface IRepositorioUsuarios{
        void Crear(Usuario usuario);
        IEnumerable<Usuario> ObtenerUsuarios();
    
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

        public IEnumerable<Usuario> ObtenerUsuarios(){
            using (var connection = new SqliteConnection(connectionString)){
                connection.Open();
                ICollection<Usuario> usuarios = new List<Usuario>();
                var command =  connection.CreateCommand();
                command.CommandText = "SELECT * FROM Usuario";
                using(var reader = command.ExecuteReader()){
                    while(reader.Read()){
                        usuarios.Add(new Usuario(){
                            Id = reader.GetInt32(0),
                            NombreDeUsuario=reader.GetString(1),
                            Password = reader.GetString(2),
                            RolUsuario = (RolUsuario) reader.GetInt16(3)
                        });
                    }
                }
                connection.Close();
                return usuarios.AsEnumerable();
            }
        }
    }
}