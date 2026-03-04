using Interconn.Hubs;
using Interconn.Models;
using Interconn.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Azure.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<InterconnDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
            sqlOptions =>
            {   
                // 自動重試
                sqlOptions.EnableRetryOnFailure(); 
            }));

//使用DI依賴注入服務
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<ChatService>();

//設定Cookie儲存登入資料,如果沒有就導向Login
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        //未登入會導向此頁 對應LoginController的Login方法
        options.LoginPath = "/Login/Login";
    });

//設定網頁授權策略,需要登入才能瀏覽網頁
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

//加入 SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//啟用身分驗證跟授權
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//將 Hub 路由映射出來
app.MapHub<ChatHub>("/chathub");

app.Run();