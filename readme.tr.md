# Personel Website

> Modern .NET ve PostgreSQL kullanılarak geliştirilmiş, Clean Architecture tabanlı güçlü bir CMS.

---

## 🔹 Genel Bakış

**Personel Website**, kişisel portföylerin ve içeriklerin yönetimi için tasarlanmış, kurumsal seviye mimari standartlara sahip modüler bir web uygulamasıdır. **Clean Architecture** prensipleri üzerine inşa edilmiştir ve katmanlar arasında net bir sorumluluk ayrımı sağlayarak sistemin **bakımını kolay**, **test edilebilir** ve **ölçeklenebilir** olmasını garanti eder.

Bu proje; katı, monolitik kişisel site şablonlarının yarattığı kısıtları ortadan kaldırmayı hedefler. Gelişmiş **kimlik ve rol yönetimi**, **dinamik dil desteği** ve **ince ayarlanabilir konfigürasyon yapısı** sunan esnek bir backend altyapısı sağlar. Tüm bu yapı, profesyonel bir admin paneli ile desteklenir.

Aşağıdaki hedef kitleler için idealdir:

* Gerçek dünya örneği üzerinden Clean Architecture incelemek isteyen .NET geliştiriciler
* Kendi markası için self-hosted ve özelleştirilebilir bir CMS arayan profesyoneller

---

## 🔹 Temel Özellikler

* **Clean Architecture**
  Domain, Application, Infrastructure ve Presentation katmanlarına sıkı şekilde ayrılmış yapı ile bağımlılık kurallarının korunması.

* **Rol Tabanlı Kimlik Yönetimi (RBAC)**
  **ASP.NET Core Identity** altyapısı üzerinde geliştirilmiş kapsamlı kullanıcı ve rol yönetimi.

* **Dinamik Lokalizasyon**
  Uygulama yeniden dağıtılmadan, veritabanı üzerinden gerçek zamanlı dil ve çeviri yönetimi.

* **Gelişmiş Admin Paneli**
  İçerik, kullanıcı ve sistem ayarlarının yönetimi için **Metronic** tabanlı yönetim arayüzü.

* **Modüler Yapı**
  Language, Configuration gibi feature-based organizasyon sayesinde kolay genişletilebilir mimari.

* **Docker Desteği**
  Uygulama ve PostgreSQL veritabanını içeren tam containerize yapı.

---

## 🔹 Teknoloji Yığını

### Çekirdek

* **Dil:** C#
* **Framework:** .NET (ASP.NET Core)
* **Veritabanı:** PostgreSQL
* **ORM:** Entity Framework Core (Npgsql)

### Mimari & Kütüphaneler

* **Mapping:** AutoMapper
* **Validation:** FluentValidation
* **Dependency Injection:** ASP.NET Core yerleşik DI

### Frontend

* **Render Engine:** ASP.NET Core MVC (Razor Views)
* **Admin Tema:** Metronic (HTML5, CSS3, JavaScript)
* **UI Kütüphaneleri:** jQuery, Bootstrap, Flatpickr, Select2

### DevOps

* **Containerization:** Docker, Docker Compose

---

## 🔹 Mimari & Tasarım

Çözüm, **Clean Architecture (Onion Architecture)** yaklaşımını temel alır:

```text
src/
├── Core/
│   ├── PW.Domain/          # İş kuralları, Entity’ler, Enum’lar (Bağımsız)
│   └── PW.Application/     # Uygulama mantığı, DTO’lar, Arayüzler (Domain’e bağımlı)
├── Infrastructure/
│   ├── PW.Persistence/     # DbContext’ler, Repository’ler, Migration’lar
│   ├── PW.Identity/        # Kimlik servisleri, Auth context, kullanıcı mantığı
│   └── PW.Services/        # Harici servisler (Dosya, E-posta vb.)
└── Presentation/
    └── PW.Web/             # MVC uygulaması, Controller’lar, View’lar
```

### Kullanılan Temel Pattern’ler

