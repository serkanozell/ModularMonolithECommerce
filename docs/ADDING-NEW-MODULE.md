# Projeye Yeni Modül Ekleme Adımları

Bu dokümanda, `Catalog` modülü referans alınarak yeni bir modülün projeye nasıl ekleneceği anlatılmaktadır.
Aşağıdaki örneklerde `<Module>` yerine eklenecek modülün adı yazılmalıdır (örn. `Ordering`, `Basket`, `Identity`).

## 0. Genel Kurallar

> **Tarih kuralı (zorunlu):** Hiçbir koşulda `DateTime.Now` kullanılmaz.
> Her zaman ve her yerde `DateTime.UtcNow` kullanılır. Bu kural tüm modüller, tüm katmanlar
> (Domain, Application, Infrastructure, Api) ve interceptor'lar için geçerlidir.

Yeni bir aggregate eklerken izlenecek adımlar için bkz. [ADDING-NEW-AGGREGATE.md](./ADDING-NEW-AGGREGATE.md).

## 1. Klasör Yapısı

Her modül `src/Modules/<Module>` altında, 5 adet **Class Library** projesinden oluşur:

```
src/
└── Modules/
	└── <Module>/
		├── <Module>.Contracts/       # Modüller arası public sözleşmeler (DTO, integration event, public interface)
		├── <Module>.Domain/          # Entity, Value Object, Domain Event, arayüzler
		├── <Module>.Application/     # Use case, CQRS handler, DTO, servis arayüzleri
		├── <Module>.Infrastructure/  # DbContext, repository, dış servis implementasyonları
		└── <Module>.Api/             # Controller / Endpoint + <Module>Module extension sınıfı
```

> **Contracts katmanı:** Bir modülün dışarıya açtığı tek yüzdür. İleride modüller birbirine
> ihtiyaç duyduğunda haberleşme **yalnızca** bu katman üzerinden yapılır. İçinde sadece
> bağımlılığı olmayan basit tipler bulunur: DTO'lar, integration event'ler ve public servis
> arayüzleri. Domain entity'leri, EF tipleri veya iç implementasyon detayları buraya konulmaz.

## 2. Projelerin Oluşturulması

Solution kök dizininde çalıştırın:

```powershell
dotnet new classlib -n <Module>.Contracts      -o src/Modules/<Module>/<Module>.Contracts
dotnet new classlib -n <Module>.Domain         -o src/Modules/<Module>/<Module>.Domain
dotnet new classlib -n <Module>.Application    -o src/Modules/<Module>/<Module>.Application
dotnet new classlib -n <Module>.Infrastructure -o src/Modules/<Module>/<Module>.Infrastructure
dotnet new classlib -n <Module>.Api            -o src/Modules/<Module>/<Module>.Api
```

Tüm projeler **.NET 10** hedeflemelidir (`<TargetFramework>net10.0</TargetFramework>`).

## 3. Solution'a Ekleme

```powershell
dotnet sln ModularMonolithECommerce.slnx add `
  src/Modules/<Module>/<Module>.Contracts/<Module>.Contracts.csproj `
  src/Modules/<Module>/<Module>.Domain/<Module>.Domain.csproj `
  src/Modules/<Module>/<Module>.Application/<Module>.Application.csproj `
  src/Modules/<Module>/<Module>.Infrastructure/<Module>.Infrastructure.csproj `
  src/Modules/<Module>/<Module>.Api/<Module>.Api.csproj
```

## 4. Proje Referansları

Bağımlılık yönü tek yönlüdür: `Api → Infrastructure → Application → Domain`
`<Module>.Contracts` hiçbir projeye referans vermez; `Application` katmanı kendi `Contracts` projesine referans verir.

```powershell
dotnet add src/Modules/<Module>/<Module>.Application/<Module>.Application.csproj `
  reference src/Modules/<Module>/<Module>.Domain/<Module>.Domain.csproj

dotnet add src/Modules/<Module>/<Module>.Application/<Module>.Application.csproj `
  reference src/Modules/<Module>/<Module>.Contracts/<Module>.Contracts.csproj

dotnet add src/Modules/<Module>/<Module>.Infrastructure/<Module>.Infrastructure.csproj `
  reference src/Modules/<Module>/<Module>.Application/<Module>.Application.csproj

dotnet add src/Modules/<Module>/<Module>.Api/<Module>.Api.csproj `
  reference src/Modules/<Module>/<Module>.Infrastructure/<Module>.Infrastructure.csproj
