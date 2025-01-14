namespace TiendaOnline.Web.Models
{
    public class LoginViewModel
    {
        public string Username { get; set; }       // Nombre de usuario
        public string Password { get; set; }       // Contraseña
        public bool RememberMe { get; set; }       // Mantener la sesión iniciada
    }
}

