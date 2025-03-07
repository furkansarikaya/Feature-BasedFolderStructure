# FeatureBasedFolderStructure

Bu proje, C# dilinde yazılmış ve MediatR ile .NET 9 kullanılarak geliştirilmiş bir ürün ve kategori yönetim API'sidir. Proje, özellik tabanlı bir klasör yapısı kullanmaktadır.

## Teknolojiler

- .NET 9
- Entity Framework Core 9.0
- PostgreSQL
- MediatR 12.4.1
- AutoMapper 14.0.0
- FluentValidation 11.11.0
- Scalar/OpenAPI
- JWT Authentication

## Mimari ve Desenler

- Clean Architecture
- CQRS Pattern
- Mediator Pattern
- Repository Pattern
- Feature-based Folder Structure

## Özellikler

- RESTful API endpoint'leri
- Ürün ve Kategori CRUD işlemleri
- Kullanıcı yönetimi ve kimlik doğrulama
- JWT tabanlı yetkilendirme
- Refresh Token desteği
- Role tabanlı yetkilendirme
- FluentValidation ile veri doğrulama
- Global exception handling
- PostgreSQL veritabanı desteği
- Scalar ile API dokümantasyonu
- Clean Architecture
- CQRS ve Mediator Pattern implementasyonu
- Repository Pattern
- Özellik tabanlı klasör yapısı

## Gereksinimler

- .NET 9.0 SDK
- PostgreSQL 15 veya üzeri

## Kurulum

1. Bu projeyi klonlayın:

    ```sh
    git clone https://github.com/furkansarikaya/FeatureBasedFolderStructure.git
    cd FeatureBasedFolderStructure
    ```

2. Gerekli bağımlılıkları yükleyin:

    ```sh
    dotnet restore
    ```

