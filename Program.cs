using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuizMaster.Models;
using QuizMaster.Data;
using QuizMaster.Mail;
using System.Configuration;

namespace QuizMaster
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("QuizMasterContextConnection") ?? throw new InvalidOperationException("Connection string 'QuizMasterContextConnection' not found.");
            var mailsettings = builder.Configuration.GetSection("MailSettings") ?? throw new InvalidOperationException("mailsettings not found");
            builder.Services.AddDbContext<QuizMasterContext>(options =>
            options.UseSqlServer(connectionString));

            builder.Services.AddMvc().AddRazorRuntimeCompilation();


            builder.Services.AddDefaultIdentity<QuizMasterUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<QuizMasterContext>();
            builder.Services.AddOptions();
            //Mail
            builder.Services.Configure<MailSettings>(mailsettings);              

            builder.Services.AddTransient<IEmailSender, SendMailService>();      

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

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
            app.UseAuthentication(); ;

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "Admin",
                pattern: "{area:exists}/{controller=Quizzes}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();
            app.Run();
        }
    }
}