using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{
    ObservableCollection<Produto> lista = new ObservableCollection<Produto>();

    public ListaProduto()
    {
        InitializeComponent();
        lst_produtos.ItemsSource = lista;
    }

    // Helper centralizado — resolve o aviso de plataforma em um único lugar
    private Task Alerta(string titulo, string msg) =>
        MainThread.InvokeOnMainThreadAsync(() => DisplayAlert(titulo, msg, "OK"));

    protected async override void OnAppearing()
    {
        try
        {
            lista.Clear();
            List<Produto> Tmp = await App.Db.GetAll();
            Tmp.ForEach(i => lista.Add(i));
            await CarregarCategorias();
        }
        catch (Exception ex)
        {
            await Alerta("Ops", ex.Message);
        }
    }

    private async Task CarregarCategorias()
    {
        List<string> categorias = await App.Db.GetCategorias();
        pkr_filtro_categoria.Items.Clear();
        pkr_filtro_categoria.Items.Add("Todas");
        categorias.ForEach(c => pkr_filtro_categoria.Items.Add(c));
        pkr_filtro_categoria.SelectedIndex = 0;
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