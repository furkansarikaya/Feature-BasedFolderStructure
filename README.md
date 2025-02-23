# FeatureBasedFolderStructure

Bu proje, C# dilinde yazılmış ve MediatR ile .NET 9 kullanılarak geliştirilmiş bir ürün yönetim API'sidir. Proje, özellik tabanlı bir klasör yapısı kullanmaktadır.

## Teknolojiler

- .NET 9
- Entity Framework Core 9.0
- PostgreSQL
- MediatR 12.4.1
- AutoMapper 14.0.0
- FluentValidation 11.11.0
- Swagger/OpenAPI

## Mimari ve Desenler

- Clean Architecture
- CQRS Pattern
- Mediator Pattern
- Repository Pattern
- Feature-based Folder Structure

## Özellikler

- RESTful API endpoints
- Ürün CRUD işlemleri
- FluentValidation ile veri doğrulama
- Global exception handling
- PostgreSQL veritabanı desteği
- Swagger UI ile API dokümantasyonu

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

### API Endpoints

- **GET** `/api/products`
   - **Açıklama**: Tüm ürünleri listeler
   - **Dönüş**: `List<ProductDto>`

- **GET** `/api/products/{id}`
   - **Açıklama**: Belirli bir ürünün detaylarını getirir
   - **Parametreler**: `id` (int)
   - **Dönüş**: `ProductDto`

- **POST** `/api/products`
   - **Açıklama**: Yeni bir ürün oluşturur
   - **Gövde**: `CreateProductCommand`
   - **Dönüş**: Oluşturulan ürünün ID'si (int)

- **PUT** `/api/products/{id}`
   - **Açıklama**: Mevcut bir ürünü günceller
   - **Parametreler**: `id` (int)
   - **Gövde**: `UpdateProductCommand`
   - **Dönüş**: `204 No Content`

## Proje Yapısı

### `src/FeatureBasedFolderStructure.API/`
- Web API katmanı
- Controller tanımlamaları
- Program başlangıç yapılandırması
- Middleware konfigürasyonları
- Swagger entegrasyonu

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
 ┃ ┃ ┗ 📂Configurations
 ┃ ┣ 📂FeatureBasedFolderStructure.Application
 ┃ ┃ ┣ 📂Common
 ┃ ┃ ┃ ┣ 📂Behaviors
 ┃ ┃ ┃ ┣ 📂Exceptions
 ┃ ┃ ┃ ┗ 📂Mappings
 ┃ ┃ ┗ 📂Features
 ┃ ┃   ┗ 📂Products
 ┃ ┃     ┣ 📂Commands
 ┃ ┃     ┣ 📂DTOs
 ┃ ┃     ┗ 📂Queries
 ┃ ┗ 📂FeatureBasedFolderStructure.Infrastructure
 ┃   ┣ 📂Persistence
 ┃   ┣ 📂Services
 ┃   ┗ 📂Migrations
 ┗ 📜README.md
