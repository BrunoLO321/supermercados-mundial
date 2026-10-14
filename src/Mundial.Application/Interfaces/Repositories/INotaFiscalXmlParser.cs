using Mundial.Application.DTOs.NotasFiscais;

namespace Mundial.Application.Interfaces;

public interface INotaFiscalXmlParser
{
    NotaFiscalXmlDto Parse(Stream xmlStream);
}