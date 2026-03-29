using MauiAppMinhasCompras.Models;
using SQLite;

namespace MauiAppMinhasCompras.Helpers
{
    public class SQLiteDatabaseHelper
    {
        readonly SQLiteAsyncConnection _conn;

        public SQLiteDatabaseHelper(string path)
        {
            _conn = new SQLiteAsyncConnection(path);
            _conn.CreateTableAsync<Produto>().Wait();
        }

        public Task<int> Insert(Produto p)
        {
            return _conn.InsertAsync(p);
        }

        // Update com SQL raw (igual ao original) + novos campos Categoria e DataCadastro
        public Task<List<Produto>> Update(Produto p)
        {
            string sql = "UPDATE Produto SET Descricao=?, Quantidade=?, Preco=?, Categoria=?, DataCadastro=? WHERE Id=?";
            return _conn.QueryAsync<Produto>(
                sql, p.Descricao, p.Quantidade, p.Preco, p.Categoria, p.DataCadastro, p.Id);
        }

        public Task<int> Delete(int id)
        {
            return _conn.Table<Produto>().DeleteAsync(i => i.Id == id);
        }

        public Task<List<Produto>> GetAll()
        {
            return _conn.Table<Produto>().ToListAsync();
        }

        // Search com SQL raw (igual ao original)
        public Task<List<Produto>> Search(string q)
        {
            string sql = "SELECT * FROM Produto WHERE Descricao LIKE ?";
            return _conn.QueryAsync<Produto>(sql, "%" + q + "%");
        }

        // ── Desafio 1: Filtro e relatório por Categoria ──────────────

        public Task<List<Produto>> GetByCategoria(string categoria)
        {
            string sql = "SELECT * FROM Produto WHERE Categoria = ?";
            return _conn.QueryAsync<Produto>(sql, categoria);
        }

        public async Task<List<string>> GetCategorias()
        {
            var todos = await _conn.Table<Produto>().ToListAsync();
            return todos
                .Where(p => !string.IsNullOrWhiteSpace(p.Categoria))
                .Select(p => p.Categoria!)
                .Distinct()
                .OrderBy(c => c)
                .ToList();
        }

        // ── Desafio 2: Filtro por Período ────────────────────────────

        public Task<List<Produto>> GetByPeriodo(DateTime dataInicio, DateTime dataFim)
        {
            string sql = "SELECT * FROM Produto WHERE DataCadastro >= ? AND DataCadastro <= ?";
            return _conn.QueryAsync<Produto>(
                sql,
                dataInicio.Date.ToString("yyyy-MM-dd"),
                dataFim.Date.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}