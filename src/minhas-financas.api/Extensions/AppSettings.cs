using System.Diagnostics.CodeAnalysis;

namespace minhas_financas.api.Extensions
{
    [ExcludeFromCodeCoverage]
    public class AppSettings
    {
        public virtual string Secret { get; set; }
        public virtual int ExpiracaoHoras { get; set; }
        public virtual string Emissor { get; set; }
        public virtual string ValidoEm { get; set; }
    }
}
