
# Honamic PayMaster Module

The **PayMaster** module is part of the **Honamic Platform**, developed and maintained as an **independent, open-source, and free** project.  
It manages the **full payment lifecycle**, currently supporting **online payments**, with planned support for **cash, remittance, and other payment methods** in the future.

---

## ✨ Features
- Complete payment lifecycle management  
- Built on the [**Honamic Framework**](https://github.com/honamic/Framework) with support for **DDD**, **CQRS**, and more  
- Follows **Onion** and **Clean Architecture** principles  
- Easy integration via `Honamic.PayMaster.Wrapper` without requiring deep knowledge of the framework  
- Extensible with multiple payment providers  

---

## 🚀 Getting Started

### 1. Install NuGet Package
```powershell
dotnet add package Honamic.PayMaster.Wrapper
```

### 2. Add PayMaster Entities to `EFDbContext`
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // ...
    modelBuilder.AddPayMasterModelsVersionX();

    base.OnModelCreating(modelBuilder);
}
```

### 3. Register Services
```csharp
builder.Services.AddPayMasterWrapper(option =>
{
    option.UseEntityFrameWorkPersistence<SampleDbContext>();
    // option.UseSqlServerPersistence(sqlServerConnection);

    option.UseSqlServerQueryModel(sqlServerConnection!);
    // option.UseEntityFrameWorkQueryModel<SampleQueryDbContext>();

    option.Configure(config =>
    {
        config.CallBackUrl = "https://localhost:7777/api/paymaster/callback/{GatewayPaymentId}/";
        config.SupportedCurrencies = ["IRR", "USD"];
    });

    option.Services.AddAllPaymentProviderServices();
    option.Services.AddDigipayPaymentProviderServices();
    option.Services.AddSandboxWebPaymentProviderServices();

    builder.Services.AddDefaultUserContextService<DefaultUserContext>();
    builder.Services.AddScoped<IAuthorization, DefaultAuthorization>();
});
```

### 4. (Optional) Add Payment Endpoints
```csharp
app.MapPayMasterEndpoints();
```

### 5. (Optional) Add Management Endpoints
```csharp
app.MapPayMasterManagementEndpoints();
```

---

## ▶️ Run the Sample Project
For a better understanding of the module and its capabilities, **run the sample project** included in the repository.  
This will demonstrate how to configure, integrate, and use the PayMaster module in a real-world scenario.

