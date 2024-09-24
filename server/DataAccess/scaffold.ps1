dotnet ef dbcontext scaffold `
  "Server=localhost;Database=dundermifflin;User Id=user;Password=pass;" `
  Npgsql.EntityFrameworkCore.PostgreSQL `
  --output-dir Models `
  --context-dir . `
  --context AppDbContext `
  --no-onconfiguring `
  --data-annotations `
  --force