```

> **Kural:** Modüller birbirinin `Domain`, `Application`, `Infrastructure` veya `Api` projesine
> **asla** referans vermez. Bir modülün başka bir modüle ihtiyacı olduğunda yalnızca o modülün
> `<OtherModule>.Contracts` projesine referans verilir:
>
> ```powershell
> dotnet add src/Modules/<Module>/<Module>.Application/<Module>.Application.csproj `
>   reference src/Modules/<OtherModule>/<OtherModule>.Contracts/<OtherModule>.Contracts.csproj
> ```
>
> `Contracts` içindeki arayüzlerin implementasyonu, ilgili modülün kendi `<OtherModule>Module`
> sınıfında DI'a kaydedilir. Böylece modüller yalnızca sözleşme seviyesinde birbirini tanır.

## 5. ASP.NET Core Desteği

`<Module>.Api` projesinin `.csproj` dosyasına framework referansı eklenir:

```xml
<ItemGroup>
  <FrameworkReference Include="Microsoft.AspNetCore.App" />
</ItemGroup>
```

## 6. `<Module>Module` Extension Sınıfı

`src/Modules/<Module>/<Module>.Api/<Module>Module.cs` dosyası, `CatalogModule` ile birebir aynı
desende oluşturulur. Bu sınıf modülün tek giriş noktasıdır (composition root).

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace <Module>.Api
{
	public static class <Module>Module
	{
		public static IServiceCollection Add<Module>Module(this IServiceCollection services, IConfiguration configuration)
		{
			// Servis, repository ve diğer bağımlılıkların kaydı burada yapılır
			// Örnek:
			// services.AddScoped<I<Module>Service, <Module>Service>();
			// services.AddScoped<I<Module>Repository, <Module>Repository>();
			return services;
		}

		public static IApplicationBuilder Use<Module>Module(this IApplicationBuilder app)
		{
			// Middleware, routing vb. yapılandırmalar burada yapılır
			return app;
		}
	}
}
```

Kurallar:
- Sınıf `static` ve `public` olmalıdır.
- Namespace `<Module>.Api` olmalıdır.
- Metot isimleri `Add<Module>Module` ve `Use<Module>Module` şeklinde olmalıdır.
- DI kayıtları modül dışına sızdırılmaz; tüm kayıtlar bu sınıf üzerinden yapılır.

## 7. Host Projesine Bağlama

Host projesine modülün Api projesi referans edilir:

```powershell
dotnet add src/Host/Api/Api.csproj reference src/Modules/<Module>/<Module>.Api/<Module>.Api.csproj
```

### 7.1. `GlobalUsings.cs` Güncellemesi (zorunlu)

Yeni eklenen **her** modülün namespace'i, Host projesindeki `src/Host/Api/GlobalUsings.cs`
dosyasına global using olarak eklenmelidir. Modül namespace'leri `Program.cs` içinde tekrar
`using` ile yazılmaz, tek merkezden yönetilir.

```csharp
global using Catalog.Api;
global using <Module>.Api;   // <-- yeni modül
```

### 7.2. `Program.cs` Güncellemesi

Ardından `src/Host/Api/Program.cs` güncellenir (using satırı gerekmez, `GlobalUsings.cs` üzerinden gelir):

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCatalogModule(builder.Configuration);
builder.Services.Add<Module>Module(builder.Configuration);   // <-- yeni modül

var app = builder.Build();

// ... mevcut middleware zinciri ...

app.UseCatalogModule();
app.Use<Module>Module();                                      // <-- yeni modül

app.Run();
```

## 8. Yapılandırma

Modüle ait ayarlar (connection string, seçenekler) Host projesindeki `appsettings.json` içinde
modül adıyla bir bölüm altında tutulur:

```json
{
  "<Module>": {
	"ConnectionString": "..."
  }
}
```

Bu değerler `Add<Module>Module` içinde `configuration.GetSection("<Module>")` ile okunur.

## 9. Doğrulama

```powershell
dotnet build ModularMonolithECommerce.slnx
```

Build başarılıysa modül eklenmiş demektir.

## Kontrol Listesi

- [ ] 5 class library projesi `src/Modules/<Module>` altında oluşturuldu (`Contracts` dahil)
- [ ] Projeler solution'a eklendi
- [ ] Katman referansları tek yönlü olarak verildi
- [ ] `<Module>.Contracts` hiçbir modül projesine referans vermiyor
- [ ] Modüller arası erişim yalnızca `<OtherModule>.Contracts` üzerinden yapılıyor
- [ ] `<Module>.Api` projesine `Microsoft.AspNetCore.App` framework referansı eklendi
- [ ] `<Module>Module` extension sınıfı oluşturuldu
- [ ] Host projesine referans verildi
- [ ] `src/Host/Api/GlobalUsings.cs` dosyasına `global using <Module>.Api;` eklendi
- [ ] `Program.cs` içinde
- [ ] `appsettings.json` yapılandırması eklendi
- [ ] Kodun hiçbir yerinde `DateTime.Now` kullanılmadı (`DateTime.UtcNow` kullanıldı)
- [ ] Solution başarıyla derlendi
