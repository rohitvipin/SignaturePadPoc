using System;
using SignaturePadPoc.ViewModels;
using Xamarin.Forms;

namespace SignaturePadPoc.Views
{
    public partial class AddDocumentPage : ContentPage
    {
        private readonly AddDocumentViewModel _addDocumentViewModel;

        public AddDocumentPage()
        {
            _addDocumentViewModel = new AddDocumentViewModel();
            InitializeComponent();
            BindingContext = _addDocumentViewModel;
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            if (await _addDocumentViewModel.SaveAsync())
            {
                await Navigation.PopAsync();
            }
        }
    }
}