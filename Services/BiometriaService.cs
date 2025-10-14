// PontoRefeitorio/Services/BiometriaService.cs
using CIDBio;
using System.Threading.Tasks;

public class BiometriaService
{
    public Task<(bool Success, string Message, string TemplateBase64)> CapturarTemplate()
    {
        return Task.Run(() => {
            try
            {
                var ret = CIDBio.Init();
                if (ret != RetCode.SUCCESS && ret != RetCode.WARNING_ALREADY_INIT)
                {
                    return (false, "Erro ao iniciar leitor: " + CIDBio.GetErrorMessage(ret), null);
                }

                ret = CIDBio.CaptureImageAndTemplate(out string template, out _, out _, out _, out _);
                if (ret != RetCode.SUCCESS)
                {
                    return (false, "Falha na captura: " + CIDBio.GetErrorMessage(ret), null);
                }

                return (true, "Captura realizada com sucesso", template);
            }
            catch (DllNotFoundException)
            {
                return (false, "Erro Crítico: DLL do leitor iDBIO não encontrada.", null);
            }
            catch (Exception ex)
            {
                return (false, $"Erro inesperado: {ex.Message}", null);
            }
            finally
            {
                CIDBio.Terminate();
            }
        });
    }
}