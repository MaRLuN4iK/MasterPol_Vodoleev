using System;
using Npgsql;
using Npgsql.EntityFrameworkCore;

namespace MasterPol.App
{
    public static class ForceLoad
    {
        // Просто обращаемся к классу, using уже подключен
        private static readonly bool _ensureAssemblyLoaded =
            NpgsqlServices.Instance != null;
    }
}