var builder = WebApplication.CreateBuilder(args);

// Add MVC Controllers and Views service
builder.Services.AddControllersWithViews();

// Register Memory Cache service for caching doctor lists[cite: 1]
builder.Services.AddMemoryCache();

// Register Response Caching service for caching controller action responses[cite: 1]
builder.Services.AddResponseCaching();

// Register Session service to store patient data across requests[cite: 1]
builder.Services.AddSession(options =>
{
    // Set session timeout duration
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    // Ensure session cookie is accessible only via HTTP
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register HttpClient to execute asynchronous HTTP web requests[cite: 1]
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable Response Caching middleware[cite: 1]
app.UseResponseCaching();

// Enable Session middleware before authorization[cite: 1]
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Doctor}/{action=Index}/{id?}");

app.Run();