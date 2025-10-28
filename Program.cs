using Microsoft.AspNetCore.Authentication.Cookies;
using Portal_Refeicoes.Services;
using System;
using System.Security.Claims; // <-- Adicionar esta diretiva

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao contêiner.
builder.Services.AddRazorPages();

// Configurar sessão
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// CORREÇÃO: Configurar autenticação por Cookies com mapeamento explícito de Role
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.LogoutPath = "/Logout";
        options.AccessDeniedPath = "/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);

    });

// Configurar o HttpClient e o ApiClient

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient<ApiClient>("ApiClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:ApiBaseUrl"]);
});

var app = builder.Build();

// Configurar o pipeline de requisições HTTP.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Habilitar sessão
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// Exige autorização para todas as páginas, exceto as marcadas com [AllowAnonymous]
app.MapRazorPages().RequireAuthorization();

app.Run();