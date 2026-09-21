# Yeni Modül / Yeni Aggregate Ekleme Rehberi

Bu doküman, `Catalog.Product` için uygulanan adımların referans hâlidir. Yeni bir aggregate veya yeni bir modül eklerken bu sırayı takip edin.

---

## A. Yeni Aggregate Ekleme (mevcut bir modül içinde)

Örnek: `Catalog` modülüne `Brand` aggregate'i ekleniyor.

### 1. Domain — Aggregate
`Modules/<Module>/<Module>.Domain/Entities/<Aggregate>.cs`

- `Aggregate<TId>` türet, `private` ctor + `private set` property'ler.
- Statik `Create(...)` factory: doğrulamaları (`ArgumentException.ThrowIfNullOrWhiteSpace`, `ArgumentOutOfRangeException.ThrowIfNegative`) burada yap, `Id = Guid.NewGuid()`, `IsActive = true`, `IsDeleted = false`.
- Davranış metotları (`UpdateDetails`, `ChangePrice`, `IncreaseStock` ...) — setter yerine metot.
- `Activate()` → `IsActive = true; IsDeleted = false;`
- `Delete()` → `IsActive = false; IsDeleted = true;`
- **`CreatedAt`/`UpdatedAt` domain içinde set edilmez** — `AuditableEntityInterceptor` halleder.
- **`DateTime.Now` kullanılmaz**, her zaman `DateTime.UtcNow`.

### 2. Domain — Domain Event'ler
`<Module>.Domain/Events/<Aggregate><Action>Event.cs`

