using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace MauiEv;

public partial class EmployeeDetailPage : ContentPage
{
    private EmployeeListViewModel employees;
    public EmployeeDetailPage()
	{
		InitializeComponent();
	}
    public EmployeeDetailPage(EmployeeListViewModel employees)
    {
        InitializeComponent();
        this.employees = employees;
        BindingContext = new EmployeeViewModel();
        Title = "Add Employee";
    }
    private async void btnAddEmployee_Clicked(object sender, EventArgs e)
    {

        if (BindingContext is EmployeeViewModel employeeViewModel)
        {
            JsonSerializerOptions _serializerOptions;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };


            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5105" : "http://localhost:5105");

            string json = System.Text.Json.JsonSerializer.Serialize<EmployeeViewModel>(employeeViewModel, _serializerOptions);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders
             .Accept
             .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.PostAsync("api/employees", content);




            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Success", "Employee saved successfully.", "OK");
                await Navigation.PopAsync(animated: true);
            }

        }


    }
}