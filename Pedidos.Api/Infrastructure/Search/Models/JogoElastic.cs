namespace Pedidos.Api.Infrastructure.Search.Models
{
    public class JogoElastic
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string Genero { get; set; }
        public decimal Valor { get; set; }
        public string Status { get; set; }
        public int Popularidade { get; set; }
    }
}
