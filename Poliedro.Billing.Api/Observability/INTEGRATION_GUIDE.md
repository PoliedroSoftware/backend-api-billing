// Ejemplo de integración de Observability en Program.cs

/*
========== PASO 1: Instalar prometheus-net via NuGet ==========

dotnet add package prometheus-net
dotnet add package prometheus-net.AspNetCore

========== PASO 2: Registrar servicios en builder.Services ==========

En la sección donde registras AddWebApi(), AddApplication(), etc., agrega:

    builder.Services
        .AddWebApi()
        .AddApplication()
        .AddObservability()  // ← AGREGAR ESTA LÍNEA
        .AddExternalPlemsi(builder.Configuration)
        .AddExternalTns(builder.Configuration)
        .AddExternalSiigo(builder.Configuration)
        .AddPersistence(builder.Configuration);

========== PASO 3: Configurar middleware ==========

Después de: var app = builder.Build();

    var app = builder.Build();
    
    app.UsePrometheusMetrics()  // ← Middleware HTTP metrics
       .MapMetrics();            // ← Mapea endpoint /metrics

    // Luego el resto de middleware...
    app.UseRouting();
    app.MapControllers();
    // ... etc

========== RESULTADO: ==========

✓ GET /metrics → Expone todas las métricas en formato Prometheus
✓ Todas las requests HTTP capturadas (latencia, status code, etc)
✓ Todos los CreateBillingCommand capturados (éxito, fallo, duración)
✓ Todas las llamadas a Plemsi capturadas (éxito, error, duración)

========== QUERIES PROMETHEUS ÚTILES ==========

# Total de facturas exitosas
billing_invoice_success_total

# Tasa de error (5 min)
rate(billing_invoice_failed_total[5m])

# P95 de latencia de facturación
histogram_quantile(0.95, billing_invoice_duration_seconds_bucket)

# Llamadas a Plemsi por segundo
rate(billing_plemsi_requests_total[1m])

# Tasa de error en Plemsi (últimos 5 min)
rate(billing_plemsi_errors_total[5m])

# P99 de latencia en Plemsi
histogram_quantile(0.99, billing_plemsi_duration_seconds_bucket)

# Estado actual de Plemsi
billing_provider_status{provider="plemsi"}

# Requests HTTP por endpoint (prometheus-net automático)
http_requests_received_total{endpoint="/api/v1/billing"}

# Latencia HTTP P95
histogram_quantile(0.95, http_request_duration_seconds_bucket)

*/