* **Repository & Unit of Work** – Veri erişim katmanının soyutlanması
* **Orchestrator Pattern** – Web katmanında Controller ile Application katmanı arasındaki akışın yönetilmesi
* **Dependency Injection** – Katmanlar arası bağımlılıkların gevşetilmesi

---

## 🔹 Kurulum & Çalıştırma

### Gereksinimler

* Docker Desktop
* .NET SDK (`global.json` ile uyumlu veya en güncel stabil sürüm)

---

### Seçenek 1: Docker Compose (Önerilen)

Uygulama ve PostgreSQL veritabanını otomatik olarak ayağa kaldırır.

```bash
git clone https://github.com/karabeyogluonur/personel-website.git
cd personel-website
docker-compose up --build
```

* Uygulama: [http://localhost:8080](http://localhost:8080)
* PostgreSQL: `5433` portu

---

### Seçenek 2: Lokal Geliştirme

#### Veritabanı Ayarları

PostgreSQL çalışır durumda olmalıdır. `appsettings.json` dosyasını güncelleyin:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=PersonelWebsiteDB;Username=your_user;Password=your_password"
}
```

#### Migration’ları Uygulama

```bash
cd src/Presentation/PW.Web
dotnet ef database update --context PWDbContext
dotnet ef database update --context AuthDbContext
```

#### Uygulamayı Çalıştırma

```bash
dotnet run
```

---

## 🔹 Kullanım

### Admin Paneli

* Uygulama ilk çalıştırmada varsayılan verileri seed eder (`IdentityInitialiser.cs`).
* `/Admin` adresinden yönetim paneline erişebilirsiniz.
* Varsayılan kullanıcı bilgileri yoksa yeni kullanıcı oluşturup rollerini atayabilirsiniz.

### Dil Yönetimi (Lokalizasyon)

* Admin panelindeki **Language** bölümünden dil ekleyebilir veya düzenleyebilirsiniz.
* Çeviri anahtarları (key/value) veritabanı üzerinden dinamik olarak yönetilir.
* Bu yapı özel olarak geliştirilmiş `LanguageService` tarafından sağlanır.

---

## 🔹 Konfigürasyon

Konfigürasyonlar `appsettings.json` ve ortam değişkenleri (Environment Variables) üzerinden yönetilir.

| Ayar Anahtarı                       | Açıklama                    | Ortam Değişkeni                        |
| ----------------------------------- | --------------------------- | -------------------------------------- |
| ConnectionStrings:DefaultConnection | PostgreSQL bağlantı bilgisi | `ConnectionStrings__DefaultConnection` |
| ASPNETCORE_ENVIRONMENT              | Çalışma ortamı              | `ASPNETCORE_ENVIRONMENT`               |

Roller, Area isimleri ve Storage Path gibi sabitler aşağıdaki dizinde tanımlıdır:

```
PW.Application/Common/Constants
```

---

## 🔹 Dağıtım (Deployment)

### Docker ile Production Dağıtımı

```bash
docker build -t personel-website .
docker run -d -p 80:8080 \
  -e ConnectionStrings__DefaultConnection="<PROD_DB_STRING>" \
  personel-website
```

> ⚠️ Production ortamında hem `PWDbContext` hem de `AuthDbContext` için migration’ların uygulanmış olduğundan emin olun.

---

## 🔹 Yol Haritası

* [ ] CMS modülleri (dinamik sayfalar, blog yönetimi)
* [ ] Headless CMS desteği (REST / GraphQL)
* [ ] Unit ve integration testleri (xUnit)
* [ ] GitHub Actions ile CI/CD kurulumu

---

## 🔹 Katkıda Bulunma

Katkılar memnuniyetle karşılanır.

```bash
git checkout -b feature/short-description
git commit -m "feat: add short feature description"
git push origin feature/short-description
```

Lütfen mevcut mimari kurallara, isimlendirme standartlarına ve clean code prensiplerine uyun.

---

## 🔹 Lisans

Bu proje açık kaynaklıdır. Lisans bilgileri için repository içerisindeki lisans dosyasını inceleyiniz. Lisans belirtilmemişse, standart telif hakları geçerlidir.
