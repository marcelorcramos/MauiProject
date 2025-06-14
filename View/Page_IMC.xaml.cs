using Maui_1.ViewModel;

namespace Maui_1.View;

public partial class Page_IMC : ContentPage
{
	IMC_VM _vm;
	public Page_IMC(IMC_VM vm)
	{
		InitializeComponent();
		_vm = vm;
		this.BindingContext = _vm;
	}
}