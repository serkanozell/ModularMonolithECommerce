# Yeni Aggregate Ekleme Adımları

Bu doküman, `Catalog` modülündeki `Product` ve `Category` aggregate'leri referans alınarak
projeye yeni bir aggregate'in nasıl ekleneceğini anlatır. **Bir aggregate oluşturulduğunda
bu doküman yol haritası ve kural seti olarak kullanılır.**

Aşağıdaki örneklerde `<Module>` yerine modül adı (örn. `Catalog`, `Ordering`),
`<Aggregate>` yerine aggregate adı (örn. `Product`, `Order`) yazılmalıdır.

---

## 0. Altın Kurallar

- Hiçbir koşulda `DateTime.Now` **kullanılmaz**. Her zaman ve her yerde `DateTime.UtcNow` kullanılır.
- `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy` alanları domain içinde **elle set edilmez**.
  Bu alanlar `SaveChanges` interceptor'ı tarafından doldurulur.
- `Activate()` çağrıldığında: `IsActive = true`, `IsDeleted = false`.
- `Delete()` çağrıldığında: `IsActive = false`, `IsDeleted = true` (soft delete; kayıt fiziksel olarak silinmez).
- Aggregate dışından state değiştirilemez: tüm property'ler `private set`, davranışlar metotlarla verilir.
- Aggregate root dışındaki entity'lere repository yazılmaz; erişim her zaman root üzerinden yapılır.

---

## 1. Aggregate Root Sınıfı (`<Module>.Domain/Entities`)

Dosya: `src/Modules/<Module>/<Module>.Domain/Entities/<Aggregate>.cs`

```csharp
using BuildingBlocks.Shared;

namespace <Module>.Domain.Entities
{
	public class <Aggregate> : Aggregate<Guid>
	{
		private <Aggregate>() { }

		private <Aggregate>(string name)
		{
			Id = Guid.NewGuid();
			Name = name;
			IsActive = true;
			IsDeleted = false;
		}

		public string Name { get; private set; } = default!;

		public static <Aggregate> Create(string name)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(name);

			return new <Aggregate>(name);
		}

		public void Update(string name)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(name);
			Name = name;
		}

		public void Activate()
		{
			IsActive = true;
			IsDeleted = false;
		}

		public void Delete()
		{
			IsActive = false;
			IsDeleted = true;
		}
	}
}
```

Kurallar:
- Aggregate root `Aggregate<TId>`, aggregate içindeki child entity'ler `Entity<TId>` türetir.
- `private` parametresiz constructor bulunmalıdır (EF Core materialization için).
- Ayrıca alanları set eden **`private` parametreli bir constructor** bulunmalıdır; nesnenin state'i burada kurulur.
- Nesne oluşturma her zaman statik `Create` factory metodu ile yapılır; `Create` doğrulamaları yapar ve nesneyi **private parametreli constructor üzerinden** üretir. Object initializer ile oluşturma yapılmaz, `public` constructor yazılmaz.
- Doğrulamalar (`ArgumentException.ThrowIfNullOrWhiteSpace`, `ArgumentOutOfRangeException.ThrowIfNegative` vb.) factory ve davranış metotlarının başında yapılır.
- Koleksiyonlar `private readonly List<T>` olarak tutulur, dışarıya `IReadOnlyCollection<T>` olarak açılır.
- Domain event gerekiyorsa `AddDomainEvent(...)` ile eklenir.
- `<Module>.Domain` projesi `BuildingBlocks.Shared` projesine referans vermelidir.

---

## 2. Repository Arayüzü (`<Module>.Domain`)

Her aggregate root için **bir** repository yazılır.

Dosya: `src/Modules/<Module>/<Module>.Domain/Repositories/I<Aggregate>Repository.cs`

```csharp
using <Module>.Domain.Entities;

namespace <Module>.Domain.Repositories
{
	public interface I<Aggregate>Repository
	{
		Task<<Aggregate>?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
		Task<IReadOnlyList<<Aggregate>>> GetAllAsync(CancellationToken cancellationToken = default);
		Task AddAsync(<Aggregate> entity, CancellationToken cancellationToken = default);
		void Update(<Aggregate> entity);
		void Remove(<Aggregate> entity);
	}
}
```

---

## 3. Repository Implementasyonu (`<Module>.Infrastructure`)

Dosya: `src/Modules/<Module>/<Module>.Infrastructure/Repositories/<Aggregate>Repository.cs`

```csharp
using <Module>.Domain.Entities;
using <Module>.Domain.Repositories;
using <Module>.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace <Module>.Infrastructure.Repositories
{
	public class <Aggregate>Repository : I<Aggregate>Repository
	{
		private readonly <Module>DbContext _context;

		public <Aggregate>Repository(<Module>DbContext context) => _context = context;

		public Task<<Aggregate>?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
			=> _context.<Aggregate>s.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

		public async Task<IReadOnlyList<<Aggregate>>> GetAllAsync(CancellationToken cancellationToken = default)
			=> await _context.<Aggregate>s.AsNoTracking().ToListAsync(cancellationToken);

		public async Task AddAsync(<Aggregate> entity, CancellationToken cancellationToken = default)
			=> await _context.<Aggregate>s.AddAsync(entity, cancellationToken);

		public void Update(<Aggregate> entity) => _context.<Aggregate>s.Update(entity);

		public void Remove(<Aggregate> entity) => entity.Delete();   // soft delete
	}
}
```

