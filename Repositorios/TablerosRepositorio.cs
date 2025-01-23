using Microsoft.Data.Sqlite;
using tl2_proyecto_2024_Daggam.Models;

namespace tl2_proyecto_2024_Daggam.Repositorios{

    public interface IRepositorioTableros{
        void Crear(Tablero tablero);
        IEnumerable<Tablero> ObtenerTableros();
        IEnumerable<Tablero> ObtenerTablerosPorUsuario(int usuarioId);
        Tablero? ObtenerTablero(int id);


        void Actualizar(Tablero tablero);

        void Borrar(int id);
    }

    public class RepositorioTablero:IRepositorioTableros{
        private readonly string connectionString;
        public RepositorioTablero(IConfiguration configuration){
            connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
        }

        public void Crear(Tablero tablero){
            using(var connection = new SqliteConnection(connectionString)){
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"INSERT INTO Tablero(id_usuario_propietario,nombre,descripcion) 
                                        VALUES (@IdUsuario,@Nombre,@Descripcion);";
                command.Parameters.AddRange([
                    new SqliteParameter("@IdUsuario",tablero.IdUsuarioPropietario),
                    new SqliteParameter("@Nombre",tablero.Nombre),
                    new SqliteParameter("@Descripcion",tablero.Descripcion)
                ]);

                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        public IEnumerable<Tablero> ObtenerTableros(){
            using(var connection = new SqliteConnection(connectionString)){
                connection.Open();
                ICollection<Tablero> tableros = new List<Tablero>();
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT * FROM Tablero";
                using(var reader = command.ExecuteReader()){
                    while(reader.Read()){
                        tableros.Add(new Tablero(){
                            Id = reader.GetInt32(0),
                            IdUsuarioPropietario = reader.GetInt32(1),
                            Nombre=reader.GetString(2),
                            Descripcion=reader.GetString(3)
                        });
                    }
                }
                connection.Close();
                return tableros.AsEnumerable();
            }
        }
        public IEnumerable<Tablero> ObtenerTablerosPorUsuario(int usuarioId){
            using(var connection = new SqliteConnection(connectionString)){
                connection.Open();
                ICollection<Tablero> tableros = new List<Tablero>();
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT * FROM Tablero WHERE id_usuario_propietario = @UsuarioId";
                command.Parameters.AddWithValue("@UsuarioId",usuarioId);
                using(var reader = command.ExecuteReader()){
                    while(reader.Read()){
                        tableros.Add(new Tablero(){
                            Id = reader.GetInt32(0),
                            IdUsuarioPropietario = reader.GetInt32(1),
                            Nombre=reader.GetString(2),
                            Descripcion=reader.GetString(3)
                        });
                    }
                }
                connection.Close();
                return tableros.AsEnumerable();
            }
        }
        public Tablero? ObtenerTablero(int id)
        {
            using(var connection = new SqliteConnection(connectionString)){
                connection.Open();
                Tablero? tablero = null;
                var command = connection.CreateCommand();
                command.CommandText =@"SELECT * FROM Tablero WHERE id=@Id";
                command.Parameters.AddWithValue("@Id",id);
                using(var reader = command.ExecuteReader()){
                    while(reader.Read()){
                        tablero=new Tablero(){
                            Id = reader.GetInt32(0),
                            IdUsuarioPropietario = reader.GetInt32(1),
                            Nombre=reader.GetString(2),
                            Descripcion=reader.GetString(3)
                        };
                    }
                }
                connection.Close();
                return tablero;
            }
        }

        public void Actualizar(Tablero tablero){
            using(var connection = new SqliteConnection(connectionString)){
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"UPDATE Tablero SET id_usuario_propietario=@IdUsuario,nombre=@Nombre,descripcion=@Descripcion WHERE id=@Id";
                command.Parameters.AddRange([
                    new SqliteParameter("@IdUsuario",tablero.IdUsuarioPropietario),
                    new SqliteParameter("@Nombre",tablero.Nombre),
                    new SqliteParameter("@Descripcion",tablero.Descripcion),
                    new SqliteParameter("@Id",tablero.Id)
                ]);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        public void Borrar(int id){
            using(var connection = new SqliteConnection(connectionString)){
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"DELETE FROM Tablero WHERE id=@Id";
                command.Parameters.AddWithValue("@Id",id);
                command.ExecuteNonQuery();
                connection.Close();
            }   
        }
    }

}