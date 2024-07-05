# Manajemen Karyawan
API yang dikembangkan menggunakan .NET Core dirancang untuk menyediakan solusi dalam manajemen karyawan, fokus pada entitas Employee dan Department. 

Video demo: https://youtu.be/4I7SldIOGPQ

Pertanyaan non teknis: https://youtu.be/6xy5IzqdDww

Hackerrank Certificate: https://www.hackerrank.com/certificates/56bd38923a10


## Cara install

### Prerequisites

- Visual Studio 2022
- SQLServer
- .NET 8

### Install
- Clone repo
- Buka dengan Visual Studio 2022
- Sesuaikan "DefaultConnection": "Server=localhost\\SQLEXPRESS02;Database=DOT;Trusted_Connection=True;TrustServerCertificate=True;" dengan SQLServer
- "Add-Migration InitialCreate" pada Package manager console
- "Update-Database" pada Package manager console
- Run
- "https://localhost:[port]/swagger"
