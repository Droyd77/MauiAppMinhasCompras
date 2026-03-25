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

    protected async override void OnAppearing()
    {
        try
        {
            lista.Clear();

            List<Produto> Tmp = await App.Db.GetAll();
	    	Tmp.ForEach( i => lista.Add(i));
        }
        catch (Exception ex)
        {
           await DisplayAlert("Ops", ex.Message, "OK");

        }
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
        try
        { 
		string q = e.NewTextValue;

            lst_produtos.IsRefreshing = true;

            lista.Clear();

        List<Produto> Tmp = await App.Db.Search(q);
        Tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "OK");

        }finally
        {
            lst_produtos.IsRefreshing = false;
        }
    }

    private void ToolbarItem_Clicked_1(object sender, EventArgs e)
    {
        try
           {
            
		double soma = lista.Sum(i => i.Total);
		string msg = $"O total é {soma:C}";
		DisplayAlert("Total dos Produtos", msg, "Ok");
        }
        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "OK");

        }
    }

    private async void MenuItem_Clicked(object sender, EventArgs e)
    {

        try
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
        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "OK");

        }
    }

    private async  void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {

        try
        {

            Produto p = e.SelectedItem as Produto;

            if (p != null)

               await Navigation.PushAsync(new Views.EditarProduto
            {
                BindingContext = p,
            });
        }

        catch (Exception ex)
        {
           await DisplayAlert("Ops", ex.Message, "OK");
        }

    }

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
            await DisplayAlert("Ops", ex.Message, "OK");

        }finally
        {
            lst_produtos.IsRefreshing = false;
        }
    }
}