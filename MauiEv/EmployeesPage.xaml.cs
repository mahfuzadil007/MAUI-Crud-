using System.Net.Http.Headers;
using System.Net.Http.Json;
namespace MauiEv;

public partial class EmployeesPage : ContentPage
{
	public EmployeesPage()
	{
		InitializeComponent();
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
        EmployeeListViewModel viewModel = new();
        viewModel.Clear();
        LoadEmployees();
    }

    private void LoadEmployees()
    {
        EmployeeListViewModel viewModel = new();
        HttpClient client = new()
        {
            BaseAddress = new Uri(DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5105" : "http://localhost:5105")
        };
        client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

        HttpResponseMessage response = client.GetAsync("api/employees").Result;
        response.EnsureSuccessStatusCode();

        IEnumerable<EmployeeViewModel> employeesFromService = response.Content.ReadFromJsonAsync<IEnumerable<EmployeeViewModel>>().Result;
        foreach (EmployeeViewModel e in employeesFromService.OrderBy(emp => emp.EmployeeName))
        {
            viewModel.Add(e);
        }

        BindingContext = viewModel;
    }

    private async void btnAdd_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EmployeeDetailPage(BindingContext as EmployeeListViewModel));
    }

    private async void Employee_Refreshing(object sender, EventArgs e)
    {
        if (sender is not ListView listView) return;
        listView.IsRefreshing = true;
        await Task.Delay(1500);
        listView.IsRefreshing = false;
    }

    private async void EmployeeMenuItem_Clicked(object sender, EventArgs e)
    {
        MenuItem menuItem = sender as MenuItem;
        if (menuItem.BindingContext is not EmployeeViewModel emp) return;

        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5105" : "http://localhost:5105");
        HttpResponseMessage response = await client.DeleteAsync($"api/employees/{emp.EmployeeId}");
        if (response.IsSuccessStatusCode)
        {
            (BindingContext as EmployeeListViewModel)?.Remove(emp);
        }
    }
}