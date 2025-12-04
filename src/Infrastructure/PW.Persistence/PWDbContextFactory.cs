using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using PW.Persistence.Contexts;
using System.IO;

namespace PW.Persistence
{
    public class PWDbContextFactory : IDesignTimeDbContextFactory<PWDbContext>
    {
        public PWDbContext CreateDbContext(string[] args)
        {
            // 1. Yapılandırma Yolunu Tanımla:
            // Design-Time operasyonları için (örneğin 'dotnet ef migrations add')
            // .NET CLI'ın varsayılan yolu, migrations'ın uygulandığı proje dizinidir (PW.Persistence).
            // Web projesinin (PW.Web) appsettings dosyasını okumak için yolu ayarlamalıyız.

            // Eğer PW.Persistence, çözüm klasörünün (Solution Folder) hemen altındaysa:
            // var projectPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "PW.Web");

            // Daha güvenilir bir yaklaşım: Yolu Web Projesi'ne göre ayarla.
            // Bu, 'dotnet ef' komutunun çalıştırıldığı dizinden bağımsız olarak genellikle çalışır.

            // Not: Gerçek uygulamada, genellikle yapılandırma dosyasının bulunduğu ana projeyi
            // (genellikle Web/API projesi) temel alarak yolu bulmak gerekir.

            // En sağlam yol, ConfigurationBuilder'ı projeler arası bağımlılıkları varsayarak kurmaktır.

            // Mevcut çalışma dizinini (genellikle PW.Persistence) alırız.
            var currentDirectory = Directory.GetCurrentDirectory();

            // Web projesinin yolu, varsayılan olarak PW.Persistence'ın bir üst dizininde
            // ve ardından PW.Web içinde varsayılır. Bu, çoğu standart çözüm yapısına uyar.
            var webProjectPath = Path.GetFullPath(Path.Combine(currentDirectory, "..", "..", "Presentation", "PW.Web"));

            // 2. ConfigurationBuilder Oluşturma:
            var config = new ConfigurationBuilder()
                // Bu adım, appsettings'i web projesi yolunda aramasını sağlar.
                .SetBasePath(webProjectPath)
                .AddJsonFile("appsettings.json", optional: true) // Production/varsayılan ayarları da ekle
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            // 3. Connection String'i Al ve DbContext Oluştur:
            var optionsBuilder = new DbContextOptionsBuilder<PWDbContext>();

            var connectionString = config.GetConnectionString("DefaultConnection");

            // Bağlantı dizesi bulunamazsa erken hata verme kontrolü
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' not found in configuration. " +
                    $"Looked in path: {webProjectPath}"
                );
            }

            optionsBuilder.UseNpgsql(connectionString);

            return new PWDbContext(optionsBuilder.Options);
        }
    }
}
