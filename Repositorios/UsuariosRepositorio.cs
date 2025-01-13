using tl2_proyecto_2024_Daggam.Models;

namespace tl2_proyecto_2024_Daggam.Repositorios{
    public interface IRepositorioUsuarios{
        
    }

    public class RepositorioUsuarios:IRepositorioUsuarios{
        private readonly string connectionString;

        public RepositorioUsuarios ( IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }
        public async Task Crear(Usuario usuario){
            
        }
    }
}