using SQLite;

namespace MauiAppMinhasCompras.Models
{
    public class Produto
    {
        string? _descricao;

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string? Descricao
        {
            get => _descricao;
            set
            {
                if (value == null)
                {
                    throw new Exception("Por favor, preencher a descrição");
                }
                _descricao = value;
            }
        }

        public double Quantidade { get; set; }
        public double Preco { get; set; }

        // Desafio 1: Campo Categoria
        public string? Categoria { get; set; }

        // Desafio 2: Campo DataCadastro
        public DateTime DataCadastro { get; set; } = DateTime.Now;

        // Propriedade calculada — apenas UMA definição com [Ignore]
        [Ignore]
        public double Total => Quantidade * Preco;
    }
}