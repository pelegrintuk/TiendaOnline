namespace TiendaOnline.Web.Models
{
    public class RegisterViewModel
    {
        public string Username { get; set; }       // Nombre de usuario
        public string Email { get; set; }          // Correo electrónico
        public string Password { get; set; }       // Contraseña
        public string ConfirmPassword { get; set; } // Confirmar contraseña
    }
}
