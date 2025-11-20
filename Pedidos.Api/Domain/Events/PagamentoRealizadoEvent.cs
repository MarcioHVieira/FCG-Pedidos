using System.Text.Json.Serialization;
using Fcg.Common.Enums;

namespace Pedidos.Api.Domain.Events
{
    public class PagamentoRealizadoEvent
    {
        [JsonPropertyName("pagamentoId")]
        public Guid PagamentoId { get; set; }
        
        [JsonPropertyName("pedidoId")]
        public Guid PedidoId { get; set; }

        [JsonPropertyName("valorPago")]
        public decimal ValorPago { get; set; }

        [JsonPropertyName("formaPagamento")]
        public FormaPagamento FormaPagamento { get; set; }

        [JsonPropertyName("dataCadastro")]
        public DateTime DataCadastro { get; set; }
        
        [JsonPropertyName("dataPagamento")]
        public DateTime DataPagamento { get; set; }

        [JsonPropertyName("clienteNome")]
        public string ClienteNome { get; set; }

        [JsonPropertyName("clienteemail")]
        public string ClienteEmail { get; set; }
    }
}