3. `appsettings.Development.json` dosyasındaki veritabanı bağlantı ayarlarını güncelleyin:

    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "PORT=5432;TIMEOUT=120;POOLING=True;MINPOOLSIZE=1;MAXPOOLSIZE=5;COMMANDTIMEOUT=180;DATABASE=FeatureBasedFolderDB;HOST=localhost;PASSWORD=1;USER ID=postgres"
      }
    }
    ```

4. Veritabanını oluşturun ve gerekli migrasyonları uygulayın:

    ```sh
    dotnet ef database update
    ```

## Kullanım

Projeyi çalıştırmak için:

```sh
dotnet run --project src/FeatureBasedFolderStructure.API
```

## 🚀 API Endpoints

### Ürünler API

| Metod | Endpoint | Açıklama | İstek Gövdesi | Dönüş Tipi |
|-------|----------|----------|---------------|------------|
| GET | `/api/products` | Tüm ürünleri listeler | - | `BaseResponse<List<ProductDto>>` |
| GET | `/api/products/{id}` | Ürün detayını getirir | - | `BaseResponse<ProductDto>` |
| POST | `/api/products` | Yeni ürün oluşturur | `CreateProductCommand` | `BaseResponse<int>` |
| PUT | `/api/products/{id}` | Ürün günceller | `UpdateProductCommand` | `BaseResponse<Unit>` |
| DELETE | `/api/products/{id}` | Ürün siler | - | `BaseResponse<Unit>` |

### Kategoriler API

| Metod | Endpoint | Açıklama | İstek Gövdesi | Dönüş Tipi |
|-------|----------|----------|---------------|------------|
| GET | `/api/categories` | Tüm kategorileri listeler | - | `BaseResponse<List<CategoryDto>>` |
| GET | `/api/categories/{id}` | Kategori detayını getirir | - | `BaseResponse<CategoryDto>` |
| POST | `/api/categories` | Yeni kategori oluşturur | `CreateCategoryCommand` | `BaseResponse<int>` |
| PUT | `/api/categories/{id}` | Kategori günceller | `UpdateCategoryCommand` | `BaseResponse<Unit>` |
| DELETE | `/api/categories/{id}` | Kategori siler | - | `BaseResponse<Unit>` |

### Auth API

| Metod | Endpoint | Açıklama | İstek Gövdesi | Dönüş Tipi                      |
|-------|----------|----------|---------------|---------------------------------|
| POST | `/api/auth/register` | Kullanıcı kaydı | `RegisterCommand` | `BaseResponse<RegisterDto>`     |
| POST | `/api/auth/login` | Kullanıcı girişi | `LoginCommand` | `BaseResponse<LoginDto>`        |
| POST | `/api/auth/refresh-token` | Token yenileme | `RefreshTokenCommand` | `BaseResponse<RefreshTokenDto>` |
| POST | `/api/auth/logout` | Kullanıcı çıkışı | `LogoutCommand` | `BaseResponse<Unit>`            |
| POST | `/api/auth/change-password` | Şifre değiştirme | `ChangePasswordCommand` | `BaseResponse<Unit>`            |
| POST | `/api/auth/forgot-password` | Şifre sıfırlama isteği | `ForgotPasswordCommand` | `BaseResponse<string>`          |
| POST | `/api/auth/reset-password` | Şifre sıfırlama | `ResetPasswordCommand` | `BaseResponse<Unit>`            |

### HTTP Durum Kodları

| Kod | Açıklama      |
|-----|---------------|
| 200 | Başarılı      |
| 201 | Oluşturuldu   |
| 204 | İçerik Yok    |
| 400 | Hatalı İstek  |
| 401 | Yetkisiz      |
| 403 | Yasaklı       |
| 404 | Bulunamadı    |
| 500 | Sunucu Hatası |

## Proje Yapısı

### `src/FeatureBasedFolderStructure.API/`
- Web API katmanı
- Controller tanımlamaları
- Program başlangıç yapılandırması
- Middleware konfigürasyonları
- Scalar entegrasyonu

### `src/FeatureBasedFolderStructure.Application/`
- İş mantığı katmanı
- CQRS pattern implementasyonları
  - Commands
  - Queries
  - DTOs
- Validasyon kuralları
- Interfaces
- Common
  - Behaviors
  - Exceptions
  - Mappings

### `src/FeatureBasedFolderStructure.Infrastructure/`
- Veritabanı işlemleri
- Entity Framework Core yapılandırması
- Repository implementasyonları
- Migrations
- Harici servis entegrasyonları

### Ana Klasörler
```sh
📦 FeatureBasedFolderStructure
 ┣ 📂src
 ┃ ┣ 📂FeatureBasedFolderStructure.API
 ┃ ┃ ┣ 📂Controllers
 ┃ ┃ ┣ 📂Configurations
 ┃ ┃ ┗ 📂Extensions
 ┃ ┣ 📂FeatureBasedFolderStructure.Application
 ┃ ┃ ┣ 📂Common
 ┃ ┃ ┃ ┣ 📂Behaviors
 ┃ ┃ ┃ ┣ 📂Exceptions
 ┃ ┃ ┃ ┣ 📂Interfaces
 ┃ ┃ ┃ ┣ 📂Mappings
 ┃ ┃ ┃ ┗ 📂Models
 ┃ ┃ ┗ 📂Features
 ┃ ┃   ┣ 📂v1
 ┃ ┃   ┃ ┣ 📂Auth
 ┃ ┃   ┃ ┃ ┣ 📂Commands
 ┃ ┃   ┃ ┃ ┃ ┣ 📂Login
 ┃ ┃   ┃ ┃ ┃ ┣ 📂Logout
 ┃ ┃   ┃ ┃ ┃ ┣ 📂ForgotPassword
 ┃ ┃   ┃ ┃ ┃ ┣ 📂ChangePassword
 ┃ ┃   ┃ ┃ ┃ ┣ 📂Register
 ┃ ┃   ┃ ┃ ┃ ┣ 📂ResetPassword
 ┃ ┃   ┃ ┃ ┃ ┗ 📂RefreshToken
 ┃ ┃   ┃ ┃ ┣ 📂DTOs
 ┃ ┃   ┃ ┃ ┣ 📂Rules
 ┃ ┃   ┃ ┃ ┗ 📂Validations
 ┃ ┃   ┃ ┣ 📂Products
 ┃ ┃   ┃ ┃ ┣ 📂Commands
 ┃ ┃   ┃ ┃ ┣ 📂DTOs
 ┃ ┃   ┃ ┃ ┗ 📂Queries
 ┃ ┃   ┃ ┗ 📂Categories
 ┃ ┃   ┃   ┣ 📂Commands
 ┃ ┃   ┃   ┣ 📂DTOs
 ┃ ┃   ┃   ┗ 📂Queries
 ┃ ┣ 📂FeatureBasedFolderStructure.Domain
 ┃ ┃ ┣ 📂Common
 ┃ ┃ ┃ ┗ 📂Interfaces
 ┃ ┃ ┣ 📂Entities
 ┃ ┃ ┗ 📂Enums
 ┃ ┗ 📂FeatureBasedFolderStructure.Infrastructure
 ┃   ┣ 📂Extensions
 ┃   ┣ 📂Persistence
 ┃   ┃ ┣ 📂Configurations
 ┃   ┃ ┣ 📂Migrations
 ┃   ┃ ┣ 📂Paging
 ┃   ┃ ┗ 📂Repositories
 ┃   ┗ 📂Services
 ┗ 📜README.md