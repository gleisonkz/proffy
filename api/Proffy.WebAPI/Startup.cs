﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Proffy.Business.Services;
using Proffy.Repository.Interfaces;
using Proffy.RepositoryEF;
using Proffy.RepositoryEF.Class;

namespace Proffy.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _ = services.AddDbContext<ProffyContext>(options =>
              {
                  options.UseSqlServer("Password=sa123456;Persist Security Info=True;User ID=sa;Initial Catalog=Proffy;Data Source=DESKTOP-2AKCSN7\\PROFFY");
              });

            services.AddCors(c => c.AddPolicy("ProffyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            services.AddScoped<IFactoryRepository, FactoryEFCoreRepository>();
            services.AddScoped<IUnityOfWork, UnityOfWork>();
            services.AddScoped<ITeacherInfoService, TeacherInfoService>();
            services.AddScoped<IConnectionService, ConnectionService>();
            services.AddScoped<ILessonService, LessonService>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("ProffyPolicy");
            app.UseMvc();
        }
    }
}
