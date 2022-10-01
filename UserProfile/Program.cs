
using Microsoft.Owin.Security.Facebook;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var FacebookOptions = new FacebookAuthenticationOptions()
{
    AppId = "1X884XXXX138XXXX",
    AppSecret = "0XXc3645XXX1a3c2f8bXXXX86c242XX4",
    Scope = {
            "public_profile", //Provides access to a subset of items that are part of a person's public profile.  
            "email"
        },
    Fields = {
            "birthday", //User's DOB  
            "picture", //User Profile Image  
            "name", //User Full Name  
            "email", //User Email  
            "gender", //user's Gender  
    },
    UserInformationEndpoint = "https://graph.facebook.com/v2.7/me?fields=id,name,email"
};


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
