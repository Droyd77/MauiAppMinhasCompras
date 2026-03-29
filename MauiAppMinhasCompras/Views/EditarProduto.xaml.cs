using MauiAppMinhasCompras.Models;

namespace MauiAppMinhasCompras.Views;

public partial class EditarProduto : ContentPage
{
    public EditarProduto()
    {
        InitializeComponent();
    }

    // Preenche Picker e DatePicker com os valores atuais do produto
    protected override void OnAppearing()
    {
        base.OnAppearing();

        Produto? p = BindingContext as Produto;
        if (p == null) return;

        // Desafio 1: seleciona a categoria atual no Picker
        if (!string.IsNullOrWhiteSpace(p.Categoria))
            pkr_categoria.SelectedItem = p.Categoria;

        // Desafio 2: preenche a data atual no DatePicker
        dtp_datacadastro.Date = p.DataCadastro == default
            ? DateTime.Today
            : p.DataCadastro;
    }

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            Produto? produto_anexado = BindingContext as Produto;
            if (produto_anexado == null) return;

            Produto p = new Produto()
            {
                Id = produto_anexado.Id,
                Descricao = txt_descricao.Text,
                Quantidade = Convert.ToDouble(txt_quantidade.Text),
                Preco = Convert.ToDouble(txt_preco.Text),
                Categoria = pkr_categoria.SelectedItem?.ToString() ?? produto_anexado.Categoria,
                DataCadastro = dtp_datacadastro.Date
            };

            await App.Db.Update(p);
            await DisplayAlert("Sucesso!", "Registro Atualizado", "ok");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }
}
