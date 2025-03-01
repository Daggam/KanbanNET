using Microsoft.Data.Sqlite;
using tl2_proyecto_2024_Daggam.Models;

namespace tl2_proyecto_2024_Daggam.Repositorios{
    public interface IRepositorioUsuarios{
        void Crear(Usuario usuario);
        IEnumerable<Usuario> ObtenerUsuarios();
        Usuario? ObtenerUsuario(int id);
        Usuario? ObtenerUsuario(String username);
        void Actualizar(Usuario usuario);
        void Borrar(int id);
        bool Existe(int id);
        bool Existe(string username);

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
    
        public Usuario? ObtenerUsuario(int id){
            //Podemos o no podemos obtener un usuario
            //En caso de no obtener uno, retornar null, por lo que mi vista retornaria a un home,NoEncontrado que me indica que el recurso que busco no existe.
            using(var connection = new SqliteConnection(connectionString)){
                connection.Open();
                var command = connection.CreateCommand();
                Usuario? usuario=null;
                command.CommandText = "SELECT * FROM Usuario WHERE id=@id";
                command.Parameters.AddWithValue("@id",id);
                using(var reader=command.ExecuteReader()){
                    while(reader.Read()){
                        usuario = new Usuario(){
                            Id=id,
                            NombreDeUsuario = reader.GetString(1),
                            Password = reader.GetString(2),
                            RolUsuario = (RolUsuario) reader.GetInt16(3)
                        };   
                    }
                }
                connection.Close();
                return usuario;
            }
        }

        public Usuario? ObtenerUsuario(string username){
            using(var connection = new SqliteConnection(connectionString)){
                connection.Open();
                Usuario? usuario = null;
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Usuario WHERE nombre_de_usuario=@Username";
                command.Parameters.AddWithValue("@Username",username);
                using(var reader = command.ExecuteReader()){
                    while(reader.Read()){
                        usuario = new Usuario(){
                            Id = reader.GetInt32(0),
                            NombreDeUsuario = reader.GetString(1),
                            Password = reader.GetString(2),
                            RolUsuario = (RolUsuario) reader.GetInt16(3)
                        };
                    }
                }
                connection.Close();
                return usuario;
            }
        }
        public void Actualizar(Usuario usuario){
            using(var connection= new SqliteConnection(connectionString)){
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"UPDATE Usuario SET nombre_de_usuario=@Username, password=@Password, rolusuario=@Rol
                                        WHERE id=@Id;";
                command.Parameters.AddRange([
                    new SqliteParameter("@Id",usuario.Id),
                    new SqliteParameter("@Username",usuario.NombreDeUsuario),
                    new SqliteParameter("@Password",usuario.Password),
                    new SqliteParameter("@Rol",(int)usuario.RolUsuario) 
                ]);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void Borrar(int id){
            using(var connection = new SqliteConnection(connectionString)){
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Usuario WHERE id = @Id";
                command.Parameters.AddWithValue("@Id",id);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public bool Existe(int id){
            using(var connection = new SqliteConnection(connectionString)){
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT 1 FROM Usuario WHERE id=@Id";
                command.Parameters.AddRange([
                    new SqliteParameter("@Id",id)
                ]);
                var response = command.ExecuteScalar();
                connection.Close();
                return response is not null;
            }
        }

        public bool Existe(string username)
        {
            using(var connection = new SqliteConnection(connectionString)){
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT 1 FROM Usuario WHERE nombre_de_usuario=@Username";
                command.Parameters.AddWithValue("@Username",username);
                var response = command.ExecuteScalar();
                connection.Close();
                return response is not null;
            }
        }
    }

}