- `record ...(...) : IDomainEvent` — **sadece primitive/serialize edilebilir alanlar**, aggregate referansı taşıma (outbox'a yazılabilir kalsın).
- Aggregate içinde ilgili davranışta `AddDomainEvent(new ...Event(...))` çağır.
- Değer gerçekten değişmediyse event yayınlama (erken `return`).

### 3. Domain — Repository Interface
`<Module>.Domain/Repositories/I<Aggregate>Repository.cs`

- Saf sözleşme; EF Core tipleri sızdırma.
- Tipik üyeler: `Add`, `Update`, `GetByIdAsync`, `ExistsBy...Async`, `GetPagedAsync`, `CountAsync`, `SaveChangesAsync`.

### 4. Infrastructure — EF Configuration
`<Module>.Infrastructure/Configuration/<Aggregate>Configuration.cs`

- `IEntityTypeConfiguration<T>` yaz (`ApplyConfigurationsFromAssembly` otomatik alır).

### 5. Infrastructure — DbSet
`<Module>.Infrastructure/Persistence/<Module>DbContext.cs`

- `public DbSet<Aggregate> ... { get; set; }` ekle.

### 6. Infrastructure — Repository Implementasyonu
`<Module>.Infrastructure/Repositories/<Aggregate>Repository.cs`

- `DbContext` primary constructor ile inject.
- Okuma sorgularında `AsNoTracking()`.
- **Liste/sayım filtreleri her zaman `!IsDeleted && IsActive`**, tekil getirmede `!IsDeleted`.

### 7. Migration
```powershell
dotnet ef migrations add Add<Aggregate> --project src/Modules/<Module>/<Module>.Infrastructure --startup-project src/Host/Api
```
Migration'lar `UseMigrations<TContext>()` ile başlangıçta uygulanır.

### 8. Application — DTO + Mapping
`<Module>.Application/Features/<Aggregates>/<Aggregate>Dto.cs` ve `<Aggregate>Mappings.cs`

- **Hiçbir mapping kütüphanesi kullanılmaz.** Elle `ToDto()` / `ToDtoList()` extension'ları veya object initializer.

### 9. Application — Feature'lar (Vertical Slice)
Her feature kendi klasöründe, **iki dosya**:

```
<Module>.Application/Features/<Aggregates>/<FeatureName>/
	<FeatureName>Handler.cs    -> Command/Query + Result + Validator + Handler
	<FeatureName>Endpoint.cs   -> Request/Response + Carter ICarterModule
```

**Handler dosyası:**
- `record XCommand(...) : ICommand<XResult>` veya `record XQuery(...) : IQuery<XResult>`
- `record XResult(...)`
- `class XCommandValidator : AbstractValidator<XCommand>` — mevcut `ValidationBehavior` otomatik çalıştırır.
  - ⚠️ `ValidationBehavior` yalnızca `ICommand<TResponse>` için çalışır; **query validasyonunu handler içinde** yapın.
- `class XCommandHandler(IXRepository repository) : ICommandHandler<XCommand, XResult>`
  - Bulunamayan kayıt → `KeyNotFoundException`
  - İş kuralı ihlali → `InvalidOperationException`

**Endpoint dosyası:**
- `record XRequest(...)`, `record XResponse(...)`
- `class XEndpoint : ICarterModule` → `AddRoutes(IEndpointRouteBuilder app)`
- Endpoint sadece request → command/query dönüşümü + `ISender.Send` + response map yapar, iş mantığı içermez.
- `.WithName(...)`, `.WithTags(...)`, `.Produces<T>(...)`, `.ProducesProblem(...)` ekle.

**Standart CRUD seti ve route'lar:**

| Feature | HTTP | Route | Not |
|---|---|---|---|
| Create | POST | `/api/<aggregates>` | `Results.Created` |
| Update | PUT | `/api/<aggregates>/{id:guid}` | |
| Delete | DELETE | `/api/<aggregates>/{id:guid}` | **soft delete** → `entity.Delete()` |
| GetById | GET | `/api/<aggregates>/{id:guid}` | |
| GetList | GET | `/api/<aggregates>` | `[AsParameters]` ile sayfalama, `IsActive && !IsDeleted` |

### 10. Application — Domain Event Handler'ları
`<Module>.Application/Features/<Aggregates>/EventHandlers/<Event>Handler.cs`

- `INotificationHandler<TDomainEvent>` implemente et.
- `DispatchDomainEventsInterceptor` SaveChanges sırasında publish eder; ek kayıt gerekmez.

### 11. DI Kaydı
`<Module>.Api/<Module>Module.cs` → `Add<Module>Module` içine:

```csharp
services.AddScoped<IXRepository, XRepository>();
```

> MediatR / behavior / validator / Carter kayıtları **modülde değil**, Host'ta yapılır.

---

## B. Yeni Modül Ekleme

Aggregate adımlarına ek olarak:

### 1. Projeleri oluştur
```
src/Modules/<Module>/<Module>.Domain          (net10.0, -> BuildingBlocks.Shared)
src/Modules/<Module>/<Module>.Application     (net10.0, -> <Module>.Domain)
src/Modules/<Module>/<Module>.Infrastructure  (net10.0, -> <Module>.Domain)
src/Modules/<Module>/<Module>.Api             (net10.0, -> <Module>.Application, <Module>.Infrastructure)
src/Modules/<Module>/<Module>.Contracts       (modüller arası sözleşmeler)
```

### 2. `<Module>.Application.csproj`
Endpoint'ler Application katmanında durduğu için:
```xml
<ItemGroup>
  <FrameworkReference Include="Microsoft.AspNetCore.App" />
</ItemGroup>
<ItemGroup>
  <PackageReference Include="Carter" />
  <PackageReference Include="MediatR" />
  <PackageReference Include="FluentValidation" />
  <PackageReference Include="Microsoft.EntityFrameworkCore" />
</ItemGroup>
```

### 3. Assembly marker
`<Module>.Application/<Module>ApplicationAssembly.cs`
```csharp
public static class <Module>ApplicationAssembly
{
	public static Assembly Instance => typeof(<Module>ApplicationAssembly).Assembly;
}
```
Host bu marker üzerinden assembly tarar (feature dosyaları yeniden adlandırılınca Host kırılmaz).

### 4. DbContext + Interceptor'lar
`<Module>.Infrastructure/Persistence/<Module>DbContext.cs`
- `modelBuilder.HasDefaultSchema("<module>")`
- `modelBuilder.ApplyConfigurationsFromAssembly(typeof(<Module>DbContext).Assembly)`

### 5. Modül extension'ı
`<Module>.Api/<Module>Module.cs`
- `Add<Module>Module(IServiceCollection, IConfiguration)` → repository'ler, interceptor'lar, `AddDbContext`
- `Use<Module>Module(IApplicationBuilder)` → `app.UseMigrations<<Module>DbContext>()`

### 6. Host'a bağla
`src/Host/Api/Program.cs`
```csharp
var catalogAssembly = CatalogApplicationAssembly.Instance;
var <module>Assembly = <Module>ApplicationAssembly.Instance;

builder.Services.AddCarterWithAssemblies(catalogAssembly, <module>Assembly);
builder.Services.AddMediatRWithAssemblies(catalogAssembly, <module>Assembly);

builder.Services.Add<Module>Module(builder.Configuration);
...
app.Use<Module>Module();
app.MapCarter();
```
`src/Host/Api/GlobalUsings.cs`'e `global using <Module>.Api;` ve `global using <Module>.Application;` ekle.

### 7. Paket sürümleri
Tüm paket sürümleri `Directory.Packages.props` içinde merkezi yönetilir; csproj'lara **versiyonsuz** `PackageReference` yazılır.

---

## C. Bitirme Kontrolleri

- [ ] `dotnet build` / solution build temiz
- [ ] Migration oluşturuldu ve uygulanıyor
- [ ] Hiçbir yerde `DateTime.UtcNow` yok
- [ ] `CreatedAt`/`UpdatedAt` domain içinde set edilmiyor
- [ ] Liste sorguları `!IsDeleted && IsActive` filtreliyor
- [ ] Delete soft delete olarak çalışıyor
- [ ] Mapping kütüphanesi kullanılmamış
- [ ] Endpoint'lerde iş mantığı yok

---

## D. Bilinen Teknik Borçlar

- `ValidationBehavior` yalnızca `ICommand<TResponse>` kısıtına sahip; query'ler için validator çalışmıyor.
- Integration event'ler için outbox + Wolverine entegrasyonu henüz yok.
- `CurrentUser` altyapısı yok; `CreatedBy`/`UpdatedBy` doldurulmuyor.
