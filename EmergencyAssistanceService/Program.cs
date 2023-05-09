
using Microsoft.OpenApi.Models;
using RoadsideAssistanceBusiness.Interfaces;
using System.Reflection;

namespace EmergencyAssistanceService
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddControllers()
               .AddJsonOptions(j => {
                   j.JsonSerializerOptions.PropertyNamingPolicy = null;
               });

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<IRoadsideAssistanceService, RoadsideAssistanceBusiness.Services.RoadsideAssistanceService>()
                .AddScoped<IEntityFactory, RoadsideAssistanceBusiness.Services.EntityFactory>()
                .AddHttpClient();

            builder.Services.AddMvc(setUpAction => { setUpAction.EnableEndpointRouting = false; })
                .AddJsonOptions(j => { j.JsonSerializerOptions.PropertyNamingPolicy = null; });

            builder.Services.AddSwaggerGen(a =>
            {
                a.SwaggerDoc("v1", new OpenApiInfo { Title = "Emergency Roadside Assitance Service", Version = "v1" });
                a.CustomSchemaIds(type => type.FullName.Replace("+", ".").ToString());
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var filePath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(filePath)) { a.IncludeXmlComments(filePath); }

            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();                
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();

        }

          

    }
}