Kurallar:
- Repository sadece **aggregate root** için yazılır.
- Fiziksel silme yapılmaz; `Remove` soft delete uygular.
- Salt okuma sorgularında `AsNoTracking()` kullanılır.
- `SaveChanges` repository içinde çağrılmaz; Unit of Work / handler seviyesinde çağrılır.

---

## 4. Entity Configuration (`<Module>.Infrastructure`)

Her aggregate ve child entity için ayrı bir `IEntityTypeConfiguration<T>` yazılır.

Dosya: `src/Modules/<Module>/<Module>.Infrastructure/Persistence/Configurations/<Aggregate>Configuration.cs`

```csharp
using <Module>.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace <Module>.Infrastructure.Persistence.Configurations
{
	public class <Aggregate>Configuration : IEntityTypeConfiguration<<Aggregate>>
	{
		public void Configure(EntityTypeBuilder<<Aggregate>> builder)
		{
			builder.ToTable("<Aggregate>s");

			builder.HasKey(x => x.Id);

			builder.Property(x => x.Name)
				   .IsRequired()
				   .HasMaxLength(200);

			builder.Ignore(x => x.DomainEvents);

			builder.HasQueryFilter(x => !x.IsDeleted);
		}
	}
}
```

Kurallar:
- Tablo adı, alan uzunlukları, `decimal` precision ve index'ler burada tanımlanır.
- `DomainEvents` her zaman `Ignore` edilir.
- Soft delete için `HasQueryFilter(x => !x.IsDeleted)` eklenir.
- İlişkiler root'un configuration'ında tanımlanır (`HasMany` / `WithOne`).

---

## 5. DbContext'e Ekleme

Dosya: `src/Modules/<Module>/<Module>.Infrastructure/Persistence/<Module>DbContext.cs`

```csharp
public DbSet<<Aggregate>> <Aggregate>s => Set<<Aggregate>>();

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
	modelBuilder.HasDefaultSchema("<module>");
	modelBuilder.ApplyConfigurationsFromAssembly(typeof(<Module>DbContext).Assembly);
	base.OnModelCreating(modelBuilder);
}
```

Kurallar:
- Her aggregate root için bir `DbSet<>` eklenir. Child entity'ler için `DbSet` eklenmez.
- Configuration'lar tek tek değil, `ApplyConfigurationsFromAssembly` ile toplu uygulanır.
- Her modül kendi şemasını kullanır (`HasDefaultSchema`).

---

## 6. DI Kaydı

`src/Modules/<Module>/<Module>.Api/<Module>Module.cs` içindeki `Add<Module>Module` metoduna eklenir:

```csharp
services.AddScoped<I<Aggregate>Repository, <Aggregate>Repository>();
```

---

## 7. Migration

```powershell
dotnet ef migrations add Add<Aggregate> `
  --project src/Modules/<Module>/<Module>.Infrastructure `
  --startup-project src/Host/Api `
  --context <Module>DbContext
```

---

## 8. Doğrulama

```powershell
dotnet build ModularMonolithECommerce.slnx
```

---

## Kontrol Listesi

- [ ] Aggregate root `Entities` klasörü altında, `Aggregate<TId>` türetilerek oluşturuldu
- [ ] `private` parametresiz constructor (EF Core) ve alanları set eden `private` parametreli constructor eklendi
- [ ] Statik `Create` factory metodu doğrulama yapıp nesneyi private parametreli constructor ile üretiyor
- [ ] Tüm property'ler `private set`, state değişimi davranış metotlarıyla yapılıyor
- [ ] `Activate()` → `IsActive = true`, `IsDeleted = false`
- [ ] `Delete()` → `IsActive = false`, `IsDeleted = true`
- [ ] `CreatedAt` / `UpdatedAt` alanları domain içinde set **edilmedi** (interceptor hallediyor)
- [ ] Hiçbir yerde `DateTime.Now` kullanılmadı (`DateTime.UtcNow` kullanıldı)
- [ ] `I<Aggregate>Repository` arayüzü Domain katmanında oluşturuldu
- [ ] `<Aggregate>Repository` implementasyonu Infrastructure katmanında oluşturuldu
- [ ] `<Aggregate>Configuration` (`IEntityTypeConfiguration<>`) yazıldı
- [ ] `DomainEvents` ignore edildi ve soft delete query filter eklendi
- [ ] `DbContext` içine `DbSet<<Aggregate>>` eklendi
- [ ] Repository DI kaydı `<Module>Module` içinde yapıldı
- [ ] Migration oluşturuldu
- [ ] Solution başarıyla derlendi
