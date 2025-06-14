namespace Maui_1.View;

public partial class Page_Pessoas : ContentPage
{

	PessoaVM _pessoaVM;
	public Page_Pessoas(PessoaVM pessoaVM)
	{
		InitializeComponent();
		_pessoaVM = pessoaVM;
		BindingContext = _pessoaVM;
	}
}