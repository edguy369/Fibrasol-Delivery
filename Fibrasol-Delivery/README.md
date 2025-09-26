# Sistema de Gestión de Entregas Fibrasol

Sistema web desarrollado en ASP.NET Core 8.0 para la gestión de órdenes de entrega, facturas, clientes y mensajeros.

## 📋 Tabla de Contenidos
- [Requisitos del Sistema](#requisitos-del-sistema)
- [Tecnologías y Librerías](#tecnologías-y-librerías)
- [Configuración del Proyecto](#configuración-del-proyecto)
- [User Secrets](#user-secrets)
- [Base de Datos](#base-de-datos)
- [Instalación y Ejecución](#instalación-y-ejecución)
- [Funcionalidades](#funcionalidades)
- [Arquitectura](#arquitectura)
- [Usuario por Defecto](#usuario-por-defecto)

## 🖥️ Requisitos del Sistema

- **.NET 8.0 SDK** o superior
- **MySQL 8.0** o superior
- **Visual Studio 2022** o **Visual Studio Code**
- **Git** para control de versiones

## 🛠️ Tecnologías y Librerías

### Framework Principal
- **ASP.NET Core 8.0** - Framework web principal

### Paquetes NuGet y Versiones

| Paquete | Versión | Descripción |
|---------|---------|-------------|
| `Dapper` | 2.1.66 | Micro ORM para acceso a datos |
| `Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation` | 8.0.20 | Compilación en tiempo real de vistas Razor |
| `MySql.Data` | 9.4.0 | Conector oficial de MySQL para .NET |
| `Tipi.Tools.Services.DoSpaces` | 8.0.1 | Servicio para manejo de archivos en Digital Ocean Spaces |

### Frontend
- **Bootstrap 5** - Framework CSS para diseño responsivo
- **jQuery 3.x** - Librería JavaScript para manipulación DOM
- **DataTables 1.13.7** - Plugin para tablas interactivas
- **Bootstrap Icons** - Iconografía del sistema

## ⚙️ Configuración del Proyecto

### User Secrets

El proyecto utiliza **User Secrets** para almacenar información sensible de configuración. Para configurar los secretos de usuario:

#### 1. Verificar User Secrets ID
El proyecto ya tiene configurado el User Secrets ID: `8c0d3baa-4826-46a3-b732-c097a2572ebc`

#### 2. Configurar User Secrets

Abra una terminal en la carpeta raíz del proyecto y ejecute los siguientes comandos:

```bash
# Configurar la cadena de conexión de MySQL
dotnet user-secrets set "ConnectionString" "server=localhost;database=fibrasol_delivery;uid=tu_usuario;password=tu_password;charset=utf8mb4;"

# Configurar credenciales de Digital Ocean Spaces (S3)
dotnet user-secrets set "S3Config:AccessKey" "tu_access_key"
dotnet user-secrets set "S3Config:SecretKey" "tu_secret_key"
dotnet user-secrets set "S3Config:BucketName" "tu_bucket_name"
dotnet user-secrets set "S3Config:Root" "fibrasol/"
dotnet user-secrets set "S3Config:EndpointUrl" "https://nyc3.digitaloceanspaces.com"
dotnet user-secrets set "S3Config:Region" "nyc3"
dotnet user-secrets set "S3Config:UseCdn" "false"
```

#### 3. Verificar User Secrets
Para verificar que los secretos se han configurado correctamente:

```bash
dotnet user-secrets list
```

### Configuración Alternativa con appsettings

Si prefieres usar archivos de configuración (NO recomendado para producción), puedes agregar la siguiente configuración a `appsettings.Development.json`:

```json
{
  "ConnectionString": "server=localhost;database=fibrasol_delivery;uid=tu_usuario;password=tu_password;charset=utf8mb4;",
  "S3Config": {
    "AccessKey": "tu_access_key",
    "SecretKey": "tu_secret_key",
    "BucketName": "tu_bucket_name",
    "Root": "fibrasol/",
    "EndpointUrl": "https://nyc3.digitaloceanspaces.com",
    "Region": "nyc3",
    "UseCdn": "false"
  }
}
```

## 🗄️ Base de Datos

### Configuración de MySQL

1. **Crear la base de datos:**
```sql
CREATE DATABASE fibrasol_delivery CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

2. **Ejecutar el script de creación:**
El archivo `Database/Create.sql` contiene todo el esquema necesario. Ejecutar este script en MySQL:

```bash
mysql -u tu_usuario -p fibrasol_delivery < Database/Create.sql
```

### Estructura de la Base de Datos

La base de datos incluye las siguientes tablas principales:

#### Tablas de Identity Framework
- `Users` - Usuarios del sistema
- `Roles` - Roles de usuario
- `UserRoles` - Relación usuarios-roles
- `UserClaims`, `RoleClaims`, `UserLogins`, `UserTokens` - Tablas auxiliares de Identity

#### Tablas del Dominio
- `Clients` - Clientes
- `Drivers` (Riders) - Mensajeros/Conductores
- `DeliveryOrder` - Órdenes de entrega
- `DeliveryOrderStatus` - Estados de las órdenes
- `DeliveryOrderDrivers` - Asignación de conductores a órdenes
- `BackOrder` - Comandas/Pedidos
- `Invoice` - Facturas

## 🚀 Instalación y Ejecución

### 1. Clonar el repositorio
```bash
git clone [URL_DEL_REPOSITORIO]
cd Fibrasol-Delivery
```

### 2. Restaurar paquetes NuGet
```bash
dotnet restore
```

### 3. Configurar User Secrets
Seguir los pasos de la sección [User Secrets](#user-secrets)

### 4. Configurar la base de datos
Seguir los pasos de la sección [Base de Datos](#base-de-datos)

### 5. Ejecutar el proyecto
```bash
dotnet run
```

### 6. Acceder a la aplicación
- **URL de desarrollo:** `https://localhost:7xxx` (el puerto se asigna automáticamente)
- **URL alternativa:** `http://localhost:5xxx`

## 🎯 Funcionalidades

### Dashboard Principal
- Contadores en tiempo real de mensajeros, clientes, órdenes y facturas
- Tabla de órdenes pendientes de firma
- Actualización automática cada 5 minutos

### Gestión de Mensajeros
- CRUD completo de conductores/mensajeros
- Asignación múltiple a órdenes de entrega

### Gestión de Clientes
- CRUD completo de clientes
- Búsqueda y filtrado

### Órdenes de Entrega
- Creación y edición de órdenes
- Gestión de comandas y facturas asociadas
- Carga de documentos (facturas y facturas firmadas)
- Asignación de múltiples mensajeros
- Impresión de itinerarios

### Sistema de Autenticación
- Login/Logout con ASP.NET Core Identity
- Control de acceso basado en roles
- Sesiones seguras

### Carga de Archivos
- Integración con Digital Ocean Spaces
- Carga de facturas en PDF/imágenes
- Gestión de facturas firmadas

## 🏗️ Arquitectura

### Patrón de Arquitectura
- **MVC (Model-View-Controller)** - Patrón principal
- **Repository Pattern** - Para acceso a datos
- **Unit of Work** - Para manejo de transacciones
- **Dependency Injection** - Para inversión de control

### Estructura de Carpetas
```
├── AuthProvider/           # Extensiones para Identity
├── Config/                # Configuración y registro de servicios
├── Controllers/           # Controladores MVC
├── Database/             # Scripts SQL
├── Models/               # Modelos de datos
├── Repository/           # Repositorios y acceso a datos
├── Request/              # DTOs para requests
├── Views/                # Vistas Razor
└── wwwroot/              # Archivos estáticos
```

### Capas de la Aplicación
1. **Capa de Presentación** - Views (Razor) y Controllers
2. **Capa de Lógica de Negocio** - Controllers y Services
3. **Capa de Acceso a Datos** - Repositories con Dapper
4. **Capa de Datos** - MySQL Database

## 👤 Usuario por Defecto

El sistema incluye un usuario administrador por defecto:

- **Email:** `root@codingtipi.com`
- **Contraseña:** `root` (cambiar en producción)
- **Rol:** Administrador

## 📝 Notas de Desarrollo

### Modo de Desarrollo
- La compilación en tiempo real de Razor está habilitada en desarrollo
- Los logs están configurados para mostrar información detallada
- HTTPS está habilitado por defecto

### Consideraciones de Seguridad
- Cambiar el usuario y contraseña por defecto
- Configurar HTTPS en producción
- Usar User Secrets para información sensible
- Implementar políticas de contraseña más estrictas en producción

### Configuración de Producción
- Configurar connection strings seguras
- Habilitar logging apropiado
- Configurar HSTS y otras cabeceras de seguridad
- Considerar usar Azure Key Vault o similar para secretos

## 🤝 Contribuciones

Para contribuir al proyecto:

1. Fork del repositorio
2. Crear una rama para la funcionalidad (`git checkout -b feature/nueva-funcionalidad`)
3. Commit de los cambios (`git commit -am 'Agregar nueva funcionalidad'`)
4. Push a la rama (`git push origin feature/nueva-funcionalidad`)
5. Crear un Pull Request

## 📄 Licencia

[Especificar la licencia del proyecto]

## 📞 Soporte

Para soporte técnico o consultas, contactar al equipo de desarrollo.

---

**Desarrollado con ❤️ usando ASP.NET Core 8.0**