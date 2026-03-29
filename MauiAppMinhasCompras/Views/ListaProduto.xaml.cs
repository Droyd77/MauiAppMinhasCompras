using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{
    ObservableCollection<Produto> lista = new ObservableCollection<Produto>();
    bool _carregando = false; // flag para evitar duplo carregamento

    public ListaProduto()
    {
        InitializeComponent();
        lst_produtos.ItemsSource = lista;
    }

    private Task Alerta(string titulo, string msg) =>
        MainThread.InvokeOnMainThreadAsync(() => DisplayAlert(titulo, msg, "OK"));

    protected async override void OnAppearing()
    {
        try
        {
            _carregando = true;

            // Salva a categoria selecionada antes de recarregar o Picker
            string? categoriaSalva = pkr_filtro_categoria.SelectedItem?.ToString();

            // 1. Recarrega categorias no Picker
            await CarregarCategorias();

            // 2. Restaura a categoria que estava selecionada (se ainda existir)
            if (!string.IsNullOrEmpty(categoriaSalva) && categoriaSalva != "Todas")
            {
                int idx = pkr_filtro_categoria.Items.IndexOf(categoriaSalva);
                if (idx >= 0)
                    pkr_filtro_categoria.SelectedIndex = idx;
            }

            // 3. Carrega a lista respeitando o filtro ativo
            lista.Clear();
            string categoriaAtual = pkr_filtro_categoria.SelectedItem?.ToString() ?? "Todas";
            List<Produto> Tmp = categoriaAtual == "Todas"
                ? await App.Db.GetAll()
                : await App.Db.GetByCategoria(categoriaAtual);
            Tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await Alerta("Ops", ex.Message);
        }
        finally
        {
            _carregando = false;
        }
    }

    private async Task CarregarCategorias()
    {
        List<string> categorias = await App.Db.GetCategorias();
        pkr_filtro_categoria.Items.Clear();
        pkr_filtro_categoria.Items.Add("Todas");
        categorias.ForEach(c => pkr_filtro_categoria.Items.Add(c));
        pkr_filtro_categoria.SelectedIndex = 0; // não dispara duplicata pois _carregando = true
    }

    // Botão Adicionar
    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new Views.NovoProduto());
        }
        catch (Exception ex)
        {
            await Alerta("Ops", ex.Message);
        }
    }

    // Busca por descrição
    private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            string q = e.NewTextValue;
            lista.Clear();
            List<Produto> Tmp = await App.Db.Search(q);
            Tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await Alerta("Ops", ex.Message);
        }
    }

    // Botão Somar
    private async void ToolbarItem_Clicked_1(object sender, EventArgs e)
    {
        try
        {
            double totalSoma = lista.Sum(i => i.Total);
            string msg = $"O total é {totalSoma:C}";
            await MainThread.InvokeOnMainThreadAsync(() => DisplayAlert("Total dos Produtos", msg, "Ok"));
        }
        catch (Exception ex)
        {
            await Alerta("Ops", ex.Message);
        }
    }

    // Desafio 1: filtro por categoria via Picker
    private async void pkr_filtro_categoria_SelectedIndexChanged(object sender, EventArgs e)
    {
        // ignora o evento enquanto OnAppearing está populando o Picker
        if (_carregando) return;

        try
        {
            string categoriaSelecionada = pkr_filtro_categoria.SelectedItem?.ToString() ?? "Todas";
            lista.Clear();

            if (string.IsNullOrEmpty(categoriaSelecionada) || categoriaSelecionada == "Todas")
            {
                List<Produto> todos = await App.Db.GetAll();
                todos.ForEach(p => lista.Add(p));
            }
            else
            {
                List<Produto> filtrados = await App.Db.GetByCategoria(categoriaSelecionada);
                filtrados.ForEach(p => lista.Add(p));
            }
        }
        catch (Exception ex)
        {
            await Alerta("Ops", ex.Message);
        }
    }

    // Desafio 2: navega para tela de Relatório
    private async void ToolbarItem_Clicked_Relatorio(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new Views.RelatorioProduto());
        }
        catch (Exception ex)
        {
            await Alerta("Ops", ex.Message);
        }
    }

    // Remover
    private async void MenuItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            MenuItem? menu = sender as MenuItem;
            Produto? produtoSelecionado = menu?.BindingContext as Produto;

            bool confirm = await MainThread.InvokeOnMainThreadAsync(() =>
                DisplayAlert("Confirmar", "Deseja remover este produto?", "Sim", "Não"));

            if (confirm && produtoSelecionado != null)
            {
                await App.Db.Delete(produtoSelecionado.Id);
                lista.Remove(produtoSelecionado);
            }
        }
        catch (Exception ex)
        {
            await Alerta("Ops", ex.Message);
        }
    }

    // Selecionar item para editar
#pragma warning disable CA1416
    private async void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        try
        {
            Produto? p = e.SelectedItem as Produto;
            if (p != null)
                await Navigation.PushAsync(new Views.EditarProduto
                {
                    BindingContext = p,
                });
        }
        catch (Exception ex)
        {
            await Alerta("Ops", ex.Message);
        }
    }
#pragma warning restore CA1416

    // Pull-to-refresh
    private async void lst_produtos_Refreshing(object sender, EventArgs e)
    {
        try
        {
            lista.Clear();
            List<Produto> Tmp = await App.Db.GetAll();
            Tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await Alerta("Ops", ex.Message);
        }
    }
}
