var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

var app = builder.Build();

if(builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
//else
//{
//    app.UseExceptionHandler("/Home/Error");
//    app.UseHsts();
//}

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();
