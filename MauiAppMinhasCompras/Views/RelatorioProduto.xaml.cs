using MauiAppMinhasCompras.Models;

namespace MauiAppMinhasCompras.Views;

public partial class RelatorioProduto : ContentPage
{
    public RelatorioProduto()
    {
        InitializeComponent();

        // Período padrão: mês atual
        dtp_inicio.Date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        dtp_fim.Date = DateTime.Today;
    }

    // ── Desafio 1: Relatório por Categoria ───────────────────────────

    private async void Btn_RelatorioCategorias_Clicked(object sender, EventArgs e)
    {
        try
        {
            List<Produto> todos = await App.Db.GetAll();

            // Agrupa por categoria e calcula total
            var grupos = todos
                .GroupBy(p => string.IsNullOrWhiteSpace(p.Categoria) ? "Sem categoria" : p.Categoria!)
                .Select(g => new
                {
                    Categoria = g.Key,
                    TotalGasto = g.Sum(p => p.Total),
                    Itens = g.Count()
                })
                .OrderByDescending(g => g.TotalGasto)
                .ToList();

            // Limpa o Grid antes de popular
            grd_categorias.Children.Clear();
            grd_categorias.RowDefinitions.Clear();
            grd_categorias.ColumnDefinitions.Clear();

            grd_categorias.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            grd_categorias.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(45) });
            grd_categorias.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(95) });

            // Cabeçalho
            grd_categorias.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            AdicionarLinhaGrid(0, "Categoria", "Itens", "Total", isHeader: true);

            double totalGeral = grupos.Sum(g => g.TotalGasto);

            // Linhas de cada categoria
            for (int i = 0; i < grupos.Count; i++)
            {
                var g = grupos[i];
                grd_categorias.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                AdicionarLinhaGrid(i + 1, g.Categoria, g.Itens.ToString(), g.TotalGasto.ToString("C"), isEven: i % 2 == 0);
            }

            // Rodapé com total geral
            grd_categorias.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            AdicionarLinhaGrid(grupos.Count + 1, "TOTAL GERAL", "", totalGeral.ToString("C"), isFooter: true);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    // Helper: adiciona uma linha ao Grid dinâmico
    private void AdicionarLinhaGrid(int row, string col0, string col1, string col2,
                                    bool isHeader = false, bool isFooter = false, bool isEven = false)
    {
        Color bgColor = isHeader ? Colors.Green
                        : isFooter ? Color.FromArgb("#2d6a4f")
                        : isEven ? Colors.White
                        : Color.FromArgb("#d8f3dc");

        Color textColor = (isHeader || isFooter) ? Colors.White : Colors.Black;
        FontAttributes fa = (isHeader || isFooter) ? FontAttributes.Bold : FontAttributes.None;

        var labels = new[] { col0, col1, col2 };
        for (int col = 0; col < 3; col++)
        {
            var lbl = new Label
            {
                Text = labels[col],
                TextColor = textColor,
                FontAttributes = fa,
                FontSize = 13,
                Padding = new Thickness(6, 4),
                BackgroundColor = bgColor,
                VerticalTextAlignment = TextAlignment.Center
            };
            Grid.SetRow(lbl, row);
            Grid.SetColumn(lbl, col);
            grd_categorias.Children.Add(lbl);
        }
    }

    // ── Desafio 2: Filtro por Período ────────────────────────────────

    private async void Btn_FiltrarPeriodo_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (dtp_inicio.Date > dtp_fim.Date)
            {
                await DisplayAlert("Atenção", "A data inicial não pode ser maior que a data final.", "OK");
                return;
            }

            List<Produto> produtos = await App.Db.GetByPeriodo(dtp_inicio.Date, dtp_fim.Date);

            lst_relatorio.ItemsSource = produtos;

            lbl_periodo_titulo.Text = $"Compras de {dtp_inicio.Date:dd/MM/yyyy} a {dtp_fim.Date:dd/MM/yyyy} — {produtos.Count} produto(s)";
            lbl_periodo_titulo.IsVisible = true;

            frm_lista.IsVisible = produtos.Count > 0;
            lbl_total_periodo.IsVisible = produtos.Count > 0;

            if (produtos.Count > 0)
            {
                double totalPeriodo = produtos.Sum(p => p.Total);
                lbl_total_periodo.Text = $"Total do período: {totalPeriodo:C}";
            }
            else
            {
                await DisplayAlert("Sem resultados", "Nenhum produto encontrado nesse período.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }
}
