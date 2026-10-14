
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mundial.Application.Interfaces;
using Mundial.Application.Interfaces.Repositories;
using Mundial.Infrastructure.Persistence;
using Mundial.Infrastructure.Repositories;
using Mundial.Application.Services;
using Mundial.Infrastructure.Xml;

namespace Mundial.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        

        services.AddDbContext<MundialDbContext>(options =>
        {
            options.UseSqlite(
                configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped<PedidoCompraService>();
        services.AddScoped<ProdutoService>();
        services.AddScoped<FornecedorService>();
        services.AddScoped<NotaFiscalXmlParser>();
        services.AddScoped<RecebimentoFiscalService>();
        services.AddScoped<IFornecedorRepository, FornecedorRepository>();
        services.AddScoped<IAcordoComercialRepository, AcordoComercialRepository>();
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<IPedidoCompraRepository, PedidoCompraRepository>();
        services.AddScoped<IDocumentoFiscalRepository, DocumentoFiscalRepository>();
        services.AddScoped<INotaFiscalXmlParser, NotaFiscalXmlParser>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
