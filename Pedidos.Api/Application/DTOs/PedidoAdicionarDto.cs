using System.ComponentModel.DataAnnotations;

namespace Pedidos.Api.Application.DTOs
{
    public class PedidoAdicionarDto
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public Guid UsuarioId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public string UsuarioNome { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public Guid JogoId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public string JogoTitulo { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public decimal Valor { get; set; }
    }
}
