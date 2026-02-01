//using DotNetEnv;
//using esyasoft.mobility.CHRGUP.service.persistence.Data;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Design;
//using Microsoft.Extensions.Configuration;

//namespace esyasoft.mobility.CHRGUP.service.persistence;

//public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
//{
//    public AppDbContext CreateDbContext(string[] args)
//    {
//        Env.Load("../../.env");
//        var cs = Environment.GetEnvironmentVariable("ConnectionStrings__DBConnection");

//        if (string.IsNullOrWhiteSpace(cs))
//            throw new InvalidOperationException(
//                "Connection string not found in .env file.");

//        var options = new DbContextOptionsBuilder<AppDbContext>()
//            .UseNpgsql(cs)
//            .Options;

//        return new AppDbContext(options);
//    }
//}