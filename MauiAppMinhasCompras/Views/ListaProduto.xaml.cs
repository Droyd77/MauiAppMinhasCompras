using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{

	ObservableCollection<Produto> lista = new ObservableCollection<Produto>();

    public ListaProduto()
	{
		InitializeComponent();

		lst_produtos.ItemsSource = lista;

	}

    protected async override void OnAppearing()
    {
        lista.Clear();

        List<Produto> Tmp = await App.Db.GetAll();
		Tmp.ForEach( i => lista.Add(i));
    }

    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {
		try
		{
			Navigation.PushAsync(new Views.NovoProduto());
		}catch (Exception ex)
		{
		DisplayAlert ("Ops", ex.Message, "OK");

		}


    }

    private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
    {
		string q = e.NewTextValue;
		lista.Clear();

        List<Produto> Tmp = await App.Db.Search(q);
        Tmp.ForEach(i => lista.Add(i));
    }

    private void ToolbarItem_Clicked_1(object sender, EventArgs e)
    {
		double soma = lista.Sum(i => i.Total);
		string msg = $"O total é {soma:C}";
		DisplayAlert("Total dos Produtos", msg, "Ok");

    }

    private async void MenuItem_Clicked(object sender, EventArgs e)
    {
        MenuItem menu = sender as MenuItem;
        Produto produtoSelecionado = menu.BindingContext as Produto;

        bool confirm = await DisplayAlert(
            "Confirmar",
            "Deseja remover este produto?",
            "Sim",
            "Não");

        if (confirm)
        {
            await App.Db.Delete(produtoSelecionado.Id);
            lista.Remove(produtoSelecionado);
        }
    }
}